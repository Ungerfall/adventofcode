using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace _2021;

public class AdventOfCode2021_Day1_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day1_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day1_Part1>();
    }

    [Function("AdventOfCode2021_Day1_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/" + Days.Day1_Part1 + ".input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var measurements = blob
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();
        int increases = 0;
        for (int i = 1; i < measurements.Length; i++)
        {
            if (measurements[i] > measurements[i - 1])
            {
                increases++;
            }
        }

        _logger.LogInformation($"Number of increases: {increases}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day1_Part1,
                Solution = increases.ToString(),
            }
        };
    }
}
