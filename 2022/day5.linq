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
int year = 2022;
int day = 5;
async Task part1()
{
	string[] lines = (await GetInputLines(year, day)).ToArray();
	//string[] lines = Sample().ToArray();
	int i = 0;
	string line = lines[i];
	int stacks = line.Length / 4 + 1;
	LinkedList<char>[] crates = Enumerable.Range(0, stacks).Select(x => new LinkedList<char>()).ToArray();
	
	while (line.Trim().StartsWith('['))
	{
		for (int j = 0; j < stacks; j++)
		{
			if (line[j*4] != '[')
			{
				continue;
			}
			
			char crate = line[j*4+1];
			crates[j].AddFirst(crate);
		}
		
		i++;
		line = lines[i];
	}
	
	Regex pattern = new(@"^move (\d+) from (\d+) to (\d+)$");
	for (i = i + 2; i <  lines.Length; i++)
	{
		line = lines[i];
		Match m = pattern.Match(line);
		int move = int.Parse(m.Groups[1].Value);
		LinkedList<char> from = crates[int.Parse(m.Groups[2].Value)-1];
		LinkedList<char> to = crates[int.Parse(m.Groups[3].Value)-1];
		char[] cargo = new char[move];
		for (int x = 0; x < move; x++)
		{
			cargo[x] = from.Last.Value;
			from.RemoveLast();
		}
		
		for (int x = 0; x < move; x++)
		{
			to.AddLast(cargo[x]);
		}
	}
	
	string.Join(string.Empty, crates.Select(x => x.Last.Value)).Dump();
}
async Task part2()
{
	string[] lines = (await GetInputLines(year, day)).ToArray();
	//string[] lines = Sample().ToArray();
	int i = 0;
	string line = lines[i];
	int stacks = line.Length / 4 + 1;
	LinkedList<char>[] crates = Enumerable.Range(0, stacks).Select(x => new LinkedList<char>()).ToArray();
	
	while (line.Trim().StartsWith('['))
	{
		for (int j = 0; j < stacks; j++)
		{
			if (line[j*4] != '[')
			{
				continue;
			}
			
			char crate = line[j*4+1];
			crates[j].AddFirst(crate);
		}
		
		i++;
		line = lines[i];
	}
	
	Regex pattern = new(@"^move (\d+) from (\d+) to (\d+)$");
	for (i = i + 2; i <  lines.Length; i++)
	{
		line = lines[i];
		Match m = pattern.Match(line);
		int move = int.Parse(m.Groups[1].Value);
		LinkedList<char> from = crates[int.Parse(m.Groups[2].Value)-1];
		LinkedList<char> to = crates[int.Parse(m.Groups[3].Value)-1];
		char[] cargo = new char[move];
		for (int x = 0; x < move; x++)
		{
			cargo[x] = from.Last.Value;
			from.RemoveLast();
		}
		
		for (int x = move-1; x >= 0; x--)
		{
			to.AddLast(cargo[x]);
		}
	}
	
	string.Join(string.Empty, crates.Select(x => x.Last.Value)).Dump();
}

async Task Main()
{
	await part1();
	await part2();
}

IEnumerable<string> Sample()
{
	return File.ReadLines(@"C:\development\adventofcode\2022\input\5.sample.txt");
}

async Task<IEnumerable<string>> GetInputLines(int year, int day)
{
	Debug.Assert(year <= DateTime.Now.Year);
	Debug.Assert(day > 0 && day <= 25);
	var root = Directory.GetParent(Path.GetDirectoryName(Util.CurrentQueryPath));
	var inputDir = Directory.CreateDirectory(Path.Combine(root.FullName, year.ToString(), "input"));
	var input = Path.Combine(inputDir.FullName, $"{day}.txt");
	if (File.Exists(input))
	{
		return File.ReadLines(input);
	}
	else
	{
		"From internet".Dump();
		using System.Net.Http.HttpClient c = new();
		string ga = Util.GetPassword("adventofcode_ga");
		string session = Util.GetPassword("adventofcode_session");
		c.DefaultRequestHeaders.Add("cookie", "_ga=" + ga + "; session=" + session);
		string uri = $"https://adventofcode.com/{year}/day/{day}/input";
		var content = (await c.GetAsync(uri)).Content;
		using (StreamWriter sw = new(input))
		{
			await content.CopyToAsync(sw.BaseStream);
		}

		return File.ReadLines(input);
	}
}
