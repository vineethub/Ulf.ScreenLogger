using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ULFScreenLogger.Mock
{
    public record Tile(int X, int Y, int Width, int Height, byte[] Pixels);

    public class TileEncoder
    {
        private readonly int _tileSize;
        public TileEncoder(int tileSize) => _tileSize = tileSize;

        public Tile[] SplitToTiles(Bitmap bmp)
        {
            int cols = (bmp.Width + _tileSize - 1) / _tileSize;
            int rows = (bmp.Height + _tileSize - 1) / _tileSize;
            var list = new List<Tile>(cols * rows);
            for (int ty = 0; ty < rows; ty++)
            for (int tx = 0; tx < cols; tx++)
            {
                int sx = tx * _tileSize;
                int sy = ty * _tileSize;
                int w = Math.Min(_tileSize, bmp.Width - sx);
                int h = Math.Min(_tileSize, bmp.Height - sy);
                var rect = new Rectangle(sx, sy, w, h);
                var data = BitmapToBgr24(bmp, rect);
                list.Add(new Tile(tx, ty, w, h, data));
            }
            return list.ToArray();
        }

        private static byte[] BitmapToBgr24(Bitmap bmp, Rectangle rect)
        {
            var crop = bmp.Clone(rect, PixelFormat.Format24bppRgb);
            var data = crop.LockBits(new Rectangle(0,0,crop.Width,crop.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                int bytes = Math.Abs(data.Stride) * crop.Height;
                var arr = new byte[bytes];
                Marshal.Copy(data.Scan0, arr, 0, bytes);
                return arr;
            }
            finally
            {
                crop.UnlockBits(data);
                crop.Dispose();
            }
        }

        // Prototype: raw pixels. We'll replace with palette+RLE later.
        public byte[] Encode(Tile tile) => tile.Pixels;
    }
}
