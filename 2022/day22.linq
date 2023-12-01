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
const int day = 22;
string[] input = GetInputLines(year, day);
//string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
async Task part1()
{
	var parts = input.Split($"{Environment.NewLine}{Environment.NewLine}").ToArray();
	var matches = Regex.Matches(parts.Last(), @"(\d+)([RL]?)");
	var instructions = matches.Select(x => (Steps: int.Parse(x.Groups[1].Value), Rotate: x.Groups[2].Value));

	var lines = parts
		.First()
		.Split(Environment.NewLine)
		.ToArray();
	var position = (X: int.MinValue, Y: int.MinValue);
	var directions = new (int X, int Y)[]
	{
			(1, 0),
			(0, 1),
			(-1, 0),
			(0, -1),
	};
	var directionIndex = 0;
	var map = new Dictionary<(int X, int Y), bool>();
	var minX = new Dictionary<int, int>();
	var maxX = new Dictionary<int, int>();
	var minY = new Dictionary<int, int>();
	var maxY = new Dictionary<int, int>();

	for (var y = 1; y <= lines.Length; y++)
	{
		var line = lines[y - 1];
		for (var x = 1; x <= line.Length; x++)
		{
			var character = line[x - 1];
			if (char.IsWhiteSpace(character))
			{
				continue;
			}
			if (!minX.ContainsKey(y))
			{
				minX[y] = x;
			}
			maxX[y] = x;
			if (!minY.ContainsKey(x))
			{
				minY[x] = y;
			}
			maxY[x] = y;

			var point = (X: x, Y: y);
			if (character == '#')
			{
				map[point] = false;
			}
			else if (character == '.')
			{
				map[point] = true;
				if (position == (int.MinValue, int.MinValue))
				{
					position = (point);
				}
			}
		}
	}

	foreach (var instruction in instructions)
	{
		var (steps, rotate) = instruction;

		for (var i = 0; i < steps; i++)
		{
			var direction = directions[directionIndex];
			var next = (position.X + direction.X, position.Y + direction.Y);
			if (!map.TryGetValue(next, out var valid))
			{
				next = direction switch
				{
					(1, 0) => (minX[position.Y], position.Y),
					(0, 1) => (position.X, minY[position.X]),
					(-1, 0) => (maxX[position.Y], position.Y),
					(0, -1) => (position.X, maxY[position.X]),
					_ => throw new Exception()
				};
				valid = map[next];
			}

			if (!valid)
			{
				break;
			}

			position = next;
		}

		if (rotate == "R")
		{
			directionIndex += 1;
			directionIndex %= 4;
		}
		else if (rotate == "L")
		{
			directionIndex += 3;
			directionIndex %= 4;
		}
	}

	return 1000 * position.Y + 4 * position.X + directionIndex;
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
