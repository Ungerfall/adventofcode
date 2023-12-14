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
const int day = 14;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
const char ROUNDED_ROCK = 'O';
const char CUBE_ROCK = '#';
const char EMPTY_SPACE = '.';
void part1()
{
	long totalLoad = 0L;
	for (int col = 0; col < input[0].Length; col++)
	{
		int lastCubeRockWeight = input.Length + 1;
		int roundedRocks = 0;
		for (int row = 0; row < input.Length; row++)
		{
			if (input[row][col] == ROUNDED_ROCK)
			{
				roundedRocks++;
			}
			else if (input[row][col] == CUBE_ROCK)
			{
				int firstRock = lastCubeRockWeight - roundedRocks;
				totalLoad += roundedRocks * (2 * firstRock + roundedRocks - 1) / 2;
				lastCubeRockWeight = input.Length - row;
				roundedRocks = 0;
			}
		}

		if (roundedRocks > 0)
		{
			int firstRock = lastCubeRockWeight - roundedRocks;
			totalLoad += roundedRocks * (2 * firstRock + roundedRocks - 1) / 2;
		}
	}

	totalLoad.Dump("TOTAL LOAD");
}
void part2()
{
	const int cycles = 1_000_000_000;
	int cycle = 1;
	int? loopedCycle = null;
	Dictionary<string, int> states = new();
	string state = string.Empty;
	char[][] mutableGrid = input.Select(x => x.ToCharArray()).ToArray();
	while (cycle < cycles)
	{
		// north
		for (int col = 0; col < mutableGrid[0].Length; col++)
		{
			int lastCubeRock = -1;
			int roundedRocks = 0;
			for (int row = 0; row < mutableGrid.Length; row++)
			{
				if (mutableGrid[row][col] == ROUNDED_ROCK)
				{
					roundedRocks++;
					mutableGrid[row][col] = EMPTY_SPACE;
				}
				else if (mutableGrid[row][col] == CUBE_ROCK)
				{
					int firstRock = lastCubeRock + 1;
					for (int i = 0; i < roundedRocks; i++)
					{
						mutableGrid[firstRock + i][col] = ROUNDED_ROCK;
					}

					lastCubeRock = row;
					roundedRocks = 0;
				}
			}

			if (roundedRocks > 0)
			{
				int firstRock = lastCubeRock + 1;
				for (int i = 0; i < roundedRocks; i++)
				{
					mutableGrid[firstRock + i][col] = ROUNDED_ROCK;
				}
			}
		}

		// west
		for (int row = 0; row < mutableGrid.Length; row++)
		{
			int lastCubeRock = -1;
			int roundedRocks = 0;
			for (int col = 0; col < mutableGrid[0].Length; col++)
			{
				if (mutableGrid[row][col] == ROUNDED_ROCK)
				{
					roundedRocks++;
					mutableGrid[row][col] = EMPTY_SPACE;
				}
				else if (mutableGrid[row][col] == CUBE_ROCK)
				{
					int firstRock = lastCubeRock + 1;
					for (int i = 0; i < roundedRocks; i++)
					{
						mutableGrid[row][firstRock + i] = ROUNDED_ROCK;
					}

					lastCubeRock = col;
					roundedRocks = 0;
				}
			}

			if (roundedRocks > 0)
			{
				int firstRock = lastCubeRock + 1;
				for (int i = 0; i < roundedRocks; i++)
				{
					mutableGrid[row][firstRock + i] = ROUNDED_ROCK;
				}
			}
		}

		// south
		for (int col = 0; col < mutableGrid[0].Length; col++)
		{
			int lastCubeRock = mutableGrid.Length;
			int roundedRocks = 0;
			for (int row = mutableGrid.Length - 1; row >= 0; row--)
			{
				if (mutableGrid[row][col] == ROUNDED_ROCK)
				{
					roundedRocks++;
					mutableGrid[row][col] = EMPTY_SPACE;
				}
				else if (mutableGrid[row][col] == CUBE_ROCK)
				{
					int firstRock = lastCubeRock - 1;
					for (int i = 0; i < roundedRocks; i++)
					{
						mutableGrid[firstRock - i][col] = ROUNDED_ROCK;
					}

					lastCubeRock = row;
					roundedRocks = 0;
				}
			}

			if (roundedRocks > 0)
			{
				int firstRock = lastCubeRock - 1;
				for (int i = 0; i < roundedRocks; i++)
				{
					mutableGrid[firstRock - i][col] = ROUNDED_ROCK;
				}
			}
		}

		// east
		for (int row = 0; row < mutableGrid.Length; row++)
		{
			int lastCubeRock = mutableGrid[0].Length;
			int roundedRocks = 0;
			for (int col = mutableGrid[0].Length - 1; col >= 0; col--)
			{
				if (mutableGrid[row][col] == ROUNDED_ROCK)
				{
					roundedRocks++;
					mutableGrid[row][col] = EMPTY_SPACE;
				}
				else if (mutableGrid[row][col] == CUBE_ROCK)
				{
					int firstRock = lastCubeRock - 1;
					for (int i = 0; i < roundedRocks; i++)
					{
						mutableGrid[row][firstRock - i] = ROUNDED_ROCK;
					}

					lastCubeRock = col;
					roundedRocks = 0;
				}
			}

			if (roundedRocks > 0)
			{
				int firstRock = lastCubeRock - 1;
				for (int i = 0; i < roundedRocks; i++)
				{
					mutableGrid[row][firstRock - i] = ROUNDED_ROCK;
				}
			}
		}

		// state management
		state = calcState(mutableGrid);
		if (states.TryGetValue(state, out int looped))
		{
			loopedCycle = looped;
			break;
		}
		else
		{
			states[state] = cycle;
		}

		cycle++;
	}

	if (loopedCycle is not null)
	{
		int loopSize = states.Values.Where(x => x >= loopedCycle.Value).Count();
		int restCycles = cycles - cycle;
		int destinationCycle = restCycles % loopSize;
		//(new { loopSize, rest = (restCycles, cycle), destinationCycle, loopedCycle}).ToString().Dump();
		if (destinationCycle == 0)
		{
			destinationCycle = loopedCycle.Value;
		}

		foreach (var (k, v) in states)
		{
			if (v == destinationCycle + loopedCycle.Value)
			{
				mutableGrid = stateToGrid(k, mutableGrid[0].Length);
				break;
			}
		}
	}
	long totalLoad = calcLoad(mutableGrid);

	totalLoad.Dump("TOTAL LOAD");
	
	static long calcLoad(char[][] grid)
	{
		long totalLoad = 0L;
		for (int row = 0; row < grid.Length; row++)
		{
			for (int col = 0; col < grid[0].Length; col++)
			{
				if (grid[row][col] == ROUNDED_ROCK)
				{
					totalLoad += grid.Length - row;
				}
			}
		}
		
		return totalLoad;
	}

	static char[][] stateToGrid(string state, int len)
	{
		return state.Chunk(len).ToArray();
	}

	static string calcState(char[][] grid)
	{
		return string.Join(string.Empty, grid.Select(x => new string(x)));
	}

	static void printGrid(char[][] grid)
	{
		for (int row = 0; row < grid.Length; row++)
		{
			(new string(grid[row])).Dump();
		}

		"".Dump();
	}
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
