<Query Kind="Program" />

void Main()
{
	using var sr = new StreamReader("F:/development/adventofcode/2019/1/input.txt");
	string line;
	long sum = 0L;
	while ((line = sr.ReadLine()) != null)
	{
		int fuel = (int) Math.Floor(int.Parse(line) / 3f) - 2;
		sum += fuel;
	}
	
	sum.Dump();
}

// Define other methods, classes and namespaces here
