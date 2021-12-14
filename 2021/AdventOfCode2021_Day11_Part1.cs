using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2021;
public class AdventOfCode2021_Day11_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day11_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day11_Part1>();
    }

    [Function("AdventOfCode2021_Day11_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/11.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        int[][] octopuses = blob
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Select(y => (int)char.GetNumericValue(y)).ToArray())
            .ToArray() ?? Array.Empty<int[]>();

        var xMax = octopuses.Length;
        var yMax = octopuses[0].Length;
        var adjX = new[] { -1, -1, -1, 0, 0, 1, 1, 1 };
        var adjY = new[] { -1, 0, 1, -1, 1, -1, 0, 1 };
        int sum = 0;

        for (int step = 0; step < 100; step++)
        {
            for (int x = 0; x < xMax; x++)
                for (int y = 0; y < yMax; y++)
                    octopuses[x][y]++;

            var flashes = new HashSet<(int x, int y)>();
            bool hasFlashes;
            do
            {
                hasFlashes = false;
                for (int x = 0; x < xMax; x++)
                    for (int y = 0; y < yMax; y++)
                    {
                        if (octopuses[x][y] > 9)
                        {
                            hasFlashes = true;
                            octopuses[x][y] = 0;
                            flashes.Add((x, y));
                            foreach (var i in Enumerable.Range(0, 8))
                            {
                                var xx = x + adjX[i];
                                var yy = y + adjY[i];
                                if (xx < 0 || xx >= xMax)
                                    continue;
                                if (yy < 0 || yy >= yMax)
                                    continue;
                                if (flashes.Contains((xx, yy)))
                                    continue;

                                octopuses[xx][yy]++;
                            }
                        }
                    }

            } while (hasFlashes);

            sum += flashes.Count;
        }

        var solution = sum;

        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day11_Part1,
                Solution = solution.ToString(),
            }
        };
    }
}
