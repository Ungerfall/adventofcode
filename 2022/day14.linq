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
	 ┗ 📜input.linq
*/
const int year = 2022;
const int day = 14;
string[] input = GetInputLines(year, day);
string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
char sand = 'o';
char rock = '#';
char empty = '.';
char sandStartChar = '+';

async Task part1()
{
	List<(int x, int y)[]> rocks = new();
	foreach (string line in input)
	{
		var r = line.Split("->")
			.Select(x =>
			{
				var coords = x.Split(',').Select(int.Parse).ToArray();
				return (coords[1], coords[0]);
			})
			.ToArray();
		rocks.Add(r);
	}

	int xMin = Math.Min(0, rocks.SelectMany(x => x).Min(x => x.x));
	int xMax = rocks.SelectMany(x => x).Max(x => x.x);
	int yMin = rocks.SelectMany(x => x).Min(x => x.y);
	int yMax = rocks.SelectMany(x => x).Max(x => x.y);
	int xLen = xMax - xMin + 1;
	int yLen = yMax - yMin + 1;
	(xMin, xMax, yMin, yMax).Dump();

	char[][] grid = Enumerable.Range(0, xLen)
		.Select(x => Enumerable.Range(0, yLen).Select(y => empty).ToArray())
		.ToArray();
	(int x, int y) sandStart = (0, 500 - yMin);
	grid[sandStart.x][sandStart.y] = sandStartChar;
	foreach (var r in rocks)
	{
		for (int i = 0; i < r.Length - 1; i++)
		{
			Debug.Assert(r[i].x == r[i + 1].x || r[i].y == r[i + 1].y);
			(int x, int y) start = new(r[i].x, r[i].y - yMin);
			(int x, int y) end = new(r[i + 1].x, r[i + 1].y - yMin);
			//(start, end).ToString().Dump();
			if (start.x == end.x)
			{
				for (int j = Math.Min(start.y, end.y); j <= Math.Max(start.y, end.y); j++)
				{
					grid[start.x][j] = rock;
				}
			}
			else
			{
				for (int j = Math.Min(start.x, end.x); j <= Math.Max(start.x, end.x); j++)
				{
					grid[j][start.y] = rock;
				}
			}
		}
	}

	bool abyss = false;
	int count = 0;
	while (!abyss)
	{
		(int x, int y) = sandStart;
		count++;
		while (true)
		{
			if (x + 1 > xMax)
			{
				abyss = true;
				break;
			}

			if (grid[x + 1][y] == rock || grid[x + 1][y] == sand)
			{
				if (x + 1 > xMax || y - 1 < xMin)
				{
					abyss = true;
					break;
				}

				if (grid[x + 1][y - 1] == empty)
				{
					x = x + 1;
					y = y - 1;
					continue;
				}

				if (grid[x + 1][y - 1] == rock || grid[x + 1][y - 1] == sand)
				{
					if (x + 1 > xMax || y + 1 > grid[0].Length-1)
					{
						abyss = true;
						break;
					}

					if (grid[x + 1][y + 1] == empty)
					{
						x = x + 1;
						y = y + 1;
						continue;
					}

					if (grid[x + 1][y + 1] == rock || grid[x + 1][y + 1] == sand)
					{
						grid[x][y] = sand;
						break;
					}
					else
					{
						grid[x + 1][y + 1] = sand;
						break;
					}
				}
				else
				{
					grid[x + 1][y - 1] = sand;
					break;
				}
			}
			else
			{
				x = x + 1;
			}
		}

		//Print(grid);
		//await Task.Delay(1000);
	}

	//Print(grid);
	(count - 1).Dump("Part 1");
}

async Task part2()
{
	List<(int x, int y)[]> rocks = new();
	foreach (string line in input)
	{
		var r = line.Split("->")
			.Select(x =>
			{
				var coords = x.Split(',').Select(int.Parse).ToArray();
				return (coords[1], coords[0]);
			})
			.ToArray();
		rocks.Add(r);
	}

	int xMin = Math.Min(0, rocks.SelectMany(x => x).Min(x => x.x));
	int xMax = rocks.SelectMany(x => x).Max(x => x.x) + 2;
	int xLen = xMax - xMin + 1;
	int yMin = rocks.SelectMany(x => x).Min(x => x.y) - (xLen);
	int yMax = rocks.SelectMany(x => x).Max(x => x.y) + (xLen);
	int yLen = yMax - yMin + 1;
	(xMin, xMax, yMin, yMax).Dump();
	rocks.Add(new[] { (xMax, yMin), (xMax, yMax) });

	char[][] grid = Enumerable.Range(0, xLen)
		.Select(x => Enumerable.Range(0, yLen).Select(y => empty).ToArray())
		.ToArray();
	(int x, int y) sandStart = (0, 500 - yMin);
	grid[sandStart.x][sandStart.y] = sandStartChar;
	foreach (var r in rocks)
	{
		for (int i = 0; i < r.Length - 1; i++)
		{
			Debug.Assert(r[i].x == r[i + 1].x || r[i].y == r[i + 1].y);
			(int x, int y) start = new(r[i].x, r[i].y - yMin);
			(int x, int y) end = new(r[i + 1].x, r[i + 1].y - yMin);
			//(start, end).ToString().Dump();
			if (start.x == end.x)
			{
				for (int j = Math.Min(start.y, end.y); j <= Math.Max(start.y, end.y); j++)
				{
					grid[start.x][j] = rock;
				}
			}
			else
			{
				for (int j = Math.Min(start.x, end.x); j <= Math.Max(start.x, end.x); j++)
				{
					grid[j][start.y] = rock;
				}
			}
		}
	}

	int count = 0;
	while (grid[sandStart.x][sandStart.y] == sandStartChar)
	{
		(int x, int y) = sandStart;
		count++;
		while (true)
		{
			if (grid[x + 1][y] == rock || grid[x + 1][y] == sand)
			{
				if (grid[x + 1][y - 1] == empty)
				{
					x = x + 1;
					y = y - 1;
					continue;
				}

				if (grid[x + 1][y - 1] == rock || grid[x + 1][y - 1] == sand)
				{
					if (grid[x + 1][y + 1] == empty)
					{
						x = x + 1;
						y = y + 1;
						continue;
					}

					if (grid[x + 1][y + 1] == rock || grid[x + 1][y + 1] == sand)
					{
						grid[x][y] = sand;
						break;
					}
					else
					{
						grid[x + 1][y + 1] = sand;
						break;
					}
				}
				else
				{
					grid[x + 1][y - 1] = sand;
					break;
				}
			}
			else
			{
				x = x + 1;
			}
		}

		//Print(grid);
		//await Task.Delay(1000);
	}

	Print(grid);
	(count).Dump("Part 2");
}

void Print(char[][] g)
{
	for (int x = 0; x < g.Length; x++)
	{
		(new string(g[x])).Dump();
	}
}

async Task Main()
{
	await part1();
	await part2();
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
