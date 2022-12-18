using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static System.Console;

namespace AoC2022
{
	partial class Program
	{
		// [UseSRL] // Uncomment if you wanna use SuperReadLine
		[NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		static void Day10(List<string> input)
		{
			int cycles = 0;
			int x = 1;
			int sum = 0;

			void cycle(int val)
			{
				for (int i = 0; i < val; i++)
				{
					int pos = cycles % 40;
					Write((pos == x) || (pos == x - 1) || (pos == x + 1) ? "#" : " ");
					if (cycles % 40 == 39)
					{
						WriteLine();
					}
					cycles++;
					if ((cycles + 20) % 40 == 0)
					{
						sum += cycles * x;
					}
				}
			}

			for (int i = 0; i < input.Count; i++)
			{
				var cmd = input[i].Split(' ');
				var num = cmd.Length > 1 ? int.Parse(cmd[1]) : 0;
				if (cmd[0] == "noop")
				{
					cycle(1);
				}
				else if (cmd[0] == "addx")
				{
					cycle(2);
					x = x + num;
				}
			}
			WriteLine($"Part 1: {sum}\nPart 2 shown above.");
		}
	}
}
