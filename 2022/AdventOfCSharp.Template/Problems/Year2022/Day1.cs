// This problem is too trivial a case to demand inclusion of nullability
// Consider removing this line on more complex solutions
// Additionally, you may add a #nullable enable line on the important sections of your code
#nullable disable

using AdventOfCSharp;
using System;
using System.Linq;

// Make sure the namespace of the problem class ends in its respective year
// For a problem for the year 2015, you would need to end the namespace in
// ".Year2015"
namespace AdventOfCode.Problems.Year2022;

// Also ensure the solution class is named DayXX, denoting the day in the year
// for which this problem is
public class Day1 : Problem<long>
{
    // This is the state of your parsed input
    private string[] lines;

    // NOTE: This example solution does not actually reflect a solution for
    //       the problem for Year 2021, Day 1

    public override long SolvePart1()
    {
        long max = 0;
        long elf = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (int.TryParse(lines[i], out int calories))
            {
                elf += calories;
            }
            else
            {
                max = Math.Max(max, elf);
                elf = 0;
            }
        }

        return Math.Max(max, elf);
    }
    public override long SolvePart2()
    {
        long[] maxSorted = new long[3];
        long elf = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (int.TryParse(lines[i], out int calories))
            {
                elf += calories;
            }
            else
            {
                if (elf <= maxSorted[0])
                {
                    elf = 0;
                    continue;
                }

                if (elf > maxSorted[2])
                {
                    maxSorted[0] = maxSorted[1];
                    maxSorted[1] = maxSorted[2];
                    maxSorted[2] = elf;
                }
                else if (elf > maxSorted[1])
                {
                    maxSorted[0] = maxSorted[1];
                    maxSorted[1] = elf;
                }
                else
                {
                    maxSorted[0] = elf;
                }

                elf = 0;
            }
        }

        if (elf > maxSorted[0])
        {
            maxSorted[0] = elf;
        }

        return maxSorted.Sum();
    }

    protected override void LoadState()
    {
        // Here you parse your input, called before solving any part

        // FileNumbersInt32 is a property that returns an array of the
        // parsed numbers of the input file's lines for this problem
        // There are other properties to use, like FileContents and FileLines
        lines = FileLines;
    }
    protected override void ResetState()
    {
        // Here you reset the state of your parsed input
        // Usually this is called to unload state
        // It is almost always unnecessary to unload the state
        // Resetting to null is completely optional, and often times redundant

        lines = null;
    }
}
