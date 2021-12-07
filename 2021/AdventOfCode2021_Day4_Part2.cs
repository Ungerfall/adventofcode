using _2021.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2021;
public class AdventOfCode2021_Day4_Part2
{
    private readonly ILogger _logger;

    public AdventOfCode2021_Day4_Part2(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdventOfCode2021_Day4_Part2>();
    }

    [Function("AdventOfCode2021_Day4_Part2")]
    public QueueOutput Run([BlobTrigger("advent-of-code-2021/4.input")] string blob)
    {
        _logger.LogInformation("C# Blob trigger function Processed blob");

        var input = blob
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToArray();
        var seq = input[0].Split(',').Select(int.Parse);
        var boards = new List<Board>();
        var buffer = new List<string>();
        for (int i = 1; i < input.Length; i++)
        {
            if (i > 1 && i % 5 == 1)
            {
                boards.Add(new Board(buffer));
                buffer = new List<string>();
            }

            buffer.Add(input[i]);
        }

        Board lastWinner = null;
        int winningNumber = 0;
        foreach (var n in seq)
        {
            foreach (var board in boards)
            {
                if (board.IsWinner())
                    continue;

                board.Cross(n);
                if (board.IsWinner())
                {
                    lastWinner = board;
                    winningNumber = n;
                }
            }
        }

        if (lastWinner == null)
            throw new Exception("No winner");

        var unmarkedSum = lastWinner.SumUnmarked();
        var solution = unmarkedSum * winningNumber;

        _logger.LogInformation($"winning number: {winningNumber}, unmarkedSum: {unmarkedSum}, "
            + $"solution: {solution}");

        return new QueueOutput
        {
            Payload = new AdventOfCodeOutput
            {
                Day = Days.Day4_Part2,
                Solution = solution.ToString(),
            }
        };
    }

    private class Board
    {
        private readonly Cell[][] cells;
        private bool winner = false;

        public Board(IEnumerable<string> raw)
        {
            cells = raw
                .Select(x =>
                    x
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new Cell { Value = int.Parse(x) })
                    .ToArray())
                .ToArray();
            if (cells.Length != 5)
                throw new ArgumentException();

            if (cells.Any(x => x.Length != 5))
                throw new ArgumentException();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            foreach (var line in cells)
            {
                sb.Append(string.Join(" ", line.Select(x => x.Value)));
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        private class Cell
        {
            internal int Value { get; set; }
            internal bool Marked { get; set; }
        }

        public void Cross(int value)
        {
            foreach (var arr in cells)
            {
                foreach (var cell in arr)
                {
                    if (cell.Value == value)
                        cell.Marked = true;
                }
            }
        }

        public long SumUnmarked()
        {
            return cells
                .SelectMany(x => x)
                .Where(x => !x.Marked)
                .Sum(x => x.Value);
        }

        public bool IsWinner()
        {
            if (winner)
                return true;

            if (cells.Any(x => x.All(y => y.Marked)))
                winner = true;

            for (int y = 0; y < 5; y++)
            {
                bool w = true;
                for (int x = 0; x < 5; x++)
                {
                    if (!cells[x][y].Marked)
                    {
                        w = false;
                        break;
                    }
                }

                if (w)
                    winner = true;
            }

            return winner;
        }
    }
}
