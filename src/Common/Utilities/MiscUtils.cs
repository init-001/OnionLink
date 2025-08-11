using Common.Exceptions;

namespace TfcPathSelector
{
    public static class PathPrompt
    {
        public static string AskPath(string promptMsg, bool getFile = false)
        {
            Console.WriteLine();
            return getFile ? CliGetFile(promptMsg) : CliGetPath(promptMsg);
        }

        private static string CliGetFile(string promptMsg)
        {
            while (true)
            {
                try
                {
                    Console.Write($"{promptMsg}: ");
                    string pathToFile = Console.ReadLine()?.Trim() ?? "";

                    if (string.IsNullOrEmpty(pathToFile))
                        throw new SoftError("File selection aborted.");

                    if (File.Exists(pathToFile))
                    {
                        if (pathToFile.StartsWith("./"))
                            pathToFile = pathToFile.Substring(2);

                        Console.WriteLine();
                        return pathToFile;
                    }

                    Console.WriteLine("File selection error.\n");
                }
                catch (SoftError)
                {
                    throw;
                }
                catch
                {
                    throw new SoftError("File selection aborted.");
                }
            }
        }

        private static string CliGetPath(string promptMsg)
        {
            while (true)
            {
                try
                {
                    Console.Write($"{promptMsg}: ");
                    string directory = Console.ReadLine()?.Trim() ?? "";

                    if (directory.StartsWith("./"))
                        directory = directory.Substring(2);

                    if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        directory += Path.DirectorySeparatorChar;

                    if (!Directory.Exists(directory))
                    {
                        Console.WriteLine("Error: Invalid directory.\n");
                        continue;
                    }

                    return directory;
                }
                catch
                {
                    throw new SoftError("File path selection aborted.");
                }
            }
        }
    }
}
