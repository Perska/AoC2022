using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
	partial class Program
	{
		// [UseSRL] // Uncomment if you wanna use SuperReadLine
		// [NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		static void Day01(List<string> input)
		{
			List<List<int>> elves = new List<List<int>>();
			bool lastBlank = true;
			for (int i = 0; i < input.Count; i++)
			{
				if (lastBlank && input[i] != "")
				{
					elves.Add(new List<int>());
					//Log("Added a new elf!");
					lastBlank = false;
				} 
				else if (!lastBlank && input[i] == "")
				{
					Log($"Completed this elf with a total of {elves.Last().Sum()} across {elves.Last().Count} different food items.");
					lastBlank = true;
				}
				if (input[i] != "")
				{
					int calorie = int.Parse(input[i]);
					elves.Last().Add(calorie);
				}
			}

			var top = elves.OrderByDescending(elf => elf.Sum()).ToList();
			
			Log($"The elf who has the most calories has: {top[0].Sum()} calories.");
			Log($"The top three elves have: {top.GetRange(0, 3).Sum(elf => elf.Sum())} calories.");
		}
	}
}
