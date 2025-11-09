using System;
using K4os.Hash.xxHash;

namespace Ulf.ScreenLogger.Encoding
{
    public class TileHasher
    {
        public ulong Hash(byte[] pixels)
        {
            return XxHash64.ComputeHash(pixels);
        }
    }
}
