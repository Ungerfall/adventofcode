using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2021;
public class AdventOfCode2021_Day10_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day10_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day10_Part2>();
    }

    [Function("AdventOfCode2021_Day10_Part2")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/10.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        string[] navigation = blob
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToArray() ?? Array.Empty<string>();

        var openByScore = new Dictionary<char, int>
        {
            ['('] = 1,
            ['['] = 2,
            ['{'] = 3,
            ['<'] = 4,
        };
        var chunks = new Dictionary<char, char>()
        {
            ['('] = ')',
            ['['] = ']',
            ['{'] = '}',
            ['<'] = '>',
        };

        var scores = new List<long>();
        foreach (var line in navigation)
        {
            var metOpenings = new Stack<char>();
            bool ok = true;
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
                        ok = false;
                        break;
                    }

                    metOpenings.Pop();
                }
                else
                    throw new Exception($"Unexpected character: {c}");
            }

            if (ok)
            {
                var score = metOpenings.Aggregate(0L, (score, cur) => checked((5 * score) + openByScore[cur]));
                scores.Add(score);
            }
        }

        scores.Sort();
        var solution = scores[scores.Count / 2];

        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day10_Part2,
                Solution = solution.ToString(),
            }
        };
    }
}
