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
const int day = 2;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	int count = 0;
	foreach (string line in input)
	{
		int[] report = line
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(int.Parse)
			.ToArray();
		bool safe = true;
		bool increasing = report[0] < report[1];
		for (int i = 1; i < report.Length; i++)
		{
			int large = increasing ? report[i] : report[i - 1];
			int small = increasing ? report[i - 1] : report[i];
			int diff = large - small;
			if (diff >= 1 && diff <= 3)
			{
				continue;
			}

			safe = false;
			break;
		}

		count += safe ? 1 : 0;
	}

	count.Dump("Safe reports count");
}
void part2()
{
	int count = 0;
	foreach (string line in input)
	{
		int[] report = line
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(int.Parse)
			.ToArray();

		LinkedList<int> reportLinkedList = new(report);
		LinkedListNode<int> node = reportLinkedList.First ?? throw new ArgumentNullException("first is null");
		reportLinkedList.Remove(node);
		bool safe = isIncOrDec(reportLinkedList);
		reportLinkedList.AddFirst(node);
		LinkedListNode<int>? iterNode = reportLinkedList.First;
		while (iterNode != null && !safe)
		{
			LinkedListNode<int>? toDelete = iterNode.Next;
			if (toDelete is not null)
			{
				reportLinkedList.Remove(toDelete);
				safe = isIncOrDec(reportLinkedList);
				reportLinkedList.AddAfter(iterNode, toDelete);
			}

			iterNode = iterNode.Next;
		}

		count += safe ? 1 : 0;
	}

	count.Dump("Safe reports count");
}

static bool isIncOrDec(LinkedList<int> report)
{
	bool safe = true;
	LinkedListNode<int>? prev = report.First;
	LinkedListNode<int>? current = prev?.Next;
	// increasing
	while (current != null && prev != null)
	{
		int diff = current.Value - prev.Value;
		if (diff < 1 || diff > 3)
		{
			safe = false;
			break;
		}

		prev = current;
		current = current.Next;
	}

	if (safe)
	{
		return true;
	}

	safe = true;
	prev = report.First;
	current = prev?.Next;
	// decreasing
	while (current != null && prev != null)
	{
		int diff = prev.Value - current.Value;
		if (diff > 3 || diff < 1)
		{
			return false;
		}

		prev = current;
		current = current.Next;
	}

	return safe;
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
