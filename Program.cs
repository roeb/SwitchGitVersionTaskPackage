using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SwitchGitVersionTaskPackage
{
    class Program
    {
        static void Main(string[] args)
        {
            if ((args.Length == 1 && args[0] == "--help") || args.Length == 0)
            {
                WriteHelp();
            }


            var solutionPath = Directory.GetCurrentDirectory();
            var enableGitVersion = true;
            if (args.Length == 2)
            {
                if (args[0] == "enable")
                    enableGitVersion = true;
                else if (args[0] == "disable")
                    enableGitVersion = false;
                else
                {
                    WriteHelp();
                }

                if (Directory.Exists(args[1]))
                {
                    var searchPatterns = new[] {"GitVersionTask", "Common.targets"};
                    solutionPath = args[1];
                    var projectFiles = GetProjectFiles(solutionPath);
                    foreach (var projectFile in projectFiles)
                    {
                        DisableGitVersionTaskPackage(projectFile, enableGitVersion, searchPatterns);
                    }
                }
                else
                {
                    Console.WriteLine($"Directory '{args[1]}' doesnt exists");
                }
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static void WriteHelp()
        {
            Console.WriteLine("SwitchGitVersionTaskPackage <action> <path>");
            Console.WriteLine(" enable     Uncomments all GitVersionTask references and Common.targets in csproj files.");
            Console.WriteLine(" disable    Comments out all GitVersionTask references and Common.targets in csproj files.");
            Console.WriteLine(" [PATH_TO_YOUR_SLN_FILE_FOLDER]");
        }

        private static List<string> GetProjectFiles(string path)
        {
            var projectFiles = new List<string>();
            if (Directory.Exists(path))
            {
                var directoryInfo = new DirectoryInfo(path);
                var files = directoryInfo.GetFiles("*.csproj", SearchOption.AllDirectories);

                projectFiles = files.Select(m => m.FullName).ToList();
            }

            Console.WriteLine($"{projectFiles.Count} .csproj files located:");
            projectFiles.ForEach(m => Console.WriteLine($"    {m.Replace(path, "")}"));

            return projectFiles;
        }

        private static void DisableGitVersionTaskPackage(string csProjFile, bool enableGitVersion, string[] searchPatterns)
        {
            Console.Write(Environment.NewLine);

            var projectFileLines = File.ReadAllLines(csProjFile).ToList();
            foreach (var searchPattern in searchPatterns)
            {
                if (projectFileLines.Any(m => m.Contains(searchPattern)))
                {
                    var gitVersionTaskLine = projectFileLines.FirstOrDefault(m => m.Contains(searchPattern));
                    var newGitVersionTaskLine = gitVersionTaskLine;
                    if (newGitVersionTaskLine.Contains("<!--") && enableGitVersion)
                    {
                        newGitVersionTaskLine = gitVersionTaskLine.Replace("<!--", "").Replace("-->", "");
                    }
                    else if (!newGitVersionTaskLine.Contains("<!--") && !enableGitVersion)
                    {
                        newGitVersionTaskLine = $"<!--{gitVersionTaskLine}-->";
                    }

                    if (newGitVersionTaskLine != gitVersionTaskLine)
                    {
                        projectFileLines = projectFileLines.Select(s => s.Replace(gitVersionTaskLine, newGitVersionTaskLine)).ToList();
                        File.WriteAllLines(csProjFile, projectFileLines);

                        if (enableGitVersion)
                            Console.WriteLine($"{searchPattern} enabled in {(new FileInfo(csProjFile)).Name}");
                        else
                            Console.WriteLine($"{searchPattern} disabled in {(new FileInfo(csProjFile)).Name}");
                    }
                    else
                    {
                        if (enableGitVersion)
                            Console.WriteLine($"{searchPattern} already enabled in {(new FileInfo(csProjFile)).Name}");
                        else
                            Console.WriteLine($"{searchPattern} already disabled in {(new FileInfo(csProjFile)).Name}");
                    }
                }
                else
                {
                    Console.WriteLine($"{searchPattern} is missing in {(new FileInfo(csProjFile)).Name}");
                }
            }
        }
    }
}
