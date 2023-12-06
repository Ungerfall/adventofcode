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
const int year = 2023;
const int day = 6;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	var split = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
	var races = input[0].Split(':')[1].Trim().Split(' ', split).Select(int.Parse)
		.Zip(input[1].Split(':')[1].Trim().Split(' ', split).Select(int.Parse),
			(first, second) => (time: first, distance: second))
		.ToArray();

	long mul = races
		.AsParallel()
		.Select(x =>
		{
			int count = 0;
			for (int speed = 0; speed <= x.time; speed++)
			{
				int distance = speed * (x.time - speed);
				//(new { speed, distance, x, time = (x.time - speed)}).ToString().Dump();
				if (distance > x.distance)
				{
					count++;
				}
			}

			return count;
		})
		.Aggregate(1L, (acc, ways) => checked(acc * ways));

	mul.Dump("PRODUCT OF NUMBER OF WAYS TO BEAT THE RECORD");
}
void part2()
{
	int time = int.Parse(input[0].Split(':')[1].Trim().Replace(" ", string.Empty));
	long record = long.Parse(input[1].Split(':')[1].Trim().Replace(" ", string.Empty));
	int left = 0;
	int right = time;
	int max = 0;
	int min = time;
	while (left <= right)
	{
		int mid = (right + left) / 2;
		long distance = Math.BigMul(mid, (time - mid));
		//(new { left, right, mid, distance, record }).ToString().Dump();
		if (distance > record)
		{
			right = mid - 1;
			min = Math.Min(min, mid);
		}
		else
		{
			left = mid + 1;
		}
	}
	
	left = 0;
	right = time;
	while (left <= right)
	{
		int mid = (right + left) / 2;
		long distance = Math.BigMul(mid, (time - mid));
		if (distance > record)
		{
			max = Math.Max(max, mid);
			left = mid + 1;
		}
		else
		{
			right = mid - 1;
		}
	}
	
	(max - min + 1).Dump("NUMBER OF WAYS TO BEAT THE RECORD");
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
