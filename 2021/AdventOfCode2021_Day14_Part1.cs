using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2021;
public class AdventOfCode2021_Day14_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day14_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day14_Part1>();
    }

    [Function("AdventOfCode2021_Day14_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/13.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        string[] text = blob
                .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        string polymer = text[0].Trim();
        Dictionary<string, string> insertions = text[1..]
            .Select(x =>
            {
                var g = x.Split("->");
                return (rule: g[0].Trim(), insertion: g[1].Trim());
            })
            .ToDictionary(k => k.rule, v => v.insertion);

        foreach (var _ in Enumerable.Range(0, 10))
        {
            var sb = new StringBuilder();
            for (int i = 0; i < polymer.Length - 1; i++)
            {
                var pair = polymer.Substring(i, 2);
                if (insertions.TryGetValue(pair, out var insertion))
                {
                    sb.Append(pair[0]);
                    sb.Append(insertion);
                }
                else
                {
                    sb.Append(pair[0]);
                }
            }

            sb.Append(polymer[^1]);
            polymer = sb.ToString();
        }

        var occurances = polymer
            .GroupBy(x => x)
            .Select(x => x.Count())
            .ToArray();
        Array.Sort(occurances);

        var solution = occurances[^1] - occurances[0];
        _logger.LogInformation($"Solution: {solution}");
        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day14_Part2,
                Solution = solution.ToString(),
            }
        };
    }
}
