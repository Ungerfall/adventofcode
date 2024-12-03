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
const int year = 2024;
const int day = 3;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	Regex template = new(@"mul\((?<op1>\d+),(?<op2>\d+)\)", RegexOptions.Compiled);
	long sum = 0L;
	foreach (string line in input)
	{
		foreach (Match match in template.Matches(line))
		{
			int op1 = int.Parse(match.Groups["op1"].Value);
			int op2 = int.Parse(match.Groups["op2"].Value);
			sum += op1 * (long)op2;
		}
	}

	sum.Dump("Sum of multiplications");
}
void part2()
{
	Regex template = new(@"(do\(\)|don't\(\)|mul\((?<op1>\d+),(?<op2>\d+)\))", RegexOptions.Compiled);
	long sum = 0L;
	bool @do = true;
	foreach (string line in input)
	{
		foreach (Match match in template.Matches(line))
		{
			if (match.Value == "do()")
			{
				@do = true;
			}
			else if (match.Value == "don't()")
			{
				@do = false;
			}
			else if (@do)
			{
				int op1 = int.Parse(match.Groups["op1"].Value);
				int op2 = int.Parse(match.Groups["op2"].Value);
				sum += op1 * (long)op2;
			}
		}
	}

	sum.Dump("Sum of multiplications");
}

void Main()
{
	part1();
	part2();
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
