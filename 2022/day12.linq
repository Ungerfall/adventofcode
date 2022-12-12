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
const int day = 12;
string[] input = GetInputLines(year, day);
string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

async Task part1()
{
	char[][] g = input.Select(x => x.ToCharArray()).ToArray();
	int xStart = 0;
	int yStart = 0;
	for (int x = 0; x < g.Length; x++)
	{
		for (int y = 0; y < g[0].Length; y++)
		{
			if (g[x][y] == 'S')
			{
				g[x][y] = 'a';
				xStart = x;
				yStart = y;
			}
		}
	}
	

	bfs(g, xStart, yStart).Dump();
}

async Task part2()
{
	char[][] g = input.Select(x => x.ToCharArray()).ToArray();
	int best = int.MaxValue;
	for (int x = 0; x < g.Length; x++)
	{
		for (int y = 0; y < g[0].Length; y++)
		{
			if (g[x][y] == 'S')
			{
				g[x][y] = 'a';
			}
			
			if (g[x][y] == 'a')
			{
				best = Math.Min(best, bfs(g, x, y));
				
			}
		}
	}
	
	best.Dump();
}

int bfs(char[][] g, int xStart, int yStart)
{
	(int x, int y)[] dirs = new[] {
		(0,1),
		(0,-1),
		(1,0),
		(-1,0)
	};
	HashSet<(int, int)> visited = new();
	Queue<(int x, int y, int steps)> bfs = new();
	bfs.Enqueue((xStart, yStart, 0));
	int best = int.MaxValue;
	int xMax = g.Length;
	int yMax = g[0].Length;
	while (bfs.Count > 0)
	{
		var (x, y, steps) = bfs.Dequeue();
		if (g[x][y] == 'E')
		{
			best = Math.Min(best, steps);
		}

		//print(g, x, y, visited);
		foreach (var i in Enumerable.Range(0, 4))
		{
			int xx = x + dirs[i].x;
			int yy = y + dirs[i].y;
			//(xx, yy).ToString().Dump();
			if (xx >= xMax || xx < 0 || yy >= yMax || yy < 0)
				continue;

			int diff = g[x][y] - (g[xx][yy] == 'E' ? 'z' : g[xx][yy]);
			//diff.Dump();
			if (!visited.Contains((xx, yy)) && diff >= -1)
			{
				bfs.Enqueue((xx, yy, steps + 1));
				visited.Add((xx, yy));
			}
		}
	}
	
	return best;
}

void print(char[][] g, int xCur, int yCur, HashSet<(int, int)> visited)
{
	Util.ClearResults();
	for (int x = 0; x < g.Length; x++)
	{
		StringBuilder sb = new();
		for (int y = 0; y < g[0].Length; y++)
		{
			char c = (x == xCur && y == yCur)
				? 'S'
				: visited.Contains((x, y))
					? '#'
					: g[x][y];
			sb.Append(c);
		}

		sb.ToString().Dump();
	}
	Thread.Sleep(10);
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
