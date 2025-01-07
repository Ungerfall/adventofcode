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
const int day = 25;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	List<int[]> locks = new();
	List<int[]> keys = new();
	for (int row = 0; row < input.Length; row += 8)
	{
		bool isLock = input[row].Equals("#####");
		int[] pins = new int[5];
		for (int col = 0; col < 5; col++)
		{
			int pinCount = 0;
			for (int window = row + 1; window < row + 6; window++)
			{
				if (input[window][col] == '#')
				{
					pinCount++;
				}
			}

			pins[col] = pinCount;
		}

		(isLock ? locks : keys).Add(pins);
	}

	int count = 0;
	for (int lockIx = 0; lockIx < locks.Count; lockIx++)
	{
		for (int keyIx = 0; keyIx < keys.Count; keyIx++)
		{
			bool valid = true;
			for (int pinIx = 0; pinIx < 5; pinIx++)
			{
				if (locks[lockIx][pinIx] + keys[keyIx][pinIx] > 5)
				{
					valid = false;
					break;
				}
			}

			if (valid)
			{
				count++;
			}
		}
	}

	count.Dump("Without overlapping");
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
