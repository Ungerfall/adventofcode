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
const int day = 9;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	int[] diskMap = new int[input[0].Length];
	for (int i = 0; i < input[0].Length; i++)
	{
		diskMap[i] = (int)char.GetNumericValue(input[0][i]);
	}

	long checksum = 0L;

	int id = 0;
	int pos = 0;
	int left = 0;
	int right = diskMap.Length - (diskMap.Length % 2 == 0 ? 2 : 1);
	while (left <= right)
	{
		if (left % 2 != 0) // free space
		{
			int free = diskMap[left];
			while (free > 0 && left < right)
			{
				int file = diskMap[right];
				int toMove = free >= file ? file : free;
				id = right / 2;
				//(new { free, left, right, toMove, file, id, map = System.Text.Json.JsonSerializer.Serialize(diskMap) }).Dump();
				diskMap[right] -= toMove;
				free -= toMove;
				diskMap[left] -= toMove;
				if (diskMap[right] == 0)
				{
					right -= 2;
				}

				checksum += sum(toMove, id * pos, id * (pos + toMove - 1));
				//(new { checksum, map = System.Text.Json.JsonSerializer.Serialize(diskMap) }).Dump();
				pos += toMove;
			}
		}
		else
		{
			int n = diskMap[left];
			id = left / 2;
			checksum += sum(n, id * pos, id * (pos + n - 1));
			//(new { checksum, map = System.Text.Json.JsonSerializer.Serialize(diskMap) }).Dump("even");
			pos += diskMap[left];
		}

		//(new { checksum, map = System.Text.Json.JsonSerializer.Serialize(diskMap) }).Dump();
		left++;
	}

	checksum.Dump("Checksum");
}

long sum(int n, int a_1, int a_n)
{
	return checked((long)n * (a_1 + a_n) / 2);
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
