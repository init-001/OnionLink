using System.Text;

namespace Common.Statics
{
    /// <summary>
    /// Contains constant values used throughout the application.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// OnionLink
        /// </summary>
        public const string OnionLink = "OnionLink";
        public const string OnionLinkVersion = "1.0";
        public const string OnionLinkDescription = "An endpoint secure chat application using Tor for anonymity.";
        public const string OnionLinkAuthor = "init-001";
        public const string OnionLinkLicense = "MIT License";
        public const string OnionLinkLicenseUrl = "https://opensource.org/license/mit/";
        public const string OnionLinkRepositoryUrl = "";
        public const string OnionLinkDocumentationUrl = "";
        
        /// <summary>
        /// Identifiers (some are byte arrays)
        /// </summary>
        public static readonly string LOCAL_ID = "localidlocalidlocalidlocalidlocalidlocalidlocalidloj7uyd";
        public static readonly byte[] LOCAL_PUBKEY = new byte[] { 0x5B, 0x84, 0x05, 0xA0, 0x6B, 0x70, 0x80, 0xB4, 0x0D, 0x6E, 0x10, 0x16, 0x81, 0xAD, 0xC2, 0x02, 0xD0, 0x35, 0xB8, 0x40, 0x5A, 0x06, 0xB7, 0x08, 0x0B, 0x40, 0xD6, 0xE1, 0x01, 0x68, 0x1A, 0xDC };
        public const string LOCAL_NICK = "local Source Computer";
        public const string DUMMY_CONTACT = "dummycontactdummycontactdummycontactdummycontactdumhsiid";
        public const string DUMMY_MEMBER = "dummymemberdummymemberdummymemberdummymemberdummymedakad";
        public const string DUMMY_NICK = "dummy_nick";
        public const string DUMMY_GROUP = "dummy_group";
        public const string TX = "tx";
        public const string RX = "rx";
        public const string NC = "nc";
        public const string TAILS = "NAME=\"Tails\"";

        /// <summary>
        /// Window identifiers
        /// </summary>
        public const string WIN_TYPE_COMMAND = "system messages";
        public const string WIN_TYPE_FILE = "incoming files";
        public const string WIN_TYPE_CONTACT = "contact";
        public const string WIN_TYPE_GROUP = "group";

        /// <summary>
        /// Window UIDs (byte arrays)
        /// </summary>
        public static readonly byte[] WIN_UID_COMMAND = Encoding.ASCII.GetBytes("win_uid_command");
        public static readonly byte[] WIN_UID_FILE = Encoding.ASCII.GetBytes("win_uid_file");

        /// <summary>
        /// Packet types
        /// </summary>
        public const string COMMAND = "command";
        public const string FILE = "file";
        public const string MESSAGE = "message";

        /// <summary>
        /// Group message IDs
        /// </summary>
        public const string NEW_GROUP = "new_group";
        public const string ADDED_MEMBERS = "added_members";
        public const string ALREADY_MEMBER = "already_member";
        public const string REMOVED_MEMBERS = "removed_members";
        public const string NOT_IN_GROUP = "not_in_group";
        public const string UNKNOWN_ACCOUNTS = "unknown_accounts";

        /// <summary>
        /// Base58 encoding
        /// </summary>
        public const string B58_ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        public static readonly byte[] MAINNET_HEADER = new byte[] { 0x80 };
        public static readonly byte[] TESTNET_HEADER = new byte[] { 0xEF };

        /// <summary>
        /// Base58 key types
        /// </summary>
        public const string B58_PUBLIC_KEY = "b58_public_key";
        public const string B58_LOCAL_KEY = "b58_local_key";

        /// <summary>
        /// Base58 key input guides
        /// </summary>
        public const string B58_PUBLIC_KEY_GUIDE = "   A       B       C       D       E       F       G       H       I       J       K       L   ";
        public const string B58_LOCAL_KEY_GUIDE = " A   B   C   D   E   F   G   H   I   J   K   L   M   N   O   P   Q ";

        /// <summary>
        /// Key exchange types
        /// </summary>
        public const string ECDHE = "X448";
        public const string PSK = "PSK";

