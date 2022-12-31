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
const int day = 21;
string[] input = GetInputLines(year, day);
//string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
async Task part1()
{
	var expressions = new Dictionary<string, Expression>(input.Length);

	foreach (var line in input)
	{
		string[] sp = line.Split();
		if (long.TryParse(sp[1], out long constant))
		{
			var expression = Expression.Constant(constant, typeof(long));
			expressions.Add(sp[0][..^1], expression);
		}
	}
	
	while (expressions.Count != input.Length)
	{
		var nonConst = input.Where(x =>
		{
			string name = x.Split()[0][..^1];
			return !expressions.ContainsKey(name);
		});
		foreach (var line in nonConst)
		{
			string[] sp = line.Split();
			var left = sp[1];
			var right = sp[3];

			var leftExpression = expressions.ContainsKey(left) ? expressions[left] : null;
			var rightExpression = expressions.ContainsKey(right) ? expressions[right] : null;

			if (leftExpression == null || rightExpression == null)
			{
				continue;
			}

			var name = sp[0][..^1];
			var @operator = sp[2];
			Expression expression = @operator switch
			{
				"+" => Expression.AddChecked(leftExpression, rightExpression),
				"-" => Expression.SubtractChecked(leftExpression, rightExpression),
				"*" => Expression.MultiplyChecked(leftExpression, rightExpression),
				"/" => Expression.Divide(leftExpression, rightExpression),
				_ => throw new Exception("Invalid operator")
			};

			expressions.Add(name, expression);
		}
	}

	var root = expressions["root"];
	var lambda = Expression.Lambda<Func<long>>(root).Compile();
	
	lambda().Dump();
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
