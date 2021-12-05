using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace _2021;

public class AdventOfCode2021_OutputSender
{
    private readonly ILogger _logger;

    public AdventOfCode2021_OutputSender(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_OutputSender>();
    }

    [Function("AdventOfCode2021_OutputSender")]
    [BlobOutput("advent-of-code-2021/{Day}.output")]
    public string Run(
        [ServiceBusTrigger("sbq-adventofcode-output", Connection = "ServiceBusConnection")] AdventOfCodeOutput q)
    {
        _logger.LogInformation($"C# ServiceBus queue message: {new { q.Day, q.Solution }}");

        return q.Solution;
    }
}