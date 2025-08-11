using System.Text;

public static class TerminalPrompts
{
    private const string CursorUpOneLine = "\u001b[A";

    public static int GetTerminalWidth()
    {
        try { return Console.WindowWidth; }
        catch { return 80; } // fallback
    }

    public static void TerminalWidthCheck(int requiredWidth)
    {
        if (GetTerminalWidth() < requiredWidth)
            throw new Exception("Terminal width too small for display.");
    }

    public static string AskConfirmationCode(string source)
    {
        string title = $"Enter confirmation code (from {source}): ";
        int inputSpace = " ff ".Length;

        string upperLine = "┌" + new string('─', title.Length + inputSpace) + "┐";
        string titleLine = "│" + title + new string(' ', inputSpace) + "│";
        string lowerLine = "└" + new string('─', title.Length + inputSpace) + "┘";

        int termWidth = GetTerminalWidth();
        upperLine = upperLine.PadLeft((termWidth + upperLine.Length) / 2);
        titleLine = titleLine.PadLeft((termWidth + titleLine.Length) / 2);
        lowerLine = lowerLine.PadLeft((termWidth + lowerLine.Length) / 2);

        TerminalWidthCheck(upperLine.Length);

        Console.WriteLine(upperLine);
        Console.WriteLine(titleLine);
        Console.WriteLine(lowerLine);
        Console.Write(CursorUpOneLine + CursorUpOneLine + CursorUpOneLine);

        int indent = titleLine.IndexOf('│');
        Console.Write(new string(' ', indent) + $"│ {title}");
        return Console.ReadLine() ?? "";
    }

    public static string BoxInput(
        string message,
        string defaultValue = "",
        int head = 0,
        int tail = 1,
        int expectedLen = 0
    )
    {
        for (int i = 0; i < head; i++) Console.WriteLine();

        int termWidth = GetTerminalWidth();
        int innerSpace = expectedLen > 0 ? expectedLen + 2 : termWidth - 2;

        string upperLine = "┌" + new string('─', innerSpace) + "┐";
        string inputLine = "│" + new string(' ', innerSpace) + "│";
        string lowerLine = "└" + new string('─', innerSpace) + "┘";
        string boxIndent = new string(' ', (termWidth - upperLine.Length) / 2);

        TerminalWidthCheck(upperLine.Length);

        Console.WriteLine(boxIndent + upperLine);
        Console.WriteLine(boxIndent + inputLine);
        Console.WriteLine(boxIndent + lowerLine);
        Console.Write(CursorUpOneLine + CursorUpOneLine);

        Console.Write(boxIndent + "│ ");
        string input = Console.ReadLine() ?? "";
        if (string.IsNullOrEmpty(input))
            input = defaultValue;

        for (int i = 0; i < tail; i++) Console.WriteLine();
        return input;
    }

    public static string PasswordPrompt(string message)
    {
        int termWidth = GetTerminalWidth();
        int inputSpace = " c ".Length;

        string upperLine = ("┌" + new string('─', message.Length + inputSpace) + "┐").PadLeft((termWidth + message.Length + inputSpace + 2) / 2);
        string titleLine = ("│" + message + new string(' ', inputSpace) + "│").PadLeft((termWidth + message.Length + inputSpace + 2) / 2);
        string lowerLine = ("└" + new string('─', message.Length + inputSpace) + "┘").PadLeft((termWidth + message.Length + inputSpace + 2) / 2);

        TerminalWidthCheck(upperLine.Length);

        Console.WriteLine(upperLine);
        Console.WriteLine(titleLine);
        Console.WriteLine(lowerLine);
        Console.Write(CursorUpOneLine + CursorUpOneLine + CursorUpOneLine);

        Console.Write(new string(' ', titleLine.IndexOf('│')) + $"│ {message}");
        return ReadPassword();
    }

    public static bool Yes(string prompt)
    {
        string fullPrompt = $"{prompt} (y/n): ";
        while (true)
        {
            Console.Write(fullPrompt);
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;

            input = input.ToLower();
            if (input == "y" || input == "yes") return true;
            if (input == "n" || input == "no") return false;
        }
    }

    private static string ReadPassword()
    {
        StringBuilder sb = new StringBuilder();
        ConsoleKeyInfo key;
        while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (key.Key == ConsoleKey.Backspace && sb.Length > 0)
            {
                sb.Length--;
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                sb.Append(key.KeyChar);
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return sb.ToString();
    }
}
