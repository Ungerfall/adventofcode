using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace _2021;

public class AdventOfCode2021_Day7_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day7_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day7_Part1>();
    }

    [Function("AdventOfCode2021_Day7_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/7.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var crabs = blob
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();
        Array.Sort(crabs);

        int median;
        if (crabs.Length % 2 == 0)
        {
            var p = crabs.Length / 2;
            p--;
            median = (crabs[p] + crabs[p+1]) / 2;
        }
        else
        {
            var p = (crabs.Length + 1) / 2;
            p--;
            median = crabs[p];
        }

        var solution = crabs.Sum(x => Math.Abs(x - median));

        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day7_Part1,
                Solution = solution.ToString(),
            }
        };
    }
}
