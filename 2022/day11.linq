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
const int day = 11;
string[] input = GetInputLines(year, day);
string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

class Monkey
{
	private Queue<int> items;
	Func<int, int> opFunc;
	int div;
	int ifTrue;
	int ifFalse;
	
	public Monkey(IEnumerable<int> items, Func<int, int> opFunc, int divisible, int ifTrue, int ifFalse)
	{
		this.items = new(items);
		this.opFunc = opFunc;
		div = divisible;
		this.ifTrue = ifTrue;
		this.ifFalse = ifFalse;
	}
	
	public void Inspect(IReadOnlyList<Monkey> monkeys)
	{
		while (items.Count > 0)
		{
			Inspections++;
			var item = items.Dequeue();
			int newWorryLevel = (int)Math.Floor(opFunc(item) / 3d);
			if (newWorryLevel % div == 0)
			{
				monkeys[ifTrue].TakeItem(newWorryLevel);
			}
			else
			{
				monkeys[ifFalse].TakeItem(newWorryLevel);
			}
		}
	}
	
	public void TakeItem(int item)
	{
		items.Enqueue(item);
	}
	
	public int Inspections { get; private set; } = 0;
}

async Task part1()
{
	var monkeys = Parse(input);
	foreach (var round in Enumerable.Range(0, 20))
	{
		for (int i = 0; i < monkeys.Count; i++)
		{
			monkeys[i].Inspect(monkeys);
		}
	}
	
	monkeys.OrderByDescending(x => x.Inspections).Take(2).Aggregate(1, (acc, el) => acc * el.Inspections).Dump();
}
async Task part2()
{
	foreach (string line in input)
	{

	}
}

IReadOnlyList<Monkey> Parse(string[] lines)
{
	List<Monkey> list = new();
	for (int i = 0; i < lines.Length; i+=7)
	{
		var split = lines[i].Split();
		int index = int.Parse(split[1].Substring(0, split[1].IndexOf(':')));
		int[] items = lines[i+1].Substring(lines[i+1].IndexOf(':')+1).Split(',').Select(int.Parse).ToArray();
		string[] op = lines[i+2].Substring(lines[i+2].IndexOf("new =") + "new =".Length)
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		ParameterExpression pem = Expression.Parameter(typeof(int));
		Expression left = int.TryParse(op[1], out int opConst)
			? Expression.Constant(opConst)
			: pem;
		Expression right = int.TryParse(op[2], out opConst)
			? Expression.Constant(opConst)
			: pem;
		Expression opExp = op[1] switch 
		{
			"+" => Expression.Add(left, right),
			"-" => Expression.Subtract(left, right),
			"*" => Expression.Multiply(left, right),
			_ => throw new Exception()
		};
		Func<int, int> opFunc = Expression.Lambda<Func<int, int>>(opExp, pem).Compile();
		int divisible = int.Parse(lines[i+3].Substring(lines[i+3].LastIndexOf(' ')));
		int ifTrue = int.Parse(lines[i+4].Substring(lines[i+4].LastIndexOf(' ')));
		int ifFalse = int.Parse(lines[i+5].Substring(lines[i+5].LastIndexOf(' ')));
		
		list.Add(new(items, opFunc, divisible, ifTrue, ifFalse));
	}
	
	return list;
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
