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
const int day = 18;
string[] input = GetInputLines(year, day);
string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
async Task part1()
{
	Dictionary<(float x, float y, float z), int> d = new();
	(int x, int y, int z)[][] edges = new[]
	{
		new[] {(0,0,0), (1,0,1)},
		new[] {(0,0,0), (1,1,0)},
		new[] {(0,0,0), (0,1,1)},
		new[] {(1,1,1), (1,0,0)},
		new[] {(1,1,1), (0,0,1)},
		new[] {(1,1,1), (0,1,0)},
	};

	foreach (string line in input)
	{
		var xyz = line.Trim().Split(',');
		int x = int.Parse(xyz[0]);
		int y = int.Parse(xyz[1]);
		int z = int.Parse(xyz[2]);

		foreach (var edge in edges)
		{
			(int x, int y, int z) d1 = (x + edge[0].x, y + edge[0].y, z + edge[0].z);
			(int x, int y, int z) d2 = (x + edge[1].x, y + edge[1].y, z + edge[1].z);
			var l = ((d1.x + d2.x) / 2f, (d1.y + d2.y) / 2f, (d1.z + d2.z) / 2f);
			if (d.TryGetValue(l, out int count))
			{
				d[l] = count + 1;
			}
			else
			{
				d[l] = 1;
			}
		}
	}

	d.Count(x => x.Value == 1).Dump("Part 1");
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
