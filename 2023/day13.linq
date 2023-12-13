<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Collections.Immutable</Namespace>
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
const int day = 13;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
const char ash = '.';
const char rock = '#';
void part1()
{
	var (patterns, current) = input
		.Aggregate((patterns: new List<List<string>>(), current: new List<string>()), (acc, line) =>
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				acc.patterns.Add(acc.current);
				return (acc.patterns, new List<string>());
			}
			else
			{
				acc.current.Add(line);
				return (acc.patterns, acc.current);
			}
		});
	patterns.Add(current);
	
	long sum = 0L;
	foreach (var pattern in patterns)
	{
		var mutablePattern = pattern
			.Select(x => x.ToCharArray())
			.ToList();
		var (foundHorizontal, colsOnLeft, bestHorizontal) = GetBestMirrorLine(mutablePattern);

		var transposed = new List<char[]>();
		for (int col = 0; col < pattern[0].Length; col++)
		{
			var columnString = new char[pattern.Count];
			for (int row = 0; row < pattern.Count; row++)
			{
				columnString[row] = pattern[row][col];
			}
			transposed.Add(columnString);
		}

		var (foundVertical, rowsAbove, bestVertical) = GetBestMirrorLine(transposed);
		//(new { bestHorizontal, bestVertical, colsOnLeft, rowsAbove }).ToString().Dump();
		if (foundHorizontal && foundVertical)
		{
			"BOTH FOUND".Dump();
		}
		if (!foundHorizontal && !foundVertical)
		{
			"BOTH NOT FOUND".Dump();
		}
		
		if (foundHorizontal)
		{
			sum += colsOnLeft;
		}
		else
		{
			sum += 100 * rowsAbove;
		}
	}

	sum.Dump("SUMMARY OF ALL NOTES");
}
void part2()
{
	var (patterns, current) = input
		.Aggregate((patterns: new List<List<string>>(), current: new List<string>()), (acc, line) =>
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				acc.patterns.Add(acc.current);
				return (acc.patterns, new List<string>());
			}
			else
			{
				acc.current.Add(line);
				return (acc.patterns, acc.current);
			}
		});
	patterns.Add(current);
	
	long sum = 0L;
	foreach (var pattern in patterns)
	{
		var mutablePattern = pattern
			.Select(x => x.ToCharArray())
			.ToList();
		var (foundHorizontal, colsOnLeft, bestHorizontal) = GetBestMirrorLine(mutablePattern);

		var transposed = new List<char[]>();
		for (int col = 0; col < pattern[0].Length; col++)
		{
			var columnString = new char[pattern.Count];
			for (int row = 0; row < pattern.Count; row++)
			{
				columnString[row] = pattern[row][col];
			}
			transposed.Add(columnString);
		}

		var (foundVertical, rowsAbove, bestVertical) = GetBestMirrorLine(transposed);
		//(new { bestHorizontal, bestVertical, colsOnLeft, rowsAbove }).ToString().Dump();
		if (foundHorizontal && foundVertical)
		{
			"BOTH FOUND".Dump();
		}
		if (!foundHorizontal && !foundVertical)
		{
			"BOTH NOT FOUND".Dump();
		}
		
		if (foundHorizontal)
		{
			var smudgedHorizontal = GetMirrorLinesSmudged(mutablePattern);
			var smudgedVertical = GetMirrorLinesSmudged(transposed);
			//smudgedHorizontal.Dump();
			//smudgedVertical.Dump();
			sum += (smudgedHorizontal.Count, smudgedVertical.Count) switch
			{
				(2, 0) => smudgedHorizontal.Except(new[] { (foundHorizontal, colsOnLeft, bestHorizontal)}).Single().Item2,
				(1, 1) => smudgedVertical.First().col * 100,
				_ => throw new ArgumentException("SMUDGED FAILED"),
			};
		}
		else
		{
			var smudgedHorizontal = GetMirrorLinesSmudged(mutablePattern);
			var smudgedVertical = GetMirrorLinesSmudged(transposed);
			//smudgedHorizontal.Dump();
			//smudgedVertical.Dump();
			//(foundVertical, rowsAbove, bestVertical).Dump();
			sum += (smudgedHorizontal.Count, smudgedVertical.Count) switch
			{
				(0, 2) => smudgedVertical.Except(new[] { (foundVertical, rowsAbove, bestVertical)}).Single().Item2 * 100,
				(1, 1) => smudgedHorizontal.First().col,
				_ => throw new ArgumentException("SMUDGED FAILED"),
			};
		}
	}

	sum.Dump("SUMMARY OF ALL NOTES");
}

static IReadOnlySet<(bool found, int col, int value)> GetMirrorLinesSmudged(IList<char[]> candidates)
{
	HashSet<(bool, int, int)> mirrors = new();
	for (int row = 0; row < candidates.Count; row++)
	{
		for (int col = 0; col < candidates[0].Length; col++)
		{
			char old = candidates[row][col];
			candidates[row][col] = candidates[row][col] == ash ? rock : ash;
			foreach (var mirror in GetMirrorLines(candidates).Where(x => x.found))
			{
				mirrors.Add(mirror);
			}
			
			candidates[row][col] = old;
		}
	}
	
	return mirrors.ToImmutableHashSet();
}

static IReadOnlySet<(bool found, int col, int value)> GetMirrorLines(IList<char[]> candidates)
{
	int[][] palindromes = new int[candidates.Count][];
	for (int i = 0; i < candidates.Count; i++)
	{
		palindromes[i] = GetEvenPalindromes(candidates[i]);
	}

	HashSet<(bool, int, int)> mirrors = new();
	for (int col = 0; col < palindromes[0].Length; col++)
	{
		int min = palindromes[0][col];
		for (int row = 0; row < palindromes.Length; row++)
		{
			min = Math.Min(min, palindromes[row][col]);
		}

		if (min >= Math.Min(col, palindromes[0].Length - col) && min > 0)
		{
			//string.Join(Environment.NewLine, candidates.Select(x => new string(x))).Dump($"MIRROR {col}");
			mirrors.Add((true, col, min));
		}
	}

	return mirrors.ToImmutableHashSet();
}

static (bool found, int col, int value) GetBestMirrorLine(IList<char[]> candidates)
{
	int[][] palindromes = new int[candidates.Count][];
	for (int i = 0; i < candidates.Count; i++)
	{
		palindromes[i] = GetEvenPalindromes(candidates[i]);
	}

	int bestValue = 0;
	int bestCol = 0;
	bool found = false;
	for (int col = 0; col < palindromes[0].Length; col++)
	{
		int min = palindromes[0][col];
		for (int row = 0; row < palindromes.Length; row++)
		{
			min = Math.Min(min, palindromes[row][col]);
		}

		//(new{min, bestValue, col, left = col + 1, rigth = palindromes[0].Length - col + 1}).ToString().Dump();
		if (min > bestValue && (min >= Math.Min(col, palindromes[0].Length - col)))
		{
			found = true;
			bestValue = min;
			bestCol = col;
		}
	}

	return (found, bestCol, bestValue);
}

static int[] GetEvenPalindromes(char[] s)
{
	int n = s.Length;
	int[] even = new int[n];
	for (int i = 0; i < n; ++i)
	{
		even[i] = 0;
		while (i - even[i] - 1 >= 0 && i + even[i] < n && s[i - even[i] - 1] == s[i + even[i]])
		{
			even[i]++;
		}
	}

	return even;
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
