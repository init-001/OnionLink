using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using static Common.Statics.Constants;

namespace Common.Crypto
{
    /// <summary>
    /// Utility functions for encoding, decoding and type conversion.
    /// Mirrors helper functions from the original Python implementation.
    /// </summary>
    public static class EncodingService
    {
        // ASCII85 characters: ASCII codes 33 ('!') through 117 ('u') inclusive (85 chars)
        private static readonly string Ascii85Chars = new string(Enumerable.Range(33, 85).Select(i => (char)i).ToArray());

        /// <summary>
        /// Double SHA-256 (sha256d) used for Base58 checksum (bitcoin style).
        /// </summary>
        public static byte[] Sha256d(byte[] message)
        {
            using var sha256 = SHA256.Create();
            var first = sha256.ComputeHash(message);
            return sha256.ComputeHash(first);
        }

        /// <summary>
        /// Encode a byte array to Base58 with checksum. If publicKey true uses TESTNET_HEADER.
        /// </summary>
        public static string B58Encode(byte[] data, bool publicKey = false)
        {
            var netId = publicKey ? TESTNET_HEADER : MAINNET_HEADER;
            var payload = netId.Concat(data).ToArray();
            payload = payload.Concat(Sha256d(payload).Take(B58_CHECKSUM_LENGTH)).ToArray();

            int originalLen = payload.Length;
            var trimmed = payload.SkipWhile(b => b == 0x00).ToArray();
            int newLen = trimmed.Length;

            // big integer - create positive big-endian integer (append zero to force unsigned interpretation)
            var intData = new BigInteger(trimmed.Reverse().Concat(new byte[] { 0 }).ToArray());

            var sb = new StringBuilder();
            while (intData > 0)
            {
                var div = BigInteger.DivRem(intData, 58, out var rem);
                intData = div;
                sb.Append(B58_ALPHABET[(int)rem]);
            }

            for (int i = 0; i < (originalLen - newLen); i++)
                sb.Append(B58_ALPHABET[0]);

            return new string(sb.ToString().Reverse().ToArray());
        }

        /// <summary>
        /// Decode Base58 with checksum verification. Throws on invalid checksum or net id.
        /// </summary>
        public static byte[] B58Decode(string s, bool publicKey = false)
        {
            var netId = publicKey ? TESTNET_HEADER : MAINNET_HEADER;
            int origLen = s.Length;
            s = s.TrimStart(B58_ALPHABET[0]);
            int newLen = s.Length;

            BigInteger intData = BigInteger.Zero;
            foreach (var c in s.Reverse())
            {
                intData = intData * 58 + B58_ALPHABET.IndexOf(c);
            }

            var bytes = new List<byte>();
            while (intData > 0)
            {
                var div = BigInteger.DivRem(intData, 256, out var rem);
                bytes.Add((byte)rem);
                intData = div;
            }

            var decoded = bytes.Reverse<byte>()
                               .Concat(Enumerable.Repeat((byte)0x00, origLen - newLen))
                               .ToArray();

            var payload = decoded.Take(decoded.Length - B58_CHECKSUM_LENGTH).ToArray();
            var checksum = decoded.Skip(decoded.Length - B58_CHECKSUM_LENGTH).ToArray();

            if (!Sha256d(payload).Take(B58_CHECKSUM_LENGTH).SequenceEqual(checksum))
                throw new ArgumentException("Invalid checksum");

            if (!decoded.Take(netId.Length).SequenceEqual(netId))
                throw new ArgumentException("Invalid network ID");

            return decoded.Skip(netId.Length).Take(decoded.Length - netId.Length - B58_CHECKSUM_LENGTH).ToArray();
        }