        /// <summary>
        /// Contact setting types
        /// </summary>
        public const string LOGGING = "logging";
        public const string STORE = "store";
        public const string NOTIFY = "notify";

        /// <summary>
        /// Command identifiers
        /// </summary>
        public const string CLEAR = "clear";
        public const string RESET = "reset";
        public const string POWEROFF = "systemctl poweroff";
        public const string GENERATE = "generate";

        /// <summary>
        /// Contact setting management
        /// </summary>
        public const int CONTACT_SETTING_HEADER_LENGTH = 2;
        public static readonly byte[] ENABLE = Encoding.ASCII.GetBytes("es");
        public static readonly byte[] DISABLE = Encoding.ASCII.GetBytes("ds");
        public const string ALL = "all";

        /// <summary>
        /// Networked Computer bypass states
        /// </summary>
        public const string NC_BYPASS_START = "nc_bypass_start";
        public const string NC_BYPASS_STOP = "nc_bypass_stop";

        /// <summary>
        /// Status messages
        /// </summary>
        public const string DONE = "DONE";
        public const string EVENT = "-!-";
        public const string ME = "Me";

        /// <summary>
        /// Data diode simulator identifiers
        /// </summary>
        public const string IDLE = "Idle";
        public const string DATA_FLOW = "Data flow";
        public const string SCNCLR = "scnclr";
        public const string SCNCRL = "scncrl";
        public const string NCDCLR = "ncdclr";
        public const string NCDCRL = "ncdcrl";

        /// <summary>
        /// VT100 codes
        /// </summary>
        public const string CURSOR_UP_ONE_LINE = "\x1b[1A";
        public const string CURSOR_RIGHT_ONE_COLUMN = "\x1b[1C";
        public const string CLEAR_ENTIRE_LINE = "\x1b[2K";
        public const string CLEAR_ENTIRE_SCREEN = "\x1b[2J";
        public const string CURSOR_LEFT_UP_CORNER = "\x1b[H";
        public const string BOLD_ON = "\u001b[1m";    // \033 is octal, \u001b is hex for ESC
        public const string NORMAL_TEXT = "\u001b[0m";

        /// <summary>
        /// Separators
        /// </summary>
        public static readonly byte[] US_BYTE = new byte[] { 0x1F };

        /// <summary>
        /// Datagram headers (byte arrays)
        /// </summary>
        public static readonly byte[] LOCAL_KEY_DATAGRAM_HEADER = new byte[] { (byte)'L' };
        public static readonly byte[] PUBLIC_KEY_DATAGRAM_HEADER = new byte[] { (byte)'P' };
        public static readonly byte[] MESSAGE_DATAGRAM_HEADER = new byte[] { (byte)'M' };
        public static readonly byte[] COMMAND_DATAGRAM_HEADER = new byte[] { (byte)'K' };
        public static readonly byte[] FILE_DATAGRAM_HEADER = new byte[] { (byte)'F' };
        public static readonly byte[] UNENCRYPTED_DATAGRAM_HEADER = new byte[] { (byte)'U' };

        public const int DATAGRAM_TIMESTAMP_LENGTH = 8;
        public const int DATAGRAM_HEADER_LENGTH = 1;

        /// <summary>
        /// Group management headers
        /// </summary>
        public const int GROUP_ID_LENGTH = 4;
        public const int GROUP_ID_ENC_LENGTH = 13;
        public const int GROUP_MSG_ID_LENGTH = 16;
        public const int GROUP_MGMT_HEADER_LENGTH = 1;

        public static readonly byte[] GROUP_MSG_INVITE_HEADER = new byte[] { (byte)'I' };
        public static readonly byte[] GROUP_MSG_JOIN_HEADER = new byte[] { (byte)'J' };
        public static readonly byte[] GROUP_MSG_MEMBER_ADD_HEADER = new byte[] { (byte)'N' };
        public static readonly byte[] GROUP_MSG_MEMBER_REM_HEADER = new byte[] { (byte)'R' };
        public static readonly byte[] GROUP_MSG_EXIT_GROUP_HEADER = new byte[] { (byte)'X' };

        /// <summary>
        /// Assembly packet headers
        /// </summary>
        public const int FILE_PACKET_CTR_LENGTH = 8;
        public const int ASSEMBLY_PACKET_HEADER_LENGTH = 1;

