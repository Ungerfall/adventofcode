<Query Kind="Program" />

void Main()
{
	var relations = File
		.ReadAllLines("D:/dev/adventofcode/2019/6/input.txt")
		.Select(x =>
		{
			var pair = x.Split(')');
			return (pair[0], pair[1]);
		})
		.ToList();
	const string root = "COM";
	int sum = 0;
	int deep = -1;
	Traverse(relations, root, ref sum, deep);
	
	// What is the total number of direct and indirect orbits in your map data?
	sum.Dump();
}

void Traverse(List<(string p, string o)> relations, string planet, ref int sum, int deep)
{
	deep++;
	sum += deep;
	foreach (var orbit in relations.Where(x => x.p == planet).Select(x => x.o))
	{
		Traverse(relations, orbit, ref sum, deep);
	}
}
