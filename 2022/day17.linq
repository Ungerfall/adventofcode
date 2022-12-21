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
const int day = 17;
string[] input = GetInputLines(year, day);
string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

class InfiniteQueue<T>
{
	int index;
	private T[] elems;

	public InfiniteQueue(IEnumerable<T> seed)
	{
		elems = seed.ToArray();
		index = 0;
	}

	public T Dequeue()
	{
		if (index >= elems.Length)
		{
			index = 0;
		}

		return elems[index++];
	}

	public override string ToString()
	{
		return $"{index} [{string.Join(",", elems)}]";
	}
}

void print(IList<char[]> g, int top, bool clear)
{
	if (clear)
		Util.ClearResults();
		
	int r = 0;
	while (r < 2022 * 4 - 30 && g[r].All(x => x == '.'))
	{
		r++;
	}

	top.Dump();
	for (; r < g.Count; r++)
	{
		(new string((char[])g[r])).Dump();
	}

	Thread.Sleep(1000);
}

async Task part1()
{
	//input = sample;
	InfiniteQueue<char> actions = new(input[0].ToCharArray());
	InfiniteQueue<char> rocks = new(new char[] { '-' , '+', 'L', 'I', 'o' });
	int n = 2022;
	List<char[]> g = Enumerable.Range(0, n * 4)
		.Select(_ => Enumerable.Range(0, 7).Select(x => '.').ToArray())
		.ToList();
	int rMax = g.Count - 1;
	int cMax = g[0].Length - 1;
	int top = g.Count;
	for (int i = 0; i < n; i++)
	{
		char rock = rocks.Dequeue();
		var (r, c) = (top - 4, 2);
		r += (rock == '+') ? -1 : 0;
		while (true)
		{
			char action = actions.Dequeue();
			//(rock, action).ToString().Dump();
			if (rock == '-')
			{
				c = action switch
				{
					var a when a == '>' && c + 4 <= cMax && g[r][c + 4] == '.' => c + 1,
					'>' => c,
					var a when a == '<' && c - 1 >= 0 && g[r][c - 1] == '.' => c - 1,
					'<' => c,
					_ => throw new ArgumentException()
				};

				if (r + 1 <= rMax && g[r + 1][c] == '.' && g[r + 1][c + 1] == '.' && g[r + 1][c + 2] == '.' && g[r + 1][c + 3] == '.')
				{
					r++;
				}
				else
				{
					Debug.Assert(g[r][c] != '#' && g[r][c + 1] != '#' && g[r][c + 2] != '#' && g[r][c + 3] != '#');
					g[r][c] = '#';
					g[r][c + 1] = '#';
					g[r][c + 2] = '#';
					g[r][c + 3] = '#';
					break;
				}
			}
			else if (rock == '+')
			{
				c = action switch
				{
					var a when a == '>' && c + 3 <= cMax && g[r][c + 3] == '.' && g[r - 1][c + 2] == '.' && g[r + 1][c + 2] == '.' => c + 1,
					'>' => c,
					var a when a == '<' && c - 1 >= 0 && g[r][c - 1] == '.' && g[r - 1][c] == '.' && g[r + 1][c] == '.' => c - 1,
					'<' => c,
					_ => throw new ArgumentException()
				};
				if (r + 2 <= rMax && g[r + 1][c] == '.' && g[r + 2][c + 1] == '.' && g[r + 1][c + 2] == '.')
				{
					r++;
				}
				else
				{
					Debug.Assert(g[r][c] != '#' && g[r][c + 1] != '#' && g[r][c + 2] != '#' && g[r - 1][c + 1] != '#' && g[r + 1][c + 1] != '#');
					g[r][c] = '#';
					g[r][c + 1] = '#';
					g[r][c + 2] = '#';
					g[r - 1][c + 1] = '#';
					g[r + 1][c + 1] = '#';
					break;
				}
			}
			else if (rock == 'L')
			{
				c = action switch
				{
					var a when a == '>' && c + 3 <= cMax && g[r][c + 3] == '.' && g[r - 1][c + 3] == '.' && g[r - 2][c + 3] == '.' => c + 1,
					'>' => c,
					var a when a == '<' && c - 1 >= 0 && g[r][c - 1] == '.' && g[r - 1][c + 1] == '.' && g[r - 2][c + 1] == '.' => c - 1,
					'<' => c,
					_ => throw new ArgumentException()
				};

				if (r + 1 <= rMax && g[r + 1][c] == '.' && g[r + 1][c + 1] == '.' && g[r + 1][c + 2] == '.')
				{
					r++;
				}
				else
				{
					Debug.Assert(g[r][c] != '#' && g[r][c + 1] != '#' && g[r][c + 2] != '#' && g[r - 1][c + 2] != '#' && g[r - 2][c + 2] != '#');
					g[r][c] = '#';
					g[r][c + 1] = '#';
					g[r][c + 2] = '#';
					g[r - 1][c + 2] = '#';
					g[r - 2][c + 2] = '#';
					break;
				}

			}
			else if (rock == 'I')
			{
				c = action switch
				{
					var a when a == '>' && c + 1 <= cMax && g[r][c + 1] == '.' && g[r - 1][c + 1] == '.' && g[r - 2][c + 1] == '.' && g[r - 3][c + 1] == '.' => c + 1,
					'>' => c,
					var a when a == '<' && c - 1 >= 0 && g[r][c - 1] == '.' && g[r - 1][c - 1] == '.' && g[r - 2][c - 1] == '.' && g[r - 3][c - 1] == '.' => c - 1,
					'<' => c,
					_ => throw new ArgumentException()
				};

				if (r + 1 <= rMax && g[r + 1][c] == '.')
				{
					r++;
				}
				else
				{
					Debug.Assert(g[r][c] != '#' && g[r - 1][c] != '#' && g[r - 2][c] != '#' && g[r - 3][c] != '#');
					g[r][c] = '#';
					g[r - 1][c] = '#';
					g[r - 2][c] = '#';
					g[r - 3][c] = '#';
					break;
				}
			}
			else if (rock == 'o')
			{
				c = action switch
				{
					var a when a == '>' && c + 2 <= cMax && g[r][c + 2] == '.' && g[r-1][c+2] == '.' => c + 1,
					'>' => c,
					var a when a == '<' && c - 1 >= 0 && g[r][c - 1] == '.' && g[r-1][c-1] == '.' => c - 1,
					'<' => c,
					_ => throw new ArgumentException()
				};

				if (r + 1 <= rMax && g[r + 1][c] == '.' && g[r + 1][c + 1] == '.')
				{
					r++;
				}
				else
				{
					Debug.Assert(g[r][c] != '#' && g[r][c + 1] != '#' && g[r-1][c] != '#' && g[r-1][c + 1] != '#');
					g[r][c] = '#';
					g[r][c + 1] = '#';
					g[r-1][c] = '#';
					g[r-1][c + 1] = '#';
					break;
				}

			}
			else
			{
				throw new ArgumentException("Unknown rock");
			}
		}

		top = g.FindIndex(x => x.Any(ch => ch == '#'));
		//print(g, top, clear: true);
	}

	//print(g, top, clear: false);
	(g.Count - top).Dump("part 1");
}
async Task part2()
{
	foreach (string line in input)
	{

	}
}

async Task Main()
{
	part1();
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
