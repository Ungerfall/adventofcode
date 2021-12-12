using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2021;
public class AdventOfCode2021_Day9_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day9_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day9_Part2>();
    }

    [Function("AdventOfCode2021_Day9_Part2")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/9.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        int[][] heatmap = blob
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Select(y => (int)char.GetNumericValue(y)).ToArray())
            .ToArray() ?? Array.Empty<int[]>();

        var xMax = heatmap.Length - 1;
        var yMax = heatmap[0].Length - 1;
        var seen = new HashSet<(int x, int y)>();
        var basins = new List<int>();
        for (int x = 0; x < heatmap.Length; x++)
        {
            for (int y = 0; y < heatmap[x].Length; y++)
            {
                if (seen.Contains((x, y)) || heatmap[x][y] == 9)
                    continue;

                int size = 0;
                var q = new Queue<(int x, int y)>();
                q.Enqueue((x, y));
                while (q.Count > 0)
                {
                    size++;
                    var (xc, yc) = q.Dequeue();
                    seen.Add((xc, yc));
                    (int, int)? left = yc - 1 < 0 || heatmap[xc][yc-1] == 9 ? null : (xc, yc-1);
                    (int, int)? top = xc - 1 < 0 || heatmap[xc-1][yc] == 9 ? null : (xc-1, yc);
                    (int, int)? right = yc + 1 > yMax || heatmap[xc][yc+1] == 9 ? null : (xc, yc+1);
                    (int, int)? bottom = xc + 1 > xMax || heatmap[xc+1][yc] == 9 ? null : (xc+1, yc);
                    if (left != null && !seen.Contains(left.Value) && !q.Contains(left.Value))
                        q.Enqueue(left.Value);

                    if (top != null && !seen.Contains(top.Value) && !q.Contains(top.Value))
                        q.Enqueue(top.Value);

                    if (right != null && !seen.Contains(right.Value) && !q.Contains(right.Value))
                        q.Enqueue(right.Value);

                    if (bottom != null && !seen.Contains(bottom.Value) && !q.Contains(bottom.Value))
                        q.Enqueue(bottom.Value);
                }

                basins.Add(size);
            }
        }

        basins.Sort();
        var solution = basins[^1] * basins[^2] * basins[^3];
        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day9_Part2,
                Solution = solution.ToString(),
            }
        };
    }
}
