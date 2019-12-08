<Query Kind="Program" />

void Main()
{
	var relations = File
		.ReadAllLines("D:/dev/adventofcode/2019/6/input2.txt")
		.Select(x =>
		{
			var pair = x.Split(')');
			return (pair[0], pair[1]);
		})
		.ToList();
	const string root = "COM";
	const string santa = "SAN";
	
	int sum = 0;
	int deep = -1;
	Traverse(relations, root, ref sum, deep);
	
	var rootNode = new Node(root);
	FillTree(relations, rootNode, root);
	//MinDisToNode(You, santa);
	// What is the total number of direct and indirect orbits in your map data?
}

const string you = "YOU";
Node You;

int MinDisToNode(Node start, string dest)
{
	return 0;
}

void Traverse(List<(string p, string o)> relations, string planet, ref int sum, int deep)
{
	deep++;
	sum += deep;
	if (planet == "YOU") $"YOU = {new { deep, sum }}".Dump();
	if (planet == "SAN") $"SAN = {new { deep, sum }}".Dump();
	foreach (var orbit in relations.Where(x => x.p == planet).Select(x => x.o))
	{
		Traverse(relations, orbit, ref sum, deep);
	}
}

void FillTree(List<(string p, string o)> relations, Node node, string planet)
{
	foreach (var orbit in relations.Where(x => x.p == planet).Select(x => x.o))
	{
		var kidNode = node.Add(orbit);
		if (orbit == you)
			You = kidNode;
		FillTree(relations, kidNode, orbit);
	}
}

class Node
{
	public string Data {get;set;}
	public Node Parent {get;set;}
	public ICollection<Node> Children {get;set;}
	
	public Node(string data)
	{
		Data = data;
		Children = new LinkedList<Node>();
	}
	
	public Node Add(string child)
	{
		var node = new Node(child) { Parent = this };
		Children.Add(node);
		return node;
	}
}

