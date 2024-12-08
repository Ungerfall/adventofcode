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
const int day = 8;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
static int Rows = input.Length;
static int Cols = input[0].Length;
record Point(int Row, int Col);
void part1()
{
	Dictionary<char, HashSet<Point>> antennas = new();
	for (int row = 0; row < Rows; row++)
	{
		for (int col = 0; col < Cols; col++)
		{
			if (input[row][col] == '.')
			{
				continue;
			};

			char frequency = input[row][col];
			if (!antennas.ContainsKey(frequency))
			{
				antennas[frequency] = new HashSet<Point>();
			}

			antennas[frequency].Add(new Point(row, col));
		}
	}

	HashSet<Point> antinodes = new();
	foreach (Point[] antenna in antennas.Values.Select(x => x.ToArray()))
	{
		for (int i = 0; i < antenna.Length; i++)
		{
			for (int j = 0; j < antenna.Length; j++)
			{
				if (i == j)
					continue;

				int antinodeRow = antenna[i].Row - (antenna[j].Row - antenna[i].Row);
				if (antinodeRow < 0 || antinodeRow >= Rows)
				{
					continue;
				}

				int antinodeCol = antenna[i].Col - (antenna[j].Col - antenna[i].Col);
				if (antinodeCol < 0 || antinodeCol >= Cols)
				{
					continue;
				}

				antinodes.Add(new Point(antinodeRow, antinodeCol));
			}
		}
	}

	antinodes.Count.Dump("Antinode locations count");
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
