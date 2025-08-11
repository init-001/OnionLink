using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.Exceptions;

namespace TfcSettings
{


    public class Settings
    {
        // Constants (set these accordingly)
        private const int ENCODED_BOOLEAN_LENGTH = 1;
        private const int ENCODED_INTEGER_LENGTH = 4;
        private const int ENCODED_FLOAT_LENGTH = 8;
        private const int MAX_INT = int.MaxValue;
        private const string DIR_USER_DATA = "user_data/";
        private const string TX = "Tx";

        // Settings properties (defaults as per Python code)
        public bool DisableGuiDialog { get; set; } = false;
        public int MaxNumberOfGroupMembers { get; set; } = 50;
        public int MaxNumberOfGroups { get; set; } = 50;
        public int MaxNumberOfContacts { get; set; } = 50;
        public bool LogMessagesByDefault { get; set; } = false;
        public bool AcceptFilesByDefault { get; set; } = false;
        public bool ShowNotificationsByDefault { get; set; } = true;
        public bool LogFileMasking { get; set; } = false;
        public bool AskPasswordForLogAccess { get; set; } = true;

        public bool NcBypassMessages { get; set; } = false;
        public bool ConfirmSentFiles { get; set; } = true;
        public bool DoubleSpaceExits { get; set; } = false;
        public bool TrafficMasking { get; set; } = false;
        public double TmStaticDelay { get; set; } = 2.0;
        public double TmRandomDelay { get; set; } = 2.0;

        public bool AllowContactRequests { get; set; } = true;

        public bool NewMessageNotifyPreview { get; set; } = false;
        public double NewMessageNotifyDuration { get; set; } = 1.0;
        public int MaxDecompressSize { get; set; } = 100_000_000;

        // Other fields
        private string fileName;
        private OnionLinkDatabase database;
        private string softwareOperation;

        // Store defaults for reference
        private readonly Dictionary<string, object> defaults;

        public Settings(MasterKey masterKey, string operation, bool localTest, bool qubes = false)
        {
            softwareOperation = operation;
            fileName = Path.Combine(DIR_USER_DATA, $"{operation}_settings");

            // Make sure dir exists
            Directory.CreateDirectory(DIR_USER_DATA);

            database = new OnionLinkDatabase(fileName, masterKey);

            // Capture defaults (property names and values)
            defaults = this.GetType()
                           .GetProperties()
                           .Where(p => p.CanRead && p.CanWrite)
                           .ToDictionary(p => p.Name, p => p.GetValue(this));

            if (File.Exists(fileName))
            {
                LoadSettings();
            }
            else
            {
                StoreSettings();
            }
        }

        public void StoreSettings()
        {
            var bytesList = new List<byte>();

            foreach (var prop in defaults.Keys)
            {
                var val = this.GetType().GetProperty(prop).GetValue(this);
                if (val is bool b)
                    bytesList.AddRange(BoolToBytes(b));
                else if (val is int i)
                    bytesList.AddRange(IntToBytes(i));
                else if (val is double d)
                    bytesList.AddRange(DoubleToBytes(d));
                else
                    throw new CriticalError($"Invalid attribute type in settings: {prop}");
            }

            database.StoreDatabase(bytesList.ToArray(), replace: true);
        }

        public void LoadSettings()
        {
            var ptBytes = database.LoadDatabase();
            int offset = 0;

            foreach (var prop in defaults.Keys)
            {
                var property = this.GetType().GetProperty(prop);
                var type = property.PropertyType;

                if (type == typeof(bool))
                {
                    property.SetValue(this, BytesToBool(ptBytes, offset));
                    offset += ENCODED_BOOLEAN_LENGTH;
                }
                else if (type == typeof(int))
                {
                    property.SetValue(this, BytesToInt(ptBytes, offset));
                    offset += ENCODED_INTEGER_LENGTH;
                }
                else if (type == typeof(double))
                {
                    property.SetValue(this, BytesToDouble(ptBytes, offset));
                    offset += ENCODED_FLOAT_LENGTH;
                }
                else
                {
                    throw new CriticalError("Invalid data type in settings default values.");
                }
            }
        }

        public void ChangeSetting(string key, string valueStr,
                                  ContactList contactList, GroupList groupList)
        {
            var property = this.GetType().GetProperty(key);
            if (property == null)
                throw new SoftError($"Invalid setting key '{key}'");

            object value;

            try
            {
                if (property.PropertyType == typeof(bool))
                {
                    if (valueStr.Equals("true", StringComparison.OrdinalIgnoreCase)) value = true;
                    else if (valueStr.Equals("false", StringComparison.OrdinalIgnoreCase)) value = false;
                    else throw new SoftError($"Invalid boolean value '{valueStr}'");
                }
                else if (property.PropertyType == typeof(int))
                {
                    int iv = int.Parse(valueStr);
                    if (iv < 0 || iv > MAX_INT) throw new SoftError($"Integer value out of range: {iv}");
                    value = iv;
                }
                else if (property.PropertyType == typeof(double))
                {
                    double dv = double.Parse(valueStr);
                    if (dv < 0.0) throw new SoftError("Float value must be >= 0.0");
                    value = dv;
                }
                else
                {
                    throw new CriticalError("Invalid attribute type in settings.");
                }
            }
            catch (FormatException)
            {
                throw new SoftError($"Error: Invalid setting value '{valueStr}'.");
            }

            ValidateKeyValuePair(key, value, contactList, groupList);

            property.SetValue(this, value);
            StoreSettings();
        }

