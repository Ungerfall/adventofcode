<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
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
const int day = 23;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	Dictionary<string, HashSet<string>> connections = new();
	foreach (string line in input)
	{
		string[] parts = line.Split('-');
		string from = parts[0];
		string to = parts[1];
		if (!connections.TryGetValue(from, out var _))
		{
			connections[from] = new HashSet<string>();
		}

		if (!connections.TryGetValue(to, out var _))
		{
			connections[to] = new HashSet<string>();
		}

		connections[from].Add(to);
		connections[to].Add(from);
	}

	HashSet<OrderedDictionary<string, object>> loops = new(new UniqueLoop());
	Queue<(string computer, OrderedDictionary<string, object> network)> bfs = new();
	foreach (var computer in connections.Keys)
	{
		OrderedDictionary<string, object> network = new(capacity: 3);
		network.Add(computer, computer);
		bfs.Enqueue((computer, network));
	}

	while (bfs.Count > 0)
	{
		var (computer, network) = bfs.Dequeue();
		if (network.Count == 3)
		{
			// check if last has connection to first
			if (connections[computer].Contains(network.GetAt(0).Key))
			{
				loops.Add(network);
			}

			continue;
		}

		foreach (var to in connections[computer])
		{
			OrderedDictionary<string, object> newNetwork = new(network);
			if (newNetwork.TryAdd(to, to))
			{
				bfs.Enqueue((to, newNetwork));
			}
		}
	}

	loops
		.Count(x => x.Keys.Count(y => y.StartsWith('t')) > 0)
		.Dump("Loops with t containing computer");
}

void part2()
{
	foreach (string line in input)
	{

	}
}

public class UniqueLoop : IEqualityComparer<OrderedDictionary<string, object>>
{
	public bool Equals(OrderedDictionary<string, object>? x, OrderedDictionary<string, object>? y)
	{
		if (ReferenceEquals(x, y))
		{
			return true;
		}

		if (x is null || y is null || x.Count != y.Count)
		{
			return false;
		}

		return !x.Keys.Except(y.Keys).Any();
	}

	public int GetHashCode([DisallowNull] OrderedDictionary<string, object> obj)
	{
		int element1 = obj.GetAt(0).Key.GetHashCode();
		int element2 = obj.GetAt(1).Key.GetHashCode();
		int element3 = obj.GetAt(2).Key.GetHashCode();

		return element1 ^ element2 ^ element3;
	}
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