        public static readonly byte[] M_S_HEADER = new byte[] { (byte)'a' };  // Short message packet
        public static readonly byte[] M_L_HEADER = new byte[] { (byte)'b' };  // First packet of multi-packet message
        public static readonly byte[] M_A_HEADER = new byte[] { (byte)'c' };  // Appended packet of multi-packet message
        public static readonly byte[] M_E_HEADER = new byte[] { (byte)'d' };  // Last packet of multi-packet message
        public static readonly byte[] M_C_HEADER = new byte[] { (byte)'e' };  // Cancelled multi-packet message
        public static readonly byte[] P_N_HEADER = new byte[] { (byte)'f' };  // Noise message packet

        public static readonly byte[] F_S_HEADER = new byte[] { (byte)'A' };  // Short file packet
        public static readonly byte[] F_L_HEADER = new byte[] { (byte)'B' };  // First packet of multi-packet file
        public static readonly byte[] F_A_HEADER = new byte[] { (byte)'C' };  // Appended packet of multi-packet file
        public static readonly byte[] F_E_HEADER = new byte[] { (byte)'D' };  // Last packet of multi-packet file
        public static readonly byte[] F_C_HEADER = new byte[] { (byte)'E' };  // Cancelled multi-packet file

        public static readonly byte[] C_S_HEADER = new byte[] { (byte)'0' };  // Short command packet
        public static readonly byte[] C_L_HEADER = new byte[] { (byte)'1' };  // First packet of multi-packet command
        public static readonly byte[] C_A_HEADER = new byte[] { (byte)'2' };  // Appended packet of multi-packet command
        public static readonly byte[] C_E_HEADER = new byte[] { (byte)'3' };  // Last packet of multi-packet command
        public static readonly byte[] C_C_HEADER = new byte[] { (byte)'4' };  // Cancelled multi-packet command (reserved but not in use)
        public static readonly byte[] C_N_HEADER = new byte[] { (byte)'5' };  // Noise command packet

        /// <summary>
        /// Unencrypted command headers
        /// </summary>
        public const int UNENCRYPTED_COMMAND_HEADER_LENGTH = 2;

        public static readonly byte[] UNENCRYPTED_SCREEN_CLEAR = Encoding.ASCII.GetBytes("UC");
        public static readonly byte[] UNENCRYPTED_SCREEN_RESET = Encoding.ASCII.GetBytes("UR");
        public static readonly byte[] UNENCRYPTED_EXIT_COMMAND = Encoding.ASCII.GetBytes("UX");
        public static readonly byte[] UNENCRYPTED_EC_RATIO = Encoding.ASCII.GetBytes("UE");
        public static readonly byte[] UNENCRYPTED_BAUDRATE = Encoding.ASCII.GetBytes("UB");
        public static readonly byte[] UNENCRYPTED_WIPE_COMMAND = Encoding.ASCII.GetBytes("UW");
        public static readonly byte[] UNENCRYPTED_ADD_NEW_CONTACT = Encoding.ASCII.GetBytes("UN");
        public static readonly byte[] UNENCRYPTED_ADD_EXISTING_CONTACT = Encoding.ASCII.GetBytes("UA");
        public static readonly byte[] UNENCRYPTED_REM_CONTACT = Encoding.ASCII.GetBytes("UD");
        public static readonly byte[] UNENCRYPTED_ONION_SERVICE_DATA = Encoding.ASCII.GetBytes("UO");
        public static readonly byte[] UNENCRYPTED_MANAGE_CONTACT_REQ = Encoding.ASCII.GetBytes("UM");
        public static readonly byte[] UNENCRYPTED_PUBKEY_CHECK = Encoding.ASCII.GetBytes("UP");
        public static readonly byte[] UNENCRYPTED_ACCOUNT_CHECK = Encoding.ASCII.GetBytes("UT");

        /// <summary>
        /// Encrypted command headers
        /// </summary>
        public const int ENCRYPTED_COMMAND_HEADER_LENGTH = 2;

