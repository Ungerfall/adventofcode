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
const int day = 13;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

record Point (long X, long Y);

const int COST_A = 3;
const int COST_B = 1;
void part1()
{
	long sum = 0L;
	for (int i = 0; i < input.Length; i+=4)
	{
		Match button_a_matches = (new Regex(@"Button A: X\+(\d+), Y\+(\d+)")).Match(input[i]);
		Match button_b_matches = (new Regex(@"Button B: X\+(\d+), Y\+(\d+)")).Match(input[i+1]);
		Match prize_matches = (new Regex(@"Prize: X=(\d+), Y=(\d+)")).Match(input[i+2]);
		Point button_a_d = new Point(int.Parse(button_a_matches.Groups[1].Value), int.Parse(button_a_matches.Groups[2].Value));
		Point button_b_d = new Point(int.Parse(button_b_matches.Groups[1].Value), int.Parse(button_b_matches.Groups[2].Value));
		Point prize = new Point(int.Parse(prize_matches.Groups[1].Value), int.Parse(prize_matches.Groups[2].Value));
		
		Dictionary<Point, long> memo = new();
		long min = minPresses(prize, new Point(0, 0), 0L, button_a_d, button_b_d, memo);
		if (min != long.MaxValue)
		{
			sum += min;
		}
	}
	
	sum.Dump("Max possible points");
}

long minPresses(Point prize, Point current, long score, Point btn_a_d, Point btn_b_d, Dictionary<Point, long> memo)
{
	if (current.X > prize.X || current.Y > prize.Y)
	{
		return long.MaxValue;
	}
	
	if (current.X == prize.X && current.Y == prize.Y)
	{
		return score;
	}
	
	if (memo.TryGetValue(current, out long minScore))
	{
		return minScore;
	}

	long min = Math.Min(
		minPresses(prize, current with { X = current.X + btn_a_d.X, Y = current.Y + btn_a_d.Y }, score + COST_A, btn_a_d, btn_b_d, memo),
		minPresses(prize, current with { X = current.X + btn_b_d.X, Y = current.Y + btn_b_d.Y }, score + COST_B, btn_a_d, btn_b_d, memo)
	);
	memo[current] = min;
	
	return min;
}

void part2()
{
	long sum = 0L;
	for (int i = 0; i < input.Length; i+=4)
	{
		Match button_a_matches = (new Regex(@"Button A: X\+(\d+), Y\+(\d+)")).Match(input[i]);
		Match button_b_matches = (new Regex(@"Button B: X\+(\d+), Y\+(\d+)")).Match(input[i+1]);
		Match prize_matches = (new Regex(@"Prize: X=(\d+), Y=(\d+)")).Match(input[i+2]);
		Point button_a_d = new Point(int.Parse(button_a_matches.Groups[1].Value), int.Parse(button_a_matches.Groups[2].Value));
		Point button_b_d = new Point(int.Parse(button_b_matches.Groups[1].Value), int.Parse(button_b_matches.Groups[2].Value));
		Point prize = new Point(
			int.Parse(prize_matches.Groups[1].Value) + 10000000000000L,
			int.Parse(prize_matches.Groups[2].Value) + 10000000000000L);
		
		Dictionary<Point, long> memo = new();
		long min = minPresses(prize, new Point(0, 0), 0L, button_a_d, button_b_d, memo);
		if (min != long.MaxValue)
		{
			sum += min;
		}
	}
	
	sum.Dump("Max possible points");
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
