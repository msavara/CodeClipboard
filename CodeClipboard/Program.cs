using System.Text;
using TextCopy;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace CodeClipboard
{
    /// <summary>
    /// Main entry point for the CodeClipboard application.
    /// Generates a structured tree of source files and their contents,
    /// formats it, and copies it to the clipboard.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Code Tree Generator");
            Console.WriteLine("------------------");

            try
            {
                // Load settings from appsettings.json
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var settings = config.Get<AppSettings>();

                // Determine the source path from arguments or settings
                string sourcePath = args.Length > 0 ? args[0] : settings!.SourcePath;
                if (string.IsNullOrWhiteSpace(sourcePath))
                {
                    sourcePath = Directory.GetCurrentDirectory();
                }

                if (!Directory.Exists(sourcePath))
                {
                    Console.WriteLine("The specified directory does not exist.");
                    return;
                }

                // Generate the code tree
                var result = GenerateCodeTree(sourcePath, settings);

                // Optionally minify the output if configured
                if (settings!.MinifyOutput)
                {
                    result = Minify(result);
                }

                // Copy result to clipboard
                ClipboardService.SetText(result);
                Console.WriteLine("\nThe code tree has been copied to the clipboard.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a textual representation of the directory structure and file content.
        /// </summary>
        /// <param name="rootPath">The root directory to scan.</param>
        /// <param name="settings">Configuration settings for inclusion and exclusion rules.</param>
        /// <returns>A string representing the directory structure and file content.</returns>
        private static string GenerateCodeTree(string rootPath, AppSettings? settings)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Code Tree for: {rootPath}");
            sb.AppendLine("=====================================");
            ProcessDirectory(rootPath, string.Empty, sb, settings);
            return sb.ToString();
        }

        /// <summary>
        /// Recursively processes a directory, appending its structure and file content to the StringBuilder.
        /// </summary>
        /// <param name="path">The directory path to process.</param>
        /// <param name="indent">Indentation string for formatting the output.</param>
        /// <param name="sb">The StringBuilder to append the output to.</param>
        /// <param name="settings">Configuration settings for inclusion and exclusion rules.</param>
        public static void ProcessDirectory(string path, string indent, StringBuilder sb, AppSettings? settings)
        {
            var dirInfo = new DirectoryInfo(path);
            sb.AppendLine($"{indent}📁 {dirInfo.Name}/");

            // Process files in the directory
            var files = dirInfo.GetFiles()
                .Where(f => settings!.IncludeExtensions.Contains(f.Extension.ToLower()) &&
                            (settings.MaxFileSizeKb <= 0 || f.Length <= settings.MaxFileSizeKb * 1024))
                .OrderBy(f => f.Name);

            foreach (var file in files)
            {
                sb.AppendLine($"{indent}├── 📄 {file.Name}");

                // Attempt to read and append file content
                try
                {
                    string content = File.ReadAllText(file.FullName);
                    sb.AppendLine($"{indent}│   Content:");
                    foreach (var line in content.Split('\n'))
                    {
                        sb.AppendLine($"{indent}│   {line.TrimEnd()}");
                    }
                    sb.AppendLine($"{indent}│");
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"{indent}│   Error reading file: {ex.Message}");
                }
            }

            // Process subdirectories recursively
            var directories = dirInfo.GetDirectories().OrderBy(d => d.Name);
            foreach (var dir in directories)
            {
                if (settings!.SkipDirectories.Contains(dir.Name.ToLower()))
                {
                    continue;
                }
                ProcessDirectory(dir.FullName, indent + "    ", sb, settings);
            }
        }

        /// <summary>
        /// Minifies the output string by removing unnecessary whitespace and blank lines.
        /// </summary>
        /// <param name="input">The input string to minify.</param>
        /// <returns>A minified version of the input string.</returns>
        private static string Minify(string input)
        {
            var lines = input.Split('\n')
                .Select(line => line.Trim()) // Remove leading and trailing whitespace
                .Where(line => !string.IsNullOrWhiteSpace(line)) // Remove empty lines
                .Select(line => Regex.Replace(line, @"\s{2,}", " ")) // Replace multiple spaces with a single space
                .ToList();

            return string.Join("", lines);
        }
    }

    /// <summary>
    /// Configuration settings for the application.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// The root directory to scan for the code tree.
        /// </summary>
        public required string SourcePath { get; set; }

        /// <summary>
        /// File extensions to include in the code tree output.
        /// </summary>
        public required string[] IncludeExtensions { get; set; }

        /// <summary>
        /// Directory names to exclude from the code tree output.
        /// </summary>
        public required string[] SkipDirectories { get; set; }

        /// <summary>
        /// The maximum file size (in kilobytes) to include in the code tree. Files larger than this are skipped.
        /// </summary>
        public int MaxFileSizeKb { get; set; }

        /// <summary>
        /// Whether to minify the output string by removing unnecessary whitespace and blank lines.
        /// </summary>
        public bool MinifyOutput { get; set; }
    }
}
