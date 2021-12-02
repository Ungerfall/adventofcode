using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace _2021
{
    public class AdventOfCode2021_OutputSender
    {
        private readonly ILogger _logger;

        public AdventOfCode2021_OutputSender(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AdventOfCode2021_OutputSender>();
        }

        [Function("AdventOfCode2021_OutputSender")]
        [BlobOutput("samples-workitems/1.input")]
        public void Run([ServiceBusTrigger("myqueue", Connection = "")] string myQueueItem)
        {
            _logger.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
