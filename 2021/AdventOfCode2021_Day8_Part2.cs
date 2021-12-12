using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace _2021;

public class AdventOfCode2021_Day8_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day8_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day8_Part2>();
    }

    [Function("AdventOfCode2021_Day8_Part2")]
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
                return (signal, output);
            })
            .ToArray();

        int sum = 0;
        foreach (var (signal, output) in input)
        {
            char top;
            char topLeft;
            char topRight;
            char center;
            char bottom;
            char bottomLeft;
            char bottomRight;
            var numbers = Enumerable.Range(1, 10).Select(_ => new HashSet<char>()).ToArray();
            var _6or9or0 = new List<string>(3);
            var _2or3or5 = new List<string>(3);
            foreach (var i in signal)
            {
                if (i.Length == 2)
                    numbers[1].UnionWith(i);

                if (i.Length == 4)
                    numbers[4].UnionWith(i);

                if (i.Length == 3)
                    numbers[7].UnionWith(i);

                if (i.Length == 7)
                    numbers[8].UnionWith(i);

                if (i.Length == 6)
                    _6or9or0.Add(i);

                if (i.Length == 5)
                    _2or3or5.Add(i);
            }

            top = numbers[7].Except(numbers[1]).First();
            var centerTopRBottomL = _6or9or0
                .SelectMany(x => x)
                .GroupBy(x => x)
                .Where(g => g.Count() == 2)
                .Select(g => g.First()).ToArray();
            topLeft = numbers[4].Except(numbers[1].Concat(centerTopRBottomL)).First();
            center = numbers[4].Except(numbers[1].Concat(new char[] { topLeft })).First();
            var topCenterBottom = _2or3or5
                .SelectMany(x => x)
                .GroupBy(x => x)
                .Where(g => g.Count() == 3)
                .Select(g => g.First()).ToArray();
            bottom = topCenterBottom.Except(new char[] { top, center }).First();
            var bottomLeftSet = new HashSet<char>(_2or3or5.SelectMany(x => x));
            bottomLeftSet.ExceptWith(numbers[1].Concat(new char[] { top, topLeft, center, bottom }));
            Debug.Assert(bottomLeftSet.Count == 1);
            bottomLeft = bottomLeftSet.First();
            topRight = centerTopRBottomL.Except(new char[] { bottomLeft, center }).First();
            bottomRight = numbers[1].Except(new char[] { topRight }).First();

            numbers[0].UnionWith(new char[] { topRight, top, topLeft, bottomRight, bottom, bottomLeft });
            numbers[2].UnionWith(new char[] { top, topRight, center, bottomLeft, bottom });
            numbers[3].UnionWith(new char[] { top, topRight, center, bottomRight, bottom });
            numbers[5].UnionWith(new char[] { topLeft, top, center, bottomRight, bottom });
            numbers[6].UnionWith(new char[] { topLeft, top, center, bottomLeft, bottom, bottomRight });
            numbers[9].UnionWith(new char[] { topLeft, top, topRight, center, bottomRight, bottom });

            var outputString = new StringBuilder();
            foreach (var o in output.Select(x => new HashSet<char>(x)))
            {
                for (int i = 0; i < numbers.Length; i++)
                {
                    if (numbers[i].SetEquals(o))
                    {
                        outputString.Append(i);
                        break;
                    }
                }
            }

            Debug.Assert(outputString.Length == 4);
            sum += Convert.ToInt32(outputString.ToString());
        }

        var solution = sum;
        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day8_Part2,
                Solution = solution.ToString(),
            }
        };
    }
}
