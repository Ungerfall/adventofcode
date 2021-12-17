using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2021;
public class AdventOfCode2021_Day13_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day13_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day13_Part1>();
    }

    [Function("AdventOfCode2021_Day13_Part1")]
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
                dots.Add((numbers[0], numbers[1]));
            }
        }
        
        var firstFold = folds[0];
        var dotsSet = new HashSet<(int x, int y)>();
        foreach (var (x, y) in dots)
        {
            string axis = firstFold.axis;
            bool xFold = axis == "x";
            int fold = firstFold.v;
            if ((xFold && fold == x) || (!xFold && fold == y))
                continue;

            var xx = xFold ? Math.Abs(fold - x) : x;
            var yy = !xFold ? Math.Abs(fold - y) : y;
            dotsSet.Add((xx, yy));
        }

        var solution = dotsSet.Count;
        _logger.LogInformation($"Solution: {solution}");
        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day13_Part1,
                Solution = solution.ToString(),
            }
        };
    }
}
