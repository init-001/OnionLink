using System.Text;

namespace Tfc.Terminal
{
    public static class TerminalPrinter
    {
        // ANSI escape codes for terminal control
        private const string ClearEntireScreen = "\u001b[2J";
        private const string CursorLeftUpCorner = "\u001b[H";
        private const string ClearEntireLine = "\u001b[2K";
        private const string CursorUpOneLine = "\u001b[1A";
        private const string BoldOn = "\u001b[1m";
        private const string NormalText = "\u001b[0m";

        public static void ClearScreen(double delaySeconds = 0.0)
        {
            if (delaySeconds > 0) Thread.Sleep((int)(delaySeconds * 1000));
            Console.Write(ClearEntireScreen + CursorLeftUpCorner);
            Console.Out.Flush();
        }

        public static void PrintMessage(
            IEnumerable<string> lines,
            bool manualProceed = false,
            bool bold = false,
            bool center = true,
            bool box = false,
            bool headClear = false,
            bool tailClear = false,
            double delaySeconds = 0,
            int maxWidth = 0,
            int head = 0,
            int tail = 0
        )
        {
            var msgList = lines.ToList();

            int terminalWidth = Console.WindowWidth;
            msgList = WrapMessages(box, maxWidth, msgList, terminalWidth, out int lenWidest);

            if (box || center)
            {
                msgList = msgList.Select(m => m.PadLeft((lenWidest + m.Length) / 2).PadRight(lenWidest)).ToList();
            }

            if (box)
            {
                var top = "┌" + new string('─', lenWidest + 2) + "┐";
                var bottom = "└" + new string('─', lenWidest + 2) + "┘";
                msgList = msgList.Select(m => $"│ {m} │").ToList();
                msgList.Insert(0, top);
                msgList.Add(bottom);
            }

            if (headClear) ClearScreen();
            PrintSpacing(head);

            foreach (var message in msgList)
            {
                var output = center ? message.PadLeft((terminalWidth + message.Length) / 2) : message;
                if (bold) output = BoldOn + output + NormalText;
                Console.WriteLine(output);
            }

            PrintSpacing(tail);
            if (delaySeconds > 0) Thread.Sleep((int)(delaySeconds * 1000));
            if (tailClear) ClearScreen();

            if (manualProceed)
            {
                Console.ReadLine();
                PrintOnPreviousLine();
            }
        }

        private static List<string> WrapMessages(bool box, int maxWidth, List<string> msgList, int terminalWidth, out int lenWidest)
        {
            lenWidest = msgList.Max(m => m.Length);
            int spcAroundMsg = box ? 4 : 2;
            int maxMsgWidth = terminalWidth - spcAroundMsg;

            if (maxWidth > 0) maxMsgWidth = Math.Min(maxWidth, maxMsgWidth);

            if (lenWidest > maxMsgWidth)
            {
                var newList = new List<string>();
                foreach (var msg in msgList)
                {
                    if (msg.Length > maxMsgWidth)
                        newList.AddRange(WrapText(msg, maxMsgWidth));
                    else
                        newList.Add(msg);
                }
                msgList = newList;
                lenWidest = msgList.Max(m => m.Length);
            }

            return msgList;
        }

        private static IEnumerable<string> WrapText(string text, int maxWidth)
        {
            var words = text.Split(' ');
            var line = new StringBuilder();
            foreach (var word in words)
            {
                if (line.Length + word.Length + 1 > maxWidth)
                {
                    yield return line.ToString();
                    line.Clear();
                }
                if (line.Length > 0) line.Append(' ');
                line.Append(word);
            }
            if (line.Length > 0) yield return line.ToString();
        }

        public static void PrintSpacing(int count)
        {
            for (int i = 0; i < count; i++) Console.WriteLine();
        }

        public static void PrintOnPreviousLine(int reps = 1, double delaySeconds = 0.0, bool flush = false)
        {
            if (delaySeconds > 0) Thread.Sleep((int)(delaySeconds * 1000));
            for (int i = 0; i < reps; i++)
            {
                Console.Write(CursorUpOneLine + ClearEntireLine);
            }
            if (flush) Console.Out.Flush();
        }
    }
}
