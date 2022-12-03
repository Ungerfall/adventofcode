<Query Kind="Program" />

static string[] lines = File.ReadAllLines(@"C:\development\adventofcode\2022\AdventOfCSharp.Template\Inputs\2022\3.txt");
void Main()
{
	part1();
	part2();
}

void part2()
{
	List<char> compartments = new();
	foreach (var line in lines.Chunk(3))
	{
		HashSet<char> parts = new(line[0]);
		parts.IntersectWith(line[1]);
		parts.IntersectWith(line[2]);
		
		Debug.Assert(parts.Count == 1);
		compartments.Add(parts.ElementAt(0));
	}

	compartments
		.Select(x =>
		{
			int v = x >= 'a'
				? x - 'a' + 1
				: x - 'A' + 27;
			return v;
		})
		.Sum()
		.Dump(); ;

}

void part1()
{
	List<char> compartments = new();
	foreach (var line in lines)
	{
		int half = line.Length / 2;
		HashSet<char> rucksack = new(line[0..half]);
		for (int i = half; i < line.Length; i++)
		{
			if (rucksack.Contains(line[i]))
			{
				compartments.Add(line[i]);
				break;
			}
		}
	}

	compartments
		.Select(x =>
		{
			int v = x >= 'a'
				? x - 'a' + 1
				: x - 'A' + 27;
			return v;
		})
		.Sum()
		.Dump(); ;
}

