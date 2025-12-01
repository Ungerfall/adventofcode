<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

/*
	!!! Fill adventofcode_ga and adventofcode_session in File->Password Manager
	
	folder structure
	
	📦root
	 ┣ 📂2021
	 ┃ ┣ 📂input
	 ┃ ┃ ┗ 📜1.txt
	 ┃ ┗ 📜day1.linq
	 ┗ 📜template.linq
*/
const int year = 2023;
const int day = 19;
//static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	foreach (string line in input)
	{
		line.Dump();
	}
}
void part2()
{
	foreach (string line in input)
	{

	}
}

void Main()
{
	part1();
	part2();
}

public enum RuleType { }
public record PartRating(int x, int m, int a, int s);
public record WorkflowResult(byte type, int? v, string? next);
Dictionary<string, Workflow> ParseWorkflows(IList<string> workflows)
{
	// tokens
	const char partX = 'x';
	const char partM = 'm';
	const char partA = 'a';
	const char partS = 's';
	const char accepted = 'A';
	const char rejected = 'R';
	
	foreach (var workflow in workflows)
	{
		// workflow format: [name]{[rule,]*[rule]}
		int i = 0;
		while (workflow[i] != '{')
		{
			i++;
		}
		
		string name = workflow[0..i];
		string[] rules = workflow[i..^1].Split(',');
		foreach (var rule in rules)
		{
			int colon = rule.IndexOf(';');
			if (colon >= 0)
			{
				string condition = rule[0..colon];
				string outcome = rule.Substring(colon + 1);
			}
			else
			{
				
			}
		}
	}
}

static string[] GetInputLines(int year, int day)
{
	Debug.Assert(year <= DateTime.Now.Year);
	Debug.Assert(day > 0 && day <= 25);
	var root = Directory.GetParent(Path.GetDirectoryName(Util.CurrentQueryPath));
	var inputDir = Directory.CreateDirectory(Path.Combine(root.FullName, year.ToString(), "input"));
	var input = Path.Combine(inputDir.FullName, $"{day}.txt");
	if (File.Exists(input))
	{
		return File.ReadLines(input).ToArray();
	}
	else
	{
		"Loaded from internet.".Dump();
		using System.Net.Http.HttpClient c = new();
		string ga = Util.GetPassword("adventofcode_ga");
		string session = Util.GetPassword("adventofcode_session");
		c.DefaultRequestHeaders.Add("cookie", "_ga=" + ga + "; session=" + session);
		string uri = $"https://adventofcode.com/{year}/day/{day}/input";
		var content = c.GetAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult().Content;
		using (StreamWriter sw = new(input))
		{
			content.CopyTo(sw.BaseStream, null, CancellationToken.None);
		}

		return File.ReadLines(input).ToArray();
	}
}
