using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql;

namespace PgRoutiner
{
    partial class Builder
    {
        public static void Run(string connectionStr)
        {
            var outputDir = GetOutputDir();
            var modelDir = GetModelDir();

            using var connection = new NpgsqlConnection(connectionStr);
            foreach (var group in connection.GetRoutinesInfoGroups())
            {
                var name = group.Key;
                var shortFilename = string.Concat(name.ToUpperCamelCase(), ".cs");
                var fullFileName = Path.Join(outputDir, shortFilename);
                var exists = File.Exists(fullFileName);

                if (exists && Settings.Value.Overwrite == false)
                {
                    Dump($"File {Settings.Value.OutputDir}/{shortFilename} exists, overwrite is set to false, skipping ...");
                    continue;
                }
                if (exists && Settings.Value.SkipIfExists != null && Settings.Value.SkipIfExists.Contains(name))
                {
                    Dump($"Skipping {Settings.Value.OutputDir}/{shortFilename}, already exists...");
                    continue;
                }
                Dump($"Building {Settings.Value.OutputDir}{Path.DirectorySeparatorChar}{shortFilename}...");
                var module = new RoutineModule(Settings.Value);
                var code = new RoutineCode(Settings.Value, name, group, connectionStr);

                module.AddItems(code.Model, code.Class);

                var result = module.ToString();
            }
        }

        private static void Dump(params string[] lines)
        {
            Program.WriteLine(ConsoleColor.Yellow, lines);
        }
    }
}
