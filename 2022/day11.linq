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
	private Queue<long> items;
	Func<long, long> opFunc;
	int ifTrue;
	int ifFalse;

	public Monkey(IEnumerable<long> items, Func<long, long> opFunc, int divisible, int ifTrue, int ifFalse)
	{
		this.items = new(items);
		this.opFunc = opFunc;
		Div = divisible;
		this.ifTrue = ifTrue;
		this.ifFalse = ifFalse;
	}

	public void Inspect(IReadOnlyList<Monkey> monkeys)
	{
		while (items.Count > 0)
		{
			Inspections++;
			var item = items.Dequeue();
			checked
			{
				long newWorryLevel = (OpFuncDecorator != null)
					? OpFuncDecorator(opFunc(item))
					: opFunc(item);
				if (newWorryLevel % Div == 0)
				{
					monkeys[ifTrue].TakeItem(newWorryLevel);
				}
				else
				{
					monkeys[ifFalse].TakeItem(newWorryLevel);
				}
			}
		}
	}

	public void TakeItem(long item)
	{
		items.Enqueue(item);
	}

	public int Inspections { get; private set; } = 0;
	public Func<long, long>? OpFuncDecorator { get; set; }
	public int Div { get; }

	public override string ToString()
	{
		return (new { items = string.Join(',', items), div = Div }).ToString();
	}
}

void part1()
{
	var monkeys = Parse(input);
	foreach (var m in monkeys)
	{
		m.OpFuncDecorator = x => (long)Math.Floor(x / 3d);
	}
	foreach (var round in Enumerable.Range(0, 20))
	{
		for (int i = 0; i < monkeys.Count; i++)
		{
			monkeys[i].Inspect(monkeys);
		}
	}

	monkeys.OrderByDescending(x => x.Inspections).Take(2).Aggregate(1, (acc, el) => acc * el.Inspections).Dump();
}

void part2()
{
	var monkeys = Parse(input);
	int[] divs = monkeys.Select(x => x.Div).ToArray();
	long lcm = lcm_of_array_elements(divs);
	var factor = monkeys.Aggregate(1L, (f, m) => f * m.Div);
	lcm.Dump();
	factor.Dump();
	foreach (var m in monkeys)
	{
		m.OpFuncDecorator = x => x % lcm;
		m.ToString().Dump();
	}
	foreach (var round in Enumerable.Range(0, 10 * 1000))
	{
		for (int i = 0; i < monkeys.Count; i++)
		{
			monkeys[i].Inspect(monkeys);
		}
		
		if ((new[] {0, 19, 999, 1999, 2999, 3999, 4999, 5999, 6999, 7999, 8999, 9999 }).Contains(round))
		{
			//monkeys.Select(x => x.Inspections).Dump();
		}
	}

	checked
	{
		monkeys.OrderByDescending(x => x.Inspections)
			.Take(2)
			.Aggregate<Monkey, long>(1L, (acc, el) => acc * el.Inspections).Dump();
	}
}

// https://www.geeksforgeeks.org/lcm-of-given-array-elements/
static long lcm_of_array_elements(int[] element_array)
{
	long lcm_of_array_elements = 1;
	int divisor = 2;

	checked
	{
		while (true)
		{

			int counter = 0;
			bool divisible = false;
			for (int i = 0; i < element_array.Length; i++)
			{

				// lcm_of_array_elements (n1, n2, ... 0) = 0.
				// For negative number we convert into
				// positive and calculate lcm_of_array_elements.
				if (element_array[i] == 0)
				{
					return 0;
				}
				else if (element_array[i] < 0)
				{
					element_array[i] = element_array[i] * (-1);
				}
				if (element_array[i] == 1)
				{
					counter++;
				}

				// Divide element_array by devisor if complete
				// division i.e. without remainder then replace
				// number with quotient; used for find next factor
				if (element_array[i] % divisor == 0)
				{
					divisible = true;
					element_array[i] = element_array[i] / divisor;
				}
			}

			// If divisor able to completely divide any number
			// from array multiply with lcm_of_array_elements
			// and store into lcm_of_array_elements and continue
			// to same divisor for next factor finding.
			// else increment divisor
			if (divisible)
			{
				lcm_of_array_elements = lcm_of_array_elements * divisor;
			}
			else
			{
				divisor++;
			}

			// Check if all element_array is 1 indicate
			// we found all factors and terminate while loop.
			if (counter == element_array.Length)
			{
				return lcm_of_array_elements;
			}
		}
	}
}

IReadOnlyList<Monkey> Parse(string[] lines)
{
	List<Monkey> list = new();
	for (int i = 0; i < lines.Length; i += 7)
	{
		var split = lines[i].Split();
		int index = int.Parse(split[1].Substring(0, split[1].IndexOf(':')));
		long[] items = lines[i + 1].Substring(lines[i + 1].IndexOf(':') + 1).Split(',').Select(long.Parse).ToArray();
		string[] op = lines[i + 2].Substring(lines[i + 2].IndexOf("new =") + "new =".Length)
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		ParameterExpression pem = Expression.Parameter(typeof(long));
		Expression left = long.TryParse(op[1], out long opConst)
			? Expression.Constant(opConst)
			: pem;
		Expression right = long.TryParse(op[2], out opConst)
			? Expression.Constant(opConst)
			: pem;
		Expression opExp = op[1] switch
		{
			"+" => Expression.Add(left, right),
			"-" => Expression.Subtract(left, right),
			"*" => Expression.Multiply(left, right),
			_ => throw new Exception()
		};
		Func<long, long> opFunc = Expression.Lambda<Func<long, long>>(opExp, pem).Compile();
		int divisible = int.Parse(lines[i + 3].Substring(lines[i + 3].LastIndexOf(' ')));
		int ifTrue = int.Parse(lines[i + 4].Substring(lines[i + 4].LastIndexOf(' ')));
		int ifFalse = int.Parse(lines[i + 5].Substring(lines[i + 5].LastIndexOf(' ')));

		list.Add(new(items, opFunc, divisible, ifTrue, ifFalse));
	}

	return list;
}

async Task Main()
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
