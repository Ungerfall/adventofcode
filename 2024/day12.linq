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
const int day = 12;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

static int Rows = input.Length;
static int Cols = input[0].Length;
record Point(int row, int col);
void part1()
{
	HashSet<Point> seen = new();
	long price = 0L;
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			Point p = new(row, col);
			if (!seen.Contains(p))
			{
				FloodFill(p, seen, ref price);
			}
		}
	}

	price.Dump("Total price");
}

static Point[] Dirs = new Point[]
{
	new (0, -1),
	new (-1, 0),
	new (0, 1),
	new (1, 0)
};
static private void FloodFill(Point p, HashSet<Point> seen, ref long price)
{
	Queue<Point> bfs = new();
	bfs.Enqueue(p);
	int square = 0;
	int perimeter = 0;
	while (bfs.Count > 0)
	{
		var point = bfs.Dequeue();
		if (seen.Contains(point))
		{
			continue;
		}

		seen.Add(point);
		square++;

		foreach (var dir in Dirs)
		{
			int row_next = point.row + dir.row;
			int col_next = point.col + dir.col;
			Point p_next = new(row_next, col_next);

			if (row_next < 0 || row_next >= Rows || col_next < 0 || col_next >= Cols)
			{
				perimeter++;
				continue;
			}

			if (input[p.row][p.col] == input[row_next][col_next])
			{
				bfs.Enqueue(p_next);
			}
			else
			{
				perimeter++;
			}
		}
	}

	(new { square, perimeter, price = (long)square * perimeter, point = input[p.row][p.col] }).ToString().Dump();
	price += (long)square * perimeter;
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
