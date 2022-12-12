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
		static void Day12(List<string> input)
		{
			int width = input[0].Length;
			int height = input.Count;
			int[,] map = new int[width, height];
			int startX = 0, startY = 0;
			int endX = 0, endY = 0;
			var explore = new Queue<(int X, int Y, int Steps)>();
			var exploreNext = new Queue<(int X, int Y, int Steps)>();
			var best = new Dictionary<(int X, int Y), int>();
			var mov = new (int X, int Y)[] { (1, 0), (0, 1), (-1, 0), (0, -1) };

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					char c = input[y][x];
					if (c == 'S') (startX, startY) = (x, y);
					else if (c == 'E') (endX, endY, map[x, y]) = (x, y, 'z' - 'a');
					else map[x, y] = c - 'a';
				}
			}
			
			best[(0, 0)] = 0;
			explore.Enqueue((startX, startY, 0));
			while (explore.Count > 0)
			{
				(int x, int y, int steps) = explore.Dequeue();
				for (int i = 0; i < 4; i++)
				{
					tryGo(x, y, steps, i);
				}
				if (explore.Count == 0) (explore, exploreNext) = (exploreNext, explore);
			}
			WriteLine(best.Read((endX, endY)));

			best.Clear();
			best[(endX, endY)] = 0;
			explore.Enqueue((endX, endY, 0));
			while (explore.Count > 0)
			{
				(int x, int y, int steps) = explore.Dequeue();
				for (int i = 0; i < 4; i++)
				{
					tryGoBack(x, y, steps, i);
				}
				if (explore.Count == 0)	(explore, exploreNext) = (exploreNext, explore);
			}
			WriteLine(best.Where(item => map[item.Key.X, item.Key.Y] == 0).Min(item => item.Value));

			void tryGo(int x1, int y1, int steps, int dir)
			{
				int x2 = x1 + mov[dir].X;
				int y2 = y1 + mov[dir].Y;
				if (x2 < 0) return;
				if (x2 >= width) return;
				if (y2 < 0) return;
				if (y2 >= height) return;
				int check = best.Read((x2, y2), int.MaxValue);
				if (steps + 1 >= check) return;
				if (map[x1, y1] + 1 >= map[x2, y2])
				{
					best[(x2, y2)] = steps + 1;
					exploreNext.Enqueue((x2, y2, steps + 1));
				}
			}

			void tryGoBack(int x1, int y1, int steps, int dir)
			{
				int x2 = x1 + mov[dir].X;
				int y2 = y1 + mov[dir].Y;
				if (x2 < 0) return;
				if (x2 >= width) return;
				if (y2 < 0) return;
				if (y2 >= height) return;
				int check = best.Read((x2, y2), int.MaxValue);
				if (steps + 1 >= check) return;
				if (map[x1, y1] <= map[x2, y2] + 1)
				{
					best[(x2, y2)] = steps + 1;
					exploreNext.Enqueue((x2, y2, steps + 1));
				}
			}
		}
	}
}
