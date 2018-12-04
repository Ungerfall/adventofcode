using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018
{
	public class Solution2p1 : ISolution
	{
		public string Solve(IEnumerable<string> task)
		{
			var occurrences = new Dictionary<int, int>();
			for (int i = 2; i < task.Max(x => x.Length); i++)
			{
				occurrences[i] = 0;
			}

			Dictionary<char, int> wordCount;
			foreach (var word in task)
			{
				wordCount = new Dictionary<char, int>();
				foreach (var ch in word)
				{
					if (wordCount.ContainsKey(ch))
					{
						wordCount[ch]++;
					}
					else
					{
						wordCount.Add(ch, 1);
					}
				}

				foreach (var value in wordCount.Values.Where(v => v >= 2).Distinct())
				{
					occurrences[value]++;
				}
			}

			return occurrences
				.Where(kv => kv.Value >= 2)
				.Aggregate(1, (x, y) => x * y.Value)
				.ToString();
		}
	}
}
