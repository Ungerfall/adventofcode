<Query Kind="Program" />

void Main()
{	
	int passwordsCount = 0;
	for (int i = 278384; i <= 824795; i++)
	{
		var val = i;
		bool ascend = true;
		int pair = 0;
		int? prev = null;
		int seq = 1;
		do
		{
			var curr = val % 10;
			if (curr > prev)
			{
				ascend = false;
				break;
			}
			
			if (curr == prev)
			{
				seq++;
				if (seq == 2)
					pair++;
				else if (seq == 3)
					pair--;
			}
			else
			{
				seq = 1;
			}
				
			prev = curr;
			val /= 10;
		} while (val > 0);
		
		if (pair >= 1 && ascend)
			passwordsCount++;
	}
	
	passwordsCount.Dump();
}
