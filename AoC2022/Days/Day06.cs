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
		// [NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		static void Day06(List<string> input)
		{
			string str = string.Join("", input);

			int find(int chars)
			{
				for (int i = 0; i < str.Length - chars; i++)
				{
					string check = str.Substring(i, chars);
					if (check.Distinct().Count() == chars)
					{
						return i + chars;
					}
				}
				return 0;
			}

			WriteLine($"Part 1: {find(4)}");
			WriteLine($"Part 2: {find(14)}");
		}
	}
}