        public static void ValidateKeyValuePair(string key, object value,
                                                ContactList contactList,
                                                GroupList groupList)
        {
            ValidateDatabaseLimit(key, value);
            ValidateMaxNumberOfGroupMembers(key, value, groupList);
            ValidateMaxNumberOfGroups(key, value, groupList);
            ValidateMaxNumberOfContacts(key, value, contactList);
            ValidateNewMessageNotifyDuration(key, value);
            ValidateTrafficMaskingDelay(key, value, contactList);
        }

        public static void ValidateDatabaseLimit(string key, object value)
        {
            if (key == "MaxNumberOfGroupMembers" ||
                key == "MaxNumberOfGroups" ||
                key == "MaxNumberOfContacts")
            {
                if ((int)value % 10 != 0 || (int)value == 0)
                    throw new SoftError("Database padding settings must be divisible by 10.");
            }
        }

        public static void ValidateMaxNumberOfGroupMembers(string key, object value, GroupList groupList)
        {
            if (key == "MaxNumberOfGroupMembers")
            {
                int minSize = RoundUp(groupList.LargestGroup());
                if ((int)value < minSize)
                    throw new SoftError($"Can't set the max number of members lower than {minSize}.");
            }
        }

        public static void ValidateMaxNumberOfGroups(string key, object value, GroupList groupList)
        {
            if (key == "MaxNumberOfGroups")
            {
                int minSize = RoundUp(groupList.Count);
                if ((int)value < minSize)
                    throw new SoftError($"Can't set the max number of groups lower than {minSize}.");
            }
        }

        public static void ValidateMaxNumberOfContacts(string key, object value, ContactList contactList)
        {
            if (key == "MaxNumberOfContacts")
            {
                int minSize = RoundUp(contactList.Count);
                if ((int)value < minSize)
                    throw new SoftError($"Can't set the max number of contacts lower than {minSize}.");
            }
        }

        public static void ValidateNewMessageNotifyDuration(string key, object value)
        {
            if (key == "NewMessageNotifyDuration" && (double)value < 0.05)
                throw new SoftError("Too small value for message notify duration.");
        }

        public static void ValidateTrafficMaskingDelay(string key, object value, ContactList contactList)
        {
            const double TRAFFIC_MASKING_MIN_STATIC_DELAY = 0.5;
            const double TRAFFIC_MASKING_MIN_RANDOM_DELAY = 0.5;

            if (key == "TmStaticDelay" && (double)value < TRAFFIC_MASKING_MIN_STATIC_DELAY)
                throw new SoftError($"Can't set static delay lower than {TRAFFIC_MASKING_MIN_STATIC_DELAY}.");

            if (key == "TmRandomDelay" && (double)value < TRAFFIC_MASKING_MIN_RANDOM_DELAY)
                throw new SoftError($"Can't set random delay lower than {TRAFFIC_MASKING_MIN_RANDOM_DELAY}.");

            // Warning for TX mode (assuming contactList.Settings.SoftwareOperation available)
            if ((key == "TmStaticDelay" || key == "TmRandomDelay") &&
                contactList.Settings.softwareOperation == TX)
            {
                Console.WriteLine("WARNING! Changing traffic masking delay can make your endpoint and traffic look unique!");

                Console.Write("Proceed anyway? (y/n): ");
                var input = Console.ReadLine();
                if (input == null || !input.Equals("y", StringComparison.OrdinalIgnoreCase))
                    throw new SoftError("Aborted traffic masking setting change.");
            }
        }

        // Helpers for rounding
        private static int RoundUp(int value)
        {
            return ((value + 9) / 10) * 10;
        }

        // Byte conversion helpers
        private static byte[] BoolToBytes(bool val)
        {
            return new byte[] { val ? (byte)1 : (byte)0 };
        }

        private static bool BytesToBool(byte[] data, int offset)
        {
            return data[offset] != 0;
        }

        private static byte[] IntToBytes(int val)
        {
            return BitConverter.GetBytes(val);
        }

        private static int BytesToInt(byte[] data, int offset)
        {
            return BitConverter.ToInt32(data, offset);
        }

        private static byte[] DoubleToBytes(double val)
        {
            return BitConverter.GetBytes(val);
        }

        private static double BytesToDouble(byte[] data, int offset)
        {
            return BitConverter.ToDouble(data, offset);
        }

        // Add other methods like PrintSettings() if needed...
    }

    // Dummy classes to satisfy references, replace with your actual implementations
    public class OnionLinkDatabase
    {
        private string fileName;
        private MasterKey masterKey;

        public OnionLinkDatabase(string fileName, MasterKey masterKey)
        {
            this.fileName = fileName;
            this.masterKey = masterKey;
        }

        public void StoreDatabase(byte[] data, bool replace)
        {
            // Encrypt and save data to fileName
            File.WriteAllBytes(fileName, data); // Replace with encryption
        }

        public byte[] LoadDatabase()
        {
            // Load and decrypt data from fileName
            return File.ReadAllBytes(fileName); // Replace with decryption
        }
    }

    public class MasterKey
    {
        // Your MasterKey implementation here
    }

    public class ContactList
    {
        public int Count => 0;  // Replace with real count
        public Settings Settings { get; set; }
    }

    public class GroupList
    {
        public int Count => 0; // Replace with real count
        public int LargestGroup() => 0; // Replace with real value
    }
}
