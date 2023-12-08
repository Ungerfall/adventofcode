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
const int day = 8;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	string instructions = input[0];
	Dictionary<string, (string left, string right)> network = new();
	foreach (var node in input[2..])
	{
		string[] split = node.Split('=');
		var key = split[0].Trim();
		split = split[1].Trim().Split(',');
		string left = split[0][1..];
		string right = split[1][1..^1];
		network[key] = (left, right);
	}

	string current = "AAA";
	string destination = "ZZZ";
	int steps = 0;
	foreach (var instruction in new InfiniteIterator<char>(instructions.ToList()))
	{
		var (left, right) = network[current];
		string cold = current;
		current = instruction == 'L' ? left : right;
		steps++;
		//(new {instruction, left, right, cold, current, steps}).ToString().Dump();
		if (current == destination)
		{
			break;
		}
	}

	steps.Dump("TO REACH ZZZ");
}
void part2()
{
	string instructions = input[0];
	Dictionary<string, (string left, string right)> network = new();
	foreach (var node in input[2..])
	{
		string[] split = node.Split('=');
		var key = split[0].Trim();
		split = split[1].Trim().Split(',');
		string left = split[0][1..];
		string right = split[1][1..^1];
		network[key] = (left, right);
	}

	string[] endsOnA = network.Keys.Where(x => x.EndsWith('A')).ToArray();
	int[] stepsForA = new int[endsOnA.Length];
	for (int i = 0; i < endsOnA.Length; i++)
	{
		int steps = 0;
		string current = endsOnA[i];
		foreach (var instruction in new InfiniteIterator<char>(instructions.ToList()))
		{
			var (left, right) = network[current];
			string cold = current;
			current = instruction == 'L' ? left : right;
			steps++;
			//(new {instruction, left, right, cold, current, steps}).ToString().Dump();
			if (current.EndsWith('Z'))
			{
				break;
			}
		}
		
		stepsForA[i] = steps;
	}

	var lcm = LCM(stepsForA);
	lcm.Dump("TO REACH ZZZ");
}

void Main()
{
	part1();
	part2();
}

public static long LCM(IList<int> list)
{
	long lcm = list[0];
	for (int i = 1; i < list.Count; i++)
	{
		lcm = LCM(lcm, list[i]);
	}
	return lcm;
}
public static long LCM(long a, long b)
{
	return checked(Math.Abs(a * b) / GCD(a, b));
}
public static long GCD(long a, long b)
{
	if (a == 0)
		return b;
	return GCD(b % a, a);
}

public class InfiniteIterator<T> : IEnumerable<T>
{
	private IList<T> list;

	public InfiniteIterator(IList<T> list)
	{
		this.list = list;
	}

	public IEnumerator<T> GetEnumerator()
	{
		int index = 0;
		while (true)
		{
			yield return this.list[index];
			index++;
			if (index >= this.list.Count)
			{
				index = 0;
			}
		}
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
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
