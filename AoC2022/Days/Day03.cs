using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace AoC2022
{
	partial class Program
	{
		// [UseSRL] // Uncomment if you wanna use SuperReadLine
		[NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		static void Day03(List<string> input)
		{
			int priority(char c)
			{
				if ('a' <= c && c <= 'z') return c - 'a' + 1;
				if ('A' <= c && c <= 'Z') return c - 'A' + 27;
				return 0;
			}
			int prioritiesA = 0;
			int prioritiesB = 0;

			int index = 0;
			string[] groups = new string[3];
			for (int i = 0; i < input.Count; i++)
			{
				string a = input[i].Substring(0, input[i].Length / 2);
				string b = input[i].Substring(input[i].Length / 2);
				var both = a.Intersect(b).First();
				//WriteLine($"{both}: {priority(both)} ({a} | {b})");
				prioritiesA += priority(both);

				groups[index] = input[i];
				index++;
				if (index == 3)
				{
					var groupAll = groups[0].Intersect(groups[1]).Intersect(groups[2]).First();
					//WriteLine($"{groupAll}: {priority(groupAll)}");
					prioritiesB += priority(groupAll);
					index = 0;
				}
			}

			WriteLine($"Part 1: {prioritiesA}");
			WriteLine($"Part 2: {prioritiesB}");
		}
	}
}
