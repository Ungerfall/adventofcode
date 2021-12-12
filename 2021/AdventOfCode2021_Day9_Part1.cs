using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace _2021;
public class AdventOfCode2021_Day9_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day9_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day9_Part1>();
    }

    [Function("AdventOfCode2021_Day9_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/9.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        int[][] heatmap = blob
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Select(y => (int)char.GetNumericValue(y)).ToArray())
            .ToArray() ?? Array.Empty<int[]>();

        var xMax = heatmap.Length - 1;
        int sum = 0;
        for (int x = 0; x < heatmap.Length; x++)
        {
            var yMax = heatmap[x].Length - 1;
            for (int y = 0; y < heatmap[x].Length; y++)
            {
                var adjucent = new[]
                {
                    x - 1 < 0 ? int.MaxValue : heatmap[x-1][y],
                    x + 1 > xMax ? int.MaxValue : heatmap[x+1][y],
                    y - 1 < 0 ? int.MaxValue : heatmap[x][y-1],
                    y + 1 > yMax ? int.MaxValue : heatmap[x][y+1]
                };
                if (heatmap[x][y] < adjucent.Min())
                {
                    sum += heatmap[x][y] + 1;
                }
            }
        }

        var solution = sum;
        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day9_Part1,
                Solution = solution.ToString(),
            }
        };
    }
}
