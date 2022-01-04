using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2021;
public class AdventOfCode2021_Day15_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day15_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day15_Part2>();
    }

    [Function("AdventOfCode2021_Day15_Part2")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/15.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        int[][] riskMap = blob
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Select(c => (int)char.GetNumericValue(c)).ToArray())
            .ToArray() ?? Array.Empty<int[]>();

        var adjX = new[] { -1, 0, 0, 1 };
        var adjY = new[] { 0, -1, 1, 0 };
        const int multiplier = 5;
        var xBlockSize = riskMap.Length;
        var yBlockSize = riskMap[0].Length;
        var xMax = xBlockSize * multiplier;
        var yMax = yBlockSize * multiplier;
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
        queue.Enqueue(start, 0);
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
                var xBlock = xx / xBlockSize;
                var yBlock = yy / yBlockSize;
                var weight = (xBlock + yBlock + riskMap[xx % xBlockSize][yy % xBlockSize]) % 9 == 0
                    ? 9
                    : (xBlock + yBlock + riskMap[xx % xBlockSize][yy % xBlockSize]) % 9;
                var currentDistance = distance[current];
                var nextDistance = currentDistance + weight;
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
                Day = Days.Day15_Part2,
                Solution = solution.ToString(),
            }
        };
    }

    internal record Point(int X, int Y);
}
