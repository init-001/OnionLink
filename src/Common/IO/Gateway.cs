using System.Text;

public static class TerminalUtils
{
    private const string BoldOn = "\u001b[1m";
    private const string BoldOff = "\u001b[22m";
    private const string ClearScreeen = "\u001b[2J";
    private const string CursorHome = "\u001b[H";

    public static void ClearScreen(double delaySeconds = 0.0)
    {
        if (delaySeconds > 0)
            System.Threading.Thread.Sleep((int)(delaySeconds * 1000));
        Console.Write(ClearScreeen + CursorHome);
    }

    public static void PrintBox(List<string> lines, bool center = true, bool bold = false)
    {
        if (lines.Count == 0) return;
        int width = lines.Max(l => l.Length);
        var top = "┌" + new string('─', width + 2) + "┐";
        var bottom = "└" + new string('─', width + 2) + "┘";

        if (bold) Console.Write(BoldOn);

        Console.WriteLine(top);
        foreach (var line in lines)
        {
            string text = center ? CenterText(line, width) : line.PadRight(width);
            Console.WriteLine($"│ {text} │");
        }
        Console.WriteLine(bottom);

        if (bold) Console.Write(BoldOff);
    }

    public static void PrintCentered(string message, bool bold = false)
    {
        if (bold) Console.Write(BoldOn);
        Console.WriteLine(CenterText(message, Console.WindowWidth));
        if (bold) Console.Write(BoldOff);
    }

    public static void PrintTimestamp(string message, bool bold = false)
    {
        string ts = DateTime.Now.ToString("MMM dd - HH:mm:ss.fff");
        if (bold) Console.Write(BoldOn);
        Console.WriteLine($"{ts} - {message}");
        if (bold) Console.Write(BoldOff);
    }

    private static string CenterText(string text, int width)
    {
        if (text.Length >= width) return text;
        int leftPadding = (width - text.Length) / 2;
        return new string(' ', leftPadding) + text;
    }

    public static List<string> WrapText(string text, int maxWidth)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var sb = new StringBuilder();
        foreach (var word in words)
        {
            if (sb.Length + word.Length + 1 > maxWidth)
            {
                lines.Add(sb.ToString());
                sb.Clear();
            }
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(word);
        }
        if (sb.Length > 0)
            lines.Add(sb.ToString());
        return lines;
    }
}
