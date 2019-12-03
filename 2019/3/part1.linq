<Query Kind="Program" />

void Main()
{
	using var sr = new StreamReader("D:/dev/adventofcode/2019/3/input.txt");
	var line = sr.ReadLine();
	var turns = line
		.Trim().Split(',')
		.Select(x => new { Dir = x[0], Steps = int.Parse(x.Substring(1)) });
	var wire1 = new HashSet<(int x, int y)>();
	var wire2 = new HashSet<(int x, int y)>();
	
	int x = 0, y = 0;
	foreach (var turn in turns)
	{
		Turn(wire1, ref x, ref y, turn.Dir, turn.Steps);
	}
	
	line = sr.ReadLine();
	turns = line
		.Trim().Split(',')
		.Select(x => new { Dir = x[0], Steps = int.Parse(x.Substring(1)) });
	x = 0; y = 0;
	foreach (var turn in turns)
	{
		Turn(wire2, ref x, ref y, turn.Dir, turn.Steps);
	}

	// What is the Manhattan distance from the central port to the closest intersection?
	wire1.Intersect(wire2)
		.Min(p => Math.Abs(0 - p.x) + Math.Abs(0 - p.y))
		.Dump();
}

void Turn(HashSet<(int, int)> visited, ref int x, ref int y, char dir, int steps)
{
	int? vert = dir == 'L' || dir == 'R'
		? dir == 'L'
			? -1
			: 1
		: (int?) null;
	int? hor = dir == 'D' || dir == 'U'
		? dir == 'D'
			? -1
			: 1
		: (int?) null;
	if (vert != null)
	{
		for (int i = 1; i <= steps; i++)
		{
			x += vert.Value;
			visited.Add((x, y));
		}
	}
	else
	{
		for (int i = 1; i <= steps; i++)
		{
			y += hor.Value;
			visited.Add((x, y));
		}
	}
}

// Define other methods, classes and namespaces here