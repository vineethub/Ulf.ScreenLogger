using System;
using System.Threading;
using ULFScreenLogger.Mock;

class Program
{
    static void Main(string[] args)
    {
        var cfg = new Config
        {
            CaptureIntervalMs = 2000, // every 2 seconds
            ThumbWidth = 320,
            ThumbHeight = 180,
            TileSize = 16,
            OutputPath = "capture.ulf"
        };

        var engine = new Engine(cfg);

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("Stopping...");
            e.Cancel = true;
            cts.Cancel();
        };

        engine.Run(cts.Token);
    }
}
