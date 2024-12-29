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
	 ┗ 📜template.linq
*/
const int year = 2024;
const int day = 17;
static string[] input = GetInputLines(year, day);
// uncomment to debug sample. Copy and save sample to /year/input/day.sample.txt
//static string[] input = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input", $"{day}.sample.txt"));

/// <summary>
/// OPCODE = 0. performs division. The numerator is the value in the A register.
/// The denominator is found by raising 2 to the power of the instruction's combo operand.
/// (So, an operand of 2 would divide A by 4 (2^2);
/// an operand of 5 would divide A by 2^B.)
/// The result of the division operation is truncated to an integer and then written to the A register.
/// </summary>
const int OPCODE_adv = 0;

/// <summary>
/// OPCODE = 1. calculates the bitwise XOR of register B and the instruction's literal operand, then stores the result in register B.
/// </summary>
const int OPCODE_bxl = 1;

/// <summary>
/// OPCODE = 2. calculates the value of its combo operand modulo 8 (thereby keeping only its lowest 3 bits),
/// then writes that value to the B register.
/// </summary>
const int OPCODE_bst = 2;

/// <summary>
/// OPCODE = 3. does nothing if the A register is 0. However, if the A register is not zero,
/// it jumps by setting the instruction pointer to the value of its literal operand;
/// if this instruction jumps, the instruction pointer is not increased by 2 after this instruction.
/// </summary>
const int OPCODE_jnz = 3;

/// <summary>
/// OPCODE = 4. calculates the bitwise XOR of register B and register C,
/// then stores the result in register B.
/// (For legacy reasons, this instruction reads an operand but ignores it.)
/// </summary>
const int OPCODE_bxc = 4;

/// <summary>
/// OPCODE = 5. calculates the value of its combo operand modulo 8,
/// then outputs that value. (If a program outputs multiple values, they are separated by commas.)
/// </summary>
const int OPCODE_out = 5;

/// <summary>
/// OPCODE = 6. works exactly like the adv instruction except that the result is stored in the B register.
/// (The numerator is still read from the A register.)
/// </summary>
const int OPCODE_bdv = 6;

/// <summary>
/// OPCODE = 7. works exactly like the adv instruction except that the result is stored in the C register.
/// (The numerator is still read from the A register.)
/// </summary>
const int OPCODE_cdv = 7;

void part1()
{
	Regex digitPattern = new(@"\d+");
	long REG_A = long.Parse(digitPattern.Match(input[0]).Value);
	long REG_B = long.Parse(digitPattern.Match(input[1]).Value);
	long REG_C = long.Parse(digitPattern.Match(input[2]).Value);

	int[] program = digitPattern
		.Matches(input[4])
		.Select(x => int.Parse(x.Value))
		.ToArray();
	int instructionPointer = 0;
	List<int> output = new();
	while (instructionPointer < program.Length)
	{
		int opcode = program[instructionPointer];
		int literalOperand = program[instructionPointer + 1];
		long comboOperand = literalOperand switch
		{
			0 => 0,
			1 => 1,
			2 => 2,
			3 => 3,
			4 => REG_A,
			5 => REG_B,
			6 => REG_C,
			_ => -1,
		};

		if (opcode == OPCODE_adv)
		{
			double division = REG_A / Math.Pow(2.0, comboOperand);
			int truncated = (int)Math.Truncate(division);
			REG_A = truncated;
		}
		else if (opcode == OPCODE_bxl)
		{
			REG_B = REG_B ^ literalOperand;
		}
		else if (opcode == OPCODE_bst)
		{
			REG_B = comboOperand % 8;
		}
		else if (opcode == OPCODE_jnz)
		{
			if (REG_A != 0)
			{
				instructionPointer = literalOperand - 2;
			}
		}
		else if (opcode == OPCODE_bxc)
		{
			REG_B = REG_B ^ REG_C;
		}
		else if (opcode == OPCODE_out)
		{
			output.Add((int)(comboOperand % 8));
		}
		else if (opcode == OPCODE_bdv)
		{
			double division = REG_A / Math.Pow(2.0, comboOperand);
			int truncated = (int)Math.Truncate(division);
			REG_B = truncated;
		}
		else if (opcode == OPCODE_cdv)
		{
			double division = REG_A / Math.Pow(2.0, comboOperand);
			int truncated = (int)Math.Truncate(division);
			REG_C = truncated;
		}
		else
		{
			throw new ArgumentException("invalid opcode " + opcode);
		}

		instructionPointer += 2;
	}

	string.Join(",", output).Dump("output");
}

void part2()
{
	foreach (string line in input)
	{

	}
}

void Main()
{
	part1();
	part2();
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
