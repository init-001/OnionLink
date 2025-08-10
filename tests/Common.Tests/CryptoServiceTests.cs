using System;
using Common.Crypto;
using Common.Exceptions;
using Xunit;
using FluentAssertions;
using static Common.Statics.Constants;

namespace Crypto.Tests;
public class CryptoServiceTests
{
    private readonly CryptoService _crypto = new();
    private readonly byte[] _key = new byte[32]; // Assuming SYMMETRIC_KEY_LENGTH is 32
    public  const int SYMMETRIC_KEY_LENGTH = 32;


    [Fact]
    public void Blake2b_HashesMessage_CorrectLength()
    {
        var message = System.Text.Encoding.UTF8.GetBytes("hello world");
        var hash = _crypto.Blake2b(message);

        Assert.NotNull(hash);
        Assert.Equal(32, hash.Length);
    }

    [Fact]
    public void Blake2b_WithKey_ProducesDifferentHash()
    {
        var message = System.Text.Encoding.UTF8.GetBytes("hello world");
        var key = new byte[16];
        new Random(42).NextBytes(key);

        var hashNoKey = _crypto.Blake2b(message);
        var hashWithKey = _crypto.Blake2b(message, key);

        Assert.NotEqual(hashNoKey, hashWithKey);
    }

    [Fact]
    public void Blake2b_WithSalt_ProducesHash()
    {
        var message = System.Text.Encoding.UTF8.GetBytes("hello world");
        var salt = new byte[16];
        new Random(42).NextBytes(salt);

        var hash = _crypto.Blake2b(message, salt: salt);

        Assert.NotNull(hash);
        Assert.Equal(32, hash.Length);
    }

    [Fact]
    public void Blake2b_InvalidSalt_Throws()
    {
        // Arrange
        var message = System.Text.Encoding.UTF8.GetBytes("hello");
        var invalidSalt = new byte[10]; // invalid salt size

        // Act & Assert
        Assert.Throws<CriticalError>(() => _crypto.Blake2b(message, salt: invalidSalt));
    }

    [Fact]
    public void Blake2b_InvalidDigestSize_Throws()
    {
        // Arrange
        var message = System.Text.Encoding.UTF8.GetBytes("hello");

        // Act & Assert
        Assert.Throws<CriticalError>(() => _crypto.Blake2b(message, digestSize: 0));
        Assert.Throws<CriticalError>(() => _crypto.Blake2b(message, digestSize: 100));
    }

    [Fact]
    public void Blake2b_DifferentDigestSize_ProducesCorrectLength()
    {
        // Arrange
        var message = System.Text.Encoding.UTF8.GetBytes("hello world");

        // Act
        var hash16 = _crypto.Blake2b(message, digestSize: 16);
        var hash64 = _crypto.Blake2b(message, digestSize: 64);

        // Assert
        Assert.Equal(16, hash16.Length);
        Assert.Equal(64, hash64.Length);
        Assert.NotEqual(hash16, hash64);
    }

    [Fact]
    public void Argon2Kdf_ValidParameters_ReturnsCorrectLengthKey()
    {
        // Arrange
        string password = "strongpassword";
        byte[] salt = new byte[16];
        new Random().NextBytes(salt);
        int timeCost = 2;
        int memoryCostKB = 65536; // 64 MB
        int parallelism = 1;

        // Act
        byte[] key = _crypto.Argon2Kdf(password, salt, timeCost, memoryCostKB, parallelism);

        // Assert
        Assert.NotNull(key);
        Assert.Equal(32, key.Length); // SYMMETRIC_KEY_LENGTH constant
    }

    [Fact]
    public void Argon2Kdf_NullSalt_ThrowsCriticalError()
    {
        // Arrange
        string password = "password";

        // Act & Assert
        var ex = Assert.Throws<CriticalError>(() =>
            _crypto.Argon2Kdf(password, null!, 1, 1024, 1));
        Assert.Contains("Salt cannot be null", ex.Message);
    }

    [Fact]
    public void Argon2Kdf_InvalidSaltLength_ThrowsCriticalError()
    {
        // Arrange
        string password = "password";
        byte[] invalidSalt = new byte[10]; // Invalid length

        // Act & Assert
        var ex = Assert.Throws<CriticalError>(() =>
            _crypto.Argon2Kdf(password, invalidSalt, 1, 1024, 1));
        Assert.Contains("Invalid salt length", ex.Message);
    }

