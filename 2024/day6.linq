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
const int day = 6;
//static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

static int Rows => input.Length;
static int Cols => input[0].Length;
const char GUARD = '^';
const char OBSTACLE = '#';
record Direction(int dRow, int dCol)
{
	public Direction Next() => new Direction(dCol, -1 * dRow);
}
record Point(int Row, int Col);

void part1()
{
	Point? start = null;
	Direction dir = new(-1, 0); // North 
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			if (input[row][col] == GUARD)
			{
				start = new Point(row, col);
			}
		}
	}

	Debug.Assert(start is not null);
	Point pos = start with { };
	HashSet<Point> visited = new();
	visited.Add(pos);
	while (true)
	{
		Point next = new Point(pos.Row + dir.dRow, pos.Col + dir.dCol);
		if (!(next.Row >= 0 && next.Row < Rows && next.Col >= 0 && next.Col < Cols))
		{
			break;
		}

		if (input[next.Row][next.Col] == OBSTACLE)
		{
			dir = dir.Next();
		}

		pos = new Point(pos.Row + dir.dRow, pos.Col + dir.dCol);
		visited.Add(pos);
	}

	visited.Count.Dump("Visited positions count");
}
void part2()
{
	Point? start = null;
	Direction dir = new(-1, 0); // North 
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			if (input[row][col] == GUARD)
			{
				start = new Point(row, col);
			}
		}
	}

	Debug.Assert(start is not null);
	Point pos = start with { };
	HashSet<(Point, Direction)> visited = new();
	HashSet<Point> loops = new();
	visited.Add((pos, dir));
	while (true)
	{
		Point next = new Point(pos.Row + dir.dRow, pos.Col + dir.dCol);
		if (!(next.Row >= 0 && next.Row < Rows && next.Col >= 0 && next.Col < Cols))
		{
			break;
		}

		if (input[next.Row][next.Col] == OBSTACLE)
		{
			dir = dir.Next();
		}
		else // assume rotate
		{
			Direction nextDir = dir.Next();
			HashSet<(Point, Direction)> visitedCopy = new HashSet<(Point, Direction)>(visited);
			if (SearchLoops(pos, nextDir, visitedCopy, loops))
			{
				loops.Add(next);
			}
		}

		pos = new Point(pos.Row + dir.dRow, pos.Col + dir.dCol);
		visited.Add((pos, dir));
	}

	loops.Count.Dump("Possible loops count");
	PrintMap(loops);
}

bool SearchLoops(Point initial, Direction dir, HashSet<(Point p, Direction dir)> visited, HashSet<Point> loops)
{
	Point pos = initial with { };
	while (true)
	{
		Point next = new Point(initial.Row + dir.dRow, initial.Col + dir.dCol);
		if (!(next.Row >= 0 && next.Row < Rows && next.Col >= 0 && next.Col < Cols))
		{
			break;
		}
		
		if (visited.Contains((next, dir)))
		{
			return true;
		}
		
		if (input[next.Row][next.Col] == OBSTACLE)
		{
			dir = dir.Next();
		}
		
		visited.Add((next, dir));
	}
	
	return false;
}

void PrintMap(HashSet<Point> loops)
{
	for (int row = 0; row < Rows; row++)
	{
		char[] mut = input[row].ToCharArray();
		for (int col = 0; col < Cols; col++)
		{
			if (loops.Contains(new Point(row, col)))
			{
				mut[col] = 'O';
			}
		}

		(new string(mut)).Dump();
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
