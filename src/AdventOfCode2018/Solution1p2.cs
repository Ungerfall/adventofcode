using System.Collections.Generic;

namespace AdventOfCode2018
{
	class Solution1p2 : ISolution
	{
		public string Solve(IEnumerable<string> task)
		{
			var frequencies = new Dictionary<int, object>();
			frequencies.Add(0, 0);
			var frequency = 0;
			var infiniteTask = new InfiniteEnumerable<string>(task);
			foreach(var changeString in infiniteTask)
			{
				var change = int.Parse(changeString);
				frequency += change;
				if (frequencies.ContainsKey(frequency))
				{
					break;
				}

				frequencies.Add(frequency, frequency);
			}

			return frequency.ToString();
		}
	}
}
