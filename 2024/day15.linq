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
const int day = 15;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample-1.txt"));

const char WALL = '#';
const char BOX = 'O';
const char EMPTY = '.';
const char ROBOT = '@';

record Point(int Row, int Col);

void part1()
{
	List<char[]> mapBuilder = new();
	int line = 0;
	while (!input[line].Trim().Equals(string.Empty))
	{
		mapBuilder.Add(input[line].ToCharArray());
		line++;
	}

	int rows = mapBuilder.Count;
	int cols = mapBuilder[0].Length;
	char[,] map = new char[mapBuilder.Count, mapBuilder[0].Length];
	Point? start = null;
	for (int row = 0; row < rows; row++)
	{
		for (int col = 0; col < cols; col++)
		{
			map[row, col] = mapBuilder[row][col];
			if (map[row, col] == ROBOT)
			{
				start = new Point(row, col);
			}
		}
	}

	Debug.Assert(start is not null);

	string moves = new string(
		input
		.Skip(line)
		.SelectMany(x => x.ToArray())
		.ToArray());

	Point pos = start!;
	//map.Dump();
	foreach (var move in moves)
	{
		Point move_d = move switch
		{
			'^' => new(-1, 0),
			'>' => new(0, 1),
			'v' => new(1, 0),
			'<' => new(0, -1),
			_ => throw new ArgumentException("invalid move " + move)
		};

		Point next = pos with { Row = pos.Row + move_d.Row, Col = pos.Col + move_d.Col };
		char nextTile = map[next.Row, next.Col];
		if (nextTile == EMPTY)
		{
			map[pos.Row, pos.Col] = EMPTY;
			pos = next;
			map[pos.Row, pos.Col] = ROBOT;
		}
		else if (nextTile == BOX)
		{
			pos = tryPush(map, pos, next, move_d);
		}

		//map.Dump(move_d.ToString());
	}

	long GPS = 0L;
	for (int row = 0; row < map.GetLength(0); row++)
	{
		for (int col = 0; col < map.GetLength(1); col++)
		{
			if (map[row, col] == BOX)
			{
				GPS += 100 * row + col;
			}
		}
	}

	GPS.Dump("Goods Positioning System");

	Point tryPush(char[,] map, Point current, Point next, Point move_d)
	{
		Point push = next with { Row = next.Row + move_d.Row, Col = next.Col + move_d.Col };
		char nextTile = map[push.Row, push.Col];
		Point? firstEmpty = null;
		while (nextTile != WALL)
		{
			if (nextTile == EMPTY)
			{
				firstEmpty = push;
				break;
			}

			push = push with { Row = push.Row + move_d.Row, Col = push.Col + move_d.Col };
			nextTile = map[push.Row, push.Col];
		}

		if (firstEmpty is not null)
		{
			map[firstEmpty.Row, firstEmpty.Col] = BOX;
			map[next.Row, next.Col] = ROBOT;
			map[next.Row - move_d.Row, next.Col - move_d.Col] = EMPTY;
			return next;
		}

		return current;
	}
}

