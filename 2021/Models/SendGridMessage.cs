using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace _2021.Models;

public class SendGridMessage
{
    public EmailAddress From { get; set; }
    public string Subject { get; set; }
    public List<Personalization> Personalizations { get; set; }
    [JsonPropertyName("Content")]
    public List<Content> Contents { get; set; }
}

public class EmailAddress
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class Personalization
{
    [JsonPropertyName("to")]
    public List<EmailAddress> Tos { get; set; }
}

public class Content
{
    public string Type { get; set; }
    public string Value { get; set; }
}