using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;

namespace _2021;

public class AdventOfCode2021_Day8_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day8_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day8_Part1>();
    }

    [Function("AdventOfCode2021_Day8_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/8.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var input = blob
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                var tenFour = x.Split('|', StringSplitOptions.RemoveEmptyEntries);
                Debug.Assert(tenFour.Length == 2);
                var signal = tenFour[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Debug.Assert(signal.Length == 10);
                var output = tenFour[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Debug.Assert(output.Length == 4);
                return new { signal, output };
            })
            .ToArray();

        var easyDigits = new[] { 2, 3, 4, 7 };
        var solution = input.Sum(x => x.output.Where(y => easyDigits.Contains(y.Length)).Count());
        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day8_Part1,
                Solution = solution.ToString(),
            }
        };
    }
}
