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
const int day = 9;
string[] input = GetInputLines(year, day);
string[] sample = @"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2".Split(Environment.NewLine);
string[] sample2 = @"R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20".Split(Environment.NewLine);

record class Point(int x, int y);
async Task part1()
{
	//input = sample;
	HashSet<Point> visited = new();
	var dirs = new Dictionary<char, Point>
	{
		['L'] = new(0, -1),
		['R'] = new(0, 1),
		['U'] = new(-1, 0),
		['D'] = new(1, 0)
	};
	Point start = new(0, 0);
	visited.Add((start));
	Point head = start;
	Point tail = start;
	foreach (string line in input)
	{
		var command = line.Split();
		char dir = command[0].Trim()[0];
		Debug.Assert(dirs.Keys.Contains(dir));
		int steps = int.Parse(command[1]);
		for (int i = 0; i < steps; i++)
		{
			int xx = dirs[dir].x;
			int yy = dirs[dir].y;
			head = new(head.x + xx, head.y + yy);
			if (head == tail)
			{
				visited.Add(tail);
				continue;
			}
			
			
			if (Math.Abs(head.y - tail.y) > 1 || Math.Abs(head.x - tail.x) > 1)
			{
				tail = new(head.x + -1 * xx, head.y + -1 * yy);
				visited.Add(tail);
			}
			
			//(dir, head, tail).ToString().Dump();
		}
	}
	
	visited.Count.Dump();
}
void part2()
{
	HashSet<Point> visited = new();
	var dirs = new Dictionary<char, Point>
	{
		['L'] = new(0, -1),
		['R'] = new(0, 1),
		['U'] = new(-1, 0),
		['D'] = new(1, 0)
	};
	Point[] rope = Enumerable.Range(0, 10).Select(x => new Point(0, 0)).ToArray();
	ref Point head = ref rope[0];
	visited.Add(head);
	foreach (string line in input)
	{
		var command = line.Split();
		char dir = command[0].Trim()[0];
		Debug.Assert(dirs.Keys.Contains(dir));
		int steps = int.Parse(command[1]);
		for (int _ = 0; _ < steps; _++)
		{
			int xx = dirs[dir].x;
			int yy = dirs[dir].y;
			head = new(head.x + xx, head.y + yy);
			for (int i = 1; i < rope.Length; i++)
			{
				int xt = rope[i].x;
				int yt = rope[i].y;
				Point h = rope[i-1];
				int xd = Math.Abs(xt - h.x);
				int yd = Math.Abs(yt - h.y);
				if (xd >= 2 && yd == 0)
				{
					rope[i] = new Point(xt + (xt > h.x ? -1 : 1), yt);
				}
				else if (yd >= 2 && xd == 0)
				{
					rope[i] = new Point(xt, yt + (yt > h.y ? -1 : 1));
				}
				else if ((xd >= 2 && yd >= 1) || (yd >= 2 && xd >= 1))
				{
					rope[i] = new Point(
						xt + (xt > h.x ? -1 : 1),
						yt + (yt > h.y ? -1 : 1));
				}
			}
			
			visited.Add(rope[^1]);
			//print(rope, line, height: 10, width: 20);
		}
		
		//print(rope, line, height: 10, width: 20);
	}
	
	visited.Count.Dump();
	
	void print(IList<Point> rope, string command, int height, int width)
	{
		command.Dump();
		for (int x = -1*height; x < height; x++)
		{
			var line = new StringBuilder();
			for (int y = -1*width; y < width; y++)
			{
				line.Append((x, y) switch
				{
					var (xx, yy) when xx == 0 && yy == 0 => "S",
					var (xx, yy) when xx == rope[0].x && yy == rope[0].y => "H",
					var (xx, yy) when rope.Contains(new Point(xx, yy)) => rope.IndexOf(new Point(xx, yy)).ToString(),
					_ => '.'
					
				});
			}
			
			line.ToString().Dump();
		}
	}
}

async Task Main()
{
	await part1();
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
