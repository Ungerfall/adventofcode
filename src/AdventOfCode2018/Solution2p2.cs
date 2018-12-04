using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018
{
	public class Solution2p2 : ISolution
	{
		public string Solve(IEnumerable<string> task)
		{
			var sortedSeq = task.OrderBy(x => x).ToArray();
			var answer = string.Empty;
			for (int i = 0; i < sortedSeq.Length - 1; i++)
			{
				for (int j = i; j < sortedSeq.Length; j++)
				{

				}
			}

			return string.Empty;
		}
	}
}
