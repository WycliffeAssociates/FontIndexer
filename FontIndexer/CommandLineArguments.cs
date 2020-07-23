using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace FontIndexer
{
    class CommandLineArguments
    {
        [Option("source", Required = true, HelpText = "Source directory")]
        public string SourceDirectory { get; set; }

        [Option("output", Required = true, HelpText = "Output file (.json)")]
        public string OutputFile { get; set; }
        [Option('r', HelpText = "Whether to recursively look through a directory")]
        public bool Recursive { get; set; }
    }
}
