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
const int day = 5;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
public record DicEntry(string id, List<(long destination, long source, long range)> values);
void part1()
{
	var dictionaries = new Dictionary<string, DicEntry>();
	var (blocks, current) = input.Aggregate(
		seed: (blocks: new List<List<string>>(), current: new List<string>()),
		(acc, line) =>
		{
			if (string.Equals(line, string.Empty))
			{
				acc.blocks.Add(acc.current);
				return (acc.blocks, new List<string>());
			}
			else
			{
				acc.current.Add(line);
				return (acc.blocks, acc.current);
			}
		});
	blocks.Add(current);
	var split = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
	long[] seeds = blocks[0][0].Split(':')[1].Trim().Split(' ', split).Select(long.Parse).ToArray();

	var template = new Regex(@"^(\w+)-to-(\w+) map:$", RegexOptions.Compiled);
	foreach (var block in blocks.Skip(1))
	{
		var match = template.Match(block[0]);
		var source = match.Groups[1].Value;
		var destination = match.Groups[2].Value;
		dictionaries[source] = new(
			destination,
			values: block
				.Skip(1)
				.Select(x =>
				{
					long[] parts = x.Split(' ', split).Select(long.Parse).ToArray();
					return (parts[0], parts[1], parts[2]);
				})
				.ToList());
	}

	long lowest = int.MaxValue;
	foreach (var seed in seeds)
	{
		long destination = seed;
		string key = "seed";
		while (dictionaries.TryGetValue(key, out DicEntry? dict))
		{
			long prevDestination = destination;
			foreach (var (d, s, r) in dict.values)
			{
				if (destination >= s && destination < s + r)
				{
					destination = d + (destination - s);
					break;
				}
			}

			//(new { key, dict.id, prevDestination, destination}).ToString().Dump();

			key = dict.id;
		}

		//destination.Dump($"LOCATION OF {seed}:");
		lowest = Math.Min(lowest, destination);
	}

	lowest.Dump("LOWEST");
}
void part2()
{
	var dictionaries = new Dictionary<string, DicEntry>();
	var (blocks, current) = input.Aggregate(
		seed: (blocks: new List<List<string>>(), current: new List<string>()),
		(acc, line) =>
		{
			if (string.Equals(line, string.Empty))
			{
				acc.blocks.Add(acc.current);
				return (acc.blocks, new List<string>());
			}
			else
			{
				acc.current.Add(line);
				return (acc.blocks, acc.current);
			}
		});
	blocks.Add(current);
	var split = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
	long[][] seedRanges = blocks[0][0]
		.Split(':')[1].Trim()
		.Split(' ', split).Select(long.Parse)
		.Chunk(2).ToArray();

	var template = new Regex(@"^(\w+)-to-(\w+) map:$", RegexOptions.Compiled);
	foreach (var block in blocks.Skip(1))
	{
		var match = template.Match(block[0]);
		var source = match.Groups[1].Value;
		var destination = match.Groups[2].Value;
		dictionaries[source] = new(
			destination,
			values: block
				.Skip(1)
				.Select(x =>
				{
					long[] parts = x.Split(' ', split).Select(long.Parse).ToArray();
					return (parts[0], parts[1], parts[2]);
				})
				.ToList());
	}

	long lowest = int.MaxValue;
	foreach (var seed in seedRanges)
	{
		seed.Dump();
		for (long i = seed[0]; i < checked(seed[0] + seed[1]); i++)
		{
			long destination = i;
			string key = "seed";
			while (dictionaries.TryGetValue(key, out DicEntry? dict))
			{
				long prevDestination = destination;
				foreach (var (d, s, r) in dict.values)
				{
					if (destination >= s && destination < s + r)
					{
						destination = d + (destination - s);
						break;
					}
				}

				//(new { key, dict.id, prevDestination, destination}).ToString().Dump();

				key = dict.id;
			}

			//destination.Dump($"LOCATION OF {seed}:");
			lowest = Math.Min(lowest, destination);

		}
	}

	lowest.Dump("LOWEST");
}

void Main()
{
	part1(); // 214922730
	part2(); // 148041808
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
