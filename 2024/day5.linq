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
const int day = 5;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	int index = 0;
	Dictionary<int, HashSet<int>> rules = ReadSortingRules(ref index);
	long sum = 0L;
	//rules.Dump();
	for (int i = index + 1; i < input.Length; i++)
	{
		int[] update = input[i].Split(',').Select(int.Parse).ToArray();
		int[] updateSorted = new int[update.Length];
		update.CopyTo(updateSorted, 0);
		Array.Sort(updateSorted, (x, y) => rules.TryGetValue(x, out var set) && set.Contains(y) ? -1 : 1);

		Debug.Assert(update.Length % 2 == 1);

		bool sorted = true;
		for (int j = 0; j < update.Length; j++)
		{
			if (update[j] != updateSorted[j])
			{
				sorted = false;
			}
		}

		if (sorted)
		{
			//update.Dump();
			//updateSorted.Dump();
			sum += update[update.Length / 2];
		}
	}

	sum.Dump("Middle page sum");
}
void part2()
{
	int index = 0;
	Dictionary<int, HashSet<int>> rules = ReadSortingRules(ref index);
	long sum = 0L;
	for (int i = index + 1; i < input.Length; i++)
	{
		int[] update = input[i].Split(',').Select(int.Parse).ToArray();
		int[] updateSorted = new int[update.Length];
		update.CopyTo(updateSorted, 0);
		Array.Sort(updateSorted, (x, y) => rules.TryGetValue(x, out var set) && set.Contains(y) ? -1 : 1);

		bool sorted = true;
		for (int j = 0; j < update.Length; j++)
		{
			if (update[j] != updateSorted[j])
			{
				sorted = false;
			}
		}

		if (!sorted)
		{
			sum += updateSorted[update.Length / 2];
		}
	}

	sum.Dump("Middle page sum");
}

private Dictionary<int, HashSet<int>> ReadSortingRules(ref int inputIndex)
{
	Dictionary<int, HashSet<int>> rules = new();
	int i = inputIndex;
	for (; i < input.Length; i++)
	{
		if (input[i].Trim() == string.Empty)
		{
			break;
		}

		int[] numbers = input[i].Split('|').Select(int.Parse).ToArray();
		if (!rules.TryGetValue(numbers[0], out var set))
		{
			rules[numbers[0]] = new HashSet<int>();
		}

		rules[numbers[0]].Add(numbers[1]);
	}

	inputIndex = i;
	return rules;
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
