using System.Security.Cryptography;
using Common.Exceptions;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.Blake2b;
using Isopoh.Cryptography.SecureArray;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Sodium;
using static Common.Statics.Constants;

namespace Common.Crypto
{
    public class CryptoService
    {
        /// <summary>
        /// Computes the BLAKE2b cryptographic hash of the given message.
        /// </summary>
        /// <param name="message">The input byte array to hash. Cannot be null.</param>
        /// <param name="key">Optional key for keyed hashing (MAC). Maximum length is 64 bytes.</param>
        /// <param name="salt">Optional 16-byte salt for randomized hashing.</param>
        /// <param name="personalization">Optional 16-byte personalization string.</param>
        /// <param name="digestSize">The desired length of the hash output in bytes (1 to 64). Default is 32 bytes (256 bits).</param>
        /// <returns>Byte array containing the computed BLAKE2b hash of the specified length.</returns>
        /// <exception cref="CriticalError">Thrown if any parameter validation fails or hashing fails.</exception>
        public byte[] Blake2b(
            byte[] message,
            byte[]? key = null,
            byte[]? salt = null,
            byte[]? personalization = null,
            int digestSize = 32)
        {
            if (message == null)
                throw new CriticalError("Message cannot be null.");

            if (digestSize <= 0 || digestSize > Blake2B.OutputLength)
                throw new CriticalError($"Digest size must be between 1 and {Blake2B.OutputLength} bytes.");

            if (salt != null && salt.Length != BLAKE2_SALT_LENGTH_MAX)
                throw new CriticalError("Salt must be exactly 16 bytes.");

            if (personalization != null && personalization.Length != 16)
                throw new CriticalError("Personalization must be exactly 16 bytes.");

            if (key != null && key.Length > 64)
                throw new CriticalError("Key length cannot exceed 64 bytes.");

            var config = new Blake2BConfig
            {
                OutputSizeInBytes = digestSize,
                Key = key,
                Salt = salt,
                Personalization = personalization
            };

            SecureArrayCall? secureArrayCall = null;

            try
            {
                return Blake2B.ComputeHash(message, config, secureArrayCall);
            }
            catch (Exception ex)
            {
                // Wrap any unexpected exceptions in CriticalError to maintain consistency
                throw new CriticalError($"Blake2b hashing failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Derives a cryptographic key from a password and salt using Argon2id.
        /// </summary>
        /// <param name="password">The password to derive the key from.</param>
        /// <param name="salt">The salt (must be exactly 16 bytes).</param>
        /// <param name="timeCost">Number of iterations.</param>
        /// <param name="memoryCostKB">Memory cost in kilobytes.</param>
        /// <param name="parallelism">Number of threads to use.</param>
        /// <returns>The derived key as a byte array.</returns>
        /// <exception cref="CriticalError">Thrown when parameters are invalid or derivation fails.</exception>
        public byte[] Argon2Kdf(
            string password,
            byte[] salt,
            int timeCost,
            int memoryCostKB,
            int parallelism)
        {
            if (salt == null)
                throw new CriticalError("Salt cannot be null.");
            if (salt.Length != ARGON2_SALT_LENGTH)
                throw new CriticalError($"Invalid salt length ({salt.Length} bytes). Expected {ARGON2_SALT_LENGTH} bytes.");
            if (string.IsNullOrEmpty(password))
                throw new CriticalError("Password cannot be null or empty.");
            if (timeCost <= 0)
                throw new CriticalError("Time cost must be greater than zero.");
            if (memoryCostKB <= 0)
                throw new CriticalError("Memory cost must be greater than zero.");
            if (parallelism <= 0)
                throw new CriticalError("Parallelism must be greater than zero.");

            try
            {
                var config = new Argon2Config
                {
                    Password = System.Text.Encoding.UTF8.GetBytes(password),
                    Salt = salt,
                    TimeCost = timeCost,
                    MemoryCost = memoryCostKB,
                    Lanes = parallelism,
                    Threads = parallelism,
                    HashLength = SYMMETRIC_KEY_LENGTH,
                    Type = Argon2Type.DataIndependentAddressing
                };

                using (var argon2 = new Argon2(config))
                {
                    var hash = argon2.Hash();
                    if (hash == null || hash.Buffer == null || hash.Buffer.Length != SYMMETRIC_KEY_LENGTH)
                        throw new CriticalError($"Derived an invalid length key from password ({hash?.Buffer?.Length ?? 0} bytes).");

                    return hash.Buffer;
                }
            }
            catch (Exception ex)
            {
                throw new CriticalError($"Argon2 key derivation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate a new X448 private key (56 bytes).
        /// </summary>
        public byte[] GenerateX448PrivateKey()
        {
            var random = new SecureRandom();
            var privateKey = new X448PrivateKeyParameters(random);
            var encoded = privateKey.GetEncoded();

            if (encoded == null || encoded.Length != 56)
                throw new CriticalError($"Generated invalid private key length: {(encoded?.Length ?? 0)}");

            return encoded;
        }

        /// <summary>
        /// Derive public key from X448 private key bytes.
        /// </summary>
        /// <param name="privateKeyBytes">56-byte private key.</param>
        /// <returns>56-byte public key.</returns>
        public byte[] DeriveX448PublicKey(byte[] privateKeyBytes)
        {
            if (privateKeyBytes == null) throw new CriticalError("Private key cannot be null.");
            if (privateKeyBytes.Length != ONIONLINK_PRIVATE_KEY_LENGTH)
                throw new CriticalError($"Private key must be {ONIONLINK_PRIVATE_KEY_LENGTH} bytes.");

            try
            {
                var privateKey = new X448PrivateKeyParameters(privateKeyBytes, 0);
                var publicKey = privateKey.GeneratePublicKey();
                var pubBytes = publicKey.GetEncoded();

                if (pubBytes.Length != ONIONLINK_PUBLIC_KEY_LENGTH)
                    throw new CriticalError($"Generated public key has invalid length {pubBytes.Length}.");

                return pubBytes;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                throw new CriticalError($"Failed to derive public key: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculate the shared secret from private key and peer public key.
        /// </summary>
        /// <param name="privateKeyBytes">Your private key (56 bytes).</param>
        /// <param name="peerPublicKeyBytes">Peer public key (56 bytes).</param>
        /// <returns>Shared secret (56 bytes).</returns>
        public byte[] CalculateX448SharedSecret(byte[] privateKeyBytes, byte[] peerPublicKeyBytes)
        {
            if (privateKeyBytes == null || peerPublicKeyBytes == null)
                throw new CriticalError("Keys cannot be null.");

            if (privateKeyBytes.Length != ONIONLINK_PRIVATE_KEY_LENGTH)
                throw new CriticalError($"Private key must be {ONIONLINK_PRIVATE_KEY_LENGTH} bytes.");

            if (peerPublicKeyBytes.Length != ONIONLINK_PUBLIC_KEY_LENGTH)
                throw new CriticalError($"Public key must be {ONIONLINK_PUBLIC_KEY_LENGTH} bytes.");

            try
            {
                var privateKey = new X448PrivateKeyParameters(privateKeyBytes, 0);
                var publicKey = new X448PublicKeyParameters(peerPublicKeyBytes, 0);
                var agreement = new X448Agreement();
                agreement.Init(privateKey);

                var sharedSecret = new byte[agreement.AgreementSize];
                agreement.CalculateAgreement(publicKey, sharedSecret, 0);

                if (sharedSecret.Length != ONIONLINK_PUBLIC_KEY_LENGTH)
                    throw new CriticalError($"Shared secret length invalid: {sharedSecret.Length} bytes.");

                return sharedSecret;
            }
            catch (InvalidOperationException ex)
            {
                throw new CriticalError($"X448 agreement failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get the cryptographically strong shared key by hashing the raw shared secret with Blake2b.
        /// </summary>
        public byte[] GetX448SharedKey(byte[] rawSharedSecret)
        {
            if (rawSharedSecret == null)
                throw new CriticalError("Raw shared secret cannot be null.");

            if (rawSharedSecret.Length != X448_SHARED_SECRET_LENGTH)
                throw new CriticalError($"Raw shared secret must be {X448_SHARED_SECRET_LENGTH} bytes.");

            // Use your existing Blake2b method with desired digest size
            return Blake2b(rawSharedSecret, digestSize: SYMMETRIC_KEY_LENGTH);
        }

        /// <summary>
        /// Derives message keys, header keys, and fingerprints from the shared key and public keys.
        /// </summary>
        /// <param name="dhSharedKey">The shared key from X448.</param>
        /// <param name="tfcPublicKeyUser">User's public key.</param>
        /// <param name="tfcPublicKeyContact">Contact's public key.</param>
        /// <returns>Tuple of keys: (tx_mk, rx_mk, tx_hk, rx_hk, tx_fp, rx_fp)</returns>
        public (byte[] tx_mk, byte[] rx_mk, byte[] tx_hk, byte[] rx_hk, byte[] tx_fp, byte[] rx_fp) DeriveSubkeys(
            byte[] dhSharedKey,
            byte[] tfcPublicKeyUser,
            byte[] tfcPublicKeyContact)
        {
            if (dhSharedKey == null || tfcPublicKeyUser == null || tfcPublicKeyContact == null)
                throw new CriticalError("Keys cannot be null.");

            byte[] tx_mk = Blake2b(tfcPublicKeyContact, dhSharedKey, personalization: MESSAGE_KEY, digestSize: SYMMETRIC_KEY_LENGTH);
            byte[] rx_mk = Blake2b(tfcPublicKeyUser, dhSharedKey, personalization: MESSAGE_KEY, digestSize: SYMMETRIC_KEY_LENGTH);

            byte[] tx_hk = Blake2b(tfcPublicKeyContact, dhSharedKey, personalization: HEADER_KEY, digestSize: SYMMETRIC_KEY_LENGTH);
            byte[] rx_hk = Blake2b(tfcPublicKeyUser, dhSharedKey, personalization: HEADER_KEY, digestSize: SYMMETRIC_KEY_LENGTH);

            byte[] tx_fp = Blake2b(tfcPublicKeyUser, dhSharedKey, personalization: FINGERPRINT, digestSize: FINGERPRINT_LENGTH);
            byte[] rx_fp = Blake2b(tfcPublicKeyContact, dhSharedKey, personalization: FINGERPRINT, digestSize: FINGERPRINT_LENGTH);

            var keyTuple = new List<byte[]> { tx_mk, rx_mk, tx_hk, rx_hk, tx_fp, rx_fp };

            if (keyTuple.Distinct(new ByteArrayComparer()).Count() != keyTuple.Count)
                throw new CriticalError("Derived subkeys were not unique.");

            return (tx_mk, rx_mk, tx_hk, rx_hk, tx_fp, rx_fp);
        }

        // Helper class to compare byte arrays for equality
        private class ByteArrayComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[]? x, byte[]? y)
            {
                if (x == null || y == null) return false;
                return x.SequenceEqual(y);
            }

            public int GetHashCode(byte[] obj)
            {
                if (obj == null) throw new ArgumentNullException(nameof(obj));
                // Simple hashcode from first 4 bytes (not great but sufficient here)
                int hash = 17;
                for (int i = 0; i < Math.Min(4, obj.Length); i++)
                    hash = hash * 31 + obj[i];
                return hash;
            }
        }

        /// <summary>
        /// Encrypts plaintext with XChaCha20-Poly1305 and returns nonce + ciphertext + tag concatenated.
        /// </summary>
        /// <param name="plaintext">Plaintext bytes to encrypt</param>
        /// <param name="key">32-byte symmetric key</param>
        /// <param name="associatedData">Optional associated data (AD)</param>
        /// <returns>nonce + ciphertext + tag</returns>
        public byte[] EncryptAndSign(
            byte[] plaintext, 
            byte[] key, 
            byte[] associatedData = null)
        {
            if (key == null || key.Length != SYMMETRIC_KEY_LENGTH)
                throw new CriticalError($"Invalid key length ({key?.Length ?? 0} bytes). Expected {SYMMETRIC_KEY_LENGTH}.");

            associatedData ??= Array.Empty<byte>();

            // Generate secure random nonce of 24 bytes
            var nonce = Sodium.SodiumCore.GetRandomBytes(XCHACHA20_NONCE_LENGTH);

            try
            {
                // Encrypt: ciphertext includes the Poly1305 tag appended automatically
                var ciphertext = SecretAeadChaCha20Poly1305IETF.Encrypt(plaintext, associatedData, nonce, key);

                // Return nonce + ciphertext concatenated
                var result = new byte[nonce.Length + ciphertext.Length];
                Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
                Buffer.BlockCopy(ciphertext, 0, result, nonce.Length, ciphertext.Length);

                return result;
            }
            catch (Exception ex)
            {
                throw new CriticalError($"Encryption failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Authenticates and decrypts ciphertext encrypted with XChaCha20-Poly1305.
        /// </summary>
        /// <param name="nonceCtTag">
        /// Concatenated byte array containing the nonce, ciphertext, and authentication tag.
        /// Must be at least <see cref="XCHACHA20_NONCE_LENGTH"/> + 16 bytes in length.
        /// </param>
        /// <param name="key">
        /// The 32-byte symmetric key used for decryption and authentication.
        /// </param>
        /// <param name="database">
        /// Optional database identifier. If provided and authentication fails, throws a <see cref="CriticalError"/> 
        /// indicating which database's data failed authentication.
        /// </param>
        /// <param name="associatedData">
        /// Optional associated data (AD) that was authenticated but not encrypted.
        /// </param>
        /// <returns>
        /// The decrypted plaintext bytes if authentication succeeds.
        /// </returns>
        /// <exception cref="CriticalError">
        /// Thrown if the key length is invalid, the ciphertext length is invalid,
        /// or if authentication fails when <paramref name="database"/> is provided.
        /// </exception>
        /// <exception cref="CryptoException">
        /// Thrown if authentication fails and no <paramref name="database"/> is provided.
        /// </exception>

        public byte[] AuthAndDecrypt(
            byte[] nonceCtTag, 
            byte[] key, 
            string? database = null, 
            byte[]? associatedData = null)
        {
            if (key == null || key.Length != SYMMETRIC_KEY_LENGTH)
                throw new CriticalError($"Invalid key length ({key?.Length ?? 0} bytes).");

            if (nonceCtTag == null || nonceCtTag.Length < XCHACHA20_NONCE_LENGTH + 16) // 16 = Poly1305 tag size
                throw new CriticalError("Invalid ciphertext length.");

            associatedData ??= Array.Empty<byte>();

            var (nonce, ctTag) = SeparateHeader(nonceCtTag, XCHACHA20_NONCE_LENGTH);

            try
            {
                return SecretAeadChaCha20Poly1305IETF.Decrypt(ctTag, associatedData, nonce, key);
            }
            catch (CryptoException)
            {
                if (!string.IsNullOrEmpty(database))
                    throw new CriticalError($"Authentication of data in database '{database}' failed.");

                throw;
            }
        }

        /// <summary>
        /// Splits the input array into nonce and ciphertext+tag parts.
        /// </summary>
        private (byte[] nonce, byte[] ctTag) SeparateHeader(
            byte[] input, 
            int nonceLength)
        {
            var nonce = new byte[nonceLength];
            var ctTagLength = input.Length - nonceLength;
            var ctTag = new byte[ctTagLength];

            Array.Copy(input, 0, nonce, 0, nonceLength);
            Array.Copy(input, nonceLength, ctTag, 0, ctTagLength);

            return (nonce, ctTag);
        }

        /// <summary>
        /// Pads a bytestring to the next multiple of 255 bytes using PKCS7.
        /// </summary>
        public byte[] BytePadding(
            byte[] bytestring)
        {
            if (bytestring == null) throw new ArgumentNullException(nameof(bytestring));

            int blockSize = PADDING_LENGTH; // bytes
            int paddingRequired = blockSize - (bytestring.Length % blockSize);
            if (paddingRequired == 0) paddingRequired = blockSize;

            byte padValue = (byte)paddingRequired;
            byte[] padded = new byte[bytestring.Length + paddingRequired];
            Buffer.BlockCopy(bytestring, 0, padded, 0, bytestring.Length);
            for (int i = bytestring.Length; i < padded.Length; i++)
                padded[i] = padValue;

            if (padded.Length % PADDING_LENGTH != 0)
                throw new CriticalError($"Padded message had an invalid length ({padded.Length}).");

            return padded;
        }

        /// <summary>
        /// Removes PKCS7 padding from a padded bytestring.
        /// </summary>
        public byte[] RemovePaddingBytes(
            byte[] paddedBytestring)
        {
            if (paddedBytestring == null) throw new ArgumentNullException(nameof(paddedBytestring));
            if (paddedBytestring.Length == 0) return Array.Empty<byte>();

            byte padValue = paddedBytestring[^1];
            if (padValue < 1 || padValue > PADDING_LENGTH)
                throw new CriticalError("Invalid padding value.");

            for (int i = paddedBytestring.Length - padValue; i < paddedBytestring.Length; i++)
            {
                if (paddedBytestring[i] != padValue)
                    throw new CriticalError("Invalid padding bytes.");
            }

            byte[] unpadded = new byte[paddedBytestring.Length - padValue];
            Buffer.BlockCopy(paddedBytestring, 0, unpadded, 0, unpadded.Length);

            return unpadded;
        }

        /// <summary>
        /// Generate cryptographically secure random bytes of specified length,
        /// then compress using Blake2b hash of that length.
        /// </summary>
        public byte[] Csprng(
            int keyLength = SYMMETRIC_KEY_LENGTH)
        {
            if (keyLength < BLAKE2_DIGEST_LENGTH_MIN || keyLength > BLAKE2_DIGEST_LENGTH_MAX)
                throw new CriticalError($"Invalid key size ({keyLength} bytes).");

            byte[] entropy = new byte[keyLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(entropy);
            }

            if (entropy.Length != keyLength)
                throw new CriticalError($"Generated entropy length mismatch ({entropy.Length} bytes).");

            // Use your existing Blake2b method to compress the entropy
            byte[] compressed = Blake2b(entropy, digestSize: keyLength);

            return compressed;
        }
    }
}
