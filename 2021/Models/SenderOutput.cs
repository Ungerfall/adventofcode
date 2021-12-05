using Microsoft.Azure.Functions.Worker;

namespace _2021.Models;

public class SenderOutput
{
    [BlobOutput("advent-of-code-2021/{Day}.output")]
    public string Blob { get; set; }

    [SendGridOutput(ApiKey = "SendGridApiKey")]
    public string EmailBody { get; set; }
}