﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace SAM.API.Types
{
    public static class SteamExtensions
    {
        public static byte ReadValueU8(this Stream stream)
        {
            return (byte)stream.ReadByte();
        }

        public static int ReadValueS32(this Stream stream)
        {
            var data = new byte[4];
            var read = stream.Read(data, 0, 4);
            Debug.Assert(read == 4);
            return BitConverter.ToInt32(data, 0);
        }

        public static uint ReadValueU32(this Stream stream)
        {
            var data = new byte[4];
            var read = stream.Read(data, 0, 4);
            Debug.Assert(read == 4);
            return BitConverter.ToUInt32(data, 0);
        }

        public static ulong ReadValueU64(this Stream stream)
        {
            var data = new byte[8];
            var read = stream.Read(data, 0, 8);
            Debug.Assert(read == 8);
            return BitConverter.ToUInt64(data, 0);
        }

        public static float ReadValueF32(this Stream stream)
        {
            var data = new byte[4];
            var read = stream.Read(data, 0, 4);
            Debug.Assert(read == 4);
            return BitConverter.ToSingle(data, 0);
        }

        public static string ReadStringInternalDynamic(this Stream stream, Encoding encoding, char end)
        {
            var characterSize = encoding.GetByteCount("e");
            Debug.Assert(characterSize == 1 || characterSize == 2 || characterSize == 4);
            var characterEnd = end.ToString(CultureInfo.InvariantCulture);

            var i = 0;
            var data = new byte[128 * characterSize];

            while (true)
            {
                if (i + characterSize > data.Length)
                {
                    Array.Resize(ref data, data.Length + (128 * characterSize));
                }

                var read = stream.Read(data, i, characterSize);
                Debug.Assert(read == characterSize);

                if (encoding.GetString(data, i, characterSize) == characterEnd)
                {
                    break;
                }

                i += characterSize;
            }

            return i == 0 ? "" : encoding.GetString(data, 0, i);
        }

        public static string ReadStringAscii(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(Encoding.ASCII, '\0');
        }

        public static string ReadStringUnicode(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(Encoding.UTF8, '\0');
        }
    }
}