        public static readonly byte[] LOCAL_KEY_RDY = Encoding.ASCII.GetBytes("LI");
        public static readonly byte[] WIN_ACTIVITY = Encoding.ASCII.GetBytes("SA");
        public static readonly byte[] WIN_SELECT = Encoding.ASCII.GetBytes("WS");
        public static readonly byte[] CLEAR_SCREEN = Encoding.ASCII.GetBytes("SC");
        public static readonly byte[] RESET_SCREEN = Encoding.ASCII.GetBytes("SR");
        public static readonly byte[] EXIT_PROGRAM = Encoding.ASCII.GetBytes("EX");
        public static readonly byte[] LOG_DISPLAY = Encoding.ASCII.GetBytes("LD");
        public static readonly byte[] LOG_EXPORT = Encoding.ASCII.GetBytes("LE");
        public static readonly byte[] LOG_REMOVE = Encoding.ASCII.GetBytes("LR");
        public static readonly byte[] CH_MASTER_KEY = Encoding.ASCII.GetBytes("MK");
        public static readonly byte[] CH_NICKNAME = Encoding.ASCII.GetBytes("NC");
        public static readonly byte[] CH_SETTING = Encoding.ASCII.GetBytes("CS");
        public static readonly byte[] CH_LOGGING = Encoding.ASCII.GetBytes("CL");
        public static readonly byte[] CH_FILE_RECV = Encoding.ASCII.GetBytes("CF");
        public static readonly byte[] CH_NOTIFY = Encoding.ASCII.GetBytes("CN");
        public static readonly byte[] GROUP_CREATE = Encoding.ASCII.GetBytes("GC");
        public static readonly byte[] GROUP_ADD = Encoding.ASCII.GetBytes("GA");
        public static readonly byte[] GROUP_REMOVE = Encoding.ASCII.GetBytes("GR");
        public static readonly byte[] GROUP_DELETE = Encoding.ASCII.GetBytes("GD");
        public static readonly byte[] GROUP_RENAME = Encoding.ASCII.GetBytes("GN");
        public static readonly byte[] KEY_EX_ECDHE = Encoding.ASCII.GetBytes("KE");
        public static readonly byte[] KEY_EX_PSK_TX = Encoding.ASCII.GetBytes("KT");
        public static readonly byte[] KEY_EX_PSK_RX = Encoding.ASCII.GetBytes("KR");
        public static readonly byte[] CONTACT_REM = Encoding.ASCII.GetBytes("CR");
        public static readonly byte[] WIPE_USR_DATA = Encoding.ASCII.GetBytes("WD");

        /// <summary>
        /// Origin headers
        /// </summary>
        public const int ORIGIN_HEADER_LENGTH = 1;
        public static readonly byte[] ORIGIN_USER_HEADER = new byte[] { (byte)'o' };
        public static readonly byte[] ORIGIN_CONTACT_HEADER = new byte[] { (byte)'i' };

        /// <summary>
        /// Message headers
        /// </summary>
        public const int MESSAGE_HEADER_LENGTH = 1;
        public const int WHISPER_FIELD_LENGTH = 1;
        public static readonly byte[] PRIVATE_MESSAGE_HEADER = new byte[] { (byte)'p' };
        public static readonly byte[] GROUP_MESSAGE_HEADER = new byte[] { (byte)'g' };
        public static readonly byte[] FILE_KEY_HEADER = new byte[] { (byte)'k' };

        /// <summary>
        /// Delays
        /// </summary>
        public const double TRAFFIC_MASKING_QUEUE_CHECK_DELAY = 0.1;
        public const double TRAFFIC_MASKING_MIN_STATIC_DELAY = 0.1;
        public const double TRAFFIC_MASKING_MIN_RANDOM_DELAY = 0.1;
        public const double LOCAL_TESTING_PACKET_DELAY = 0.1;
        public const int RELAY_CLIENT_MAX_DELAY = 8;
        public const double RELAY_CLIENT_MIN_DELAY = 0.125;
        public const double CLIENT_OFFLINE_THRESHOLD = 4.0;

        /// <summary>
        /// Constant time delay types
        /// </summary>
        public const string STATIC = "static";
        public const string TRAFFIC_MASKING = "traffic_masking";

