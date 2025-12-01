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
const int day = 21;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

char[,] numpad = new char[4, 3] {
	{ '7', '8', '9' },
	{ '4', '5', '6' },
	{ '1', '2', '3' },
	{ ' ', '0', 'A' },
};
char[,] dirpad = new char[2, 3] {
	{ ' ', '^', 'A' },
	{ '<', 'v', '>' }
};

record Point(int Row, int Col);

void part1()
{
	foreach (var code in input)
	{
		Point me = new Point(0, 2);
		Point robot_dirpad_1 = new Point(0, 2);
		Point robot_dirpad_2 = new Point(0, 2);
		Point robot_numpad = new Point(3, 2);
		Queue<(Point me, Point robot_dirpad_1, Point robot_dirpad_2, Point robot_numpad, string crt)> bfs = new();
		string crt = string.Empty;
		bfs.Enqueue((me, robot_dirpad_1, robot_dirpad_2, robot_numpad, string.Empty));
		while (bfs.Count > 0)
		{
			(me, robot_dirpad_1, robot_dirpad_2, robot_numpad, crt) = bfs.Dequeue();
			if (crt.Equals(code))
			{
				
			}
		}
	}
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
