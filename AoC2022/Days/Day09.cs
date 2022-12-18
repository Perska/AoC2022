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
		static void Day09(List<string> input)
		{
			const int KNOTS = 10;
			(int X, int Y)[] tails = new (int X, int Y)[KNOTS];
			var visited = new Dictionary<(int X, int Y), bool>();
			var visited10 = new Dictionary<(int X, int Y), bool>();

			for (int i = 0; i < input.Count; i++)
			{
				var cmd = input[i].Split(' ');
				int move = int.Parse(cmd[1]);
				for (int j = 0; j < move; j++)
				{
					switch (cmd[0])
					{
						case "U":
							tails[0].Y -= 1;
							break;
						case "R":
							tails[0].X += 1;
							break;
						case "D":
							tails[0].Y += 1;
							break;
						case "L":
							tails[0].X -= 1;
							break;
					}
					for (int k = 1; k < KNOTS; k++)
					{
						if (Math.Abs(tails[k - 1].X - tails[k].X) > 1 || Math.Abs(tails[k - 1].Y - tails[k].Y) > 1)
						{
							tails[k].X += Math.Min(Math.Max(-1, tails[k - 1].X - tails[k].X), 1);
							tails[k].Y += Math.Min(Math.Max(-1, tails[k - 1].Y - tails[k].Y), 1);
						}
					}
					visited[tails[1]] = true;
					visited10[tails[KNOTS - 1]] = true;
				}
			}
			
			WriteLine(visited.Count);
			WriteLine(visited10.Count);
		}
	}
}