        /// <summary>
        /// Default directories
        /// </summary>
        public const string DIR_USER_DATA = "Desktop/";
        public const string DIR_RECV_FILES = "received_files/";
        public const string DIR_OnionLink = "OnionLink/";
        public const string TEMP_SUFFIX = "_temp";
        public const string DIR_TAILS_PERS = "Persistent/";

        /// <summary>
        /// Key exchange status states
        /// </summary>
        public const int KEX_STATUS_LENGTH = 1;
        public static readonly byte KEX_STATUS_NONE = 0xA0;
        public static readonly byte KEX_STATUS_PENDING = 0xA1;
        public static readonly byte KEX_STATUS_UNVERIFIED = 0xA2;
        public static readonly byte KEX_STATUS_VERIFIED = 0xA3;
        public static readonly byte KEX_STATUS_NO_RX_PSK = 0xA4;
        public static readonly byte KEX_STATUS_HAS_RX_PSK = 0xA5;
        public static readonly byte KEX_STATUS_LOCAL_KEY = 0xA6;

        /// <summary>
        /// Queue dictionary keys (byte arrays)
        /// </summary>
        public static readonly byte[] EXIT_QUEUE = Encoding.ASCII.GetBytes("exit");
        public static readonly byte[] GATEWAY_QUEUE = Encoding.ASCII.GetBytes("gateway");
        public static readonly byte[] UNIT_TEST_QUEUE = Encoding.ASCII.GetBytes("unit_test");

        /// <summary>
        /// Transmitter queues
        /// </summary>
        public static readonly byte[] MESSAGE_PACKET_QUEUE = Encoding.ASCII.GetBytes("message_packet");
        public static readonly byte[] COMMAND_PACKET_QUEUE = Encoding.ASCII.GetBytes("command_packet");
        public static readonly byte[] TM_MESSAGE_PACKET_QUEUE = Encoding.ASCII.GetBytes("tm_message_packet");
        public static readonly byte[] TM_FILE_PACKET_QUEUE = Encoding.ASCII.GetBytes("tm_file_packet");
        public static readonly byte[] TM_COMMAND_PACKET_QUEUE = Encoding.ASCII.GetBytes("tm_command_packet");
        public static readonly byte[] TM_NOISE_PACKET_QUEUE = Encoding.ASCII.GetBytes("tm_noise_packet");
        public static readonly byte[] TM_NOISE_COMMAND_QUEUE = Encoding.ASCII.GetBytes("tm_noise_command");
        public static readonly byte[] RELAY_PACKET_QUEUE = Encoding.ASCII.GetBytes("relay_packet");
        public static readonly byte[] LOG_PACKET_QUEUE = Encoding.ASCII.GetBytes("log_packet");
        public static readonly byte[] LOG_SETTING_QUEUE = Encoding.ASCII.GetBytes("log_setting");
        public static readonly byte[] TRAFFIC_MASKING_QUEUE = Encoding.ASCII.GetBytes("traffic_masking");
        public static readonly byte[] LOGFILE_MASKING_QUEUE = Encoding.ASCII.GetBytes("logfile_masking");
        public static readonly byte[] KEY_MANAGEMENT_QUEUE = Encoding.ASCII.GetBytes("key_management");
        public static readonly byte[] KEY_MGMT_ACK_QUEUE = Encoding.ASCII.GetBytes("key_mgmt_ack");
        public static readonly byte[] SENDER_MODE_QUEUE = Encoding.ASCII.GetBytes("sender_mode");
        public static readonly byte[] WINDOW_SELECT_QUEUE = Encoding.ASCII.GetBytes("window_select");

