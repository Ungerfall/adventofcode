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
const int day = 17;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
static int maxRow = input.Length;
static int maxCol = input[0].Length;
static (int row, int col)[] Dirs = new[] { (0, -1), (0, 1), (-1, 0), (1, 0) };
void part1()
{
	int[,] map = new int[maxRow, maxCol];
	for (int row = 0; row < maxRow; row++)
	{
		for (int col = 0; col < maxCol; col++)
		{
			map[row, col] = (int)Char.GetNumericValue(input[row][col]);
		}
	}

	//map.Dump();
	int min = Dijkstra(map, min: 0, max: 3);
	min.Dump("THE LEAST HEAT LOSS");
}
void part2()
{
	int[,] map = new int[maxRow, maxCol];
	for (int row = 0; row < maxRow; row++)
	{
		for (int col = 0; col < maxCol; col++)
		{
			map[row, col] = (int)Char.GetNumericValue(input[row][col]);
		}
	}

	int min = Dijkstra(map, min: 4, max: 10);
	min.Dump("THE LEAST HEAT LOSS");
}

public record State(int row, int col, int dr, int dc, int count);
int Dijkstra(int[,] grid, int min, int max)
{
	DefaultDictionary distances = new(int.MaxValue / 2);
	State startToRight = new(0, 0, 0, 1, 0);
	State startToDown = new(0, 0, 1, 0, 0);
	distances.Add(startToRight, 0);
	distances.Add(startToDown, 0);
	PriorityQueue<State, int> heap = new();
	heap.Enqueue(startToRight, priority: 0);
	heap.Enqueue(startToDown, priority: 0);
	while (heap.Count > 0)
	{
		var current = heap.Dequeue();
		var (row, col, dr, dc, sameDirectionCount) = current;
		if (row == maxRow - 1 && col == maxCol - 1 && sameDirectionCount >= min)
		{
			return distances.Get(current);
		}

		foreach (var dir in Dirs)
		{
			int rr = row + dir.row;
			int cc = col + dir.col;
			if (rr < 0 || rr >= maxRow || cc < 0 || cc >= maxCol) // boundaries
			{
				continue;
			}

			if (dir.row + dr == 0 && dir.col + dc == 0) // opposite
			{
				continue;
			}

			bool sameDirection = dr == dir.row && dc == dir.col;
			if (!sameDirection && sameDirectionCount < min)
			{
				continue;
			}
			
			int newSameDirectionCount = sameDirection
				? sameDirectionCount + 1
				: 1;
			if (newSameDirectionCount <= max)
			{
				State adj = new(rr, cc, dir.row, dir.col, newSameDirectionCount);
				int newDist = distances.Get(current) + grid[rr, cc];
				if (newDist < distances.Get(adj))
				{
					distances.Add(adj, newDist);
					heap.Enqueue(adj, newDist);
				}
			}
		}
	}

	throw new ArgumentException("Shortest path has not been found");
}

public class DefaultDictionary
{
	private readonly int _default;
	private readonly Dictionary<State, int> _state = new();
	public DefaultDictionary(int defaultValue)
	{
		_default = defaultValue;
	}

	public void Add(State key, int value)
	{
		_state[key] = value;
	}

	public int Get(State key)
	{
		if (_state.TryGetValue(key, out int value))
		{
			return value;
		}
		else
		{
			_state[key] = _default;
			return _default;
		}
	}

	public void Dump()
	{
		var grouped = _state
			.GroupBy(x => (x.Key.row, x.Key.col), (k, c) => new { key = k, value = string.Join(",", c.Select(x => x.Value)) });
		string[,] grid = new string[maxRow, maxCol];
		grouped.Count().Dump();
		foreach (var g in grouped)
		{
			grid[g.key.row, g.key.col] = g.value;
		}

		grid.Dump();
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
