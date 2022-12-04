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
int year = 2022;
int day = 4;
async Task part1()
{
	int res = 0;
	foreach (string line in await GetInputLines(year, day))
	{
		int[] ranges = line
			.Split(',')
			.Select(x => x.Split('-'))
			.SelectMany(x => x)
			.Select(x => int.Parse(x))
			.ToArray();
		var (a0, a1) = (ranges[0], ranges[1]);
		var (b0, b1) = (ranges[2], ranges[3]);
		if ((a0 <= b0 && a1 >= b1)
			|| (b0 <= a0) && b1 >= a1)
		{
			res++;
		}
	}
	
	res.Dump();
}
async Task part2()
{
	int res = 0;
	foreach (string line in await GetInputLines(year, day))
	{
		int[] ranges = line
			.Split(',')
			.Select(x => x.Split('-'))
			.SelectMany(x => x)
			.Select(x => int.Parse(x))
			.ToArray();
		var (a0, a1) = (ranges[0], ranges[1]);
		var (b0, b1) = (ranges[2], ranges[3]);
		if (a1 < b0 || a0 > b1)
		 continue;
		
		res++;
	}
	
	res.Dump();
}

async Task Main()
{
	await part1();
	await part2();
}

async Task<IEnumerable<string>> GetInputLines(int year, int day)
{
	Debug.Assert(year <= DateTime.Now.Year);
	Debug.Assert(day > 0 && day <= 25);
	var root = Directory.GetParent(Path.GetDirectoryName(Util.CurrentQueryPath));
	var inputDir = Directory.CreateDirectory(Path.Combine(root.FullName, year.ToString(), "input"));
	var input = Path.Combine(inputDir.FullName, $"{day}.txt");
	if (File.Exists(input))
	{
		return File.ReadLines(input);
	}
	else
	{
		using System.Net.Http.HttpClient c = new();
		string ga = Util.GetPassword("adventofcode_ga");
		string session = Util.GetPassword("adventofcode_session");
		c.DefaultRequestHeaders.Add("cookie", "_ga=" + ga + "; session=" + session);
		string uri = $"https://adventofcode.com/{year}/day/{day}/input";
		var content = (await c.GetAsync(uri)).Content;
		using (StreamWriter sw = new(input))
		{
			await content.CopyToAsync(sw.BaseStream);
		}

		return File.ReadLines(input);
	}
}
