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
const int year = 2023;
const int day = 4;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	long sum = 0L;
	foreach (string line in input)
	{
		StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
		var winningAndMy = line.Split('|');
		var winning = winningAndMy[0].Trim().Split(':')[1].Trim().Split(' ', splitOptions).Select(int.Parse).ToHashSet<int>();
		var my = winningAndMy[1].Trim().Split(' ', splitOptions).Select(int.Parse).ToHashSet<int>();
		
		my.IntersectWith(winning);
		sum += (long)Math.Pow(2d, my.Count - 1);
	}
	
	sum.Dump("POINTS");
}
void part2()
{
	int[] cards = new int[input.Length];
	Array.Fill(cards, 1);
	int cardIndex = 0;
	foreach (string line in input)
	{
		StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
		var winningAndMy = line.Split('|');
		var winning = winningAndMy[0].Trim().Split(':')[1].Trim().Split(' ', splitOptions).Select(int.Parse).ToHashSet<int>();
		var my = winningAndMy[1].Trim().Split(' ', splitOptions).Select(int.Parse).ToHashSet<int>();
		
		my.IntersectWith(winning);
		int points = my.Count;
		for (int i = cardIndex + 1; i <= cardIndex + points && i < cards.Length; i++)
		{
			cards[i] += cards[cardIndex];
		}
		
		cardIndex++;
	}
	
	cards.Sum().Dump("SCRATCHCARDS");
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
