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
const int day = 10;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample-1.txt"));
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample-2.txt"));
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample-3.txt"));
public record Direction(int row, int col);
static Direction N = new(-1, 0);
static Direction S = new(1, 0);
static Direction W = new(0, -1);
static Direction E = new(0, 1);
static Dictionary<Direction, HashSet<char>> ValidPipes = new Dictionary<Direction, HashSet<char>>
{
	[N] = new char[] { '|', '7', 'F' }.ToHashSet(),
	[S] = new char[] { '|', 'L', 'J' }.ToHashSet(),
	[W] = new char[] { '-', 'F', 'L' }.ToHashSet(),
	[E] = new char[] { '-', 'J', '7' }.ToHashSet(),
};
static Dictionary<(Direction, char), Direction> Router = new Dictionary<(Direction, char), Direction>
{
	[(N, '|')] = N,
	[(S, '|')] = S,
	[(E, '7')] = S,
	[(N, '7')] = W,
	[(W, '-')] = W,
	[(E, '-')] = E,
	[(N, 'F')] = E,
	[(W, 'F')] = S,
	[(S, 'J')] = W,
	[(E, 'J')] = N,
	[(S, 'L')] = E,
	[(W, 'L')] = N,
};

const char GROUND = '.';
const char START = 'S';

public class Maze
{
	private readonly char[,] _grid;
	private readonly int _colMax;
	private readonly int _rowMax;
	private readonly int _startRow;
	private readonly int _startCol;
	public Maze(char[,] grid)
	{
		_grid = grid;
		_rowMax = grid.GetLength(0);
		_colMax = grid.GetLength(1);
		int? startRow = null;
		int? startCol = null;
		for (int row = 0; row < _rowMax; row++)
		{
			for (int col = 0; col < _colMax; col++)
			{
				if (_grid[row, col] == START)
				{
					startRow = row;
					startCol = col;
					goto ENDSEARCH;
				}
			}
		}
	ENDSEARCH:
		if (startRow is null || startCol is null)
		{
			throw new ArgumentException("start is not equal to S");
		}

		_startRow = startRow.Value;
		_startCol = startCol.Value;
	}

	public int GetEnclosedByLoop(bool[,] visited)
	{
		Dump(visited);
		int enclosed = 0;
		HashSet<char> tilesIn = new(new char[] { '|', '7', 'F', });
		for (int r = 0; r < _grid.GetLength(0); r++)
		{
			bool inOut = false;
			for (int c = 0; c < _grid.GetLength(1); c++)
			{
				char tile = _grid[r, c];
				if (visited[r, c] && tilesIn.Contains(tile))
				{
					inOut = !inOut;
				}
				else
				{
					if (inOut && !visited[r, c])
					{
						enclosed++;
					}
				}
			}
		}

		return enclosed;
	}

	public (bool[,] visited, int length) GetFarthestLoop()
	{
		var loop = new Direction[]
			{
				N,
				E,
				S,
				W,
			}
			.AsParallel()
			.Select(direction =>
			{
				bool[,] visited = new bool[_rowMax, _colMax];
				visited[_startRow, _startCol] = true;
				int row = _startRow;
				int col = _startCol;
				int length = 1;
				while (true)
				{
					int rr = row + direction.row;
					int cc = col + direction.col;
					if (!CheckBoundaries(rr, cc))
					{
						length = 0;
						break;
					}

					char next = _grid[rr, cc];
					if (next == START)
					{
						break;
					}

					if (visited[rr, cc])
					{
						length = 0;
						break;
					}

					if (next == GROUND)
					{
						length = 0;
						break;
					}

					if (!ValidPipes[direction].Contains(next))
					{
						length = 0;
						break;
					}

					direction = Router[(direction, next)];
					visited[rr, cc] = true;
					row = rr;
					col = cc;
					length++;
				}

				return (length, visited);
			})
			.MaxBy(x => x.length);

		return (loop.visited, loop.length / 2);
	}

	private bool CheckBoundaries(int row, int col)
	{
		return row >= 0 && row < _rowMax
			&& col >= 0 && col < _colMax;
	}

	private void Dump(bool[,] visited)
	{
		Util.ClearResults();
		var sb = new StringBuilder();
		for (int r = 0; r < _grid.GetLength(0); r++)
		{
			sb.Clear();
			for (int c = 0; c < _grid.GetLength(1); c++)
			{
				sb.Append(visited[r, c] ? 'X' : _grid[r, c]);
			}

			sb.ToString().Dump();
		}
	}
}

void part1()
{
	char[,] grid = new char[input.Length, input[0].Length];
	input.SelectMany((s, i) => s.Select((c, j) => new { i, j, c }))
		   .ToList()
		   .ForEach(item => grid[item.i, item.j] = item.c);
	var maze = new Maze(grid);
	maze.GetFarthestLoop().length.Dump("FARTHEST IN THE LOOP");
}
void part2()
{
	char[,] grid = new char[input.Length, input[0].Length];
	input.SelectMany((s, i) => s.Select((c, j) => new { i, j, c }))
		   .ToList()
		   .ForEach(item => grid[item.i, item.j] = item.c);
	var maze = new Maze(grid);
	var (visited, _) = maze.GetFarthestLoop();
	var enclosed = maze.GetEnclosedByLoop(visited);
	enclosed.Dump("ENCLOSED BY THE LOOP");
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
