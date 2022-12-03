#nullable disable

using AdventOfCSharp;
using Garyon.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Problems.Year2022;

public class Day3 : Problem<long>
{
    private string[] lines;

    public override long SolvePart1()
    {
        List<char> compartments = new();
        foreach (var line in lines)
        {
            int half = line.Length / 2;
            HashSet<char> rucksack = new(line[0..half]);
            for (int i = half + 1; i < line.Length; i++)
            {
                if (rucksack.Contains(line[i]))
                {
                    compartments.Add(line[i]);
                    break;
                }
            }
        }

        return compartments
            .Select(x =>
            {
                int v = x >= 'a'
                    ? x - 'a' + 1
                    : x - 'A' + 27;
                return v;
            })
            .Sum();
    }

    public override long SolvePart2()
    {
        return 1;
    }

    protected override void LoadState()
    {
        lines = FileLines;
    }
    protected override void ResetState()
    {
        lines = null;
    }
}
