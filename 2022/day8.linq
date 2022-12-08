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
const int day = 8;
string[] input = GetInputLines(year, day);
/*
string[] input = @"30373
25512
65332
33549
35390".Split(Environment.NewLine);
*/
async Task part1()
{
	char[][] grid = input
		.Select(x => x.Trim().ToCharArray())
		.ToArray();
	
	HashSet<(int x, int y)> visible = new();
	int xMax = grid.Length;
	int yMax = grid[0].Length;
	// rows
	for (int x = 1; x < xMax-1; x++)
	{
		int highest = grid[x][0];
		for (int y = 1; y < yMax - 1; y++)
		{
			if (grid[x][y] > highest)
			{
				visible.Add((x, y));
				highest = grid[x][y];
			}
		}
		
		highest = grid[x][yMax-1];
		for (int y = yMax-2; y >= 1; y--)
		{
			if (grid[x][y] > highest)
			{
				visible.Add((x, y));
				highest = grid[x][y];
			}
		}
	}
	// columns
	for (int y = 1; y < yMax-1; y++)
	{
		int highest = grid[0][y];
		for (int x = 1; x < xMax - 1; x++)
		{
			if (grid[x][y] > highest)
			{
				visible.Add((x, y));
				highest = grid[x][y];
			}
		}
		
		highest = grid[xMax-1][y];
		for (int x = xMax-2; x >= 1; x--)
		{
			if (grid[x][y] > highest)
			{
				visible.Add((x, y));
				highest = grid[x][y];
			}
		}
	}
	
	(visible.Count + 2*grid.Length + 2*grid[0].Length - 4).Dump();
}
async Task part2()
{
	foreach (string line in input)
	{

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
