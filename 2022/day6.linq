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
	 ┗ 📜input.linq
*/
const int year = 2022;
const int day = 6;
string[] input = GetInputLines(year, day);
async Task part1()
{
	string line = input[0];
	int len = line.Length;
	for (int i = 3; i < len; i++)
	{
		char first = line[i-3];
		char second = line[i-2];
		char third = line[i-1];
		char forth = line[i];
		if (first != second && first != third && first != forth
			&& second != third && second != forth
			&& third != forth)
		{
			(i+1).Dump();
			return;
		}
	}
}
void part2()
{
	const int markerLen = 14;
	ReadOnlySpan<char> line = input[0].AsSpan();
	int len = line.Length;
	for (int i = markerLen; i < len; i++)
	{
		HashSet<char> set = new(line.Slice(i-markerLen, markerLen).ToArray());
		if (set.Count == markerLen)
		{
			(i).Dump();
			return;
		}
	}
}

async Task Main()
{
	await part1();
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
