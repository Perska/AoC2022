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
		static void Day04(List<string> input)
		{
			(int a, int b) intersect((int a, int b) a, (int a, int b) b)
			{
				return (Math.Max(a.a, b.a), Math.Min(a.b, b.b));
			}

			int overlaps = 0, overlaps2 = 0;
			foreach (var line in input)
			{
				var ranges = line.SplitToIntArray(',', '-');
				(int a, int b) = intersect((ranges[0], ranges[1]), (ranges[2], ranges[3]));
				if ((a == ranges[0] && b == ranges[1]) || (a == ranges[2] && b == ranges[3])) overlaps++;
				if (a <= b) overlaps2++;
			}

			WriteLine($"Part 1: {overlaps} redundant ranges");
			WriteLine($"Part 2: {overlaps2} overlapping ranges");
		}
	}
}
