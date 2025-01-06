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
const int day = 20;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

static int Rows = input.Length;
static int Cols = input[0].Length;

const char WALL = '#';
const char EMPTY = '.';
const char END = 'E';

record Point(int Row, int Col);
void part1()
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
				start = new(row, col);
			}

			if (tile == END)
			{
				end = new(row, col);
			}
		}
	}

	Debug.Assert(start is not null);
	Debug.Assert(end is not null);

	var shortest = GetShortestPath(start, end);
	int cleanScore = shortest.Count - 1;
	const int savingsGoal = 100;
	int count = 0;
	foreach (var (pos, score, dRow, dCol) in shortest
		.Where(x => cleanScore - x.Value >= savingsGoal)
		.SelectMany(
			x => new[] { (0, -1), (-1, 0), (0, 1), (1, 0) },
			(kv, tcol) => (kv.Key, kv.Value, tcol.Item1, tcol.Item2)
		))
	{
		Point next = pos with { Row = pos.Row + dRow, Col = pos.Col + dCol };
		if (next.Row < 0 || next.Row >= Rows || next.Col < 0 || next.Col >= Cols || input[next.Row][next.Col] != WALL)
		{
			continue;
		}

		Point next_next = next with { Row = next.Row + dRow, Col = next.Col + dCol };
		if (!CheckBoundaries(next_next) || input[next_next.Row][next_next.Col] == WALL)
		{
			continue;
		}

		var cheat = GetShortestPath(next_next, end);
		int cheatScore = cheat.Count + score + 1; // +1 for a jump
		int saved = cleanScore - cheatScore;
		if (saved >= savingsGoal)
		{
			//(cheatScore, saved, score, pos, next_next).ToString().Dump();
			count++;
		}
	}

	count.Dump($"Saved at least {savingsGoal} cheats");
}
void part2()
{
	/*
	List<(int score, int cheatsLeft)> paths = new();
	bool[,,] visited = new bool[Rows, Cols, 2];
	int?[,,] memo = new int?[Rows, Cols, 2];
	int? cleanRun = null;
	traverse(start, score: 0, cheatsLeft: 1);
	const int savingsGoal = 2;

	void traverse(Point pos, int score, int cheatsLeft)
	{
		if (pos == end)
		{
			if (cheatsLeft == 1)
			{
				cleanRun = Math.Min(cleanRun ?? int.MaxValue, score);
			}

			paths.Add((score, cheatsLeft));
			return;
		}

		if (cleanRun is not null && score + savingsGoal > cleanRun)
		{
			return;
		}
		
		int? cached = memo[pos.Row, pos.Col, cheatsLeft];
		if (cached is not null)
		{
						
		}

		visited[pos.Row, pos.Col, cheatsLeft] = true;
		foreach (var (dRow, dCol) in new[]
			{
				(-1, 0),
				(0, 1),
				(1, 0),
				(0, -1)
			})
		{
			Point next = pos with { Row = pos.Row + dRow, Col = pos.Col + dCol };
			if (next.Row >= 0 && next.Row < Rows
				&& next.Col >= 0 && next.Col < Cols
				&& input[next.Row][next.Col] != WALL
				&& !visited[next.Row, next.Col, cheatsLeft])
			{
				traverse(next, score + 1, cheatsLeft);
			}
		}
		
		foreach (var (dRow, dCol) in new[]
			{
				(-1, 0),
				(0, 1),
				(1, 0),
				(0, -1)
			})
		{
			Point next = pos with { Row = pos.Row + dRow, Col = pos.Col + dCol };
			if (cheatsLeft > 0 && input[next.Row][next.Col] == WALL)
			{
				Point next_next = next with { Row = next.Row + dRow, Col = next.Col + dCol };
				if (next_next.Row >= 0 && next_next.Row < Rows
					&& next_next.Col >= 0 && next_next.Col < Cols
					&& (input[next_next.Row][next_next.Col] == EMPTY || input[next_next.Row][next_next.Col] == END)
					&& !visited[next_next.Row, next_next.Col, cheatsLeft - 1])
				{
					traverse(next_next, score + 2, cheatsLeft - 1);
				}
			}
		}

		visited[pos.Row, pos.Col, cheatsLeft] = false;
	}

	paths.Dump();
	Debug.Assert(cleanRun is not null);
	int count = 0;
	foreach (var path in paths)
	{
		if (cleanRun - path.score >= savingsGoal)
		{
			count++;
		}
	}
	*/
}

static bool CheckBoundaries(Point pos)
{
	return pos.Row >= 0 && pos.Row < Rows
		&& pos.Col >= 0 && pos.Col < Cols;
}

static Dictionary<Point, int> GetShortestPath(Point start, Point destination)
{
	Point?[,] parent = new Point?[Rows, Cols];
	int[,] best = new int[Rows, Cols];
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			best[row, col] = int.MaxValue;
		}
	}

	best[start.Row, start.Col] = 0;
	var dijkstra = new PriorityQueue<Point, long>();
	dijkstra.Enqueue(start, priority: 0);
	while (dijkstra.Count > 0)
	{
		var pos = dijkstra.Dequeue();
		var (row, col) = pos;
		if (pos == destination)
		{
			break;
		}

		foreach (var (dRow, dCol) in new[]
			{
				(0, -1),
				(-1, 0),
				(0, 1),
				(1, 0)
			})
		{
			Point next = pos with { Row = pos.Row + dRow, Col = pos.Col + dCol };
			if (next.Row >= Rows || next.Row < 0
				|| next.Col >= Cols || next.Col < 0
				|| input[next.Row][next.Col] == WALL)
			{
				continue;
			}

			int newDist = best[pos.Row, pos.Col] + 1;
			if (newDist < best[next.Row, next.Col])
			{
				best[next.Row, next.Col] = newDist;
				parent[next.Row, next.Col] = pos;
				dijkstra.Enqueue(next, newDist);
			}
		}
	}

	Dictionary<Point, int> path = new();
	path[destination] = best[destination.Row, destination.Col];
	Point? prev = parent[destination.Row, destination.Col];
	while (prev != null)
	{
		path[prev] = best[prev.Row, prev.Col];
		prev = parent[prev.Row, prev.Col];
	}

	return path;
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
