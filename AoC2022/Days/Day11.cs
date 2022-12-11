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
		static void Day11(List<string> input)
		{
			var monkeys = new List<(List<long> items, string operation, int test, int truthy, int falsey)>();
			var inspected = new List<long>();

			var monkeys2 = new List<(List<long> items, string operation, int test, int truthy, int falsey)>();
			var inspected2 = new List<long>();
			long commonDivisor = 1;

			for (int i = 0; i < input.Count; i++)
			{
				if (input[i].StartsWith("Monkey"))
				{
					i++;
					var s = input[i].SplitToStringArray(": ", false);
					var items = s[1].SplitToLongArray(", ").ToList();
					i++;
					string operation = input[i].SplitToStringArray(" = ", false)[1];
					i++;
					int test = int.Parse(input[i].SplitToStringArray(" ", true)[3]);
					i++;
					int truthy = int.Parse(input[i].SplitToStringArray(" ", true)[5]);
					i++;
					int falsey = int.Parse(input[i].SplitToStringArray(" ", true)[5]);
					monkeys.Add((items, operation, test, truthy, falsey));
					inspected.Add(0);
					monkeys2.Add((items.ToList(), operation, test, truthy, falsey));
					inspected2.Add(0);
					commonDivisor *= test;
				}
			}

			void monkeyTurn(int id, bool relief)
			{
				var monkey = monkeys[id];
				while (monkey.items.Count > 0)
				{
					inspected[id]++;
					long worry = monkey.items[0];
					monkey.items.RemoveAt(0);
					string[] cmd = monkey.operation.Split(' ');
					long use = cmd[2] == "old" ? worry : long.Parse(cmd[2]);
					switch (cmd[1])
					{
						case "+":
							worry += use;
							break;
						case "*":
							worry *= use;
							break;
					}
					if (relief)
						worry = worry / 3;
					else
						worry = worry % commonDivisor;
					if (worry % monkey.test == 0)
						monkeys[monkey.truthy].items.Add(worry);
					else
						monkeys[monkey.falsey].items.Add(worry);
				}
			}

			for (int i = 0; i < 20; i++)
			{
				for (int j = 0; j < monkeys.Count; j++)
				{
					monkeyTurn(j, true);
				}
			}
			var top = inspected.OrderByDescending(x => x).ToList();
			long monkeyBusiness = top[0] * top[1];
			WriteLine($"Part 1: {monkeyBusiness}");

			monkeys = monkeys2;
			inspected = inspected2;

			for (int i = 0; i < 10000; i++)
			{
				for (int j = 0; j < monkeys.Count; j++)
				{
					monkeyTurn(j, false);
				}
			}
			top = inspected.OrderByDescending(x => x).ToList();
			long realMonkeyBusiness = top[0] * top[1];
			WriteLine($"Part 2: {realMonkeyBusiness}");
		}
	}
}
