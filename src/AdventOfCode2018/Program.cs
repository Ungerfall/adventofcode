using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AdventOfCode2018
{
	class Program
	{
		const string INPUT_FILE_MASK = "{0}.input";

		private static Dictionary<string, ISolution> taskSolutionDictionry;

		static void Main(string[] args)
		{
			taskSolutionDictionry = new Dictionary<string, ISolution>
			{
				["1"] = new Solution1()
			};
			var helpWords = new string[] {"--help", "-h", "?", "\\?"};
			if (args == null || args.Length <= 0 || args[0] == null || helpWords.Contains(args[0]))
			{
				ShowHelp();
				return;
			}

			var task = args[0];
			if (!taskSolutionDictionry.ContainsKey(task))
			{
				ShowHelp();
				return;
			}

			var solution = taskSolutionDictionry[task];
			var input = ParseInput(task);
			Console.WriteLine(solution.Solve(input));
		}

		static IEnumerable<string> ParseInput(string task)
		{
			var filename = string.Format(INPUT_FILE_MASK, task);
			if (!File.Exists(filename))
			{
				Console.WriteLine($"file {filename} not exists");
			}

			using (var streamReader = new StreamReader(filename, System.Text.Encoding.UTF8))
			{
				string line = null;
				while ((line = streamReader.ReadLine()) != null)
				{
					yield return line;
				}
			}

		}

		static void ShowHelp()
		{
			Console.WriteLine("Type a number of task. Examle: program.exe 1");
		}
	}
}
