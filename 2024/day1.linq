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
const int day = 1;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	var (left, right) = Parse();

	left.Sort();
	right.Sort();

	long sum = 0L;
	for (int i = 0; i < left.Count; i++)
	{
		sum += Math.Abs(left[i] - right[i]);
	}

	sum.Dump("Total distance");
}
void part2()
{
	var (left, right) = Parse();

	Dictionary<int, int> rightCounter = new();
	for (int i = 0; i < right.Count; i++)
	{
		if (rightCounter.TryGetValue(right[i], out int count))
		{
			rightCounter[right[i]] = count + 1;
		}
		else
		{
			rightCounter[right[i]] = 1;
		}
	}

	long similarity = 0L;
	for (int i = 0; i < left.Count; i++)
	{
		if (rightCounter.TryGetValue(left[i], out int count))
		{
			similarity += left[i] * (long)count;
		}
	}

	similarity.Dump("Similarity score");
}

(List<int> left, List<int> right) Parse()
{
	List<int> left = new();
	List<int> right = new();
	foreach (string line in input)
	{
		int[] lineNumbers = line
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(int.Parse)
			.ToArray();
		left.Add(lineNumbers[0]);
		right.Add(lineNumbers[1]);
	}

	return (left, right);
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
