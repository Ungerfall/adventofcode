using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace _2021
{
    public class AdventOfCode2021_1
    {
        private readonly ILogger _logger;

        public AdventOfCode2021_1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AdventOfCode2021_1>();
        }

        [Function("AdventOfCode2021_1")]
        public QueueOutput Run([BlobTrigger("advent-of-code-2021/1.input")] string myBlob)
        {
            _logger.LogInformation("C# Blob trigger function Processed blob");

            var measurements = myBlob
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();
            int increases = 0;
            for (int i = 1; i < measurements.Length; i++)
            {
                if (measurements[i] > measurements[i - 1])
                {
                    increases++;
                }
            }

            _logger.LogInformation($"Number of increases: {increases}");

            return new QueueOutput
            {
                Payload = new AdventOfCodeOutput
                {
                    Day = "1",
                    Solution = increases.ToString(),
                }
            };
        }
    }
}
