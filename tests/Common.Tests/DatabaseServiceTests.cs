using System;
using System.IO;
using System.Linq;
using System.Text;
using Common.DatabaseService;
using Common.Statics;
using Common.Exceptions;
using Xunit;
using static Common.DatabaseService.Utils;
using static Common.DatabaseService.Utils.MasterKey;

namespace Crypto.Tests;
public class OnionChatDatabaseTests : IDisposable
{
    private readonly string testDbPath = "testlog.db";
    private readonly string testTempPath;
    private readonly MasterKey masterKey;
    private readonly OnionChatDatabase db;

    public OnionChatDatabaseTests()
    {
        testTempPath = testDbPath + Constants.TEMP_SUFFIX;
        var key = new byte[32];
        new Random().NextBytes(key);
        masterKey = new MasterKey(key);
        db = new OnionChatDatabase(testDbPath, masterKey);
    }

    [Fact]
    public void StoreAndLoadDatabase_ShouldRoundtrip()
    {
        var plaintext = Encoding.UTF8.GetBytes("Hello encrypted world!");

        db.StoreDatabase(plaintext);
        var loaded = db.LoadDatabase();

        Assert.Equal(plaintext, loaded);
    }

    [Fact]
    public void EnsureTempWrite_WritesValidEncryptedFile()
    {
        var data = Encoding.UTF8.GetBytes("Some test data");
        var encrypted = new Common.Crypto.CryptoService().EncryptAndSign(data, masterKey.MasterKeyBytes);

        db.EnsureTempWrite(encrypted);

        Assert.True(File.Exists(testTempPath));
        Assert.True(db.VerifyFile(testTempPath));
    }

    [Fact]
    public void ReplaceDatabase_ShouldReplaceFile()
    {
        File.WriteAllText(testTempPath, "temp data");
        File.WriteAllText(testDbPath, "old data");

        db.ReplaceDatabase();

        Assert.False(File.Exists(testTempPath));
        Assert.True(File.Exists(testDbPath));
        Assert.Equal("temp data", File.ReadAllText(testDbPath));
    }

    public void Dispose()
    {
        if (File.Exists(testDbPath))
            File.Delete(testDbPath);
        if (File.Exists(testTempPath))
            File.Delete(testTempPath);
    }
}

public class OnionChatUnencryptedDatabaseTests : IDisposable
{
    private readonly string testDbPath = "testlog.db";
    private readonly string testTempPath;
    private readonly TFCUnencryptedDatabase db;

    public OnionChatUnencryptedDatabaseTests()
    {
        testTempPath = testDbPath + Constants.TEMP_SUFFIX;
        db = new TFCUnencryptedDatabase(testDbPath);
    }

    [Fact]
    public void StoreAndLoadDatabase_ShouldRoundtrip()
    {
        var data = Encoding.UTF8.GetBytes("Unencrypted test data");
        db.StoreUnencryptedDatabase(data);

        var loaded = db.LoadDatabase();
        Assert.Equal(data, loaded);
    }

    [Fact]
    public void VerifyFile_ReturnsFalseForInvalidFile()
    {
        File.WriteAllText(testDbPath, "invalid content");
        Assert.False(db.VerifyFile(testDbPath));
    }

    public void Dispose()
    {
        if (File.Exists(testDbPath))
            File.Delete(testDbPath);
        if (File.Exists(testTempPath))
            File.Delete(testTempPath);
    }
}

public class MessageLogTests : IDisposable
{
    private readonly string testDbPath = "testlog.db";
    private readonly byte[] key;
    private readonly MessageLog log;

    public MessageLogTests()
    {
        key = new byte[32];
        new Random().NextBytes(key);
        log = new MessageLog(testDbPath, key);
    }

    [Fact]
    public void InsertLogEntry_And_GetDecryptedLogEntries_ShouldMatch()
    {
        var entry1 = Encoding.UTF8.GetBytes("First log entry");
        var entry2 = Encoding.UTF8.GetBytes("Second log entry");

        log.InsertLogEntry(entry1);
        log.InsertLogEntry(entry2);

        var entries = log.GetDecryptedLogEntries().ToList();

        Assert.Contains(entry1, entries);
        Assert.Contains(entry2, entries);
    }

    [Fact]
    public void VerifyFile_ReturnsTrueForValidLog()
    {
        var entry = Encoding.UTF8.GetBytes("Test log entry");
        log.InsertLogEntry(entry);

        Assert.True(log.VerifyFile(testDbPath));
    }

    public void Dispose()
    {
        log.Dispose();

        if (File.Exists(testDbPath))
            File.Delete(testDbPath);
        var tempFile = testDbPath + Constants.TEMP_SUFFIX;
        if (File.Exists(tempFile))
            File.Delete(tempFile);
    }
}
