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
const int day = 9;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	long sum = 0L;
	foreach (string line in input)
	{
		int[] history = line.Split().Select(int.Parse).ToArray();
		int last = history[^1];
		int extrapolation = 0;
		for (int i = 0; i < history.Length; i++)
		{
			int prev = history[i];
			for (int j = i; j < history.Length; j++)
			{
				int current = history[j] - prev;
				prev = history[j];
				history[j] = current;
			}
			
			extrapolation += history[^1];
		}
		
		sum += extrapolation + last;
	}
	
	sum.Dump("SUM OF EXTRAPOLATED VALUES");
}
void part2()
{
	long sum = 0L;
	foreach (string line in input)
	{
		int[] history = line.Split().Select(int.Parse).ToArray();
		for (int i = 0; i < history.Length; i++)
		{
			int prev = history[i];
			history[i] = history[i];
			for (int j = i + 1; j < history.Length; j++)
			{
				int current = history[j] - prev;
				prev = history[j];
				history[j] = current;
			}
		}
		
		long extrapolation = history[^1];
		for (int i = history.Length - 2; i >= 0; i--)
		{
			extrapolation = history[i] - extrapolation;
			
		}
		
		sum += extrapolation;
	}
	
	sum.Dump("SUM OF EXTRAPOLATED VALUES");
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
