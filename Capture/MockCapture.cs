using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ULFScreenLogger.Mock
{
    // Generates synthetic frames so you can develop on macOS.
    public class MockCapture
    {
        private readonly int _w, _h;
        private int _tick = 0;
        public MockCapture(int w, int h) { _w = w; _h = h; }

        public Bitmap Capture()
        {
            var bmp = new Bitmap(_w, _h, PixelFormat.Format24bppRgb);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.FromArgb(30, 30, 30));

            // moving rectangle
            int rw = Math.Max(12, _w / 3), rh = Math.Max(12, _h / 3);
            int x = Math.Abs(((_tick * 7) % (_w + rw)) - rw);
            int y = Math.Abs(((_tick * 5) % (_h + rh)) - rh);
            var r = new Rectangle(x, y, rw, rh);
            g.FillRectangle(Brushes.DarkCyan, r);

            // moving text
            var txt = $"tick {_tick}";
            using var font = new Font("Consolas", 10);
            g.DrawString(txt, font, Brushes.White, new PointF((_tick * 3) % _w, _h - 18));

            _tick++;
            return bmp;
        }
    }
}
