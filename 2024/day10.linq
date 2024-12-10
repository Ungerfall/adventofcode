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
const int day = 10;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
static int Rows = input.Length;
static int Cols = input[0].Length;
void part1()
{
	long score = 0L;
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			if (input[row][col] == '0')
			{
				score += traverse(row, col);
			}
		}
	}

	score.Dump("Scores of all trailheads");
}

static long traverse(int rowStart, int colStart)
{
	Queue<(int row, int col)> bfs = new();
	bfs.Enqueue((rowStart, colStart));

	(int row_d, int col_d)[] dirs = new (int row_d, int col_d)[]
	{
		(0, -1),
		(-1, 0),
		(1, 0),
		(0, 1)
	};
	HashSet<(int row, int col)> summits = new();
	while (bfs.Count > 0)
	{
		var (row, col) = bfs.Dequeue();
		int h = (int)char.GetNumericValue(input[row][col]);
		if (h == 9)
		{
			summits.Add((row, col));
			continue;
		}

		foreach (var (row_d, col_d) in dirs)
		{
			int row_next = row + row_d;
			int col_next = col + col_d;
			if (row_next >= 0 && row_next < Rows && col_next >= 0 && col_next < Cols)
			{
				int h_next = (int)char.GetNumericValue(input[row_next][col_next]);
				if (h_next - h == 1)
				{
					bfs.Enqueue((row_next, col_next));
				}
			}
		}
	}

	return summits.Count;
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
