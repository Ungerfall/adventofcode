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
const int year = 2024;
const int day = 4;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	(int rowd, int cold)[] dirs = new[]
	{
		(-1, -1),
		(-1, 0),
		(-1, 1),
		(0, -1),
		(0, 1),
		(1, -1),
		(1, 0),
		(1, 1)
	};
	int rows = input.Length;
	int cols = input[0].Length;
	int count = 0;
	for (int row = 0; row < rows; row++)
	{
		for (int col = 0; col < cols; col++)
		{
			if (input[row][col] != 'X')
			{
				continue;
			}

			foreach (var dir in dirs)
			{
				int row_end = row + 3 * dir.rowd;
				int col_end = col + 3 * dir.cold;
				char[] word = new char[4];
				word[0] = 'X';
				if (row_end >= 0 && row_end < rows && col_end >= 0 && col_end < cols)
				{
					// iterate from center towards directions
					for (int i = 1; i <= 3; i++)
					{
						word[i] = input[row + i * dir.rowd][col + i * dir.cold];
					}

					if ((new string(word)).Equals("XMAS"))
					{
						count++;
					}
				}
			}
		}
	}

	count.Dump("Number of XMAS");
}
void part2()
{
	(int rowd, int cold)[] diag = new[]
	{
		(-1, -1),
		(0, 0),
		(1, 1)
	};
	(int rowd, int cold)[] backdiag = new[]
	{
		(1, -1),
		(0, 0),
		(-1, 1)
	};
	int rows = input.Length;
	int cols = input[0].Length;
	int count = 0;
	for (int row = 0; row < rows; row++)
	{
		for (int col = 0; col < cols; col++)
		{
			if (input[row][col] != 'A')
			{
				continue;
			}

			char[] candidateDiag = new char[3];
			char[] candidateBackdiag = new char[3];
			for (int i = 0; i < 3; i++)
			{
				int diag_row_index = row + diag[i].rowd;
				int diag_col_index = col + diag[i].cold;
				if (diag_row_index >= 0 && diag_row_index < rows && diag_col_index >= 0 && diag_col_index < cols)
				{
					candidateDiag[i] = input[diag_row_index][diag_col_index];
				}

				int backdiag_row_index = row + backdiag[i].rowd;
				int backdiag_col_index = col + backdiag[i].cold;
				if (backdiag_row_index >= 0 && backdiag_row_index < rows && backdiag_col_index >= 0 && backdiag_col_index < cols)
				{
					candidateBackdiag[i] = input[backdiag_row_index][backdiag_col_index];
				}
			}

			string[] valid = new[] { "MAS", "SAM" };
			if (valid.Contains(new string(candidateDiag)) && valid.Contains(new string(candidateBackdiag)))
			{
				count++;
			}
		}
	}

	count.Dump("Number of X-MAS");
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
