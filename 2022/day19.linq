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
const int day = 19;
string[] input = GetInputLines(year, day);
string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

record class Cost(int ore, int clay, int obsidian);
record class Blueprint(
	int id,
	Cost oreRobot,
	Cost clayRobot,
	Cost obsidianRobot,
	Cost geodeRobot);
record struct Stats(int ore, int clay, int obsidian, int geode, int oreR, int clayR, int obsR, int geodeR, int mins);
async Task part1()
{
	Regex pattern = new(@"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.");
	List<Blueprint> blueprints = new();
	foreach (string line in input)
	{
		var g = pattern.Match(line).Groups;
		blueprints.Add(new(
			int.Parse(g[1].Value),
			new Cost(int.Parse(g[2].Value), 0, 0),
			new Cost(int.Parse(g[3].Value), 0, 0),
			new Cost(int.Parse(g[4].Value), int.Parse(g[5].Value), 0),
			new Cost(int.Parse(g[6].Value), 0, int.Parse(g[7].Value))
		));
	}

	int quality = 0;
	foreach (var bp in blueprints)
	{
		int maxOre = bp.oreRobot.ore + bp.clayRobot.ore + bp.obsidianRobot.ore + bp.geodeRobot.ore;
		int maxClay = bp.oreRobot.clay + bp.clayRobot.clay + bp.obsidianRobot.clay + bp.geodeRobot.clay;
		int maxObs = bp.oreRobot.obsidian + bp.clayRobot.obsidian + bp.obsidianRobot.obsidian + bp.geodeRobot.obsidian;
		//(maxOre, maxClay, maxObs, bp).ToString().Dump();
		Queue<Stats> bfs = new();
		HashSet<Stats> seen = new();
		bfs.Enqueue(new Stats(0, 0, 0, 0, oreR: 1, 0, 0, 0, 24));
		int best = int.MinValue;
		while (bfs.Count > 0)
		{
			var stats = bfs.Dequeue();
			var (ore, clay, obsidian, geode, oreR, clayR, obsR, geodeR, mins) = stats;
			//new { ore, clay, obsidian, geode, oreR, clayR, obsR, geodeR, mins = 24-mins}.ToString().Dump();
			Debug.Assert(ore >= 0 && clay >= 0 && obsidian >= 0);
			if (mins <= 0)
			{
				best = Math.Max(best, geode);
				continue;
			}
			
			if (seen.Contains(stats))
				continue;
			seen.Add(stats);

			bfs.Enqueue(new Stats(ore + oreR, clay + clayR, obsidian + obsR, geode + geodeR, oreR, clayR, obsR, geodeR, mins - 1));
			if (bp.oreRobot.ore <= ore && bp.oreRobot.clay <= clay && bp.oreRobot.obsidian <= obsidian && oreR < maxOre)
			{
				bfs.Enqueue(new Stats(ore - bp.oreRobot.ore + oreR,
					clay - bp.oreRobot.clay + clayR,
					obsidian - bp.oreRobot.obsidian + obsR,
					geode + geodeR, oreR + 1, clayR, obsR, geodeR, mins - 1));
			}
			if (bp.clayRobot.ore <= ore && bp.clayRobot.clay <= clay && bp.clayRobot.obsidian <= obsidian && clayR < maxClay)
			{
				bfs.Enqueue(new Stats(ore - bp.clayRobot.ore + oreR,
					clay - bp.clayRobot.clay + clayR,
					obsidian - bp.clayRobot.obsidian + obsR,
					geode + geodeR, oreR, clayR + 1, obsR, geodeR, mins - 1));
			}
			if (bp.obsidianRobot.ore <= ore && bp.obsidianRobot.clay <= clay && bp.obsidianRobot.obsidian <= obsidian && obsR < maxObs)
			{
				bfs.Enqueue(new Stats(ore - bp.obsidianRobot.ore + oreR,
					clay - bp.obsidianRobot.clay + clayR,
					obsidian - bp.obsidianRobot.obsidian + obsR,
					geode + geodeR, oreR, clayR, obsR + 1, geodeR, mins - 1));
			}
			if (bp.geodeRobot.ore <= ore && bp.geodeRobot.clay <= clay && bp.geodeRobot.obsidian <= obsidian)
			{
				bfs.Enqueue(new Stats(ore - bp.geodeRobot.ore + oreR,
					clay - bp.geodeRobot.clay + clayR,
					obsidian - bp.geodeRobot.obsidian + obsR,
					geode + geodeR, oreR, clayR, obsR, geodeR + 1, mins - 1));
				geodeR++;
			}
		}
		
		quality += bp.id * best;
	}

	quality.Dump("part 1.");
}

