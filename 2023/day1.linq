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
const int day = 1;
string[] input = GetInputLines(year, day);
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
async Task part1()
{
	long sum = 0L;
	foreach (string line in input)
	{
		int index = 0;
		while (line[index] < '0' || line[index] >'9')
		{
			index++;
		}
		
		char firstDigit = line[index];
		char lastDigit = firstDigit;
		for (int i = index + 1; i < line.Length; i++)
		{
			if (line[i] >= '0' && line[i] <= '9')
			{
				lastDigit = line[i];
			}
		}

		sum += int.Parse(new char[] { firstDigit, lastDigit });
	}
	
	Console.WriteLine(sum);
}
async Task part2()
{
	long sum = 0L;
	var digitWords = new Dictionary<string, string>
	{
		["oneight"] = "18",
		["twone"] = "21",
		["threeight"] = "38",
		["fiveight"] = "58",
		["sevenine"] = "79",
		["eighthree"] = "83",
		["eightwo"] = "82",
		["nineight"] = "98",
		
		["one"] = "1",
		["two"] = "2",
		["three"] = "3",
		["four"] = "4",
		["five"] = "5",
		["six"] = "6",
		["seven"] = "7",
		["eight"] = "8",
		["nine"] = "9",
	};
	foreach (string line in input)
	{
		string calibration = line;
		foreach (var (k, v) in digitWords)
		{
			calibration = calibration.Replace(k, v);
		}

		int index = 0;
		while (calibration[index] < '0' || calibration[index] >'9')
		{
			index++;
		}
		
		char firstDigit = calibration[index];
		char lastDigit = firstDigit;
		for (int i = index + 1; i < calibration.Length; i++)
		{
			if (calibration[i] >= '0' && calibration[i] <= '9')
			{
				lastDigit = calibration[i];
			}
		}

		sum += int.Parse(new char[] { firstDigit, lastDigit });
	}
	
	Console.WriteLine(sum);
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
