using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2021;
public class AdventOfCode2021_Day15_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day15_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day15_Part1>();
    }

    [Function("AdventOfCode2021_Day15_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/13.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        int[][] riskMap = blob
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Select(c => (int)char.GetNumericValue(c)).ToArray())
            .ToArray() ?? Array.Empty<int[]>();

        var adjX = new[] { -1, 0, 0, 1 };
        var adjY = new[] { 0, -1, 1, 0 };
        var xMax = riskMap.Length;
        var yMax = riskMap[0].Length;
        Point start = new(0, 0);
        Point end = new(xMax - 1, yMax - 1);

        Dictionary<Point, int> distance = new();
        for (int x = 0; x < xMax; x++)
        {
            for (int y = 0; y < yMax; y++)
            {
                var p = new Point(x, y);
                distance[p] = int.MaxValue;
            }
        }

        distance[start] = 0;
        var queue = new PriorityQueue<Point, int>();
        queue.Enqueue(start, distance[start]);
        var solution = int.MaxValue;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var (x, y) = current;
            if (current == end)
            {
                solution = distance[current];
                break;
            }

            foreach (var i in Enumerable.Range(0, 4))
            {
                var xx = x + adjX[i];
                var yy = y + adjY[i];
                if (xx < 0 || xx >= xMax)
                    continue;
                if (yy < 0 || yy >= yMax)
                    continue;

                var next = new Point(xx, yy);
                var weight = riskMap[xx][yy];
                var currentDistance = distance[current];
                var nextDistance = currentDistance + riskMap[xx][yy];
                if (distance[next] > nextDistance)
                {
                    distance[next] = nextDistance;
                    queue.Enqueue(next, nextDistance);
                }
            }
        }

        _logger.LogInformation($"Solution: {solution}");
        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day15_Part1,
                Solution = solution.ToString(),
            }
        };
    }

    internal record Point(int X, int Y);
}
