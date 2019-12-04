<Query Kind="Program" />

void Main()
{
	using var sr = new StreamReader("F:/development/adventofcode/2019/3/input2.txt");
	var line = sr.ReadLine();
	var turns = line
		.Trim().Split(',')
		.Select(x => new { Dir = x[0], Steps = int.Parse(x.Substring(1)) });
	var wire1 = new HashSet<(int x, int y, int steps)>();
	var wire2 = new HashSet<(int x, int y, int steps)>();
	
	int x = 0, y = 0, steps = 0;
	foreach (var turn in turns)
	{
		Turn(wire1, ref x, ref y, ref steps, turn.Dir, turn.Steps);
	}
	
	line = sr.ReadLine();
	turns = line
		.Trim().Split(',')
		.Select(x => new { Dir = x[0], Steps = int.Parse(x.Substring(1)) });
	x = 0; y = 0; steps = 0;
	foreach (var turn in turns)
	{
		Turn(wire2, ref x, ref y, ref steps, turn.Dir, turn.Steps);
	}

	// What is the Manhattan distance from the central port to the closest intersection?
	wire1
		.Join(
			wire2,
			w1 => w1,
			w2 => w2,
			(w1, w2) => new
			{
				w1x = w1.Item1, w1y = w1.Item2, w1s = w1.Item3,
				w2x = w2.Item1, w2y = w2.Item2, w2s = w2.Item3
			},
			new WiresComparer())
		.Min(x => x.w1s + x.w2s)
		.Dump();
}

class WiresComparer : IEqualityComparer<ValueTuple<int, int, int>>
{
	public bool Equals(ValueTuple<int, int, int> w1, ValueTuple<int, int, int> w2)
    {
        return w1.Item1 == w2.Item1
			&& w1.Item2 == w2.Item2;
    }

    public int GetHashCode(ValueTuple<int, int, int> w)
    {
        int hCode = w.Item1 ^ w.Item2;
        return hCode.GetHashCode();
    }
}

void Turn(HashSet<(int, int, int)> visited, ref int x, ref int y, ref int steps, char dir, int stepsCount)
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
		for (int i = 1; i <= stepsCount; i++)
		{
			x += vert.Value;
			steps++;
			visited.Add((x, y, steps));
		}
	}
	else
	{
		for (int i = 1; i <= stepsCount; i++)
		{
			y += hor.Value;
			steps++;
			visited.Add((x, y, steps));
		}
	}
}

// Define other methods, classes and namespaces here