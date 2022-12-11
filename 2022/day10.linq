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
const int day = 10;
string[] input = GetInputLines(year, day);
string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", "10.sample.txt"));
async Task part1()
{
	long sum = 0;
	int cycle = 0;
	int register = 1;
	int checkpoint = 20;
	foreach (string line in input)
	{
		if (checkpoint > 220)
			break;
			
		var instruction = line.Split();
		cycle += instruction[0] == "noop" ? 1 : 2;
		if (cycle >= checkpoint)
		{
			sum += checkpoint * register;
			checkpoint += 40;
		}
		
		if (instruction[0] == "addx")
		{
			register += int.Parse(instruction[1]);
		}
	}
	
	sum.Dump();
}
async Task part2()
{
	//input = sample;
	int cycle = 0;
	int register = 1;
	int width = 40;
	int end = 240;
	StringBuilder crt = new();
	foreach (string line in input)
	{
		if (cycle >= end)
			break;
			
		var instruction = line.Split();
		int newCycles = instruction[0] == "noop" ? 1 : 2;
		while (newCycles > 0)
		{
			if (cycle % width >= register - 1 && cycle % width <= register + 1)
			{
				crt.Append('#');
			}
			else
			{
				crt.Append('.');
			}
				
			cycle++;
			newCycles--;
		}
		
		if (instruction[0] == "addx")
		{
			register += int.Parse(instruction[1]);
		}
	}
	
	print(crt);
	
	void print(StringBuilder crt)
	{
		foreach (var line in crt.ToString().Chunk(40))
		{
			(new string(line)).Dump();
		}
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
