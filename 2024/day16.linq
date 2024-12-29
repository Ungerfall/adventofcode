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
const int day = 16;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

record Point(int Row, int Col);
record Direction(int dRow, int dCol)
/*
	up: -1, 0
	right: 0, 1
	down: 1, 0
	left: 0, -1
	clockwise: up->right->down->left->up
	counter-clockwise: up->left->down->right->up
*/
{
	public Direction Clockwise() => new Direction(dCol, -1 * dRow);
	public Direction CounterClockwise() => new Direction(-1 * dCol, dRow);
	public int Numeric => (dRow, dCol) switch
	{
		(-1, 0) => 0,
		(0, 1) => 1,
		(1, 0) => 2,
		(0, -1) => 3,
		_ => -1
	};
	public char Symbol => (dRow, dCol) switch
	{
		(-1, 0) => '^',
		(0, 1) => '>',
		(1, 0) => '˅',
		(0, -1) => '<',
		_ => '*'
	};
}
record Reindeer(Point Pos, Direction Dir);
record MazePath(Reindeer Reindeer, long Score);

int Rows => input.Length;
int Cols => input[0].Length;

const int ROTATION_SCORE = 1000;
void part1()
{
	var (start, end) = InitMap();
	long[,,] best = new long[input.Length, input[0].Length, 4];
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			for (int dir = 0; dir < 4; dir++)
			{
				best[row, col, dir] = long.MaxValue;
			}
		}
	}

	Direction right = new Direction(0, 1);
	best[start.Row, start.Col, right.Numeric] = 0;
	var dijkstra = new PriorityQueue<(Point pos, Direction dir), long>();
	dijkstra.Enqueue((start, right), priority: 0);
	long shortest = long.MaxValue;
	while (dijkstra.Count > 0)
	{
		var (pos, dir) = dijkstra.Dequeue();
		var (row, col) = pos;
		if (pos == end)
		{
			shortest = best[row, col, dir.Numeric];
			break;
		}

		foreach (var (nextDir, rotationScore) in new[]
			{
				(dir, 0),
				(dir.Clockwise(), ROTATION_SCORE),
				(dir.CounterClockwise(), ROTATION_SCORE),
			})
		{
			Point next = pos with { Row = pos.Row + nextDir.dRow, Col = pos.Col + nextDir.dCol };
			if (!CheckBounderies(next))
			{
				continue;
			}

			long newDist = best[row, col, dir.Numeric] + rotationScore + 1;
			if (newDist < best[next.Row, next.Col, nextDir.Numeric])
			{
				best[next.Row, next.Col, nextDir.Numeric] = newDist;
				dijkstra.Enqueue((next, nextDir), priority: newDist);
			}
		}
	}

	string[,] map = new string[Rows, Cols];
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			if (input[row][col] == WALL)
			{
				map[row, col] = WALL.ToString();
			}
			else
			{
				map[row, col] = (
					Math.Min(best[row, col, 0],
						Math.Min(best[row, col, 1],
							Math.Min(best[row, col, 2], best[row, col, 3])))
				).ToString();
			}
		}
	}

	shortest.Dump("The lowest score");
}

const char WALL = '#';
bool CheckBounderies(Point p)
{
	return p.Row >= 0 && p.Row < Rows
		&& p.Col >= 0 && p.Col < Cols
		&& input[p.Row][p.Col] != WALL;
}


(Point start, Point end) InitMap()
{
	Point? start = null;
	Point? end = null;
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			char tile = input[row][col];
			if (tile == 'S')
			{
				start = new Point(row, col);
			}
			else if (tile == 'E')
			{
				end = new Point(row, col);
			}
		}
	}

	Debug.Assert(start is not null);
	Debug.Assert(end is not null);

	return (start, end);
}

void part2()
{
	var (start, end) = InitMap();
	long[,,] best = new long[Rows, Cols, 4];
	HashSet<(int row, int col, Direction dir)>[,,] parent = new HashSet<(int, int, Direction)>[Rows, Cols, 4];
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			for (int dir = 0; dir < 4; dir++)
			{
				best[row, col, dir] = long.MaxValue;
				parent[row, col, dir] = new HashSet<(int, int, Direction)>();
			}
		}
	}

	Direction right = new Direction(0, 1);
	best[start.Row, start.Col, right.Numeric] = 0;
	var dijkstra = new PriorityQueue<(Point pos, Direction dir), long>();
	dijkstra.Enqueue((start, right), priority: 0);
	long shortest = long.MaxValue;
	long safeSpots = 0;
	while (dijkstra.Count > 0)
	{
		var (pos, dir) = dijkstra.Dequeue();
		var (row, col) = pos;
		if (pos == end)
		{
			safeSpots = calculateSafeSpots(parent, start, end, dir);
			shortest = best[row, col, dir.Numeric];
			break;
		}

		//(new { up = best[7, 5, 0], right = best[7, 5, 1], down = best[7, 5, 2], left = best[7, 5, 3] }).Dump();

		foreach (var (nextDir, rotationScore) in new[]
			{
				(dir, 0),
				(dir.Clockwise(), ROTATION_SCORE),
				(dir.CounterClockwise(), ROTATION_SCORE),
			})
		{
			Point next = pos with { Row = pos.Row + nextDir.dRow, Col = pos.Col + nextDir.dCol };
			if (!CheckBounderies(next))
			{
				continue;
			}

			long newDist = best[row, col, dir.Numeric] + rotationScore + 1;
			if (newDist == best[next.Row, next.Col, nextDir.Numeric])
			{
				parent[next.Row, next.Col, nextDir.Numeric].Add((row, col, dir));
			}
			else if (newDist < best[next.Row, next.Col, nextDir.Numeric])
			{
				best[next.Row, next.Col, nextDir.Numeric] = newDist;
				parent[next.Row, next.Col, nextDir.Numeric].Add((row, col, dir));
				dijkstra.Enqueue((next, nextDir), priority: newDist);
			}
		}
	}

	shortest.Dump("The lowest score");
	safeSpots.Dump("Safe spots");
}

long calculateSafeSpots(HashSet<(int row, int col, Direction dir)>[,,] parent, Point start, Point end, Direction endDir)
{
	char[,] path = new char[Rows, Cols];
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			path[row, col] = input[row][col];
		}
	}

	Queue<(int row, int col, Direction dir)> q = new();
	var p = parent[end.Row, end.Col, endDir.Numeric];
	foreach (var element in p)
	{
		q.Enqueue(element);
	}

	HashSet<Point> safe = new();
	safe.Add(end);
	while (q.Count > 0)
	{
		var (row, col, dir) = q.Dequeue();
		safe.Add(new(row, col));
		path[row, col] = 'O';
		foreach (var element in parent[row, col, dir.Numeric])
		{
			q.Enqueue(element);
		}
	}

	path[start.Row, start.Col] = 'S';
	path[end.Row, end.Col] = 'E';
	path.Dump();

	return safe.Count;
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