        /// <summary>
        /// Relay queues
        /// </summary>
        public static readonly byte[] DST_COMMAND_QUEUE = Encoding.ASCII.GetBytes("dst_command");
        public static readonly byte[] DST_MESSAGE_QUEUE = Encoding.ASCII.GetBytes("dst_message");
        public static readonly byte[] SRC_TO_RELAY_QUEUE = Encoding.ASCII.GetBytes("src_to_relay");
        public static readonly byte[] RX_BUF_KEY_QUEUE = Encoding.ASCII.GetBytes("rx_buf_key");
        public static readonly byte[] TX_BUF_KEY_QUEUE = Encoding.ASCII.GetBytes("tx_buf_key");
        public static readonly byte[] URL_TOKEN_QUEUE = Encoding.ASCII.GetBytes("url_token");
        public static readonly byte[] GROUP_MGMT_QUEUE = Encoding.ASCII.GetBytes("group_mgmt");
        public static readonly byte[] GROUP_MSG_QUEUE = Encoding.ASCII.GetBytes("group_msg");
        public static readonly byte[] CONTACT_REQ_QUEUE = Encoding.ASCII.GetBytes("contact_req");
        public static readonly byte[] C_REQ_MGMT_QUEUE = Encoding.ASCII.GetBytes("c_req_mgmt");
        public static readonly byte[] CONTACT_MGMT_QUEUE = Encoding.ASCII.GetBytes("contact_mgmt");
        public static readonly byte[] C_REQ_STATE_QUEUE = Encoding.ASCII.GetBytes("c_req_state");
        public static readonly byte[] ONION_KEY_QUEUE = Encoding.ASCII.GetBytes("onion_key");
        public static readonly byte[] ONION_CLOSE_QUEUE = Encoding.ASCII.GetBytes("close_onion");
        public static readonly byte[] TOR_DATA_QUEUE = Encoding.ASCII.GetBytes("tor_data");
        public static readonly byte[] PUB_KEY_CHECK_QUEUE = Encoding.ASCII.GetBytes("pubkey_check");
        public static readonly byte[] PUB_KEY_SEND_QUEUE = Encoding.ASCII.GetBytes("pubkey_send");
        public static readonly byte[] ACCOUNT_CHECK_QUEUE = Encoding.ASCII.GetBytes("account_check");
        public static readonly byte[] ACCOUNT_SEND_QUEUE = Encoding.ASCII.GetBytes("account_send");
        public static readonly byte[] USER_ACCOUNT_QUEUE = Encoding.ASCII.GetBytes("user_account");
        public static readonly byte[] GUI_INPUT_QUEUE = Encoding.ASCII.GetBytes("gui_input");

        /// <summary>
        /// Queue signals
        /// </summary>
        public const string KDB_ADD_ENTRY_HEADER = "ADD";
        public const string KDB_REMOVE_ENTRY_HEADER = "REM";
        public const string KDB_M_KEY_CHANGE_HALT_HEADER = "HALT";
        public const string KDB_HALT_ACK_HEADER = "HALT_ACK";
        public const string KDB_UPDATE_SIZE_HEADER = "STO";
        public const string RP_ADD_CONTACT_HEADER = "RAC";
        public const string RP_REMOVE_CONTACT_HEADER = "RRC";
        public const string EXIT = "EXIT";
        public const string WIPE = "WIPE";

        /// <summary>
        /// Serial interface
        /// </summary>
        public const int BAUDS_PER_BYTE = 10;
        public const double SERIAL_RX_MIN_TIMEOUT = 0.05;

        /// <summary>
        /// CLI indents
        /// </summary>
        public const int CONTACT_LIST_INDENT = 4;
        public const int FILE_TRANSFER_INDENT = 4;
        public const int SETTINGS_INDENT = 2;

        /// <summary>
        /// Compression
        /// </summary>
        public const int COMPRESSION_LEVEL = 9;
        public const int MAX_MESSAGE_SIZE = 100_000; // bytes

        /// <summary>
        /// Traffic masking
        /// </summary>
        public const int NOISE_PACKET_BUFFER = 100;

        /// <summary>
        /// Local testing
        /// </summary>
        public const string LOCALHOST = "localhost";
        public const int SRC_DD_LISTEN_SOCKET = 5005;
        public const int RP_LISTEN_SOCKET = 5006;
        public const int DST_DD_LISTEN_SOCKET = 5007;
        public const int DST_LISTEN_SOCKET = 5008;
        public const int DD_ANIMATION_LENGTH = 16;
        public const int DD_OFFSET_FROM_CENTER = 4;

        /// <summary>
        /// Qubes related
        /// </summary>
        public const string QUBES_NET_VM_NAME = "OnionLink-Networker";
        public const string QUBES_DST_VM_NAME = "OnionLink-Destination";
        public const string QUBES_SRC_NET_POLICY = "OnionLink.SourceNetworker";
        public const string QUBES_NET_DST_POLICY = "OnionLink.NetworkerDestination";
        public const string QUBES_BUFFER_INCOMING_DIR = ".buffered_incoming_packets";
        public const string QUBES_BUFFER_INCOMING_PACKET = "buffered_incoming_packet";

