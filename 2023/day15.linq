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
const int day = 15;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	string[] initializationSequence = input[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	long sum = 0L;
	foreach (string s in initializationSequence)
	{
		long currentValue = 0L;
		foreach (var code in Encoding.ASCII.GetBytes(s))
		{
			currentValue += code;
			currentValue *= 17;
			currentValue = currentValue % 256;
		}

		sum += currentValue;
	}

	sum.Dump("SUM OF HASHES");
}
void part2()
{
	string[] initializationSequence = input[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	const int size = 256;
	var boxes = new Box[size];
	for (int i = 0; i < size; i++)
	{
		boxes[i] = new Box();
	}

	foreach (string s in initializationSequence)
	{
		var (lens, operation, focalLength) = parse(s);
		int hash = getHashCode(lens);
		//(new { lens, operation, focalLength, hash}).ToString().Dump();
		if (operation == "-")
		{
			boxes[hash].Remove(lens);
		}
		else if (operation == "=")
		{
			boxes[hash].Add(lens, focalLength.Value);
		}
		else
		{
			throw new InvalidOperationException($"unknown operation {operation}");
		}
	}

	long power = 0L;
	for (int i = 0; i < size; i++)
	{
		foreach (var (slot, lens, focalLength) in boxes[i])
		{
			power += (1 + i) * slot * focalLength;
		}
	}
	
	power.Dump("FOCUSING POWER"); // 303404

	(string lens, string operation, int? focalLength) parse(string element)
	{
		int opStartIndex = 1;
		while (element[opStartIndex] != '=' && element[opStartIndex] != '-')
		{
			opStartIndex++;
		}
		
		string lens = element[0..opStartIndex];
		string operation = element[opStartIndex].ToString();
		if (int.TryParse(element.Substring(opStartIndex + 1), out int focalLength))
		{
			return (lens, operation, focalLength);
		}
		else
		{
			return (lens, operation, null);
		}
	}

	int getHashCode(string element)
	{
		int hash = 0;
		foreach (var code in Encoding.ASCII.GetBytes(element))
		{
			hash += code;
			hash *= 17;
			hash = hash % size;
		}

		return hash;
	}
}

public class Box : IEnumerable<(int slot, string lens, int focalLength)>
{
	private readonly Dictionary<string, LinkedListNode<(string, int)>> _v = new();
	private readonly LinkedList<(string, int)> _ll = new();

	public bool Add(string lens, int focalLength)
	{
		if (_v.TryGetValue(lens, out LinkedListNode<(string, int)>? node))
		{
			node.ValueRef = (lens, focalLength);
			return true;
		}
		else
		{
			var newNode = _ll.AddLast((lens, focalLength));
			_v[lens] = newNode;
			return false;
		}
	}
	
	public void Remove(string lens)
	{
		if (_v.Remove(lens, out LinkedListNode<(string, int)>? node))
		{
			_ll.Remove(node);
		}
	}

	public IEnumerator<(int slot, string lens, int focalLength)> GetEnumerator()
	{
		var node = _ll.First;
		int slot = 1;
		while (node != null)
		{
			var (lens, focalLength) = node.Value;
			yield return (slot, lens, focalLength);
			node = node.Next;
			slot++;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
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
