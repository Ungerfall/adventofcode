using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace _2021;

public class AdventOfCode2021_Day7_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day7_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day7_Part2>();
    }

    [Function("AdventOfCode2021_Day7_Part2")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/7.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var crabs = blob
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();
        Array.Sort(crabs);

        int fuelMin = int.MaxValue;
        int min = crabs[0];
        int max = crabs[^1] + 1;
        for (int i = min; i < max; i++)
        {
            var sum = 0;
            foreach (var crab in crabs)
            {
                var n = Math.Abs(crab - i);
                var fuel = n * (n + 1) / 2;
                sum += fuel;
            }

            if (sum < fuelMin)
                fuelMin = sum;
        }

        var solution = fuelMin;
        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day7_Part2,
                Solution = solution.ToString(),
            }
        };
    }
}
