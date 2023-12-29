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
const int day = 20;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample2.txt"));
const string BROADCASTER = "broadcaster";
const string FLIP_FLOP = "%";
const string CONJUCTION = "&";
const string OUTPUT = "output";
const byte LOW = 0;
const byte HIGH = 1;
const bool OFF = false;
const bool ON = true;
public record Module(string name, string type, bool onOff);
public record Signal(string name, string sender, byte level);
public record Configuration(string type, List<string> destinations);
void part1()
{
	Dictionary<string, Configuration> cfg = new();
	Dictionary<string, Module> modules = new();
	Dictionary<string, Dictionary<string, byte>> conjuctions = new();
	foreach (string line in input)
	{
		string[] leftAndRight = line.Split("->");
		string source = leftAndRight[0].Trim();
		List<string> destination = leftAndRight[1].Split(',').Select(x => x.Trim()).ToList();
		if (source == BROADCASTER)
		{
			cfg[source] = new(BROADCASTER, destination);
			modules[source] = new(BROADCASTER, string.Empty, false);
		}
		else
		{
			string type = source[0..1];
			source = source[1..];
			cfg[source] = new(type, destination);
			modules[source] = new(source, type, false);
		}
	}

	conjuctions = cfg
		.Where(x => x.Value.type == CONJUCTION)
		.ToDictionary(k => k.Key, v =>
			cfg.Where(x => x.Value.destinations.Contains(v.Key))
				.Select(x => x.Key)
				.ToHashSet()
				.ToDictionary(x => x, v => LOW)
		);
	long lowPulsesCount = 0L;
	long highPulsesCount = 0L;
	const int buttonPressCount = 1000;
	foreach (var _ in Enumerable.Range(0, buttonPressCount))
	{
		//(new string('-', 32) + " " + _).Dump();
		//"button -low-> broadcaster".Dump();
		lowPulsesCount++; // button press
		Queue<Signal> q = new();
		var (t, dest) = cfg[BROADCASTER];
		foreach (var n in dest)
		{
			q.Enqueue(new(n, BROADCASTER, LOW));
			lowPulsesCount++;
		}

		while (q.Count > 0)
		{
			var (receiver, sender, currInputSignal) = q.Dequeue();
			//$"{sender} -{(currInputSignal == LOW ? "low" : "high")}-> {receiver}".Dump();
			if (!modules.TryGetValue(receiver, out var module))
			{
				continue;
			}
			
			var (_, tt, onOff) = module;
			byte? signal = null;
			if (currInputSignal == HIGH && tt == FLIP_FLOP)
			{
				continue;
			}
			else if (currInputSignal == LOW && tt == FLIP_FLOP)
			{
				modules[receiver] = new(receiver, tt, !onOff);
				signal = onOff ? LOW : HIGH;
			}
			else if (tt == CONJUCTION)
			{
				conjuctions[receiver][sender] = currInputSignal;
				signal = conjuctions[receiver].All(x => x.Value == HIGH) ? LOW : HIGH;
			}
			else
			{
				throw new ArgumentException("invalind type");
			}
			
			Debug.Assert(signal is not null);
			foreach (var d in cfg[receiver].destinations)
			{
				if (signal == LOW)
				{
					lowPulsesCount++;
				}
				else
				{
					highPulsesCount++;
				}
				
				q.Enqueue(new(d, sender: receiver, signal.Value));
			}
		}
	}
	
	(checked(lowPulsesCount * highPulsesCount)).Dump("TOTAL NUMBER OF PULSES SENT");
}
void part2()
{
	Dictionary<string, Configuration> cfg = new();
	Dictionary<string, Module> modules = new();
	Dictionary<string, Dictionary<string, byte>> conjuctions = new();
	foreach (string line in input)
	{
		string[] leftAndRight = line.Split("->");
		string source = leftAndRight[0].Trim();
		List<string> destination = leftAndRight[1].Split(',').Select(x => x.Trim()).ToList();
		if (source == BROADCASTER)
		{
			cfg[source] = new(BROADCASTER, destination);
			modules[source] = new(BROADCASTER, string.Empty, false);
		}
		else
		{
			string type = source[0..1];
			source = source[1..];
			cfg[source] = new(type, destination);
			modules[source] = new(source, type, false);
		}
	}

	conjuctions = cfg
		.Where(x => x.Value.type == CONJUCTION)
		.ToDictionary(k => k.Key, v =>
			cfg.Where(x => x.Value.destinations.Contains(v.Key))
				.Select(x => x.Key)
				.ToHashSet()
				.ToDictionary(x => x, v => LOW)
		);
	int buttonPressCount = 0;
	Dictionary<string, int> kzConjuction = new();
	while (true)
	{
		//string.Join(string.Empty, modules.Values.Select(x => x.onOff ? 1 : 0)).Dump();
		buttonPressCount++;
		//(new string('-', 32) + " " + buttonPressCount).Dump();
		//"button -low-> broadcaster".Dump();
		Queue<Signal> q = new();
		var (t, dest) = cfg[BROADCASTER];
		foreach (var n in dest)
		{
			q.Enqueue(new(n, BROADCASTER, LOW));
		}

		while (q.Count > 0)
		{
			var (receiver, sender, currInputSignal) = q.Dequeue();
			//$"{sender} -{(currInputSignal == LOW ? "low" : "high")}-> {receiver}".Dump();
			if (!modules.TryGetValue(receiver, out var module))
			{
				if (currInputSignal == LOW)
				{
					goto END;
				}
				
				continue;
			}
			
			var (_, tt, onOff) = module;
			byte? signal = null;
			if (currInputSignal == HIGH && tt == FLIP_FLOP)
			{
				continue;
			}
			else if (currInputSignal == LOW && tt == FLIP_FLOP)
			{
				modules[receiver] = new(receiver, tt, !onOff);
				signal = onOff ? LOW : HIGH;
			}
			else if (tt == CONJUCTION)
			{
				conjuctions[receiver][sender] = currInputSignal;
				signal = conjuctions[receiver].All(x => x.Value == HIGH) ? LOW : HIGH;
			}
			else
			{
				throw new ArgumentException("invalind type");
			}
			
			foreach (var d in cfg[receiver].destinations)
			{
				if (d == "kz" && signal.Value == HIGH)
				{
					kzConjuction[receiver] = buttonPressCount;
					if (kzConjuction.Count == 4)
					{
						goto END;
					}
				}
				
				q.Enqueue(new(d, sender: receiver, signal.Value));
			}
		}
	}
	
END:
	var lcm = LCM(kzConjuction.Values.ToArray());
	lcm.Dump("rx with LOW signal");
}

public long LCM(int[] arr)
{
	long lcm = arr[0];
	for (int i = 1; i < arr.Length; i++)
	{
		lcm = checked((lcm / GCD(lcm, arr[i]) * arr[i]));
	}
	
	return lcm;
}
public long GCD(long a, long b)
{
	while(b != 0)
	{
		long temp = b;
		b = a % b;
		a = temp;
	}
	
	return a;
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
		"Loaded from local file".Dump();
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
