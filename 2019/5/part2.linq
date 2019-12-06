<Query Kind="Program" />

void Main()
{
	var line = File.ReadAllText("F:/development/adventofcode/2019/5/input2.txt");
	var intCodes = line
		.Trim().Split(',')
		.Select(x => int.Parse(x))
		.ToArray();
		
	const int input = 5;
	
	for (int i = 0; i < intCodes.Length;)
	{
		$"{i}: {intCodes[i]}".Dump();
		var instruction = Instruction(intCodes[i]);
		(int p1, int p2) Params()
		{
			return (
				instruction.arg1mode == Mode.Position
					? intCodes[intCodes[i+1]]
					: intCodes[i+1],
				instruction.arg2mode == Mode.Position
					? intCodes[intCodes[i+2]]
					: intCodes[i+2]
			);
		}
		
		switch (instruction.opcode)
		{
			case OpCode.Add:
				var pa = Params();
				intCodes[intCodes[i+3]] = pa.p1 + pa.p2;
				i += 4;
				continue;
			case OpCode.Mul:
				var pm = Params();
				intCodes[intCodes[i+3]] = pm.p1 * pm.p2;
				i += 4;
				continue;
			case OpCode.JmpTrue:
				var pt = Params();
				i = (pt.p1 != 0)
					? pt.p2 
					: i + 3;
				continue;
			case OpCode.JmpFalse:
				var pf = Params();
				i = (pf.p1 == 0)
					? pf.p2 
					: i + 3;
				continue;
			case OpCode.LessThan:
				var pl = Params();
				intCodes[intCodes[i+3]] = pl.p1 < pl.p2
					? 1
					: 0;
				i += 4;
				continue;
			case OpCode.Equals:
				var pe = Params();
				intCodes[intCodes[i+3]] = pe.p1 == pe.p2
					? 1
					: 0;
				i += 4;
				continue;
			case OpCode.In:
				intCodes[intCodes[i+1]] = input;
				i += 2;
				continue;
			case OpCode.Out:
				$"diagnostic code at {intCodes[i+1]} = {intCodes[intCodes[i+1]]}".Dump();
				i += 2;
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
	JmpTrue = 5,
	JmpFalse = 6,
	LessThan = 7,
	Equals = 8,
	Term = 99
}
enum Mode
{
	Position = 0,
	Immediate = 1
}
