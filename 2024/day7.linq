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
const int day = 7;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
void part1()
{
	long sum = 0L;
	foreach (string line in input)
	{
		string[] parts = line.Split();
		long result = long.Parse(parts[0][..^1]);
		long[] numbers = parts.Skip(1).Select(long.Parse).ToArray();
		IEnumerable<long> found = numbers
			.Skip(1)
			.Aggregate(
				seed: numbers
					.Take(1)
					.Select(x => Expression.Add(Expression.Constant(0L), Expression.Constant(x))),
				(acc, next) => acc.SelectMany<BinaryExpression, BinaryExpression>(x =>
				[
					Expression.Add(x, Expression.Constant(next)),
					Expression.Multiply(x, Expression.Constant(next)),
				]))
			.Select(x => Expression.Lambda<Func<long>>(x).Compile()())
			.Where(x => x == result);

		if (found.Any())
		{
			sum += found.ElementAt(0);
		}
	}

	sum.Dump("Total calibration result");
}
void part2()
{
	long sum = 0L;
	foreach (string line in input)
	{
		string[] parts = line.Split();
		long result = long.Parse(parts[0][..^1]);
		long[] numbers = parts.Skip(1).Select(long.Parse).ToArray();
		IEnumerable<BinaryExpression> combinations = numbers
			.Skip(1)
			.Aggregate(
				seed: numbers
					.Take(1)
					.Select(x => Expression.Add(Expression.Constant(0L), Expression.Constant(x))),
				(acc, next) => acc.SelectMany<BinaryExpression, BinaryExpression>(x =>
					[
						Expression.Add(x, Expression.Constant(next)),
						Expression.MultiplyChecked(x, Expression.Constant(next)),
						ConcatIntegers(x, next),
					]));

		long? correct = combinations
			.Select(x =>
			{
				var lambda = Expression.Lambda<Func<long>>(x);
				return lambda.Compile()();
			})
			.Where(x => x == result)
			.ElementAtOrDefault(0);
		if (correct is not null)
		{
			sum += correct.Value;
		}
	}

	sum.Dump("Total calibration result");
}

BinaryExpression ConcatIntegers(BinaryExpression left, long right)
{
	int numOfDig = (int)Math.Floor(Math.Log10(right)) + 1;
	long evaluated = Expression.Lambda<Func<long>>(left).Compile()();
	return BinaryExpression.Add(
		Expression.Constant(0L),
		Expression.Constant(evaluated * (int)Math.Pow(10, numOfDig) + right));
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
