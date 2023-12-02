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
const int day = 2;
string[] input = GetInputLines(year, day);
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
async Task part1()
{
	long possibleGames = 0L;
	const int red = 12;
	const int green = 13;
	const int blue = 14;
	foreach (string line in input)
	{
		bool possible = true;
		string[] gameAndReveals = line.Split(':');
		int gameId = int.Parse(gameAndReveals[0].Split()[1].Trim());
		string[] reveals = gameAndReveals[1].Trim().Split(';');
		for (int i = 0; i < reveals.Length; i++)
		{
			if (!possible)
			{
				break;
			}
			
			foreach (string[] cubeAndColor in reveals[i].Trim().Split(',').Select(x => x.Trim().Split()))
			{
				int numberOfCubes = int.Parse(cubeAndColor[0].Trim());
				string color = cubeAndColor[1].Trim();
				possible &= (color, numberOfCubes) switch
				{
					("red", > red) => false,
					("green", > green) => false,
					("blue", > blue) => false,
					_ => true,
				};
			}
		}

		if (possible)
		{
			possibleGames += gameId;
		}
	}

	possibleGames.Dump("Possible games");
}
async Task part2()
{
	long totalPower = 0L;
	foreach (string line in input)
	{
		string[] gameAndReveals = line.Split(':');
		int gameId = int.Parse(gameAndReveals[0].Split()[1].Trim());
		string[] reveals = gameAndReveals[1].Trim().Split(';');
		int greenMax = int.MinValue;
		int blueMax = int.MinValue;
		int redMax = int.MinValue;
		for (int i = 0; i < reveals.Length; i++)
		{
			foreach (string[] cubeAndColor in reveals[i].Trim().Split(',').Select(x => x.Trim().Split()))
			{
				int numberOfCubes = int.Parse(cubeAndColor[0].Trim());
				string color = cubeAndColor[1].Trim();
				if (string.Equals(color, "red"))
				{
					redMax = Math.Max(redMax, numberOfCubes);
				}
				else if (string.Equals(color, "blue"))
				{
					blueMax = Math.Max(blueMax, numberOfCubes);
				}
				else
				{
					greenMax = Math.Max(greenMax, numberOfCubes);
				}
			}
		}
		
		totalPower += (long)redMax * Math.BigMul(blueMax, greenMax);
	}

	totalPower.Dump("Total power");
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