        /// <summary>
        /// Relay Program's ciphertext buffering
        /// </summary>
        public const string RELAY_BUFFER_OUTGOING_F_DIR = ".buffered_outgoing_files";
        public const string RELAY_BUFFER_OUTGOING_M_DIR = ".buffered_outgoing_messages";
        public const string RELAY_BUFFER_OUTGOING_FILE = "buffered_file";
        public const string RELAY_BUFFER_OUTGOING_MESSAGE = "buffered_message";

        /// <summary>
        /// Field lengths
        /// </summary>
        public const int ENCODED_BOOLEAN_LENGTH = 1;
        public const int ENCODED_BYTE_LENGTH = 1;
        public const int TIMESTAMP_LENGTH = 4;
        public const int ENCODED_INTEGER_LENGTH = 8;
        public const int ENCODED_FLOAT_LENGTH = 8;
        public const int FILE_ETA_FIELD_LENGTH = 8;
        public const int FILE_SIZE_FIELD_LENGTH = 8;
        public const int GROUP_DB_HEADER_LENGTH = 32;
        public const int PADDED_UTF32_STR_LENGTH = 1024;
        public const int CONFIRM_CODE_LENGTH = 1;
        public const int PACKET_CHECKSUM_LENGTH = 16;

        /// <summary>
        /// Onion address format
        /// </summary>
        public static readonly byte[] ONION_ADDRESS_CHECKSUM_ID = Encoding.ASCII.GetBytes(".onion checksum");
        public static readonly byte[] ONION_SERVICE_VERSION = new byte[] { 0x03 };
        public const int ONION_SERVICE_VERSION_LENGTH = 1;
        public const int ONION_ADDRESS_CHECKSUM_LENGTH = 2;
        public const int ONION_ADDRESS_LENGTH = 56;

        /// <summary>
        /// Misc
        /// </summary>
        public const int BITS_PER_BYTE = 8;
        public static readonly ulong MAX_INT = ulong.MaxValue; // 2^64 - 1
        public const int B58_CHECKSUM_LENGTH = 4;
        public const int TRUNC_ADDRESS_LENGTH = 5;
        public const int TOR_CONTROL_PORT = 951;
        public const int TOR_SOCKS_PORT = 9050;
        public const int DB_WRITE_RETRY_LIMIT = 10;
        public const double ACCOUNT_RATIO_LIMIT = 0.75;

        /// <summary>
        /// Key derivation
        /// </summary>
        public const int ARGON2_MIN_TIME_COST = 1;
        public const int ARGON2_MIN_MEMORY_COST = 8;
        public const int ARGON2_MIN_PARALLELISM = 1;
        public const int ARGON2_SALT_LENGTH = 32;
        public const int ARGON2_PSK_TIME_COST = 25;
        public const int ARGON2_PSK_MEMORY_COST = 512 * 1024; // kibibytes
        public const int ARGON2_PSK_PARALLELISM = 2;
        public const double MIN_KEY_DERIVATION_TIME = 3.0; // seconds
        public const double MAX_KEY_DERIVATION_TIME = 4.0; // seconds
        public const int PASSWORD_MIN_BIT_STRENGTH = 128;

        /// <summary>
        /// Cryptographic field sizes
        /// </summary>
        public const int ONIONLINK_PRIVATE_KEY_LENGTH = 56;
        public const int ONIONLINK_PUBLIC_KEY_LENGTH = 56;
        public const int X448_SHARED_SECRET_LENGTH = 56;
        public const int FINGERPRINT_LENGTH = 32;
        public const int ONION_SERVICE_PRIVATE_KEY_LENGTH = 32;
        public const int ONION_SERVICE_PUBLIC_KEY_LENGTH = 32;
        public const int XCHACHA20_NONCE_LENGTH = 24;
        public const int SYMMETRIC_KEY_LENGTH = 32;
        public const int POLY1305_TAG_LENGTH = 16;
        public const int BLAKE2_DIGEST_LENGTH = 32;
        public const int BLAKE2_DIGEST_LENGTH_MIN = 1;
        public const int BLAKE2_DIGEST_LENGTH_MAX = 64;
        public const int BLAKE2_KEY_LENGTH_MAX = 64;
        public const int BLAKE2_SALT_LENGTH_MAX = 16;
        public const int BLAKE2_PERSON_LENGTH_MAX = 16;
        public const int HARAC_LENGTH = 8;
        public const int PADDING_LENGTH = 255;
        public const int ENCODED_B58_PUB_KEY_LENGTH = 84;
        public const int ENCODED_B58_KDK_LENGTH = 51;

