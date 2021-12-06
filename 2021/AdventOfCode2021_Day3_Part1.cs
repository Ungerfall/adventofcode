using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2021;
public class AdventOfCode2021_Day3_Part1
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day3_Part1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day3_Part1>();
    }

    [Function("AdventOfCode2021_Day3_Part1")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/3.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var report = blob
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        var positions = new Dictionary<int, (int zeros, int ones)>();
        foreach (var s in report)
        {
            foreach (var (c, index) in s.WithIndex())
            {
                positions.TryGetValue(index, out var tuple);
                var (zeros, ones) = tuple;
                if (c == '1')
                    ones++;
                else
                    zeros++;

                positions[index] = (zeros, ones);
            }
        }

        var (mostCommon, lessCommon) = positions.Aggregate(
            (mostCommonSb: new StringBuilder(), lessCommonSb: new StringBuilder()),
            (prev, current) =>
            {
                var (zeros, ones) = current.Value;
                if (zeros == ones)
                    throw new Exception("Equal amount of zeros and ones");

                prev.mostCommonSb.Append(zeros > ones ? '0' : '1');
                prev.lessCommonSb.Append(zeros > ones ? '1' : '0');
                return prev;
            },
            r => (
                Convert.ToInt32(r.mostCommonSb.ToString(), 2),
                Convert.ToInt32(r.lessCommonSb.ToString(), 2)
            ));

        _logger.LogInformation($"most common: {mostCommon}, less common: {lessCommon}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day3_Part1,
                Solution = (mostCommon * lessCommon).ToString(),
            }
        };
    }
}
