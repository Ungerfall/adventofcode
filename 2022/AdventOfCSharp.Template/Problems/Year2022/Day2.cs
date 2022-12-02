#nullable disable

using AdventOfCSharp;
using System;

namespace AdventOfCode.Problems.Year2022;

public class Day2 : Problem<long>
{
    private string[] lines;

    public override long SolvePart1()
    {
        long sum = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            Hand his = lines[i][0] switch
            {
                'A' => Hand.Rock,
                'B' => Hand.Paper,
                'C' => Hand.Scissors,
                _ => throw new NotImplementedException()
            };
            Hand mine = lines[i][2] switch
            {
                'X' => Hand.Rock,
                'Y' => Hand.Paper,
                'Z' => Hand.Scissors,
                _ => throw new NotImplementedException()
            };
            sum += mine switch
            {
                Hand.Rock => 1,
                Hand.Paper => 2,
                Hand.Scissors => 3,
                _ => throw new NotImplementedException()
            };
            sum += (his, mine) switch
            {
                (Hand.Rock, Hand.Scissors) or (Hand.Paper, Hand.Rock) or (Hand.Scissors, Hand.Paper) => 0,
                (Hand.Rock, Hand.Paper) or (Hand.Paper, Hand.Scissors) or (Hand.Scissors, Hand.Rock) => 6,
                _ => 3
            };
        }

        return sum;
    }

    public override long SolvePart2()
    {
        long sum = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            Hand his = lines[i][0] switch
            {
                'A' => Hand.Rock,
                'B' => Hand.Paper,
                'C' => Hand.Scissors,
                _ => throw new NotImplementedException()
            };
            Hand mine = lines[i][2] switch
            {
                'X' => his switch
                {
                    Hand.Rock => Hand.Scissors,
                    Hand.Paper => Hand.Rock,
                    Hand.Scissors => Hand.Paper,
                    _ => throw new NotImplementedException()
                },
                'Y' => his,
                'Z' => his switch
                {
                    Hand.Rock => Hand.Paper,
                    Hand.Paper => Hand.Scissors,
                    Hand.Scissors => Hand.Rock,
                    _ => throw new NotImplementedException()
                },
                _ => throw new NotImplementedException()
            };
            sum += mine switch
            {
                Hand.Rock => 1,
                Hand.Paper => 2,
                Hand.Scissors => 3,
                _ => throw new NotImplementedException()
            };
            sum += (his, mine) switch
            {
                (Hand.Rock, Hand.Scissors) or (Hand.Paper, Hand.Rock) or (Hand.Scissors, Hand.Paper) => 0,
                (Hand.Rock, Hand.Paper) or (Hand.Paper, Hand.Scissors) or (Hand.Scissors, Hand.Rock) => 6,
                _ => 3
            };
        }

        return sum;
    }

    protected override void LoadState()
    {
        lines = FileLines;
    }
    protected override void ResetState()
    {
        lines = null;
    }

    internal enum Hand
    {
        Rock = 0,
        Paper,
        Scissors
    };
}
