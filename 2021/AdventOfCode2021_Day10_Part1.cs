using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2021;
public class AdventOfCode2021_Day10_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day10_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day10_Part1>();
    }

    [Function("AdventOfCode2021_Day10_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/10.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        string[] navigation = blob
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToArray() ?? Array.Empty<string>();

        var scores = new Dictionary<char, int>
        {
            [')'] = 3,
            [']'] = 57,
            ['}'] = 1197,
            ['>'] = 25137,
        };
        var corrupted = new Dictionary<char, int>();
        var chunks = new Dictionary<char, char>()
        {
            ['('] = ')',
            ['['] = ']',
            ['{'] = '}',
            ['<'] = '>',
        };

        foreach (var line in navigation)
        {
            var metOpenings = new Stack<char>();
            foreach (var c in line)
            {
                if (chunks.Keys.Contains(c))
                {
                    metOpenings.Push(c);
                }
                else if (chunks.Values.Contains(c))
                {
                    if (c != chunks[metOpenings.Peek()])
                    {
                        corrupted.AddOrIncrement(c, 1);
                        break;
                    }

                    metOpenings.Pop();
                }
                else
                    throw new Exception($"Unexpected character: {c}");
            }
        }

        var solution = corrupted.Sum(kv => kv.Value * scores[kv.Key]);

        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day10_Part1,
                Solution = solution.ToString(),
            }
        };
    }
}
