using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using Common.Exceptions;
using Common.Crypto;
using static Common.Statics.Constants;


namespace Common.DatabaseService
{


    public static class Utils
    {
        // Ensure directory exists
        public static void EnsureDir(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        // Atomic file write with fsync
        public static void WriteToFile(string filename, byte[] data)
        {
            using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            fs.Write(data, 0, data.Length);
            fs.Flush(true); // flush and fsync
        }

        // Separate trailer from data (last N bytes = digest)
        public static (byte[] Data, byte[] Trailer) SeparateTrailer(byte[] input, int trailerLength)
        {
            if (input.Length < trailerLength)
                throw new ArgumentException("Input too short to separate trailer.");

            byte[] data = new byte[input.Length - trailerLength];
            byte[] trailer = new byte[trailerLength];
            Array.Copy(input, 0, data, 0, data.Length);
            Array.Copy(input, data.Length, trailer, 0, trailerLength);
            return (data, trailer);
        }



        // Placeholder for your MasterKey class
        public class MasterKey
        {
            public byte[] MasterKeyBytes { get; }
            public MasterKey(byte[] keyBytes)
            {
                MasterKeyBytes = keyBytes;
            }
        }

        // Placeholder for your encrypt_and_sign and auth_and_decrypt
        // Use Sodium.SecretBox or other authenticated encryption you prefer

        public class OnionLinkDatabase
        {
            private readonly CryptoService _crypto = new();
            private readonly string databaseName;
            private readonly string databaseTemp;
            private readonly byte[] databaseKey;

            public OnionLinkDatabase(string databaseName, MasterKey masterKey)
            {
                this.databaseName = databaseName;
                this.databaseTemp = databaseName + TEMP_SUFFIX;
                this.databaseKey = masterKey.MasterKeyBytes;
            }

            public void WriteToFile(string fileName, byte[] data)
            {
                Utils.WriteToFile(fileName, data);
            }

            public bool VerifyFile(string fileName)
            {
                try
                {
                    var fileData = File.ReadAllBytes(fileName);
                    _ = _crypto.AuthAndDecrypt(fileData, databaseKey);
                    return true;
                }
                catch (CryptographicException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public void EnsureTempWrite(byte[] ctBytes)
            {
                WriteToFile(databaseTemp, ctBytes);

                int retries = 0;
                while (!VerifyFile(databaseTemp))
                {
                    retries++;
                    if (retries >= DB_WRITE_RETRY_LIMIT)
                        throw new CriticalError($"Writing to database '{databaseTemp}' failed after {retries} retries.");

                    WriteToFile(databaseTemp, ctBytes);
                }
            }

            public void StoreDatabase(byte[] ptBytes, bool replace = true)
            {
                var ctBytes = _crypto.EncryptAndSign(ptBytes, databaseKey);
                Utils.EnsureDir(DIR_USER_DATA);
                EnsureTempWrite(ctBytes);

                if (replace)
                    ReplaceDatabase();
            }

            public void ReplaceDatabase()
            {
                File.Replace(databaseTemp, databaseName, null);
            }

            public byte[] LoadDatabase()
            {
                if (File.Exists(databaseTemp))
                {
                    if (VerifyFile(databaseTemp))
                        ReplaceDatabase();
                    else
                        File.Delete(databaseTemp);
                }

                var databaseData = File.ReadAllBytes(databaseName);
                return _crypto.AuthAndDecrypt(databaseData, databaseKey);
            }
        }

        public class TFCUnencryptedDatabase
        {
            private readonly CryptoService _crypto = new();
            private readonly string databaseName;
            private readonly string databaseTemp;

            public TFCUnencryptedDatabase(string databaseName)
            {
                this.databaseName = databaseName;
                this.databaseTemp = databaseName + TEMP_SUFFIX;
            }

            public void WriteToFile(string fileName, byte[] data)
            {
                Utils.WriteToFile(fileName, data);
            }

            public bool VerifyFile(string fileName)
            {
                var fileData = File.ReadAllBytes(fileName);
                var (purpData, digest) = Utils.SeparateTrailer(fileData, BLAKE2_DIGEST_LENGTH);
                var computedDigest = _crypto.Blake2b(purpData);
                return ByteArrayEquals(computedDigest, digest);
            }

            private bool ByteArrayEquals(byte[] a1, byte[] a2)
            {
                if (a1.Length != a2.Length) return false;
                for (int i = 0; i < a1.Length; i++)
                    if (a1[i] != a2[i]) return false;
                return true;
            }

            public void EnsureTempWrite(byte[] data)
            {
                WriteToFile(databaseTemp, data);

                int retries = 0;
                while (!VerifyFile(databaseTemp))
                {
                    retries++;
                    if (retries >= DB_WRITE_RETRY_LIMIT)
                        throw new CriticalError($"Writing to database '{databaseTemp}' failed after {retries} retries.");

                    WriteToFile(databaseTemp, data);
                }
            }

            public void StoreUnencryptedDatabase(byte[] data)
            {
                EnsureDir(DIR_USER_DATA);
                var dataWithDigest = new byte[data.Length + BLAKE2_DIGEST_LENGTH];
                Array.Copy(data, 0, dataWithDigest, 0, data.Length);
                Array.Copy(_crypto.Blake2b(data), 0, dataWithDigest, data.Length, BLAKE2_DIGEST_LENGTH);

                EnsureTempWrite(dataWithDigest);
                File.Replace(databaseTemp, databaseName, null);
            }

            public void ReplaceDatabase()
            {
                if (File.Exists(databaseTemp))
                    File.Replace(databaseTemp, databaseName, null);
            }

            public byte[] LoadDatabase()
            {
                if (File.Exists(databaseTemp))
                {
                    if (VerifyFile(databaseTemp))
                        File.Replace(databaseTemp, databaseName, null);
                    else
                        File.Delete(databaseTemp);
                }

                var databaseData = File.ReadAllBytes(databaseName);
                var (data, digest) = Utils.SeparateTrailer(databaseData, BLAKE2_DIGEST_LENGTH);

                if (!ByteArrayEquals(_crypto.Blake2b(data), digest))
                    throw new CriticalError($"Invalid data in login database {databaseName}");

                return data;
            }
        }

        public class MessageLog : IDisposable
        {
            private readonly CryptoService _crypto = new();
            private readonly string databaseName;
            private readonly string databaseTemp;
            private readonly byte[] databaseKey;
            private SqliteConnection connection;
            private SqliteCommand command;

            public MessageLog(string databaseName, byte[] databaseKey)
            {
                this.databaseName = databaseName;
                this.databaseTemp = databaseName + TEMP_SUFFIX;
                this.databaseKey = databaseKey;

                Utils.EnsureDir(DIR_USER_DATA);

                if (File.Exists(databaseName))
                    CheckForTempDatabase();

                connection = new SqliteConnection($"Data Source={databaseName};Version=3;");
                connection.Open();

                command = connection.CreateCommand();

                CreateTable();
            }

            private void CreateTable()
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS log_entries (id INTEGER PRIMARY KEY, log_entry BLOB NOT NULL)";
                command.ExecuteNonQuery();
            }

            public void InsertLogEntry(byte[] ptLogEntry)
            {
                var ctLogEntry = _crypto.EncryptAndSign(ptLogEntry, databaseKey);

                try
                {
                    using var insertCmd = connection.CreateCommand();
                    insertCmd.CommandText = "INSERT INTO log_entries (log_entry) VALUES (@log_entry)";
                    insertCmd.Parameters.AddWithValue("@log_entry", ctLogEntry);
                    insertCmd.ExecuteNonQuery();
                }
                catch (SqliteException)
                {
                    // Reconnect and retry
                    connection?.Close();
                    connection = new SqliteConnection($"Data Source={databaseName};Version=3;");
                    connection.Open();
                    InsertLogEntry(ptLogEntry);
                }
            }

            public bool VerifyFile(string fileName)
            {
                try
                {
                    using var conn = new SqliteConnection($"Data Source={fileName};Version=3;");
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT log_entry FROM log_entries";
                    using var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var ctLogEntry = (byte[])reader["log_entry"];
                        try
                        {
                            _ = _crypto.AuthAndDecrypt(ctLogEntry, databaseKey);
                        }
                        catch (CryptographicException)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                catch (SqliteException)
                {
                    return false;
                }
            }

            public void CheckForTempDatabase()
            {
                if (File.Exists(databaseTemp))
                {
                    if (VerifyFile(databaseTemp))
                        ReplaceDatabase();
                    else
                        File.Delete(databaseTemp);
                }
            }

            public void ReplaceDatabase()
            {
                if (File.Exists(databaseTemp))
                    File.Replace(databaseTemp, databaseName, null);
            }

            public System.Collections.Generic.IEnumerable<byte[]> GetDecryptedLogEntries()
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT log_entry FROM log_entries";
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var ctLogEntry = (byte[])reader["log_entry"];
                    yield return _crypto.AuthAndDecrypt(ctLogEntry, databaseKey);
                }
            }

            public void Dispose()
            {
                command?.Dispose();
                connection?.Close();
                connection?.Dispose();
            }
        }
    }
}

