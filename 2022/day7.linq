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
const int day = 7;
string[] input = GetInputLines(year, day);
/*
string[] input = @"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k".Split(Environment.NewLine);
*/

class Folder
{
	private Folder? parent = null;
	
	public Folder(string name, Folder? parent = null)
	{
		Name = name;
		this.parent = parent;
	}
	
	public void AddSub(string name)
	{
		Subs.Add(new Folder(name, this));
	}
	
	public void AddFile(int size)
	{
		Size += size;
		parent?.AddFile(size);
	}
	
	public Folder MoveOut()
	{
		if (parent == null)
		{
			throw new ArgumentException("Cannot move out from root");
		}
		
		return parent;
	}
	
	public Folder MoveIn(string name)
	{
		return Subs.First(x => x.Name == name);
	}
	
	public void Traverse(Action<Folder> action)
	{
		Queue<Folder> q = new(new[] { this });
		while (q.Count > 0)
		{
			Folder f = q.Dequeue();
			action(f);
			foreach (var s in f.Subs)
			{
				q.Enqueue(s);
			}
		}
	}

	public int Size { get; private set; }
	public string Name { get; init; }
	public ICollection<Folder> Subs { get; init; } = new List<Folder>();
}

async Task part1()
{
	Debug.Assert(input[0].Trim() == "$ cd /");
	var root = new Folder("root");
	Folder wd = root;
	for (int i = 1; i < input.Length; i++)
	{
		var parts = input[i].Split(' ');
		if (parts[0] == "$")
		{
			string command = parts[1];
			if (command == "cd")
			{
				wd = (parts[2] == "..") ? wd.MoveOut() : wd.MoveIn(parts[2]);
			}
			else if (command == "ls")
			{
				continue;
			}
			else
			{
				throw new ArgumentException($"unknown command {command}");
			}
		}
		else
		{
			if (parts[0] == "dir")
			{
				wd.AddSub(parts[1]);
			}
			else if (int.TryParse(parts[0], out int size))
			{
				wd.AddFile(size);
			}
			else
			{
				throw new ArgumentException($"unknown command {parts[0]}");
			}
		}
	}
	
	long sum = 0;
	root.Traverse(x =>
	{
		if (x.Size <= 100*1000)
		{
			sum += x.Size;
		}
	});
	sum.Dump();
}
async Task part2()
{
	const int spaceAvailable = 70000000;
	const int unusedNeeded = 30000000;
	Debug.Assert(input[0].Trim() == "$ cd /");
	var root = new Folder("root");
	Folder wd = root;
	for (int i = 1; i < input.Length; i++)
	{
		var parts = input[i].Split(' ');
		if (parts[0] == "$")
		{
			string command = parts[1];
			if (command == "cd")
			{
				wd = (parts[2] == "..") ? wd.MoveOut() : wd.MoveIn(parts[2]);
			}
			else if (command == "ls")
			{
				continue;
			}
			else
			{
				throw new ArgumentException($"unknown command {command}");
			}
		}
		else
		{
			if (parts[0] == "dir")
			{
				wd.AddSub(parts[1]);
			}
			else if (int.TryParse(parts[0], out int size))
			{
				wd.AddFile(size);
			}
			else
			{
				throw new ArgumentException($"unknown command {parts[0]}");
			}
		}
	}
	
	int unused = spaceAvailable - root.Size;
	int toDelete = unusedNeeded - unused;
	int min = int.MaxValue;
	root.Traverse(x =>
	{
		if (x.Size < toDelete)
			return;
		
		min = Math.Min(min, x.Size);
	});
	
	min.Dump();
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
