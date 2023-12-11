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
const int day = 11;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	List<List<char>> universe = new();
	foreach (var line in input)
	{
		bool empty = true;
		foreach (var tile in line)
		{
			if (tile == '#')
			{
				empty = false;
				break;
			}
		}
		
		var lineList = new List<char>(line);
		universe.Add(lineList);
		if (empty)
		{
			universe.Add(new List<char>(line));
		}
	}
	
	List<int> emptyCol = new();
	for (int col = 0; col < universe[0].Count; col++)
	{
		bool empty = true;
		for (int row = 0; row < universe.Count; row++)
		{
			if (universe[row][col] == '#')
			{
				empty = false;
				break;
			}
		}
		
		if (empty)
		{
			emptyCol.Add(col);
		}
	}
	
	foreach (var col in emptyCol.OrderByDescending(x => x))
	{
		ExpandColumn(universe, col);
	}
	
	List<(int row, int col)> galaxies = new();
	for (int row = 0; row < universe.Count; row++)
	{
		for (int col = 0; col < universe[0].Count; col++)
		{
			if (universe[row][col] == '#')
			{
				galaxies.Add((row, col));
			}
		}
	}
	
	long sum = 0L;
	for (int i = 0; i < galaxies.Count - 1; i++)
	{
		for (int j = i + 1; j < galaxies.Count; j++)
		{
			sum += GetLength(galaxies[i], galaxies[j]);
		}
	}
	
	sum.Dump("SUM OF SHORTEST LENGTHS BETWEEN GALAXIES");
}
void part2()
{
	List<List<char>> universe = new();
	List<int> emptyRow = new();
	List<int> emptyCol = new();
	for (int row = 0; row < input.Length; row++)
	{
		bool empty = true;
		for (int col = 0; col < input[0].Length; col++)
		{
			if (input[row][col] == '#')
			{
				empty = false;
				break;
			}
		}
		
		var lineList = new List<char>(input[row]);
		universe.Add(lineList);
		if (empty)
		{
			emptyRow.Add(row);
		}
	}
	
	for (int col = 0; col < universe[0].Count; col++)
	{
		bool empty = true;
		for (int row = 0; row < universe.Count; row++)
		{
			if (universe[row][col] == '#')
			{
				empty = false;
				break;
			}
		}
		
		if (empty)
		{
			emptyCol.Add(col);
		}
	}
	
	List<(int row, int col)> galaxies = new();
	for (int row = 0; row < universe.Count; row++)
	{
		for (int col = 0; col < universe[0].Count; col++)
		{
			if (universe[row][col] == '#')
			{
				galaxies.Add((row, col));
			}
		}
	}
	
	const int expandSize = 1_000_000 - 1;
	long sum = 0L;
	for (int i = 0; i < galaxies.Count - 1; i++)
	{
		for (int j = i + 1; j < galaxies.Count; j++)
		{
			sum += GetLength(galaxies[i], galaxies[j], emptyRow, emptyCol, expandSize);
		}
	}
	
	sum.Dump("SUM OF SHORTEST LENGTHS BETWEEN GALAXIES");
}

long GetLength((int row, int col) one, (int row, int col) another, List<int> emptyRow, List<int> emptyCol, int expandSize)
{
	long emptyRowsBetween = 0L;
	long emptyColsBetween = 0L;
	foreach (var empty in emptyRow)
	{
		if (empty > Math.Min(one.row, another.row) && empty < Math.Max(one.row, another.row))
		{
			emptyRowsBetween += expandSize;
		}
	}
	
	foreach (var empty in emptyCol)
	{
		if (empty > Math.Min(one.col, another.col) && empty < Math.Max(one.col, another.col))
		{
			emptyColsBetween += expandSize;
		}
	}
	
	return GetLength(one, another) + emptyRowsBetween + emptyColsBetween;
}

int GetLength((int row, int col) one, (int row, int col) another)
{
	return Math.Abs(one.row - another.row) + Math.Abs(one.col - another.col);
}

void Dump(List<List<char>> universe)
{
	foreach (var line in universe)
	{
		string.Join(string.Empty, line).Dump();
	}
}

List<List<char>> ExpandRow(List<List<char>> universe, int row)
{
	int len = universe[0].Count;
	universe.Insert(row, Enumerable.Range(0, len).Select(x => '.').ToList());
	return universe;
}

List<List<char>> ExpandColumn(List<List<char>> universe, int col)
{
	foreach (var row in universe)
	{
		row.Insert(col, '.');
	}
	
	return universe;
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