void part2()
{
	const string WALL_WIDE = "##";
	const string BOX_WIDE = "[]";
	const string EMPTY_WIDE = "..";
	const string ROBOT_WIDE = "@.";
	List<StringBuilder> mapBuilder = new();
	int line = 0;
	while (!input[line].Trim().Equals(string.Empty))
	{
		StringBuilder sb = new();
		mapBuilder.Add(sb);
		foreach (var tile in input[line].ToCharArray())
		{
			sb.Append(
				tile switch
				{
					WALL => WALL_WIDE,
					BOX => BOX_WIDE,
					EMPTY => EMPTY_WIDE,
					ROBOT => ROBOT_WIDE,
					_ => throw new ArgumentException("invalid tile " + tile),
				});
		}

		line++;
	}

	int rows = mapBuilder.Count;
	int cols = mapBuilder[0].Length;
	char[,] map = new char[mapBuilder.Count, mapBuilder[0].Length];
	Point? start = null;
	for (int row = 0; row < rows; row++)
	{
		for (int col = 0; col < cols; col++)
		{
			map[row, col] = mapBuilder[row][col];
			if (map[row, col] == ROBOT)
			{
				start = new Point(row, col);
			}
		}
	}

	Debug.Assert(start is not null);

	IEnumerable<char> moves = input
		.Skip(line)
		.SelectMany(x => x);

	Point pos = start!;
	//map.Dump();
	foreach (var move in moves)
	{
		Point move_d = move switch
		{
			'^' => new(-1, 0),
			'>' => new(0, 1),
			'v' => new(1, 0),
			'<' => new(0, -1),
			_ => throw new ArgumentException("invalid move " + move)
		};

		Point next = pos with { Row = pos.Row + move_d.Row, Col = pos.Col + move_d.Col };
		char nextTile = map[next.Row, next.Col];
		if (nextTile == EMPTY)
		{
			map[pos.Row, pos.Col] = EMPTY;
			pos = next;
			map[pos.Row, pos.Col] = ROBOT;
		}
		else if (BOX_WIDE.Contains(nextTile))
		{
			pos = tryPush(map, pos, next, (move, move_d));
		}

	}

	long GPS = 0L;
	for (int row = 0; row < map.GetLength(0); row++)
	{
		for (int col = 0; col < map.GetLength(1); col++)
		{
			if (map[row, col] == BOX_WIDE[0])
			{
				GPS += 100 * row + col;
			}
		}
	}

	GPS.Dump("Goods Positioning System");
	//map.Dump();

	Point tryPush(char[,] map, Point current, Point next, (char, Point) move)
	{
		const char TOP = '^';
		const char RIGHT = '>';
		const char DOWN = 'v';
		const char LEFT = '<';

		var (move_sign, move_d) = move;
		if (move_sign == LEFT || move_sign == RIGHT)
		{
			Point push = next with { Row = next.Row + move_d.Row, Col = next.Col + move_d.Col };
			char nextTile = map[push.Row, push.Col];
			Point? firstEmpty = null;
			while (nextTile != WALL)
			{
				if (nextTile == EMPTY)
				{
					firstEmpty = push;
					break;
				}

				push = push with { Row = push.Row + move_d.Row, Col = push.Col + move_d.Col };
				nextTile = map[push.Row, push.Col];
			}

			if (firstEmpty is not null)
			{
				Point movingCell = next;
				while (movingCell != firstEmpty)
				{
					movingCell = movingCell with { Row = movingCell.Row + move_d.Row, Col = movingCell.Col + move_d.Col };
					// flip box tile
					map[movingCell.Row, movingCell.Col] = map[movingCell.Row, movingCell.Col] == BOX_WIDE[0]
						? BOX_WIDE[1]
						: BOX_WIDE[0];
				}

				map[firstEmpty.Row, firstEmpty.Col] = move_sign == LEFT ? BOX_WIDE[0] : BOX_WIDE[1];
				map[next.Row, next.Col] = ROBOT;
				map[current.Row, current.Col] = EMPTY;
				return next;
			}

			return current;
		}
		else
		{
			return pushBoxes(map, move_d, next) ? next : current;
		}
	}

	bool pushBoxes(char[,] map, Point move_d, Point pos)
	{
		Queue<Point> bfs = new();
		bfs.Enqueue(pos);
		HashSet<Point> seen = new();
		char[,] mut = new char[map.GetLength(0), map.GetLength(1)];
		Array.Copy(map, mut, map.Length);
		mut[pos.Row - move_d.Row, pos.Col - move_d.Col] = EMPTY;
		bool possible = true;
		while (bfs.Count > 0)
		{
			Point p = bfs.Dequeue();
			if (seen.Contains(p))
			{
				continue;
			}

			seen.Add(p);
			char tile = mut[p.Row, p.Col];
			if (tile == WALL)
			{
				possible = false;
				break;
			}

			if (tile == EMPTY)
			{
				mut[p.Row, p.Col] = map[p.Row - move_d.Row, p.Col - move_d.Col];
			}
			else if (tile == BOX_WIDE[0])
			{
				bfs.Enqueue(p with { Col = p.Col + 1 });
				bfs.Enqueue(p with { Row = p.Row + move_d.Row, Col = p.Col + move_d.Col });
				mut[p.Row, p.Col] = seen.Contains(new Point(p.Row - move_d.Row, p.Col - move_d.Col))
					? map[p.Row - move_d.Row, p.Col - move_d.Col]
					: EMPTY;
			}
			else if (tile == BOX_WIDE[1])
			{
				bfs.Enqueue(p with { Col = p.Col - 1 });
				bfs.Enqueue(p with { Row = p.Row + move_d.Row, Col = p.Col + move_d.Col });
				mut[p.Row, p.Col] = seen.Contains(new Point(p.Row - move_d.Row, p.Col - move_d.Col))
					? map[p.Row - move_d.Row, p.Col - move_d.Col]
					: EMPTY;
			}
			else
			{
				throw new ArgumentException("shouldn't reach that");
			}
		}

		mut[pos.Row, pos.Col + (mut[pos.Row + move_d.Row, pos.Col] == BOX_WIDE[0] ? 1 : -1)] = EMPTY;
		mut[pos.Row, pos.Col] = ROBOT;
		//map.Dump();
		//mut.Dump("try push mutable " + possible);
		if (possible)
		{
			Array.Copy(mut, map, mut.Length);
		}

		return possible;
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
