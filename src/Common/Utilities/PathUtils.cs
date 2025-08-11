using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Common.Exceptions;

public static class NativePathPicker
{
    public static bool DisableGuiDialog { get; set; } = false;

    public static string AskPath(string promptMsg, bool getFile = false)
    {
        if (!DisableGuiDialog)
        {
            try
            {
                string guiResult = AskPathGui(promptMsg, getFile);
                if (!string.IsNullOrEmpty(guiResult))
                    return guiResult;
            }
            catch
            {
                // GUI dialog failed or disabled, fallback
            }
        }

        return AskPathCli(promptMsg, getFile);
    }

    private static string AskPathGui(string promptMsg, bool getFile)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows: fallback to CLI or you can extend here to call native dialog via COM
            throw new Exception("GUI dialog not implemented on Windows (no WinForms allowed)");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // Use AppleScript to show native dialog
            string script = getFile
                ? $"set theFile to choose file with prompt \"{EscapeApplescript(promptMsg)}\"\nreturn POSIX path of theFile"
                : $"set theFolder to choose folder with prompt \"{EscapeApplescript(promptMsg)}\"\nreturn POSIX path of theFolder";

            string result = RunProcess("osascript", $"-e \"{script}\"");
            if (string.IsNullOrEmpty(result))
                throw new Exception("User cancelled");
            return result.Trim();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Use zenity or kdialog
            if (IsCommandAvailable("zenity"))
            {
                string zenityArgs = getFile
                    ? $"--file-selection --title=\"{promptMsg}\""
                    : $"--file-selection --directory --title=\"{promptMsg}\"";

                string result = RunProcess("zenity", zenityArgs);
                if (string.IsNullOrEmpty(result))
                    throw new Exception("User cancelled");
                return result.Trim();
            }
            else if (IsCommandAvailable("kdialog"))
            {
                string kdialogArgs = getFile
                    ? "--getopenfilename"
                    : "--getexistingdirectory";

                string result = RunProcess("kdialog", kdialogArgs);
                if (string.IsNullOrEmpty(result))
                    throw new Exception("User cancelled");
                return result.Trim();
            }
            else
            {
                throw new Exception("No GUI dialog tool found");
            }
        }
        else
        {
            throw new Exception("Unsupported OS for GUI dialogs");
        }
    }

    private static string AskPathCli(string promptMsg, bool getFile)
    {
        Console.WriteLine();
        if (getFile)
            return CliGetFile(promptMsg);
        else
            return CliGetPath(promptMsg);
    }

    private static string CliGetFile(string promptMsg)
    {
        var completer = new PathCompleter(getFile: true);

        while (true)
        {
            Console.Write($"{promptMsg}: ");
            string input = ReadLineWithCompletion(completer, promptMsg);
            input = input?.Trim() ?? "";

            if (string.IsNullOrEmpty(input))
                throw new SoftError("File selection aborted.");

            if (File.Exists(input))
            {
                return input;
            }
            Console.WriteLine("File selection error.\n");
        }
    }

    private static string CliGetPath(string promptMsg)
    {
        var completer = new PathCompleter(getFile: false);

        while (true)
        {
            Console.Write($"{promptMsg}: ");
            string input = ReadLineWithCompletion(completer, promptMsg);
            input = input?.Trim() ?? "";

            if (!input.EndsWith(Path.DirectorySeparatorChar))
                input += Path.DirectorySeparatorChar;

            if (Directory.Exists(input))
            {
                return input;
            }
            Console.WriteLine("Error: Invalid directory.\n");
        }
    }

    private static string RunProcess(string fileName, string args)
    {
        try
        {
            var psi = new ProcessStartInfo(fileName, args)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var proc = Process.Start(psi);
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output.Trim();
        }
        catch
        {
            return null;
        }
    }

    private static bool IsCommandAvailable(string command)
    {
        try
        {
            var psi = new ProcessStartInfo("which", command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var proc = Process.Start(psi);
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return !string.IsNullOrEmpty(output);
        }
        catch
        {
            return false;
        }
    }

    private static string EscapeApplescript(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    // --- Simple path tab-completion for CLI ---

    private class PathCompleter
    {
        private readonly bool getFile;

        public PathCompleter(bool getFile)
        {
            this.getFile = getFile;
        }

        public IEnumerable<string> Complete(string text)
        {
            string dir, prefix;

            if (string.IsNullOrEmpty(text))
            {
                dir = Directory.GetCurrentDirectory();
                prefix = "";
            }
            else if (Directory.Exists(text))
            {
                dir = text;
                prefix = "";
            }
            else
            {
                dir = Path.GetDirectoryName(text) ?? Directory.GetCurrentDirectory();
                prefix = Path.GetFileName(text) ?? "";
            }

            if (!Directory.Exists(dir))
                return Array.Empty<string>();

            var entries = new List<string>();
            foreach (var entry in Directory.EnumerateFileSystemEntries(dir))
            {
                string name = Path.GetFileName(entry);
                if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    if (Directory.Exists(entry))
                        entries.Add(name + Path.DirectorySeparatorChar);
                    else if (getFile)
                        entries.Add(name);
                }
            }
            return entries;
        }
    }

    // Very basic ReadLine with tab completion (only on Unix shells or Windows Terminal)
    // You can enhance this with third-party libs or your own advanced readline implementation
    private static string ReadLineWithCompletion(PathCompleter completer, string promptMsg)
    {
        var input = new StringBuilder();
        int cursor = 0;

        ConsoleKeyInfo keyInfo;
        while (true)
        {
            keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (keyInfo.Key == ConsoleKey.Tab)
            {
                var completions = completer.Complete(input.ToString());
                if (completions != null)
                {
                    // Cycle completions (simplified: just take first match)
                    foreach (var c in completions)
                    {
                        // Replace input with completion
                        ClearCurrentConsoleLine();
                        Console.Write(promptMsg + ": ");
                        Console.Write(c);
                        input.Clear();
                        input.Append(c);
                        cursor = input.Length;
                        break;
                    }
                }
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0 && cursor > 0)
                {
                    input.Remove(cursor - 1, 1);
                    cursor--;
                    ClearCurrentConsoleLine();
                    Console.Write(promptMsg + ": " + input);
                    // Set cursor position accordingly
                }
            }
            else
            {
                input.Insert(cursor, keyInfo.KeyChar);
                cursor++;
                Console.Write(keyInfo.KeyChar);
            }
        }

        return input.ToString();
    }

    private static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, currentLineCursor);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }
}
