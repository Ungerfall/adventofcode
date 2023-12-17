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
const int day = 16;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
static char[][] cave = input.Select(x => x.ToCharArray()).ToArray();
static int maxRow = cave.Length;
static int maxCol = cave[0].Length;
public enum Direction
{
	Right = 1,
	Down = 2,
	Left = 3,
	Up = 4,
}
const char EMPTY = '.';
const char MIRROR = '/';
const char BACK_MIRROR = '\\';
const char FLAT_SPLITTER = '-';
const char WALL_SPLITTER = '|';

void part1()
{
	int startRow = 0;
	int startCol = 0;
	Direction startDir = Direction.Right;
	int energizedCount = GetEnergized(startDir, startRow, startCol);
	energizedCount.Dump("ENERGIZED");
}
void part2()
{
	int maxEnergized = 0;
	for (int col = 0; col < maxCol; col++)
	{
		Direction down = Direction.Down;
		maxEnergized = Math.Max(maxEnergized, GetEnergized(down, 0, col));
	}
	
	for (int col = 0; col < maxCol; col++)
	{
		Direction up = Direction.Up;
		maxEnergized = Math.Max(maxEnergized, GetEnergized(up, maxRow - 1, col));
	}
	
	for (int row = 0; row < maxRow; row++)
	{
		Direction right = Direction.Right;
		maxEnergized = Math.Max(maxEnergized, GetEnergized(right, row, 0));
	}
	
	for (int row = 0; row < maxRow; row++)
	{
		Direction left = Direction.Left;
		maxEnergized = Math.Max(maxEnergized, GetEnergized(left, row, maxCol - 1));
	}
	
	maxEnergized.Dump("MAX ENERGIZED");
}

int GetEnergized(Direction startDirection, int startRow, int startCol)
{
	HashSet<(Direction, int row, int col)> visited = new();
	bool[,] energized = new bool[maxRow, maxCol];
	var q = new Queue<(int row, int col, Direction direction)>();
	q.Enqueue((startRow, startCol, startDirection));
	visited.Add((startDirection, startRow, startCol));
	energized[startRow, startCol] = true;
	while (q.Count > 0)
	{
		var (row, col, dir) = q.Dequeue();
		char tile = cave[row][col];
		var paths = (
			(dir, tile) switch
			{
				(Direction.Right, EMPTY) => new[] { (row, col: col + 1, dir: Direction.Right) },
				(Direction.Right, MIRROR) => new[] { (row: row - 1, col, dir: Direction.Up) },
				(Direction.Right, BACK_MIRROR) => new[] { (row: row + 1, col, dir: Direction.Down) },
				(Direction.Right, FLAT_SPLITTER) => new[] { (row, col: col + 1, dir: Direction.Right) },
				(Direction.Right, WALL_SPLITTER) => new[] { (row: row - 1, col, dir: Direction.Up), (row: row + 1, col, dir: Direction.Down) },

				(Direction.Down, EMPTY) => new[] { (row: row + 1, col, dir: Direction.Down) },
				(Direction.Down, MIRROR) => new[] { (row, col: col - 1, dir: Direction.Left) },
				(Direction.Down, BACK_MIRROR) => new[] { (row, col: col + 1, dir: Direction.Right) },
				(Direction.Down, FLAT_SPLITTER) => new[] { (row, col: col - 1, dir: Direction.Left), (row, col: col + 1, dir: Direction.Right) },
				(Direction.Down, WALL_SPLITTER) => new[] { (row: row + 1, col, dir: Direction.Down) },

				(Direction.Left, EMPTY) => new[] { (row, col: col - 1, dir: Direction.Left) },
				(Direction.Left, MIRROR) => new[] { (row: row + 1, col, dir: Direction.Down) },
				(Direction.Left, BACK_MIRROR) => new[] { (row: row - 1, col, dir: Direction.Up) },
				(Direction.Left, FLAT_SPLITTER) => new[] { (row, col: col - 1, dir: Direction.Left) },
				(Direction.Left, WALL_SPLITTER) => new[] { (row: row - 1, col, dir: Direction.Up), (row: row + 1, col, dir: Direction.Down) },

				(Direction.Up, EMPTY) => new[] { (row: row - 1, col, dir: Direction.Up) },
				(Direction.Up, MIRROR) => new[] { (row, col: col + 1, dir: Direction.Right) },
				(Direction.Up, BACK_MIRROR) => new[] { (row, col: col - 1, dir: Direction.Left) },
				(Direction.Up, FLAT_SPLITTER) => new[] { (row, col: col - 1, dir: Direction.Left), (row, col: col + 1, dir: Direction.Right) },
				(Direction.Up, WALL_SPLITTER) => new[] { (row: row - 1, col, dir: Direction.Up) },

				_ => throw new ArgumentException($"Invalid dir and tile ({dir}, {tile}) combination"),
			})
		.Where(x => x.row >= 0 && x.row < maxRow && x.col >= 0 && x.col < maxCol)
		.Where(x => !visited.Contains((x.dir, x.row, x.col)));
		foreach (var path in paths)
		{
			q.Enqueue(path);
			visited.Add((path.dir, path.row, path.col));
			energized[path.row, path.col] = true;
		}
	}

	int energizedCount = 0;
	for (int row = 0; row < energized.GetLength(0); row++)
	{
		for (int col = 0; col < energized.GetLength(1); col++)
		{
			if (energized[row, col])
			{
				energizedCount++;
			}
		}
	}

	(startDirection, startRow, startCol, energizedCount).ToString().Dump();
	return energizedCount;
}

static void PrintEnergized(bool[,] energized)
{
	var sb = new StringBuilder();
	for (int row = 0; row < energized.GetLength(0); row++)
	{
		sb.Clear();
		for (int col = 0; col < energized.GetLength(1); col++)
		{
			sb.Append(energized[row, col] ? '#' : '.');
		}
		sb.ToString().Dump();
	}

	"".Dump();
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
