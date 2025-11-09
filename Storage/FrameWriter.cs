using System;
using System.IO;
using System.Collections.Generic;
using K4os.Compression.LZ4.Streams; // ✅ important
using K4os.Compression.LZ4;         // ✅ for LZ4Level enum

namespace ULFScreenLogger.Mock
{
    public class FrameWriter : IDisposable
    {
        private readonly FileStream _fs;

        public FrameWriter(string path)
        {
            _fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            WriteHeader();
        }

        private void WriteHeader()
        {
            var hdr = System.Text.Encoding.UTF8.GetBytes("ULF1\n");
            _fs.Write(hdr, 0, hdr.Length);
            _fs.Flush();
        }

        // Simple prototype frame writer:
        // [meta_len][meta_json][tile_count][for each tile: x,y,w,h,comp_len,comp_bytes]
        public void WriteFrame(DateTime ts, string title, Tile[] tiles, Dictionary<int, ulong> changedTileIndexToHash, Func<Tile, byte[]> encoder)
        {
            var meta = new { ts = ts.ToString("o"), title };
            var metaB = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(meta));
            _fs.Write(BitConverter.GetBytes(metaB.Length));
            _fs.Write(metaB, 0, metaB.Length);

            _fs.Write(BitConverter.GetBytes(tiles.Length));

            for (int i = 0; i < tiles.Length; i++)
            {
                var t = tiles[i];
                if (!changedTileIndexToHash.TryGetValue(i, out var h))
                {
                    // unchanged marker
                    _fs.Write(BitConverter.GetBytes((int)-1));
                    continue;
                }

                var raw = encoder(t);
                using var ms = new MemoryStream();

                // ✅ updated to correct API
                using (var lz4 = LZ4Stream.Encode(ms, LZ4Level.L12_MAX))
                {
                    lz4.Write(raw, 0, raw.Length);
                }

                var comp = ms.ToArray();

                _fs.Write(BitConverter.GetBytes(t.X));
                _fs.Write(BitConverter.GetBytes(t.Y));
                _fs.Write(BitConverter.GetBytes(t.Width));
                _fs.Write(BitConverter.GetBytes(t.Height));
                _fs.Write(BitConverter.GetBytes(comp.Length));
                _fs.Write(comp, 0, comp.Length);
            }

            _fs.Flush();
        }

        public void Dispose() => _fs.Dispose();
    }
}
