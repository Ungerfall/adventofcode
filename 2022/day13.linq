<Query Kind="Program">
  <NuGetReference>SuperLinq</NuGetReference>
  <Namespace>SuperLinq</Namespace>
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
const int day = 13;
string[] input = GetInputLines(year, day);
string[] sample = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

ArrayList Parse(ReadOnlySpan<char> input, ref int i)
{
	ArrayList n = new();
	for (; i < input.Length; i++)
	{
		if (input[i] == ']')
		{
			return n;
		}

		if (input[i] == '[')
		{
			i++;
			n.Add(Parse(input, ref i));
			continue;
		}

		if (input[i] == ',')
		{
			continue;
		}

		StringBuilder sb = new();
		sb.Append(input[i]);
		while (i + 1 < input.Length && input[i + 1] >= '0' && input[i + 1] <= '9')
		{
			sb.Append(input[i + 1]);
			i++;
		}

		n.Add(int.Parse(sb.ToString()));
	}

	return n;
}

static int Compare(object? l, object? r)
{
	int? leftInt = l as int?;
	int? rightInt = r as int?;
	ArrayList? leftArr = l as ArrayList;
	ArrayList? rightArr = r as ArrayList;
	if (leftInt != null && rightInt != null)
	{
		if (leftInt < rightInt)
			return -1;
		else if (leftInt == rightInt)
			return 0;
		return 1;
	}

	if (leftArr != null && rightArr != null)
	{
		int i = 0;
		int j = 0;
		while (i < leftArr.Count && j < rightArr.Count)
		{
			if (Compare(leftArr[i], rightArr[j]) == -1)
			{
				return -1;
			}
			else if (Compare(leftArr[i], rightArr[j]) == 1)
			{
				return 1;
			}

			i++;
			j++;
		}

		if (leftArr.Count == i && rightArr.Count > j)
		{
			return -1;
		}

		if (leftArr.Count > i && rightArr.Count == j)
		{
			return 1;
		}

		return 0;
	}
	else if (leftInt != null && rightArr != null)
	{
		leftArr = new(new[] { leftInt });
		return Compare(leftArr, rightArr);
	}
	else if (leftArr != null && rightInt != null)
	{
		rightArr = new(new[] { rightInt });
		return Compare(leftArr, rightArr);
	}

	throw new ArgumentException("Invalid combination");
}

async Task part1()
{
	List<(ArrayList left, ArrayList right)> pairs = new();
	foreach (var pair in input.Split(string.Empty))
	{
		//pair.Dump();
		int i = 1;
		int j = 1;
		Debug.Assert(pair.ElementAt(0)[0] == '[');
		Debug.Assert(pair.ElementAt(1)[0] == '[');
		ArrayList left = Parse(pair.ElementAt(0).AsSpan(), ref i);
		ArrayList right = Parse(pair.ElementAt(1).AsSpan(), ref j);
		//pair.Dump();
		//left.Dump();
		//right.Dump();
		pairs.Add((left, right));
	}

	List<int> rightIndicies = new();
	//roots.Dump();
	for (int index = 0; index < pairs.Count; index++)
	{
		var (left, right) = pairs[index];
		if (Compare(left, right) == -1)
		{
			rightIndicies.Add(index + 1);
		}
	}

	//rightIndicies.Dump();
	rightIndicies.Sum().Dump("Part 1");
}
async Task part2()
{
	List<object> packets = new();
		int i = 1;
		int j = 1;
	foreach (var pair in input.Split(string.Empty))
	{
		i = 1;
		j = 1;
		//pair.Dump();
		Debug.Assert(pair.ElementAt(0)[0] == '[');
		Debug.Assert(pair.ElementAt(1)[0] == '[');
		packets.Add(Parse(pair.ElementAt(0).AsSpan(), ref i));
		packets.Add(Parse(pair.ElementAt(1).AsSpan(), ref j));
	}
	
	i = 1;
	j = 1;
	packets.Add(Parse("[[2]]".AsSpan(), ref i)); 
	int ix2 = packets.Count - 1;
	packets.Add(Parse("[[6]]".AsSpan(), ref j)); 
	int ix6 = packets.Count - 1;
	
	List<(object el, int index)> packetsOriginalIx = packets.Select<object, (object el, int index)>((el, i) => (el, i)).ToList();
	packetsOriginalIx.Select(x => System.Text.Json.JsonSerializer.Serialize(x.el)).Dump();
	packetsOriginalIx.Sort((one, another) => Compare(one.el, another.el));
	packetsOriginalIx.Select(x => System.Text.Json.JsonSerializer.Serialize(x.el)).Dump();
	int res = (packetsOriginalIx.FindIndex(x => x.index == ix2) + 1)
		* (packetsOriginalIx.FindIndex(x => x.index == ix6) + 1);
	
	res.Dump();
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
