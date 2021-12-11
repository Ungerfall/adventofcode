using Microsoft.Azure.Functions.Worker;

namespace _2021.Models;

public class SenderOutput
{
    [BlobOutput("advent-of-code-2021/{Day}.output")]
    public string? Blob { get; init; }

    [SendGridOutput(ApiKey = "SendGridApiKey")]
    public SendGridMessage? EmailBody { get; init; }
}