        /// <summary>
        /// This function encodes 4-byte groups into 5 ASCII85 characters; final partial group
        /// produces (n + 1) output characters (n = number of remaining input bytes).
        /// </summary>
        public static string B85Encode(byte[] data)
        {
            if (data == null || data.Length == 0) return string.Empty;

            var sb = new StringBuilder();
            uint value = 0;
            int count = 0;

            foreach (var b in data)
            {
                value = (value << 8) + b;
                count++;
                if (count == 4)
                {
                    uint divisor = 85u * 85u * 85u * 85u; // 85^4
                    for (int i = 0; i < 5; i++)
                    {
                        sb.Append(Ascii85Chars[(int)(value / divisor)]);
                        value %= divisor;
                        divisor /= 85u;
                    }
                    value = 0;
                    count = 0;
                }
            }

            if (count > 0)
            {
                // pad remaining bytes to 4 bytes (left shift)
                for (int i = count; i < 4; i++)
                    value <<= 8;

                uint divisor = 85u * 85u * 85u * 85u; // 85^4
                // output count + 1 chars for the partial block
                for (int i = 0; i < count + 1; i++)
                {
                    sb.Append(Ascii85Chars[(int)(value / divisor)]);
                    value %= divisor;
                    divisor /= 85u;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Encode fingerprint bytes as a decimal string (big integer base10).
        /// </summary>
        public static string B10Encode(byte[] fingerprint)
        {
            if (fingerprint == null || fingerprint.Length == 0) return "0";

            BigInteger v = BigInteger.Zero;
            foreach (var b in fingerprint)
            {
                v = (v << 8) | b;
            }
            return v.ToString();
        }

        /// <summary>
        /// Pad a Unicode string to PADDING_LENGTH characters (UTF-32 storage in DB).
        /// </summary>
        public static string UnicodePadding(string s)
        {
            if (s.Length >= PADDING_LENGTH)
                throw new InvalidOperationException("Invalid input size.");

            int length = PADDING_LENGTH - (s.Length % PADDING_LENGTH);
            s += new string((char)length, length);

            if (s.Length != PADDING_LENGTH)
                throw new InvalidOperationException("Invalid padded string size.");

            return s;
        }

        /// <summary>
        /// Remove PKCS-like unicode padding added by UnicodePadding.
        /// </summary>
        public static string RemovePadding(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            int pad = (int)s[^1];
            return s[..^pad];
        }

        /// <summary>
        /// Convert bool to single byte.
        /// </summary>
        public static byte[] BoolToBytes(bool b) => new[] { (byte)(b ? 1 : 0) };

        /// <summary>
        /// Convert unsigned 64-bit to big-endian 8 bytes.
        /// </summary>
        public static byte[] IntToBytes(ulong i) => BitConverter.GetBytes(i).Reverse().ToArray();

        /// <summary>
        /// Convert double to 8-byte representation (little-endian by BitConverter).
        /// </summary>
        public static byte[] DoubleToBytes(double d) => BitConverter.GetBytes(d);

        /// <summary>
        /// Pad and UTF-32 encode a string for DB storage (uses UnicodePadding).
        /// </summary>
        public static byte[] StrToBytes(string s) => Encoding.UTF32.GetBytes(UnicodePadding(s));

        /// <summary>
        /// Convert 1-byte array to bool.
        /// </summary>
        public static bool BytesToBool(byte[] b) => b != null && b.Length > 0 && b[0] != 0;

        /// <summary>
        /// Convert 8 big-endian bytes to unsigned 64-bit integer.
        /// </summary>
        public static ulong BytesToInt(byte[] b) => BitConverter.ToUInt64(b.Reverse().ToArray());

        /// <summary>
        /// Convert 8 bytes to double (BitConverter uses machine endianness).
        /// </summary>
        public static double BytesToDouble(byte[] b) => BitConverter.ToDouble(b, 0);

        /// <summary>
        /// Decode padded UTF-32 bytes to string and remove padding.
        /// </summary>
        public static string BytesToStr(byte[] b) => RemovePadding(Encoding.UTF32.GetString(b));

        /// <summary>
        /// Convert a 4-byte little-endian Unix timestamp to DateTime (UTC).
        /// </summary>
        public static DateTime BytesToTimestamp(byte[] b)
        {
            if (b == null || b.Length < 4) throw new ArgumentException("Timestamp must be 4 bytes.");
            uint seconds = BitConverter.ToUInt32(b, 0);
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        }
    }
}
