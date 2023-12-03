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
const int day = 3;
string[] input = GetInputLines(year, day);
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
public record Number(int value, int row, int colStart, int colEnd);
void part1()
{
	var numbers = new List<Number>();
	for (int row = 0; row < input.Length; row++)
	{
		ReadOnlySpan<char> line = input[row].AsSpan();
		int currentNumberLength = line[0] >= '0' && line[0] <= '9' ? 1 : 0;
		for (int col = 1; col < line.Length; col++)
		{
			if (line[col] < '0' || line[col] > '9')
			{
				if (currentNumberLength > 0)
				{
					int number = int.Parse(line.Slice(col - currentNumberLength, currentNumberLength));
					numbers.Add(new(number, row, col - currentNumberLength, col - 1));
					currentNumberLength = 0;
				}
			}
			else
			{
				currentNumberLength++;
			}
		}

		if (currentNumberLength > 0)
		{
			int number = int.Parse(line.Slice(line.Length - currentNumberLength, currentNumberLength));
			numbers.Add(new(number, row, line.Length - currentNumberLength, line.Length - 1));
		}
	}

	int rowEnd = input[0].Length;
	int colEnd = input.Length;
	long sum = 0L;
	foreach (var number in numbers)
	{
		bool adjacent = false;
		int left = Math.Max(number.colStart - 1, 0);
		int right = Math.Min(number.colEnd + 2, rowEnd);
		if (number.row > 0)
		{
			for (int i = left; i < right; i++)
			{
				if (input[number.row - 1][i] != '.')
				{
					adjacent = true;
					break;
				}
			}
		}

		char leftChar = input[number.row][left];
		char rightChar = input[number.row][right - 1];
		if ((leftChar < '0' || leftChar > '9') && leftChar != '.')
		{
			adjacent = true;
		}

		if ((rightChar < '0' || rightChar > '9') && rightChar != '.')
		{
			adjacent = true;
		}

		if (number.row < colEnd - 1)
		{
			for (int i = left; i < right; i++)
			{
				if (input[number.row + 1][i] != '.')
				{
					adjacent = true;
					break;
				}
			}
		}

		if (adjacent)
		{
			sum += number.value;
		}
	}

	sum.Dump("SUM");
}
void part2()
{
	var numbers = new List<Number>();
	for (int row = 0; row < input.Length; row++)
	{
		ReadOnlySpan<char> line = input[row].AsSpan();
		int currentNumberLength = line[0] >= '0' && line[0] <= '9' ? 1 : 0;
		for (int col = 1; col < line.Length; col++)
		{
			if (line[col] < '0' || line[col] > '9')
			{
				if (currentNumberLength > 0)
				{
					int number = int.Parse(line.Slice(col - currentNumberLength, currentNumberLength));
					numbers.Add(new(number, row, col - currentNumberLength, col - 1));
					currentNumberLength = 0;
				}
			}
			else
			{
				currentNumberLength++;
			}
		}

		if (currentNumberLength > 0)
		{
			int number = int.Parse(line.Slice(line.Length - currentNumberLength, currentNumberLength));
			numbers.Add(new(number, row, line.Length - currentNumberLength, line.Length - 1));
		}
	}

	int rowEnd = input.Length;
	int colEnd = input[0].Length;
	const char gearChar = '*';
	HashSet<(int row, int col)>? gears = new();
	for (int row = 0; row < colEnd; row++)
	{
		for (int col = 0; col < colEnd; col++)
		{
			if (input[row][col] == gearChar)
			{
				gears.Add((row, col));
			}
		}
	}

	long sum = 0L;
	foreach (var gear in gears)
	{
		HashSet<Number> power = new();
		int? top = gear.row == 0 ? null : gear.row - 1;
		int? bottom = gear.row == rowEnd - 1 ? null : gear.row + 1;
		int left = Math.Max(gear.col - 1, 0);
		int right = Math.Min(gear.col + 1, colEnd - 1);
		foreach (var number in numbers)
		{
			if (top is not null)
			{
				if (top.Value == number.row
					&& (
						(left <= number.colStart && right >= number.colStart)
						|| (left >= number.colStart && left <= number.colEnd)
						|| (right >= number.colEnd && right <= number.colEnd))
					)
				{
					power.Add(number);
					continue;
				}
			}

			if (bottom is not null)
			{
				if (bottom.Value == number.row
					&& (
						(left <= number.colStart && right >= number.colStart)
						|| (left >= number.colStart && left <= number.colEnd)
						|| (right >= number.colEnd && right <= number.colEnd))
					)
				{
					power.Add(number);
					continue;
				}
			}

			if (gear.row == number.row && (left == number.colEnd || right == number.colStart))
			{
				power.Add(number);
				continue;
			}
		}

		/*
		if (power.Count != 2)
		{
			(new { gear, top, bottom, left, right, power = $"[{string.Join(',', power)}]" }).ToString().Dump();
			for (int row = Math.Max(0, (top ?? 0) - 5); row < Math.Min(rowEnd, (bottom ?? rowEnd) + 5); row++)
			{
				for (int col = Math.Max(0, left - 5); col < Math.Min(colEnd, right + 5); col++)
				{
					Console.Write(input[row][col]);
				}

				Console.WriteLine();
			}
		}
		*/

		if (power.Count == 2)
		{
			sum += power.Aggregate(1L, (acc, num) => acc * num.value);
		}
	}

	sum.Dump("SUM OF POWERS");
}

async Task Main()
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
