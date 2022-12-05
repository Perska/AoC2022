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
		static void Day05(List<string> input)
		{
			var init = new List<List<char>>();
			var stacks = new List<Stack<char>>();
			var stacks2 = new List<Stack<char>>();

			int i;
			for (i = 1; i < input[0].Length; i += 4)
			{
				init.Add(new List<char>());
				stacks.Add(new Stack<char>());
				stacks2.Add(new Stack<char>());
			}
			for (i = 0; input[i].IndexOf("[") != -1; i++)
			{
				for (int j = 1; j < input[i].Length; j += 4)
				{
					if (input[i][j] != ' ')
						init[j / 4].Add(input[i][j]);
				}
			}
			i += 2;

			for (int j = 0; j < init.Count; j++)
			{
				for (int k = init[j].Count - 1; k >= 0; k--)
				{
					stacks[j].Push(init[j][k]);
					stacks2[j].Push(init[j][k]);
				}
			}

			var lift = new Stack<char>();
			for (; i < input.Count; i++)
			{
				var cmd = input[i].SplitToStringArray(" ", true);
				int moveCount = int.Parse(cmd[1]);
				int moveFrom = int.Parse(cmd[3]) - 1;
				int moveTo = int.Parse(cmd[5]) - 1;
				for (int j = 0; j < moveCount; j++)
				{
					stacks[moveTo].Push(stacks[moveFrom].Pop());
					lift.Push(stacks2[moveFrom].Pop());
				}
				for (int j = 0; j < moveCount; j++)
				{
					stacks2[moveTo].Push(lift.Pop());
				}

			}

			Write("Part 1: ");
			for (int j = 0; j < stacks.Count; j++)
			{
				Write(stacks[j].Peek());
			}
			WriteLine();

			Write("Part 2: ");
			for (int j = 0; j < stacks.Count; j++)
			{
				Write(stacks2[j].Peek());
			}
			WriteLine();
		}
	}
}
