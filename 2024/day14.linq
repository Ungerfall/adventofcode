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
const int day = 14;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

const int Rows = 103;
const int Cols = 101;
//const int Rows = 7;
//const int Cols = 11;
record RobotPosition(int Row, int Col, int VelocityRow, int VelocityCol, string Id);
void part1()
{
	List<RobotPosition> robots = new();
	foreach (string line in input)
	{
		Match match = new Regex(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)").Match(line);
		robots.Add(new RobotPosition(
			Id: line.Trim(),
			Row: int.Parse(match.Groups[2].Value),
			Col: int.Parse(match.Groups[1].Value),
			VelocityRow: int.Parse(match.Groups[4].Value),
			VelocityCol: int.Parse(match.Groups[3].Value)));
	}

	int q1 = 0;
	int q2 = 0;
	int q3 = 0;
	int q4 = 0;
	const int steps = 100;
	List<RobotPosition> robots_after_100 = new();
	foreach (var robot in robots)
	{
		int row_after_100 = mod(robot.Row + steps * robot.VelocityRow, Rows);
		int col_after_100 = mod(robot.Col + steps * robot.VelocityCol, Cols);
		int quadrant = (row_after_100, col_after_100) switch
		{
			(row_after_100: < Rows / 2, col_after_100: < Cols / 2) => 1,
			(row_after_100: < Rows / 2, col_after_100: > Cols / 2) => 2,
			(row_after_100: > Rows / 2, col_after_100: < Cols / 2) => 3,
			(row_after_100: > Rows / 2, col_after_100: > Cols / 2) => 4,
			(row_after_100: Rows / 2, _) => -1,
			(_, col_after_100: Cols / 2) => -1,
		};

		//(row_after_100, col_after_100, robot).ToString().Dump();
		q1 += quadrant == 1 ? 1 : 0;
		q2 += quadrant == 2 ? 1 : 0;
		q3 += quadrant == 3 ? 1 : 0;
		q4 += quadrant == 4 ? 1 : 0;
		robots_after_100.Add(robot with { Row = row_after_100, Col = col_after_100 });
	}

	//(new { q1, q2, q3, q4 }).ToString().Dump();
	long safetyFactor = checked((long)q1 * q2 * q3 * q4);

	//Print(robots_after_100);
	safetyFactor.Dump("Safety factor");
}
void part2()
{
	List<RobotPosition> robots = new();
	foreach (string line in input)
	{
		Match match = new Regex(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)").Match(line);
		robots.Add(new RobotPosition(
			Id: line.Trim(),
			Row: int.Parse(match.Groups[2].Value),
			Col: int.Parse(match.Groups[1].Value),
			VelocityRow: int.Parse(match.Groups[4].Value),
			VelocityCol: int.Parse(match.Groups[3].Value)));
	}

	int step = 1;
	while (true)
	{
		if (longest_line(robots) >= 10)
		{
			Print(robots);
			if (Console.ReadLine() == "stop")
			{
				break;
			}
		}

		Util.ClearResults();
		for (int i = 0; i < robots.Count; i++)
		{
			RobotPosition robot = robots[i];
			int rr = mod(robot.Row + step * robot.VelocityRow, Rows);
			int cc = mod(robot.Col + step * robot.VelocityCol, Cols);
			robots[i] = robot with { Row = rr, Col = cc };
		}

		step++;
	}
}

int longest_line(List<RobotPosition> robots)
{
	f
}

int mod(int x, int m)
{
	return (x % m + m) % m;
}

void Print(List<RobotPosition> robots)
{
	Dictionary<(int Row, int Col), int> counter = new();
	foreach (var robot in robots)
	{
		var key = (robot.Row, robot.Col);
		if (counter.TryGetValue(key, out int count))
		{
			counter[key] = count + 1;
		}
		else
		{
			counter[key] = 1;
		}
	}

	for (int row = 0; row < Rows; row++)
	{
		char[] mut = Enumerable.Repeat('.', Cols).ToArray();
		for (int col = 0; col < Cols; col++)
		{
			if (counter.TryGetValue((row, col), out int count))
			{
				mut[col] = (char)('0' + count);
			}
		}

		(new string(mut)).Dump();
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
