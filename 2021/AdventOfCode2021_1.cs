using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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
        [ServiceBusOutput("sbq-adventofcode-output", Connection = "ServiceBusConnection")]
        public string Run([BlobTrigger("samples-workitems/1.input")] string myBlob, string name)
        {
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {myBlob}");

            return "Hello!";
        }
    }
}
