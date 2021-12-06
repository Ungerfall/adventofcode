using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2021;

public class AdventOfCode2021_Day3_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day3_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day3_Part2>();
    }

    [Function("AdventOfCode2021_Day3_Part2")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/3.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var report = blob
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        var bits = report[0].Length;
        var oxygen = new List<string>(report);
        for (int position = 0; position < bits; position++)
        {
            if (oxygen.Count == 1)
                break;
            var (zeros, ones) = CountBits(oxygen, position);
            if (ones >= zeros)
                oxygen = oxygen.Where(x => x[position] == '1').ToList();
            else
                oxygen = oxygen.Where(x => x[position] == '0').ToList();
        }

        var co2 = new List<string>(report);
        for (int position = 0; position < bits; position++)
        {
            if (co2.Count == 1)
                break;
            var (zeros, ones) = CountBits(co2, position);
            if (zeros <= ones)
                co2 = co2.Where(x => x[position] == '0').ToList();
            else
                co2 = co2.Where(x => x[position] == '1').ToList();
        }

        var oxygenInt = Convert.ToInt32(oxygen[0], 2);
        var co2Int = Convert.ToInt32(co2[0], 2);

        _logger.LogInformation($"Oxygen: {oxygenInt}, CO2: {co2Int}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day3_Part2,
                Solution = (oxygenInt * co2Int).ToString(),
            }
        };
    }

    private (int zeros, int ones) CountBits(IEnumerable<string> input, int position)
    {
        int zeros = 0;
        int ones = 0;
        foreach (var s in input)
        {
            if (s[position] == '1')
                ones++;
            else
                zeros++;
        }

        return (zeros, ones);
    }
}
