using Microsoft.Azure.Functions.Worker;

namespace _2021.Models;

public class QueueOutput
{
    [ServiceBusOutput("sbq-adventofcode-output", Connection = "ServiceBusConnection")]
    public AdventOfCodeOutput Payload { get; set; }
}