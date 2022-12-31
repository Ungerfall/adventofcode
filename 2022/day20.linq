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
const int day = 20;
string[] input = GetInputLines(year, day);
string[] sample = new[] {
	"1",
	"2",
	"-3",
	"3",
	"-2",
	"0",
	"4",
};
async Task part1()
{
	var numAndIndex = new List<(int value, int index)>();
	for (int i = 0; i < input.Length; i++)
	{
		numAndIndex.Add((int.Parse(input[i]), i));

	}

	var mixed = new List<(int value, int index)>(numAndIndex);
	var count = numAndIndex.Count;
	for (int i = 0; i < count; i++)
	{
		var num = numAndIndex[i];
		var oldIx = mixed.IndexOf(num);
		var newIx = (oldIx + num.value) % (count - 1);

		if (newIx < 0)
			newIx = count + newIx - 1;

		mixed.Remove(num);
		mixed.Insert(newIx, num);
	}

	var indexZero = mixed.FindIndex(e => e.value == 0);
	var index1000 = (1000 + indexZero) % count;
	var index2000 = (2000 + indexZero) % count;
	var index3000 = (3000 + indexZero) % count;

	var coordinatesSum = mixed[index1000].value + mixed[index2000].value + mixed[index3000].value;

	coordinatesSum.Dump("Part 1");
}
async Task part2()
{
	foreach (string line in input)
	{

	}
}

async Task Main()
{
	await part1();
	await part2();
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
