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
const int day = 18;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	List<(long x, long y)> vertices = new();
	int x = 0;
	int y = 0;
	vertices.Add((x, y));
	long perimeter = 0L;
	foreach (string line in input)
	{
		string[] dig = line.Split();
		string dir = dig[0];
		int count = int.Parse(dig[1]);
		perimeter += count;
		(x, y) = dir switch
		{
			"R" => (x, y + count),
			"D" => (x + count, y),
			"L" => (x, y - count),
			"U" => (x - count, y),
			_ => throw new ArgumentException(),
		};
		vertices.Add((x, y));
	}

	long shoelaceArea = CalculateShoelaceArea(vertices);
	long area = shoelaceArea + (perimeter / 2) + 1;
	
	area.Dump("AREA");
}
void part2()
{
	List<(long x, long y)> vertices = new();
	long x = 0L;
	long y = 0L;
	vertices.Add((x, y));
	long perimeter = 0L;
	foreach (string line in input)
	{
		string[] dig = line.Split();
		string hex = dig[2];
		int count = Convert.ToInt32(hex[2..^2], 16);
		perimeter += count;
		(x, y) = hex[^2..^1] switch
		{
			"0" => (x, y + count),
			"1" => (x + count, y),
			"2" => (x, y - count),
			"3" => (x - count, y),
			_ => throw new ArgumentException(),
		};
		vertices.Add((x, y));
	}

	long shoelaceArea = CalculateShoelaceArea(vertices);
	long area = shoelaceArea + (perimeter / 2) + 1;
	
	area.Dump("AREA");
}

long CalculateShoelaceArea(List<(long x, long y)> vertices)
{
	long area = 0L;
	int j = vertices.Count - 1;
	for (int i = 0; i < vertices.Count; i++)
	{
		area += (vertices[j].x + vertices[i].x) * (vertices[j].y - vertices[i].y);
		j = i;
	}

	return Math.Abs(area / 2);
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
