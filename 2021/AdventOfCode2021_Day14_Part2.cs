using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2021;
public class AdventOfCode2021_Day14_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day14_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day14_Part2>();
    }

    [Function("AdventOfCode2021_Day14_Part2")]
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

        Dictionary<string, long> counter = new();
        for (int i = 0; i < polymer.Length - 1; i++)
            counter.AddOrIncrement(polymer.Substring(i, 2), 1);

        foreach (var _ in Enumerable.Range(0, 40))
        {
            Dictionary<string, long> counterNew = new();
            foreach (var (k, v) in counter)
            {
                if (insertions.TryGetValue(k, out var ch))
                {
                    var left = k[0] + ch;
                    var right = ch + k[1];
                    counterNew.AddOrIncrement(left, v);
                    counterNew.AddOrIncrement(right, v);
                }
                else
                {
                    counterNew[k] = v;
                }
            }

            counter = counterNew;
        }

        Dictionary<char, long> charCounter = new();
        foreach (var (k, v) in counter)
        {
            char @char = k[0];
            charCounter.AddOrIncrement(@char, v);
        }

        charCounter.AddOrIncrement(polymer[^1], 1);

        var solution = charCounter.Values.Max() - charCounter.Values.Min();
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
