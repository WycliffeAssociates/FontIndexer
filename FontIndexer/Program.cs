using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Typography.OpenFont;

namespace FontIndexer
{
    class Program
    {
        static char[] IgnoredChars = new[] { (char)160 };
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments(args).WithParsed<CommandLineArguments>((arguments) =>
            {
                var files = GetFiles(arguments.SourceDirectory, arguments.Recursive);
                var mapping = CreateMapping(files);
                WriteJSONOutput(mapping, arguments.OutputFile);
            });
        }
        private static List<string> GetFiles(string directory, bool recursive)
        {
            if (recursive)
            {
                return Directory.GetFiles(directory, "*.ttf", SearchOption.AllDirectories).ToList();
            }

            return Directory.GetFiles(directory, "*.ttf").ToList();

        }
        private static Dictionary<int,string> CreateMapping(List<string> files)
        {
            const int MAXVALUE = 1000000;
            Dictionary<int, string> output = new Dictionary<int, string>(10000);
            foreach (var f in files)
            {
                OpenFontReader reader = new OpenFontReader();
                using (var stream = File.OpenRead(f))
                {
                    Typeface typeface;
                    try
                    {
                        typeface = reader.Read(stream);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Can't open {f}");
                        continue;
                    }
                    for (var i = 33; i < MAXVALUE; i++)
                    {
                        if (IgnoredChars.Contains((char)i))
                        {
                            continue;
                        }
                        if (output.ContainsKey(i))
                        {
                            continue;
                        }

                        if (typeface.Languages.ContainGlyphForUnicode(i))
                        {
                            output.Add(i, typeface.Name);
                        }
                    }
                }
            }
            return output;
        }

        static void WriteJSONOutput(Dictionary<int,string> mapping, string fileName)
        {
            File.WriteAllText(fileName, JsonSerializer.Serialize(mapping.ToDictionary(i => i.Key.ToString(), i=> i.Value)));
        }
    }
}
