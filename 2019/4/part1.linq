<Query Kind="Program" />

void Main()
{	
	int passwordsCount = 0;
	for (int i = 278384; i <= 824795; i++)
	{
		var val = i;
		bool valid = false;
		int? prev = null;
		do
		{
			var curr = val % 10;
			if (curr > prev)
			{
				valid = false;
				break;
			}
			
			if (curr == prev)
				valid = true;
				
			prev = curr;
			val /= 10;
		} while (val > 0);
		
		if (valid)
		{
			i.Dump();
			passwordsCount++;
		}
	}
	
	passwordsCount.Dump();
}

// Define other methods, classes and namespaces here
