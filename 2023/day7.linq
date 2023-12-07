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
const int day = 7;
string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));
public record Hand(string hand, int bid, Strength str);
void part1()
{
	var hands = new Hand[input.Length];
	int index = 0;
	var cardStrength = new Dictionary<char, int>
	{
		{'A', 14},
		{'K', 13},
		{'Q', 12},
		{'J', 11},
		{'T', 10},
		{'9', 9},
		{'8', 8},
		{'7', 7},
		{'6', 6},
		{'5', 5},
		{'4', 4},
		{'3', 3},
		{'2', 2},
	};
	var handComparer = new HandComperer(cardStrength);
	foreach (string line in input)
	{
		var handAndBid = line.Split();
		string hand = handAndBid[0].Trim();
		int bid = int.Parse(handAndBid[1].Trim());
		var counter = new Counter<char>();
		foreach (var ch in hand)
		{
			counter.Increment(ch);
		}

		Strength str = counter switch
		{
			{ Count: 1 } => Strength.FiveOfAKind,
			{ Count: 2, Values: [1, 4] } => Strength.FourOfAKind,
			{ Count: 2, Values: [2, 3] } => Strength.FullHouse,
			{ Count: 3, Values: [1, 1, 3] } => Strength.ThreeOfAKind,
			{ Count: 3, Values: [1, 2, 2] } => Strength.TwoPair,
			{ Count: 4 } => Strength.OnePair,
			_ => Strength.HighCard,
		};
		hands[index] = new(hand, bid, str);
		index++;
	}

	Array.Sort(hands, handComparer);
	long sum = 0L;
	for (int i = 0; i < hands.Length; i++)
	{
		sum += hands[i].bid * (i + 1);
	}

	sum.Dump("TOTAL WINNING");
}
void part2()
{
	var hands = new Hand[input.Length];
	int index = 0;
	var cardStrength = new Dictionary<char, int>
	{
		{'A', 14},
		{'K', 13},
		{'Q', 12},
		{'T', 10},
		{'9', 9},
		{'8', 8},
		{'7', 7},
		{'6', 6},
		{'5', 5},
		{'4', 4},
		{'3', 3},
		{'2', 2},
		{'J', 1},
	};
	var handComparer = new HandComperer(cardStrength);
	foreach (string line in input)
	{
		var handAndBid = line.Split();
		string hand = handAndBid[0].Trim();
		int bid = int.Parse(handAndBid[1].Trim());
		var counter = new Counter<char>();
		Strength str = Strength.HighCard;
		if (hand.Contains('J'))
		{
			foreach (var card in cardStrength.Keys)
			{
				counter = new Counter<char>();
				var betterHandCandidate = hand.Replace('J', card);
				//betterHandCandidate.Dump();
				foreach (var ch in betterHandCandidate)
				{
					counter.Increment(ch);
				}
				
				str = (Strength)Math.Max((int)str, (int)(counter switch
				{
					{ Count: 1 } => Strength.FiveOfAKind,
					{ Count: 2, Values: [1, 4] } => Strength.FourOfAKind,
					{ Count: 2, Values: [2, 3] } => Strength.FullHouse,
					{ Count: 3, Values: [1, 1, 3] } => Strength.ThreeOfAKind,
					{ Count: 3, Values: [1, 2, 2] } => Strength.TwoPair,
					{ Count: 4 } => Strength.OnePair,
					_ => Strength.HighCard,
				}));
			}
		}
		else
		{
			foreach (var ch in hand)
			{
				counter.Increment(ch);
			}

			str = counter switch
			{
				{ Count: 1 } => Strength.FiveOfAKind,
				{ Count: 2, Values: [1, 4] } => Strength.FourOfAKind,
				{ Count: 2, Values: [2, 3] } => Strength.FullHouse,
				{ Count: 3, Values: [1, 1, 3] } => Strength.ThreeOfAKind,
				{ Count: 3, Values: [1, 2, 2] } => Strength.TwoPair,
				{ Count: 4 } => Strength.OnePair,
				_ => Strength.HighCard,
			};
		}

		hands[index] = new(hand, bid, str);
		index++;
	}

	Array.Sort(hands, handComparer);
	//hands.Dump();
	long sum = 0L;
	for (int i = 0; i < hands.Length; i++)
	{
		sum += hands[i].bid * (i + 1);
	}

	sum.Dump("TOTAL WINNING");
}

void Main()
{
	part1();
	part2();
}

public class Counter<TKey>
{
	private readonly Dictionary<TKey, int> _counter = new();
	public int Count { get => _counter.Count; }
	public int[] Values { get => _counter.Values.AsEnumerable().OrderBy(x => x).ToArray(); }
	public void Increment(TKey key)
	{
		if (_counter.TryGetValue(key, out int count))
		{
			_counter[key] = count + 1;
		}
		else
		{
			_counter[key] = 1;
		}
	}
	public void Dump()
	{
		_counter.Dump();
	}
}
public class HandComperer : IComparer<Hand>
{
	private readonly Dictionary<char, int> _cardStr;
	public HandComperer(Dictionary<char, int> cardStr)
	{
		_cardStr = cardStr;
	}
	int IComparer<Hand>.Compare(Hand? x, Hand? y)
	{
		if (x.str > y.str)
		{
			return 1;
		}

		if (x.str < y.str)
		{
			return -1;
		}

		int equal = 0;
		for (int i = 0; i < 5; i++)
		{
			if (x.hand[i] == y.hand[i])
				continue;

			return _cardStr[x.hand[i]] > _cardStr[y.hand[i]] ? 1 : -1;
		}

		return equal;
	}
}

public enum Strength
{
	HighCard = 1,
	OnePair = 2,
	TwoPair = 3,
	ThreeOfAKind = 4,
	FullHouse = 5,
	FourOfAKind = 6,
	FiveOfAKind = 7,
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
