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
		static void Day21(List<string> input)
		{
			var numbers = new Dictionary<string, long>();
			var waiting = new List<(string name, string a, string operation, string b)>();

			foreach (var line in input)
			{
				string monkey = line.Substring(0, 4);
				string rest = line.Substring(6);
				if ('0' <= rest[0] && rest[0] <= '9')
				{
					numbers.Add(monkey, long.Parse(rest));
				}
				else
				{
					string[] split = rest.Split(' ');
					waiting.Add((monkey, split[0], split[1], split[2]));
				}
			}


			var numbersBase = new Dictionary<string, long>(numbers);
			numbersBase.Remove("humn");
			var waitingBase = new List<(string name, string a, string operation, string b)>(waiting);
			var root = waitingBase.First(w => w.name == "root");
			waitingBase.Remove(root);

			while (waiting.Count > 0)
			{
				var item = waiting.First(w => numbers.ContainsKey(w.a) && numbers.ContainsKey(w.b));
				waiting.Remove(item);

				switch (item.operation)
				{
					case "+":
						numbers[item.name] = numbers[item.a] + numbers[item.b];
						break;
					case "-":
						numbers[item.name] = numbers[item.a] - numbers[item.b];
						break;
					case "*":
						numbers[item.name] = numbers[item.a] * numbers[item.b];
						break;
					case "/":
						numbers[item.name] = numbers[item.a] / numbers[item.b];
						break;
					default:
						throw new Exception();
				}
			}

			WriteLine($"Part 1: {numbers["root"]}");

			long crack = 0;
			while (true)
			{
				numbers = new Dictionary<string, long>(numbersBase);
				waiting = new List<(string name, string a, string operation, string b)>(waitingBase);
				numbers["humn"] = crack;
				while (waiting.Count > 0)
				{
					var item = waiting.First(w => numbers.ContainsKey(w.a) && numbers.ContainsKey(w.b));
					waiting.Remove(item);

					switch (item.operation)
					{
						case "+":
							numbers[item.name] = numbers[item.a] + numbers[item.b];
							break;
						case "-":
							numbers[item.name] = numbers[item.a] - numbers[item.b];
							break;
						case "*":
							numbers[item.name] = numbers[item.a] * numbers[item.b];
							break;
						case "/":
							numbers[item.name] = numbers[item.a] / numbers[item.b];
							break;
						default:
							throw new Exception();
					}
				}
				if (numbers[root.a] == numbers[root.b]) break;
				long diff = numbers[root.a] - numbers[root.b];
				if (diff < 100)
				{
					crack++;
				}
				else
				{
					crack += diff / 100;
				}
			}
			WriteLine($"Part 2: {crack}");
		}
	}
}
