using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace _2021;

public class AdventOfCode2021_OutputSender
{
    private readonly ILogger _logger;
    private readonly string _email = "Leonid_Petrov1@epam.com";

    public AdventOfCode2021_OutputSender(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_OutputSender>();
    }

    [Function("AdventOfCode2021_OutputSender")]
    public SenderOutput Run(
        [ServiceBusTrigger("sbq-adventofcode-output", Connection = "ServiceBusConnection")]
        AdventOfCodeOutput q)
    {
        _logger.LogInformation($"C# ServiceBus queue message: {new { q.Day, q.Solution }}");

        var message = new SendGridMessage
        {
            From = new EmailAddress
            {
                Email = _email,
                Name = "Leonid Petrov",
            },
            Subject = $"Solution for {q.Day}",
            Personalizations = new List<Personalization>
            {
                new()
                {
                    Tos = new List<EmailAddress>
                    {
                        new() { Name = "Receiver", Email = _email }
                    }
                }
            },
            Contents = new List<Content>
            {
                new() { Type = "text/plain", Value = q.Solution }
            }
        };

        return new SenderOutput
        {
            Blob = q.Solution,
            EmailBody = message,
        };
    }
}
