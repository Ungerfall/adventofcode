using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2021;

public class AdventOfCode2021_Day5_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day5_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day5_Part1>();
    }

    [Function("AdventOfCode2021_Day5_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/5.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var segmentPattern = new Regex(@"^(\d+),(\d+)\s+->\s+(\d+),(\d+)");
        var input = blob
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                var g = segmentPattern.Match(x).Groups;
                return (x1: toInt(g[1]), y1: toInt(g[2]), x2: toInt(g[3]), y2: toInt(g[4]));
                int toInt(Group g) => int.Parse(g.Value);
            })
            .ToArray();
        var density = new Dictionary<Point, int>();
        foreach (var (x1, y1, x2, y2) in input)
        {
            var dx = x1 == x2 ? 0 : x1 < x2 ? 1 : -1;
            var dy = y1 == y2 ? 0 : y1 < y2 ? 1 : -1;
            var isLine = (dx == 0 && dy != 0) || (dy == 0 && dx != 0);
            if (!isLine)
                continue;

            var x = x1;
            var y = y1;
            for (int i = 0; i <= Math.Abs(x1 - x2) + Math.Abs(y1 - y2); i++)
            {
                var point = new Point(x, y);
                density.TryGetValue(point, out int value);
                density[point] = ++value;
                x += dx;
                y += dy;
            }
        }

        var solution = density.Values.Count(x => x >= 2);

        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day5_Part1,
                Solution = solution.ToString(),
            }
        };
    }

    public record Point(int X, int Y);
}
