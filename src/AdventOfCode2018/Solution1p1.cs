using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2018
{
	public class Solution1p1 : ISolution
	{
		public string Solve(IEnumerable<string> task)
		{
			return task
				.Select(t => int.Parse(t))
				.Sum()
				.ToString();
		}
	}
}
