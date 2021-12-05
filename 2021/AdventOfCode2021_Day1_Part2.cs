using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace _2021;
public class AdventOfCode2021_Day1_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day1_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day1_Part2>();
    }

    [Function("AdventOfCode2021_Day1_Part2")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/1.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var measurements = blob
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();
        int increases = 0;
        const int windowSize = 3;
        for (int i = 0; i < measurements.Length - windowSize; i++)
        {
            var currentWindow = measurements.Slice(i, windowSize).Sum();
            var nextWindow = measurements.Slice(i + 1, windowSize).Sum();
            if (nextWindow > currentWindow)
            {
                increases++;
            }
        }

        _logger.LogInformation($"Number of increases: {increases}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day1_Part2,
                Solution = increases.ToString(),
            }
        };
    }
}
