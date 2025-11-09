using System;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using Ulf.ScreenLogger.Encoding;

namespace ULFScreenLogger.Mock
{
    public class Config
    {
        public int CaptureIntervalMs { get; set; } = 5000;
        public int ThumbWidth { get; set; } = 160;
        public int ThumbHeight { get; set; } = 90;
        public int TileSize { get; set; } = 16;
        public string OutputPath { get; set; } = "capture.ulf";
    }

    public class Engine
    {
        private readonly Config _cfg;
        private readonly MockCapture _capture;
        private readonly TileEncoder _encoder;
        private readonly TileHasher _hasher;
        private readonly FrameWriter _writer;
        private Dictionary<int, ulong> _prevHashes = new();

        public Engine(Config cfg)
        {
            _cfg = cfg;
            _capture = new MockCapture(cfg.ThumbWidth, cfg.ThumbHeight);
            _encoder = new TileEncoder(cfg.TileSize);
            _hasher = new TileHasher();
            _writer = new FrameWriter(cfg.OutputPath);
        }

        public void Run(CancellationToken ct)
        {
            Console.WriteLine("Engine started. Press Ctrl+C to stop.");
            while (!ct.IsCancellationRequested)
            {
                using var bmp = _capture.Capture();
                var tiles = _encoder.SplitToTiles(bmp);
                var changed = new Dictionary<int, ulong>();
                for (int i = 0; i < tiles.Length; i++)
                {
                    var t = tiles[i];
                    var h = _hasher.Hash(t.Pixels);
                    if (!_prevHashes.TryGetValue(i, out var ph) || ph != h)
                    {
                        changed[i] = h;
                        _prevHashes[i] = h;
                    }
                }

                _writer.WriteFrame(DateTime.UtcNow, "mock", tiles, changed, tile => _encoder.Encode(tile));
                Thread.Sleep(_cfg.CaptureIntervalMs);
            }

            _writer.Dispose();
        }
    }
}
