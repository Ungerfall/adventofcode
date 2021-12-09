using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2021;

public class AdventOfCode2021_Day6_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day6_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day6_Part1>();
    }

    [Function("AdventOfCode2021_Day6_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/6.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var fish = blob
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .GroupBy(x => x)
            .ToDictionary(k => k.Key, v => v.Count());
        for (int i = 0; i < 80; i++)
        {
            var newFish = new Dictionary<int, int>();
            foreach (var key in fish.Keys)
            {
                if (key == 0)
                {
                    newFish[8] = fish[0];
                    newFish.AddOrIncrement(6, fish[0]);
                }
                else
                    newFish.AddOrIncrement(key - 1, fish[key]);
            }

            fish = newFish;
        }

        var solution = fish.Values.Sum();

        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day6_Part1,
                Solution = solution.ToString(),
            }
        };
    }
}
