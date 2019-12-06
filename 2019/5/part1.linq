<Query Kind="Program" />

void Main()
{
	var line = File.ReadAllText("F:/development/adventofcode/2019/5/input.txt");
	var intCodes = line
		.Trim().Split(',')
		.Select(x => int.Parse(x))
		.ToArray();
		
	const int input = 1;
	int op1;
	int op2;
	
	for (int i = 0; i < intCodes.Length; i++)
	{
		var instruction = Instruction(intCodes[i]);
		switch (instruction.opcode)
		{
			case OpCode.Add:
				op1 = instruction.arg1mode == Mode.Position
					? intCodes[intCodes[i+1]]
					: intCodes[i+1];
				op2 = instruction.arg2mode == Mode.Position
					? intCodes[intCodes[i+2]]
					: intCodes[i+2];
				intCodes[intCodes[i+3]] = op1 + op2;
				i += 3;
				continue;
			case OpCode.Mul:
				op1 = instruction.arg1mode == Mode.Position
					? intCodes[intCodes[i+1]]
					: intCodes[i+1];
				op2 = instruction.arg2mode == Mode.Position
					? intCodes[intCodes[i+2]]
					: intCodes[i+2];
				intCodes[intCodes[i+3]] = op1 * op2;
				i += 3;
				continue;
			case OpCode.In:
				intCodes[intCodes[i+1]] = input;
				i += 1;
				continue;
			case OpCode.Out:
				$"{intCodes[i+1]}: {intCodes[intCodes[i+1]]}".Dump();
				i += 1;
				continue;
			case OpCode.Term:
				i = intCodes.Length;
				continue;
			default:
				throw new Exception();
		}
	}
}

(OpCode opcode, Mode arg1mode, Mode arg2mode) Instruction(int intCode)
{
	return (
		(OpCode)(intCode % 100),
		(Mode)(intCode / 100 % 10),
		(Mode)(intCode / 1000 % 10)
	);
}

enum OpCode
{
	Add = 1,
	Mul = 2,
	In = 3,
	Out = 4,
	Term = 99
}
enum Mode
{
	Position = 0,
	Immediate = 1
}