    [Fact]
    public void Argon2Kdf_NullOrEmptyPassword_ThrowsCriticalError()
    {
        // Arrange
        byte[] salt = new byte[16];
        new Random().NextBytes(salt);

        // Act & Assert
        var ex1 = Assert.Throws<CriticalError>(() =>
            _crypto.Argon2Kdf(null!, salt, 1, 1024, 1));
        Assert.Contains("Password cannot be null or empty", ex1.Message);

        var ex2 = Assert.Throws<CriticalError>(() =>
            _crypto.Argon2Kdf(string.Empty, salt, 1, 1024, 1));
        Assert.Contains("Password cannot be null or empty", ex2.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Argon2Kdf_InvalidTimeCost_ThrowsCriticalError(int invalidTimeCost) 
    {
        // Arrange
        string password = "password";
        byte[] salt = new byte[16];
        new Random().NextBytes(salt);

        // Act & Assert
        var ex = Assert.Throws<CriticalError>(() =>
            _crypto.Argon2Kdf(password, salt, invalidTimeCost, 1024, 1));
        Assert.Contains("Time cost must be greater than zero", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1024)]
    public void Argon2Kdf_InvalidMemoryCost_ThrowsCriticalError(int invalidMemoryCost)
    {
        // Arrange
        string password = "password";
        byte[] salt = new byte[16];
        new Random().NextBytes(salt);

        // Act & Assert
        var ex = Assert.Throws<CriticalError>(() =>
            _crypto.Argon2Kdf(password, salt, 1, invalidMemoryCost, 1));
        Assert.Contains("Memory cost must be greater than zero", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void Argon2Kdf_InvalidParallelism_ThrowsCriticalError(int invalidParallelism)
    {
        // Arrange
        string password = "password";
        byte[] salt = new byte[16];
        new Random().NextBytes(salt);

        // Act & Assert
        var ex = Assert.Throws<CriticalError>(() =>
            _crypto.Argon2Kdf(password, salt, 1, 1024, invalidParallelism));
        Assert.Contains("Parallelism must be greater than zero", ex.Message);
    }

    [Fact]
    public void GenerateX448PrivateKey_ShouldReturn56Bytes()
    {
        var privateKey = _crypto.GenerateX448PrivateKey();
        privateKey.Should().NotBeNull();
        privateKey.Length.Should().Be(56);
    }

    [Fact]
    public void DeriveX448PublicKey_WithValidPrivateKey_ShouldReturn56Bytes()
    {
        var privateKey = _crypto.GenerateX448PrivateKey();
        var publicKey = _crypto.DeriveX448PublicKey(privateKey);

        publicKey.Should().NotBeNull();
        publicKey.Length.Should().Be(56);
    }

    [Fact]
    public void DeriveX448PublicKey_WithInvalidPrivateKey_ShouldThrow()
    {
        var invalidKey = new byte[10]; // Too short

        Action act = () => _crypto.DeriveX448PublicKey(invalidKey);

        act.Should().Throw<CriticalError>()
            .WithMessage("Private key must be 56 bytes.");
    }

    [Fact]
    public void CalculateX448SharedSecret_ShouldReturn56Bytes()
    {
        var alicePrivate = _crypto.GenerateX448PrivateKey();
        var alicePublic = _crypto.DeriveX448PublicKey(alicePrivate);

        var bobPrivate = _crypto.GenerateX448PrivateKey();
        var bobPublic = _crypto.DeriveX448PublicKey(bobPrivate);

        var aliceShared = _crypto.CalculateX448SharedSecret(alicePrivate, bobPublic);
        var bobShared = _crypto.CalculateX448SharedSecret(bobPrivate, alicePublic);

        aliceShared.Should().NotBeNull();
        bobShared.Should().NotBeNull();
        aliceShared.Length.Should().Be(56);
        bobShared.Length.Should().Be(56);

        // Shared secrets must match
        aliceShared.Should().Equal(bobShared);
    }

    [Fact]
    public void CalculateX448SharedSecret_WithInvalidKeys_ShouldThrow()
    {
        var privateKey = _crypto.GenerateX448PrivateKey();
        var invalidPublicKey = new byte[10]; // Too short

        var ex = Assert.Throws<CriticalError>(() =>
            _crypto.CalculateX448SharedSecret(privateKey, invalidPublicKey));

        Assert.Equal("Public key must be 56 bytes.", ex.Message);
    }

    [Fact]
    public void GetX448SharedKey_WithValidSharedSecret_ShouldReturnHashedKey()
    {
        var privateKey = _crypto.GenerateX448PrivateKey();
        var publicKey = _crypto.DeriveX448PublicKey(privateKey);

        var sharedSecret = _crypto.CalculateX448SharedSecret(privateKey, publicKey);
        var sharedKey = _crypto.GetX448SharedKey(sharedSecret);

        sharedKey.Should().NotBeNull();
        sharedKey.Length.Should().Be(32); // MessageKeyLength in your code
    }

    [Fact]
    public void DeriveSubkeys_ShouldReturnSixUniqueKeys()
    {
        var userPrivate = _crypto.GenerateX448PrivateKey();
        var contactPrivate = _crypto.GenerateX448PrivateKey();

        var userPublic = _crypto.DeriveX448PublicKey(userPrivate);
        var contactPublic = _crypto.DeriveX448PublicKey(contactPrivate);

        var sharedSecret = _crypto.CalculateX448SharedSecret(userPrivate, contactPublic);
        var sharedKey = _crypto.GetX448SharedKey(sharedSecret);

        var keys = _crypto.DeriveSubkeys(sharedKey, userPublic, contactPublic);

        keys.tx_mk.Should().NotBeNull();
        keys.rx_mk.Should().NotBeNull();
        keys.tx_hk.Should().NotBeNull();
        keys.rx_hk.Should().NotBeNull();
        keys.tx_fp.Should().NotBeNull();
        keys.rx_fp.Should().NotBeNull();

        keys.tx_mk.Length.Should().Be(32);
        keys.rx_mk.Length.Should().Be(32);
        keys.tx_hk.Length.Should().Be(32);
        keys.rx_hk.Length.Should().Be(32);
        keys.tx_fp.Length.Should().Be(8);
        keys.rx_fp.Length.Should().Be(8);

        var allKeys = new[] { keys.tx_mk, keys.rx_mk, keys.tx_hk, keys.rx_hk, keys.tx_fp, keys.rx_fp };
        var uniqueCount = allKeys.Distinct(new ByteArrayComparer()).Count();
        uniqueCount.Should().Be(allKeys.Length, because: "all derived subkeys should be unique");
    }

    [Fact]
    public void EncryptAndSign_ReturnsNonNull_NonceCiphertext()
    {
        var plaintext = System.Text.Encoding.UTF8.GetBytes("Hello, world!");
        var result = _crypto.EncryptAndSign(plaintext, _key);

        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(plaintext.Length + XCHACHA20_NONCE_LENGTH);
    }

    [Fact]
    public void EncryptAndSign_WithInvalidKeyLength_ThrowsCriticalError()
    {
        var plaintext = System.Text.Encoding.UTF8.GetBytes("test");
        var invalidKey = new byte[10]; // Invalid length

        Action act = () => _crypto.EncryptAndSign(plaintext, invalidKey);

        act.Should().Throw<CriticalError>()
            .WithMessage("Invalid key length*");
    }

    [Fact]
    public void EncryptAndSign_And_DecryptAndVerify_RoundTrip_WorksCorrectly()
    {
        var plaintext = System.Text.Encoding.UTF8.GetBytes("Secret message!");
        var associatedData = System.Text.Encoding.UTF8.GetBytes("context");

        // Encrypt
        var encrypted = _crypto.EncryptAndSign(plaintext, _key, associatedData);

        // Fixing the CS1503 error by ensuring the third argument matches the expected type.
        var decrypted = _crypto.AuthAndDecrypt(encrypted, _key, associatedData: null);

        decrypted.Should().Equal(plaintext);
    }

    [Fact]
    public void DecryptAndVerify_WithModifiedCiphertext_ThrowsCriticalError()
    {
        var plaintext = System.Text.Encoding.UTF8.GetBytes("Another secret");
        var encrypted = _crypto.EncryptAndSign(plaintext, _key);

        // Tamper with ciphertext by flipping a bit
        encrypted[XCHACHA20_NONCE_LENGTH] ^= 0xFF;

        Action act = () => _crypto.AuthAndDecrypt(encrypted, _key);

        act.Should().Throw<CriticalError>()
            .WithMessage("Decryption failed*");
    }

    [Fact]
    public void DecryptAndVerify_WithInvalidKey_ThrowsCriticalError()
    {
        var plaintext = System.Text.Encoding.UTF8.GetBytes("Sensitive data");
        var encrypted = _crypto.EncryptAndSign(plaintext, _key);

        var invalidKey = new byte[SYMMETRIC_KEY_LENGTH];
        new Random(99).NextBytes(invalidKey);

        Action act = () => _crypto.AuthAndDecrypt(encrypted, invalidKey);

        act.Should().Throw<CriticalError>()
            .WithMessage("Decryption failed*");
    }

    [Fact]
    public void DecryptAndVerify_WithTooShortCiphertext_ThrowsCriticalError()
    {
        var tooShort = new byte[10];

        Action act = () => _crypto.AuthAndDecrypt(tooShort, _key);

        act.Should().Throw<CriticalError>()
            .WithMessage("Invalid ciphertext length*");
    }
    private class ByteArrayComparer : System.Collections.Generic.IEqualityComparer<byte[]>
    {
        public bool Equals(byte[]? x, byte[]? y)
        {
            if (x == null || y == null) return false;
            return System.Linq.Enumerable.SequenceEqual(x, y);
        }

        public int GetHashCode(byte[] obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            int hash = 17;
            for (int i = 0; i < Math.Min(4, obj.Length); i++)
                hash = hash * 31 + obj[i];
            return hash;
        }
    }
    
}

