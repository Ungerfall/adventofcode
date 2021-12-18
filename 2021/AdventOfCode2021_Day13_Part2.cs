using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2021;
public class AdventOfCode2021_Day13_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day13_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day13_Part2>();
    }

    [Function("AdventOfCode2021_Day13_Part2")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/13.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        string[] text = blob
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        var folds = new List<(string axis, int v)>();
        var dots = new List<(int x, int y)>();
        foreach (var line in text)
        {
            if (line.TrimStart().StartsWith("fold"))
            {
                var g = System.Text.RegularExpressions.Regex.Match(line, @"(y|x)=(\d+)").Groups;
                folds.Add((g[1].Value.Trim(), int.Parse(g[2].Value)));
            }
            else
            {
                var numbers = line.Split(",").Select(int.Parse).ToArray();
                dots.Add((numbers[1], numbers[0]));
            }
        }

        foreach (var (axis, pos) in folds)
        {
            var dotsSet = new HashSet<(int x, int y)>();
            bool xFold = axis == "y";
            foreach (var (x, y) in dots)
            {
                if ((xFold && pos == x) || (!xFold && pos == y))
                    continue;

                var xx = xFold && x > pos ? (pos - (x - pos)) : x;
                var yy = !xFold && y > pos ? (pos - (y - pos)) : y;
                dotsSet.Add((xx, yy));
            }

            dots = dotsSet.ToList();
        }

        var solution = DrawDots(dots);
        _logger.LogInformation($"Solution: {solution}");
        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day13_Part2,
                Solution = solution.ToString(),
            }
        };
    }

    private static string DrawDots(List<(int x, int y)> dots)
    {
        var xMax = dots.Max(x => x.x) + 1;
        var yMax = dots.Max(x => x.y) + 1;
        var sb = new StringBuilder();
        for (int x = 0; x < xMax; x++)
        {
            for (int y = 0; y < yMax; y++)
            {
                if (dots.Contains((x, y)))
                    sb.Append('#');
                else
                    sb.Append(' ');
            }

            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }
}
