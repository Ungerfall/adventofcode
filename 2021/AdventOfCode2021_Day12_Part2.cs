using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2021;
public class AdventOfCode2021_Day12_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day12_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day12_Part2>();
    }

    [Function("AdventOfCode2021_Day12_Part2")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/12.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var connectionPattern = new Regex(@"^(\w+)-(\w+)");
        (string source, string dest)[] paths = blob
            .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                var g = connectionPattern.Match(x).Groups;
                return (source: g[1].Value, dest: g[2].Value);
            })
            .ToArray();

        var graph = new Dictionary<string, HashSet<string>>();
        foreach (var p in paths)
        {
            if (!graph.ContainsKey(p.source))
                graph[p.source] = new HashSet<string>(new[] { p.dest });
            else
                graph[p.source].Add(p.dest);


            if (!graph.ContainsKey(p.dest))
                graph[p.dest] = new HashSet<string>(new[] { p.source });
            else
                graph[p.dest].Add(p.source);
        }

        var solution = 0;
        var path = new Stack<(string, Dictionary<string, int>)>();
        path.Push(("start", new Dictionary<string, int> { ["start"] = 0 }));
        while (path.Count > 0)
        {
            var (node, visited) = path.Pop();
            if (node == "end")
            {
                solution++;
                continue;
            }

            foreach (var adj in graph[node])
            {
                var isNewCave = !visited.TryGetValue(adj, out var remaining);
                if (isNewCave || remaining > 0)
                {
                    var newVisitedSmall = new Dictionary<string, int>(visited);
                    if (char.IsLower(adj[0]))
                    {
                        if (isNewCave)
                            newVisitedSmall[adj] = 1;
                        else
                            newVisitedSmall[adj] = 0;
                    }

                    path.Push((adj, newVisitedSmall));
                }
            }
        }

        _logger.LogInformation($"Solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day12_Part2,
                Solution = solution.ToString(),
            }
        };
    }
}