record struct State(int ore, int clay, int obsidian, int geode, int oreR, int clayR, int obsR, int geodeR);
async Task part2()
{
	Regex pattern = new(@"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.");
	List<Blueprint> blueprints = new();
	foreach (string line in input.Take(3))
	{
		var g = pattern.Match(line).Groups;
		blueprints.Add(new(
			int.Parse(g[1].Value),
			new Cost(int.Parse(g[2].Value), 0, 0),
			new Cost(int.Parse(g[3].Value), 0, 0),
			new Cost(int.Parse(g[4].Value), int.Parse(g[5].Value), 0),
			new Cost(int.Parse(g[6].Value), 0, int.Parse(g[7].Value))
		));
	}

	int quality = 1;
	foreach (var bp in blueprints)
	{
		int maxOre = (bp.oreRobot.ore + bp.clayRobot.ore + bp.obsidianRobot.ore + bp.geodeRobot.ore) * 2;
		int maxClay = (bp.oreRobot.clay + bp.clayRobot.clay + bp.obsidianRobot.clay + bp.geodeRobot.clay) * 2;
		int maxObs = (bp.oreRobot.obsidian + bp.clayRobot.obsidian + bp.obsidianRobot.obsidian + bp.geodeRobot.obsidian) * 2;
		(maxOre, maxClay, maxObs, bp).ToString().Dump();
		Queue<Stats> bfs = new();
		HashSet<State> seen = new();
		bfs.Enqueue(new Stats(0, 0, 0, 0, oreR: 1, 0, 0, 0, 32));
		int best = int.MinValue;
		while (bfs.Count > 0)
		{
			var stats = bfs.Dequeue();
			var (ore, clay, obsidian, geode, oreR, clayR, obsR, geodeR, mins) = stats;
			//new { ore, clay, obsidian, geode, oreR, clayR, obsR, geodeR, mins = 24-mins}.ToString().Dump();
			if (mins == 1)
			{
				best = Math.Max(best, geode + geodeR);
				continue;
			}

			ore = Math.Min(ore, maxOre);
			clay = Math.Min(clay, maxClay);
			obsidian = Math.Min(obsidian, maxObs);
			
			State s = new(ore, clay, obsidian, geode, oreR, clayR, obsR, geodeR);
			if (seen.Contains(s))
				continue;
			seen.Add(s);

			int canBuild = 0;
			if (bp.oreRobot.ore <= ore && bp.oreRobot.clay <= clay && bp.oreRobot.obsidian <= obsidian && oreR < maxOre)
			{
				bfs.Enqueue(new Stats(ore - bp.oreRobot.ore + oreR,
					clay - bp.oreRobot.clay + clayR,
					obsidian - bp.oreRobot.obsidian + obsR,
					geode + geodeR, oreR + 1, clayR, obsR, geodeR, mins - 1));
				canBuild++;
			}
			if (bp.clayRobot.ore <= ore && bp.clayRobot.clay <= clay && bp.clayRobot.obsidian <= obsidian && clayR < maxClay)
			{
				bfs.Enqueue(new Stats(ore - bp.clayRobot.ore + oreR,
					clay - bp.clayRobot.clay + clayR,
					obsidian - bp.clayRobot.obsidian + obsR,
					geode + geodeR, oreR, clayR + 1, obsR, geodeR, mins - 1));
				canBuild++;
			}
			if (bp.obsidianRobot.ore <= ore && bp.obsidianRobot.clay <= clay && bp.obsidianRobot.obsidian <= obsidian && obsR < maxObs)
			{
				bfs.Enqueue(new Stats(ore - bp.obsidianRobot.ore + oreR,
					clay - bp.obsidianRobot.clay + clayR,
					obsidian - bp.obsidianRobot.obsidian + obsR,
					geode + geodeR, oreR, clayR, obsR + 1, geodeR, mins - 1));
				canBuild++;
			}
			if (bp.geodeRobot.ore <= ore && bp.geodeRobot.clay <= clay && bp.geodeRobot.obsidian <= obsidian)
			{
				bfs.Enqueue(new Stats(ore - bp.geodeRobot.ore + oreR,
					clay - bp.geodeRobot.clay + clayR,
					obsidian - bp.geodeRobot.obsidian + obsR,
					geode + geodeR, oreR, clayR, obsR, geodeR + 1, mins - 1));
				canBuild++;
			}
			
			if (canBuild < 4)
			{
				bfs.Enqueue(new(ore + oreR, clay + clayR, obsidian + obsR, geode + geodeR, oreR, clayR, obsR, geodeR, mins - 1));
			}
		}
		
		seen.Count.Dump();
		best.Dump();
		quality *= best;
	}

	quality.Dump("part 2.");
}

async Task Main()
{
	//await part1();
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
