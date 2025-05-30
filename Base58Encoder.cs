﻿// ReSharper disable InconsistentNaming
namespace FindstakeNet
{
    public class Base58Encoder : DataEncoder
    {
        static readonly char[] pszBase58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

        private static readonly int[] mapBase58 =
        [
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, -1, -1, -1, -1, -1, -1,
            -1, 9, 10, 11, 12, 13, 14, 15, 16, -1, 17, 18, 19, 20, 21, -1,
            22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, -1, -1, -1, -1, -1,
            -1, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, -1, 44, 45, 46,
            47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
        ];
        /// <summary>
        /// Fast check if the string to know if base58 str
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public virtual bool IsMaybeEncoded(string str)
        {
            bool maybeb58 = true;
            if (maybeb58)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (!Base58Encoder.pszBase58.Contains(str[i]))
                    {
                        maybeb58 = false;
                        break;
                    }
                }
            }
            return maybeb58 && str.Length > 0;
        }

        public override string EncodeData(byte[] data, int offset, int count)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            // Skip & count leading zeroes.
            int zeroes = 0;
            int length = 0;
            while (offset != count && data[offset] == 0)
            {
                offset++;
                zeroes++;
            }
            // Allocate enough space in big-endian base58 representation.
            int size = (count - offset) * 138 / 100 + 1; // log(256) / log(58), rounded up.

            byte[] b58 = new byte[size];

            // Process the bytes.
            while (offset != count)
            {
                int carry = data[offset];
                int i = 0;
                // Apply "b58 = b58 * 256 + ch".
                for (int it = size - 1; (carry != 0 || i < length) && it >= 0; i++, it--)
                {
                    carry += 256 * b58[it];
                    b58[it] = (byte)(carry % 58);
                    carry /= 58;
                }

                length = i;
                offset++;
            }
            // Skip leading zeroes in base58 result.
            int it2 = (size - length);
            while (it2 != size && b58[it2] == 0)
                it2++;
            // Translate the result into a string.

            var str = new char[zeroes + size - it2];

            ArrayFill(str, '1', 0, zeroes);

            int i2 = zeroes;
            while (it2 != size)
                str[i2++] = pszBase58[b58[it2++]];
            return new string(str);
        }

        static void ArrayFill<T>(T[] array, T value, int index, int count)
        {
            for (int i = index; i < index + count; i++)
            {
                array[i] = value;
            }
        }



        public override byte[] DecodeData(string encoded)
        {
            if (encoded == null) throw new ArgumentNullException(nameof(encoded));
            int psz = 0;
            // Skip leading spaces.
            while (psz < encoded.Length && IsSpace(encoded[psz]))
                psz++;
            // Skip and count leading '1's.
            int zeroes = 0;
            int length = 0;
            while (psz < encoded.Length && encoded[psz] == '1')
            {
                zeroes++;
                psz++;
            }
            // Allocate enough space in big-endian base256 representation.
            int size = (encoded.Length - psz) * 733 / 1000 + 1; // log(58) / log(256), rounded up.

            byte[] b256 = new byte[size];

            // Process the characters.
            while (psz < encoded.Length && !IsSpace(encoded[psz]))
            {
                // Decode base58 character
                int carry = mapBase58[(byte)encoded[psz]];
                if (carry == -1)  // Invalid b58 character
                    throw new FormatException("Invalid base58 data");
                int i = 0;
                for (int it = size - 1; (carry != 0 || i < length) && it >= 0; i++, it--)
                {
                    carry += 58 * b256[it];
                    b256[it] = (byte)(carry % 256);
                    carry /= 256;
                }
                length = i;
                psz++;
            }
            // Skip trailing spaces.
            while (psz < encoded.Length && IsSpace(encoded[psz]))
                psz++;
            if (psz != encoded.Length)
                throw new FormatException("Invalid base58 data");
            // Skip leading zeroes in b256.
            var it2 = size - length;
            // Copy result into output vector.
            var vch = new byte[zeroes + size - it2];

            ArrayFill<byte>(vch, 0, 0, zeroes);

            int i2 = zeroes;
            while (it2 != size)
                vch[i2++] = (b256[it2++]);
            return vch;
        }
    }


    public abstract class DataEncoder
    {
        // char.IsWhiteSpace fits well but it match other whitespaces
        // characters too and also works for unicode characters.
        public static bool IsSpace(char c)
        {
            switch (c)
            {
                case ' ':
                case '\t':
                case '\n':
                case '\v':
                case '\f':
                case '\r':
                    return true;
            }
            return false;
        }

        internal DataEncoder()
        {
        }

        public string EncodeData(byte[] data)
        {
            return EncodeData(data, 0, data.Length);
        }

        public abstract string EncodeData(byte[] data, int offset, int count);

        public abstract byte[] DecodeData(string encoded);
    }

    public static class Encoders
    {
        //		static readonly ASCIIEncoder _ASCII = new ASCIIEncoder();
        //		public static DataEncoder ASCII
        //		{
        //			get
        //			{
        //				return _ASCII;
        //			}
        //		}

        // static readonly HexEncoder _Hex = new HexEncoder();
        // public static DataEncoder Hex
        // {
        //     get
        //     {
        //         return _Hex;
        //     }
        // }

        static readonly Base58Encoder _Base58 = new Base58Encoder();
        public static DataEncoder Base58
        {
            get
            {
                return _Base58;
            }
        }

        //		static readonly Base32Encoder _Base32 = new Base32Encoder();
        //		public static DataEncoder Base32
        //		{
        //			get
        //			{
        //				return _Base32;
        //			}
        //		}

        //		private static readonly Base58CheckEncoder _Base58Check = new Base58CheckEncoder();
        //		public static DataEncoder Base58Check
        //		{
        //			get
        //			{
        //				return _Base58Check;
        //			}
        //		}
        //


        // static readonly Base64Encoder _Base64 = new Base64Encoder();
        // public static DataEncoder Base64
        // {
        //     get
        //     {
        //         return _Base64;
        //     }
        // }

        //		public static Bech32Encoder Bech32(string hrp)
        //		{
        //			return new Bech32Encoder(hrp);
        //		}
        //		public static Bech32Encoder Bech32(byte[] hrp)
        //		{
        //			return new Bech32Encoder(hrp);
        //		}

    }

}
