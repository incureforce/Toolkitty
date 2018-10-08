using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace ToolKitty
{
    public static class UUIDGenerator
    {
        private static readonly RandomNumberGenerator
            RNG = new RNGCryptoServiceProvider();

        private static readonly int[]
            UUIDMap = {
                0x03,
                0x02,
                0x01,
                0x00,
                0x05,
                0x04,
                0x07,
                0x06,
                0x08,
                0x09,
                0x0A,
                0x0B,
                0x0C,
                0x0D,
                0x0E,
                0x0F,
            };

        public static Guid CreateRandom()
        {
            var seeds = new byte[][] {
            };

            return Create(seeds);
        }

        public static Guid CreateUTCRandom()
        {
            var seeds = new byte[][] {
                GetTimestampSeed(),
            };

            return Create(seeds);
        }

        public static Guid CreateUTCPIDRandom()
        {
            var seeds = new byte[][] {
                GetTimestampSeed(),
                GetProcessSeed(),
            };

            return Create(seeds);
        }

        public static Guid Create(byte[][] seeds)
        {
            var bytes = new byte[16];
            var bytesIndex = 0;

            RNG.GetBytes(bytes);

            foreach (var seed in seeds) {
                Array.Copy(seed, 0, bytes, bytesIndex, seed.Length);

                bytesIndex += seed.Length;
            }

            var guidBytes = new byte[bytes.Length];

            for (var guidIndex = 0; guidIndex < guidBytes.Length; ++guidIndex) {
                guidBytes[UUIDMap[guidIndex]] = bytes[guidIndex];
            }

            return new Guid(guidBytes);
        }

        public static byte[] GetProcessSeed()
        {
            var process = Process.GetCurrentProcess();

            var source = BitConverter.GetBytes(process.Id);
            var target = new byte[2];

            target[0] = source[1];
            target[1] = source[0];

            return target;
        }

        public static byte[] GetTimestampSeed()
        {
            var timestamp = Timestamp.GetTimestamp();

            var source = BitConverter.GetBytes(timestamp);
            var target = new byte[6];

            target[0] = source[5];
            target[1] = source[4];
            target[2] = source[3];
            target[3] = source[2];
            target[4] = source[1];
            target[5] = source[0];

            return target;
        }
    }
}