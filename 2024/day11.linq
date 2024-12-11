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
const int day = 11;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	long[] numbers = input[0].Split().Select(long.Parse).ToArray();
	LinkedList<long> stones = new();
	for (int i = 0; i < numbers.Length; i++)
	{
		stones.AddLast(numbers[i]);
	}

	const int blinks = 25;
	for (int blink = 0; blink < blinks; blink++)
	{
		LinkedListNode<long>? stone = stones.First;
		//string.Join(",", stones).Dump();
		while (stone != null)
		{
			if (stone.Value == 0)
			{
				stone.Value = 1;
			}
			else if (stone.Value.ToString() is string stoneString && stoneString.Length % 2 == 0)
			{
				int mid = stoneString.Length / 2;
				int left = int.Parse(stoneString[..mid]);
				int right = int.Parse(stoneString[mid..]);
				stone.Value = left;
				stone = stones.AddAfter(stone, right);
			}
			else
			{
				checked
				{
					stone.Value *= 2024;
				}
			}

			stone = stone.Next;
		}
	}

	stones.Count.Dump("Stones count");
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
