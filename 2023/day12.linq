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
const int day = 12;
//string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	long totalArrangements = 0L;
	foreach (string line in input)
	{
		string[] puzzleAndKey = line.Split();
		string puzzle = puzzleAndKey[0];
		int[] key = puzzleAndKey[1].Split(',').Select(int.Parse).ToArray();
		Stack<(string, int)> paths = new();
		paths.Push((puzzle, 0));
		while (paths.Count > 0)
		{
			var (p, i) = paths.Pop();
			if (i >= p.Length)
			{
				totalArrangements += IsValid(p, key) ? 1 : 0;
				continue;
			}

			while (i < p.Length && p[i] != '?')
			{
				i++;
			}

			if (i >= p.Length)
			{
				paths.Push((p, i));
			}
			else
			{
				paths.Push((Replace(p, i, '#'), i));
				paths.Push((Replace(p, i, '.'), i));
			}
		}
	}

	totalArrangements.Dump("TOTAL ARRANGEMENTS");
}
void part2()
{
	long totalArrangements = 0L;
	foreach (var line in input)
	{
		string[] puzzleAndKey = line.Split();
		string puzzle = string.Join('?', Enumerable.Repeat(puzzleAndKey[0], 5));
		int[] key = Enumerable
			.Repeat(puzzleAndKey[1].Split(',').Select(int.Parse), 5)
			.SelectMany(x => x)
			.ToArray();
		Stack<(string, int)> paths = new();
		paths.Push((puzzle, 0));
		while (paths.Count > 0)
		{
			var (p, i) = paths.Pop();
			if (i >= p.Length)
			{
				totalArrangements += IsValid(p, key) ? 1 : 0;
				continue;
			}

			bool stop = false;
			while (i < p.Length && p[i] != '?')
			{
				if (p[i] == '.' && IsValid(p, i, key))
				{
					
				}
				
				i++;
			}
			
			if (stop)
			{
				continue;
			}

			if (i >= p.Length)
			{
				paths.Push((p, i));
			}
			else
			{
				paths.Push((Replace(p, i, '#'), i));
				paths.Push((Replace(p, i, '.'), i));
			}
		}
	}

	totalArrangements.Dump("TOTAL ARRANGEMENTS");
}

public string Replace(string s, int index, char newValue)
{
	if (index >= s.Length)
	{
		return s;
	}

	char[] mutable = s.ToCharArray();
	mutable[index] = newValue;
	return new string(mutable);
}
public bool IsValid(string candidate, int index, IReadOnlyList<int> key)
{
	
}
public bool IsValid(string candidate, IReadOnlyList<int> key)
{
	int[] parts = candidate
		.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		.Select(x => x.Length)
		.ToArray();
	if (parts.Length != key.Count)
	{
		return false;
	}

	for (int i = 0; i < key.Count; i++)
	{
		if (parts[i] != key[i])
		{
			return false;
		}
	}

	return true;
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
