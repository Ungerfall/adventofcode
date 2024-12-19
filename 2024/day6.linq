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
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample-1.txt"));

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
	HashSet<(Point, Direction)> path = new();
	visited.Add(pos);
	path.Add((pos, dir));
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
		path.Add((pos, dir));
	}

	visited.Count.Dump("Visited positions count");
	//PrintMap(path, start);
}
void part2()
{
	Point? start = null;
	Direction north = new(-1, 0); // North 
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
	HashSet<(Point, Direction)> visited = new();
	HashSet<Point> loops = new();
	Queue<(Point, Direction)> bfs = new();
	bfs.Enqueue((start, north));
	while (bfs.Count > 0)
	{
		var (pos, dir) = bfs.Dequeue();
		visited.Add((pos, dir));
		Point next = new Point(pos.Row + dir.dRow, pos.Col + dir.dCol);
		if (!(next.Row >= 0 && next.Row < Rows && next.Col >= 0 && next.Col < Cols))
		{
			break;
		}

		if (input[next.Row][next.Col] == OBSTACLE)
		{
			dir = dir.Next();
			next = pos;
		}
		else
		{
			var searchLoopDir = dir.Next();
			var searchLoopVisited = new HashSet<(Point, Direction)>(visited);
			SearchLoops(pos, searchLoopDir, searchLoopVisited, loops, start, next);
		}

		bfs.Enqueue((next, dir));
	}

	visited.Select(x => x.Item1).ToHashSet().Count.Dump("Visited position count Part 1");
	loops.Count.Dump("Possible loops count");
	//PrintMap(visited, start);
}

void part2_bruteforce()
{
	Point? start = null;
	Direction north = new(-1, 0); // North 
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
	HashSet<Point> loops = new();
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			HashSet<(Point, Direction)> visited = new();
			SearchLoops(start, north, visited, loops, start, new Point(row, col));
		}
	}

	loops.Count.Dump("Possible loops count");
	//PrintMap(visited, start);
}

void SearchLoops(Point initial, Direction initialDir, HashSet<(Point p, Direction dir)> visited, HashSet<Point> loops, Point start, Point obstruction)
{
	Queue<(Point, Direction)> bfs = new();
	bfs.Enqueue((initial, initialDir));
	bool foundLoop = false;
	while (bfs.Count > 0)
	{
		var (pos, dir) = bfs.Dequeue();
		if (visited.Contains((pos, dir)))
		{
			// found a loop
			loops.Add(obstruction);
			foundLoop = true;
			break;
		}

		visited.Add((pos, dir));

		Point next = new Point(pos.Row + dir.dRow, pos.Col + dir.dCol);
		if (!(next.Row >= 0 && next.Row < Rows && next.Col >= 0 && next.Col < Cols))
		{
			break;
		}

		if (input[next.Row][next.Col] == OBSTACLE || next == obstruction)
		{
			dir = dir.Next();
			next = pos;
		}

		bfs.Enqueue((next, dir));
	}

	if (foundLoop || obstruction is { Row: 8, Col: 1 })
	{
		//PrintMap(visited, start, obstruction);
	}
}

void PrintMap(HashSet<(Point, Direction)> path, Point start, Point? obstruction = null)
{
	Dictionary<Point, HashSet<Direction>> p = new();
	foreach (var (pos, dir) in path)
	{
		if (p.TryGetValue(pos, out var dirs))
		{
			dirs.Add(dir);
		}
		else
		{
			p[pos] = new HashSet<Direction>();
			p[pos].Add(dir);
		}
	}

	start.Dump("Map");
	for (int row = 0; row < Rows; row++)
	{
		char[] mut = input[row].ToCharArray();
		for (int col = 0; col < Cols; col++)
		{
			Point pos = new(row, col);
			if (obstruction is not null && obstruction == pos)
			{
				mut[col] = 'O';
			}
			else if (pos == start)
			{
				mut[col] = '^';
			}
			else if (p.TryGetValue(new Point(row, col), out HashSet<Direction>? dirs))
			{
				mut[col] = dirs.ToList() switch
				{
					{ Count: > 1 and <= 4 } => '*',
					[{ dRow: -1, dCol: 0 }] => '↑',
					[{ dRow: 1, dCol: 0 }] => '↓',
					[{ dRow: 0, dCol: -1 }] => '←',
					[{ dRow: 0, dCol: 1 }] => '→',
					_ => throw new InvalidOperationException("wtf was that"),
				};
			}
		}

		(new string(mut)).Dump();
	}
}

void Main()
{
	part1();
	part2();
	part2_bruteforce();
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
