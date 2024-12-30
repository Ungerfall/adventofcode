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
const int day = 18;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

int Rows = 71;
int Cols = 71;
const char SAFE = '.';
const char CORRUPTED = '#';

record Point(int Row, int Col);
record Direction(int dRow, int dCol)
/*
	up: -1, 0
	right: 0, 1
	down: 1, 0
	left: 0, -1
*/
{
	public Direction RotateClockwise() => new Direction(dCol, -1 * dRow);
	public static Direction RIGHT = new Direction(0, 1);
}
void part1()
{
	char[,] grid = new char[Rows, Cols];
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			grid[row, col] = SAFE;
		}
	}

	foreach (string line in input.Take(1024))
	{
		int[] coords = line.Trim().Split(',').Select(int.Parse).ToArray();
		grid[coords[0], coords[1]] = CORRUPTED;
	}

	Point start = new(0, 0);
	Point end = new(Rows - 1, Cols - 1);
	Queue<(Point pos, long score, Direction dir)> bfs = new();
	bfs.Enqueue((start, 0, Direction.RIGHT));
	HashSet<Point> seen = new();
	long best = long.MaxValue;
	while (bfs.Count > 0)
	{
		var (pos, score, dir) = bfs.Dequeue();
		if (pos == end)
		{
			best = Math.Min(best, score);
			continue;
		}

		if (seen.Contains(pos))
		{
			continue;
		}

		seen.Add(pos);
		foreach (Direction nextDir in new[]
			{
				dir,
				dir.RotateClockwise(),
				dir.RotateClockwise().RotateClockwise().RotateClockwise(),
			})
		{
			Point next = pos with { Row = pos.Row + nextDir.dRow, Col = pos.Col + nextDir.dCol };
			if (next.Row < 0 || next.Row >= Rows
				|| next.Col < 0 || next.Col >= Cols
				|| grid[next.Row, next.Col] == CORRUPTED)
			{
				continue;
			}

			bfs.Enqueue((next, score + 1, nextDir));
		}
	}

	best.Dump("the minimum number of steps");
}
void part2()
{
	foreach (string line in input)
	{

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
