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
const int day = 15;
string[] input = GetInputLines(year, day);
string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));


long distance(long x0, long y0, long x1, long y1)
{
	return Math.Abs(x0 - x1) + Math.Abs(y0 - y1);
}

void print(List<(int xS, int yS, int xB, int yB)> sensors, HashSet<(int x, int y)>? impossible = null)
{
	HashSet<(int x, int y)> s = sensors.Select(x => (x.xS, x.yS)).ToHashSet();
	HashSet<(int x, int y)> b = sensors.Select(x => (x.xB, x.yB)).ToHashSet();
	int xMin = s.Union(b).Min(x => x.x);
	int xMax = s.Union(b).Max(x => x.x);
	int yMin = s.Union(b).Min(x => x.y);
	int yMax = s.Union(b).Max(x => x.y);
	for (int x = xMin; x <= xMax; x++)
	{
		StringBuilder sb = new();
		sb.Append(x.ToString().PadRight(3));
		for (int y = yMin; y <= yMax; y++)
		{
			sb.Append(s.Contains((x, y))
				? 'S'
				: b.Contains((x, y))
					? 'B'
					: (impossible != null && impossible.Contains((x, y)))
						? '#'
						: '.'
			);
		}

		sb.ToString().Dump();
	}
}

async Task part1()
{
	Regex pattern = new(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");
	List<(int xS, int yS, int xB, int yB)> sensors = new();
	foreach (string line in input)
	{
		var g = pattern.Match(line).Groups;
		int sY = int.Parse(g[1].Value);
		int sX = int.Parse(g[2].Value);
		int bY = int.Parse(g[3].Value);
		int bX = int.Parse(g[4].Value);

		//(sX, sY, bX, bY).ToString().Dump();
		sensors.Add((sX, sY, bX, bY));
	}

	int xCheck = 2_000_000;
	HashSet<(long x, long y)> bOnCheck = new();
	HashSet<(long x, long y)> impossible = new();
	foreach (var s in sensors)
	{
		if (s.xB == xCheck)
		{
			bOnCheck.Add((s.xB, s.yB));
		}

		long toBeacon = distance(s.xS, s.yS, s.xB, s.yB);
		long toCheck = distance(s.xS, s.yS, xCheck, s.yS);
		if (toCheck < toBeacon)
		{
			for (long y = s.yS - toBeacon; y <= s.yS + toBeacon; y++)
			{
				//(s, toBeacon, toCheck).ToString().Dump();
				if (distance(s.xS, s.yS, xCheck, y) <= toBeacon)
				{
					impossible.Add((xCheck, y));
				}
			}
		}
	}

	impossible.ExceptWith(bOnCheck);
	//print(sensors, xMin, xMax, yMin, yMax, impossible);
	impossible.Count.Dump("Part 1");
}

record class Sensor(long x, long y, long toBeacon);
void part2()
{
	const int most = 4_000_000;
	Regex pattern = new(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");
	List<Sensor> sensors = new();
	foreach (string line in input)
	{
		var g = pattern.Match(line).Groups;
		int xS = int.Parse(g[1].Value);
		int yS = int.Parse(g[2].Value);
		int xB = int.Parse(g[3].Value);
		int yB = int.Parse(g[4].Value);
		long toBeacon = distance(xS, yS, xB, yB);

		//(sX, sY, bX, bY).ToString().Dump();
		sensors.Add(new(xS, yS, toBeacon));
	}

	long? xLost = null;
	long? yLost = null;
	for (long y = 0; y <= most; y++)
	{
		List<(long xLeft, long xRight)> segments = new();
		segments.Add((0, 0));
		foreach (var s in sensors)
		{
			long toLine = Math.Abs(y - s.y);
			if (toLine <= s.toBeacon)
			{
				segments.Add((Math.Max(0, s.x - (s.toBeacon - toLine)), Math.Min(most, s.x + (s.toBeacon - toLine))));
			}
		}
		
		foreach (var seg in segments)
		{
			if (seg.xRight >= most)
				continue;
				
			bool found = false;
			foreach (var inner in segments)
			{
				if (seg.xRight + 1 >= inner.xLeft && seg.xRight < inner.xRight)
				{
					found = true;
					break;
				}
			}
			
			if (!found)
			{
				xLost = seg.xRight + 1;
				yLost = y;
				goto STOP;
			}
		}
	}

STOP:
	(xLost, yLost, checked(xLost * 4_000_000 + yLost)).Dump();
	foreach (var s in sensors)
	{
		long d = distance(xLost!.Value, yLost!.Value, s.x, s.y);
		if (d <= s.toBeacon)
		{
			s.Dump("Reach to lost");
		}
	}	
}

async Task Main()
{
	//await part1();
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
