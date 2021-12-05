using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2021;

public class AdventOfCode2021_Day2_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day2_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day2_Part2>();
    }

    [Function("AdventOfCode2021_Day2_Part2")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/2.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var linePattern = new Regex(@"^(\w+)\s+(\d+)", RegexOptions.Compiled);
        var instructions = blob
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                var g = linePattern.Match(x).Groups;
                return (direction: g[1].Value, len: int.Parse(g[2].Value));
            })
            .ToArray();
        int horizontal = 0;
        int depth = 0;
        int aim = 0;
        foreach (var i in instructions)
        {
            if (i.direction == "down")
                aim += i.len;
            else if (i.direction == "up")
                aim -= i.len;
            else
            {
                horizontal += i.len;
                depth += aim * i.len;
            }
        }

        _logger.LogInformation($"horizontal: {horizontal}; depth: {depth}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day2_Part1,
                Solution = (horizontal * depth).ToString(),
            }
        };
    }
}