        /// <summary>
        /// Domain separation
        /// </summary>
        public static readonly byte[] MESSAGE_KEY = Encoding.ASCII.GetBytes("message_key");
        public static readonly byte[] HEADER_KEY = Encoding.ASCII.GetBytes("header_key");
        public static readonly byte[] FINGERPRINT = Encoding.ASCII.GetBytes("fingerprint");
        public static readonly byte[] BUFFER_KEY = Encoding.ASCII.GetBytes("buffer_key");

        /// <summary>
        /// Forward secrecy
        /// </summary>
        public const int INITIAL_HARAC = 0;
        public const int HARAC_WARN_THRESHOLD = 100_000;

        /// <summary>
        /// Special messages
        /// </summary>
        public static readonly byte[] PLACEHOLDER_DATA = Combine(P_N_HEADER, new byte[PADDING_LENGTH]);

        /// <summary>
        /// Field lengths
        /// </summary>
        public const int ASSEMBLY_PACKET_LENGTH = ASSEMBLY_PACKET_HEADER_LENGTH + PADDING_LENGTH;

        public const int HARAC_CT_LENGTH = XCHACHA20_NONCE_LENGTH + HARAC_LENGTH + POLY1305_TAG_LENGTH;

        public const int ASSEMBLY_PACKET_CT_LENGTH = XCHACHA20_NONCE_LENGTH + ASSEMBLY_PACKET_LENGTH + POLY1305_TAG_LENGTH;

        public const int MESSAGE_LENGTH = HARAC_CT_LENGTH + ASSEMBLY_PACKET_CT_LENGTH;

        public const int COMMAND_LENGTH = DATAGRAM_HEADER_LENGTH + MESSAGE_LENGTH;

        public const int PACKET_LENGTH = DATAGRAM_HEADER_LENGTH + MESSAGE_LENGTH + ORIGIN_HEADER_LENGTH;

        public const int GROUP_STATIC_LENGTH = PADDED_UTF32_STR_LENGTH + GROUP_ID_LENGTH + 2 * ENCODED_BOOLEAN_LENGTH;

        public const int CONTACT_LENGTH = ONION_SERVICE_PUBLIC_KEY_LENGTH + 2 * FINGERPRINT_LENGTH + 4 * ENCODED_BOOLEAN_LENGTH + PADDED_UTF32_STR_LENGTH;

        public const int KEYSET_LENGTH = ONION_SERVICE_PUBLIC_KEY_LENGTH + 4 * SYMMETRIC_KEY_LENGTH + 2 * HARAC_LENGTH;

        public const int PSK_FILE_SIZE = XCHACHA20_NONCE_LENGTH + ARGON2_SALT_LENGTH + 2 * SYMMETRIC_KEY_LENGTH + POLY1305_TAG_LENGTH;

        public const int LOG_ENTRY_LENGTH = ONION_SERVICE_PUBLIC_KEY_LENGTH + TIMESTAMP_LENGTH + ORIGIN_HEADER_LENGTH + ASSEMBLY_PACKET_LENGTH;

        public const int MASTERKEY_DB_SIZE = ARGON2_SALT_LENGTH + BLAKE2_DIGEST_LENGTH + 3 * ENCODED_INTEGER_LENGTH;

        public const int SETTING_LENGTH = XCHACHA20_NONCE_LENGTH + 4 * ENCODED_INTEGER_LENGTH + 3 * ENCODED_FLOAT_LENGTH + 12 * ENCODED_BOOLEAN_LENGTH + POLY1305_TAG_LENGTH;

        // Helper method to combine two byte arrays
        private static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
    }
}
