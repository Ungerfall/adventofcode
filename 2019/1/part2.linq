<Query Kind="Program" />

void Main()
{
	using var sr = new StreamReader("F:/development/adventofcode/2019/1/input2.txt");
	string line;
	long sum = 0L;
	int fuel = 0;
	while ((line = sr.ReadLine()) != null)
	{
		int mass = int.Parse(line);
		fuel = CalcFuel(mass);
		sum += fuel;
		// calc fuel for fuel
		while (true)
		{
			fuel = CalcFuel(fuel);
			if (fuel <= 0)
				break;
			
			sum += fuel;
		}
	}
	
	sum.Dump();
}

int CalcFuel(long mass)
{
	return (int) Math.Floor(mass / 3f) - 2;
}

// Define other methods, classes and namespaces here
