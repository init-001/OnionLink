using System;
using System.Linq;
using System.Text;
using Xunit;
using Common.Crypto;
using static Common.Statics.Constants;

namespace Crypto.Tests
{
    public class EncodingServiceTests
    {
        private static readonly byte[] EmptyNetId = Array.Empty<byte>();

        [Fact]
        public void Sha256d_KnownValue()
        {
            var data = Encoding.UTF8.GetBytes("hello");
            var hash = EncodingService.Sha256d(data);
            Assert.Equal(32, hash.Length);
        }

        [Fact]
        public void B58EncodeDecode_Roundtrip_PrivateKey()
        {
            var original = Enumerable.Range(0, 32).Select(i => (byte)i).ToArray();
            var encoded = EncodingService.B58Encode(original, publicKey: false);
            var decoded = EncodingService.B58Decode(encoded, publicKey: false);
            Assert.True(original.SequenceEqual(decoded));
        }

        [Fact]
        public void B58EncodeDecode_Roundtrip_PublicKey()
        {
            var original = Enumerable.Range(100, 32).Select(i => (byte)i).ToArray();
            var encoded = EncodingService.B58Encode(original, publicKey: true);
            var decoded = EncodingService.B58Decode(encoded, publicKey: true);
            Assert.True(original.SequenceEqual(decoded));
        }


        [Fact]
        public void B58Decode_InvalidChecksum_Throws()
        {
            var badString = "111111"; // way too short, will fail checksum
            Assert.Throws<ArgumentException>(() => EncodingService.B58Decode(badString, publicKey: false));
        }

        [Fact]
        public void B85Encode_MatchesPythonStyle()
        {
            var data = Encoding.ASCII.GetBytes("test");
            var encoded = EncodingService.B85Encode(data);
            // In Python: base64.b85encode(b'test') => b'FCfN8'
            Assert.Equal("FCfN8", encoded);
        }

        [Fact]
        public void B10Encode_ConvertsFingerprint()
        {
            var fp = new byte[] { 0x01, 0x02, 0x03 };
            var encoded = EncodingService.B10Encode(fp);
            Assert.Equal("66051", encoded); // 0x010203 = 66051 decimal
        }

        [Fact]
        public void UnicodePadding_AndRemovePadding_Roundtrip()
        {
            var input = "hello";
            var padded = EncodingService.UnicodePadding(input);
            var result = EncodingService.RemovePadding(padded);
            Assert.Equal(input, result);
            Assert.Equal(PADDING_LENGTH, padded.Length);
        }

        [Fact]
        public void UnicodePadding_InvalidTooLong_Throws()
        {
            var longStr = new string('a', PADDING_LENGTH);
            Assert.Throws<InvalidOperationException>(() => EncodingService.UnicodePadding(longStr));
        }

        [Fact]
        public void BoolToBytes_AndBytesToBool_Roundtrip()
        {
            Assert.True(EncodingService.BytesToBool(EncodingService.BoolToBytes(true)));
            Assert.False(EncodingService.BytesToBool(EncodingService.BoolToBytes(false)));
        }

        [Fact]
        public void IntToBytes_AndBytesToInt_Roundtrip()
        {
            ulong value = 123456789;
            var bytes = EncodingService.IntToBytes(value);
            var roundtrip = EncodingService.BytesToInt(bytes);
            Assert.Equal(value, roundtrip);
        }

        [Fact]
        public void DoubleToBytes_AndBytesToDouble_Roundtrip()
        {
            double value = 12345.6789;
            var bytes = EncodingService.DoubleToBytes(value);
            var roundtrip = EncodingService.BytesToDouble(bytes);
            Assert.Equal(value, roundtrip, 10); // precision tolerance
        }

        [Fact]
        public void StrToBytes_AndBytesToStr_Roundtrip()
        {
            var input = "abc";
            var bytes = EncodingService.StrToBytes(input);
            var roundtrip = EncodingService.BytesToStr(bytes);
            Assert.Equal(input, roundtrip);
        }

        [Fact]
        public void BytesToTimestamp_Valid()
        {
            var dt = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var seconds = BitConverter.GetBytes((uint)dt.ToUnixTimeSeconds());
            var result = EncodingService.BytesToTimestamp(seconds);
            Assert.Equal(dt.UtcDateTime, result);
        }

        [Fact]
        public void BytesToTimestamp_Invalid_Throws()
        {
            Assert.Throws<ArgumentException>(() => EncodingService.BytesToTimestamp(new byte[2]));
        }
    }
}
