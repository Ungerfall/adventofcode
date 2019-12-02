<Query Kind="Program" />

void Main()
{
	const int opcodeAdd = 1;
	const int opcodeMul = 2;
	const int opcodeTerm = 99;
	const int opcodeStepForward = 3;
	var line = File.ReadAllText("D:/dev/adventofcode/2019/2/input.txt");
	var intCodes = line
		.Trim().Split(',')
		.Select(x => int.Parse(x))
		.ToArray();
	/*
		before running the program, replace position 1 with 
		the value 12 and replace position 2 with the value 2
	*/
	intCodes[1] = 12;
	intCodes[2] = 2;
	
	for (int i = 0; i < intCodes.Length; i++)
	{
		if (intCodes[i] == opcodeAdd)
		{
			int op1 = intCodes[intCodes[i+1]];
			int op2 = intCodes[intCodes[i+2]];
			intCodes[intCodes[i+3]] = op1 + op2;
			i += opcodeStepForward;
			continue;
		}
		
		if (intCodes[i] == opcodeMul)
		{
			int op1 = intCodes[intCodes[i+1]];
			int op2 = intCodes[intCodes[i+2]];
			intCodes[intCodes[i+3]] = op1 * op2;
			i += opcodeStepForward;
			continue;
		}
		
		if (intCodes[i] == opcodeTerm)
			break;
	}
	
	// What value is left at position 0 after the program halts
	intCodes[0].Dump();
}

// Define other methods, classes and namespaces here