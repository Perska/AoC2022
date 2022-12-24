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
		[Flags]
		enum Blizzard
		{
			None, Up = 1, Down = 2, Left = 4, Right = 8, Blizzard = Up | Down | Left | Right, Wall = 16, Any = Blizzard | Wall
		}

		// [UseSRL] // Uncomment if you wanna use SuperReadLine
		[NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		static void Day24(List<string> input)
		{
			int width = input[0].Length;
			int height = input.Count;
			Blizzard[,] map = new Blizzard[width, height];
			Blizzard[,] nextMap = new Blizzard[width, height];

			bool B(Blizzard check, Blizzard test)
			{
				return (check & test) != 0 ? true : false;
			}

			bool C(int x, int y, Blizzard test)
			{
				if (x < 0 || y < 0) return true;
				if (x >= width || y >= height) return true;
				return B(nextMap[x, y], test);
			}

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					char c = input[y][x];
					Blizzard put = 0;
					switch (c)
					{
						case '#':
							put = Blizzard.Wall;
							break;
						case '^':
							put = Blizzard.Up;
							break;
						case 'v':
							put = Blizzard.Down;
							break;
						case '<':
							put = Blizzard.Left;
							break;
						case '>':
							put = Blizzard.Right;
							break;
					}
					map[x, y] = put;
				}
			}

			var move = new (int x, int y)[] { (1, 0), (0, 1), (-1, 0), (0, -1) };

			var tests = new (Blizzard Direction, int X, int Y)[] { (Blizzard.Up, 0, -1), (Blizzard.Down, 0, 1), (Blizzard.Left, -1, 0), (Blizzard.Right, 1, 0) };

			var explore = new Dictionary<(int X, int Y), bool>();
			var exploreNext = new Dictionary<(int X, int Y), bool>();

			explore[(1, 0)] = true;
			int time = 0;
			int part1 = 0;
			int part2 = 0;
			int state = 0;
			while (state < 3)
			{
				Array.Clear(nextMap, 0, width * height);
				exploreNext.Clear();
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						Blizzard put = map[x, y];
						if (put == Blizzard.Wall)
						{
							nextMap[x, y] = put;
						}
						else if (put != Blizzard.None)
						{
							for (int i = 0; i < 4; i++)
							{
								var (bx, by) = (x - 1, y - 1);
								var test = tests[i];
								if (B(put, test.Direction))
								{
									bx += test.X;
									by += test.Y;
								}
								else
								{
									continue;
								}
								bx = (bx + width - 2) % (width - 2) + 1;
								by = (by + height - 2) % (height - 2) + 1;
								nextMap[bx, by] |= test.Direction;
							}
						}
					}
				}

				foreach (var spot in explore)
				{
					var (ex, ey) = spot.Key;
					if (!C(ex, ey, Blizzard.Any)) exploreNext[(ex, ey)] = true;
					for (int i = 0; i < 4; i++)
					{
						var (nex, ney) = (ex + move[i].x, ey + move[i].y);
						if (!C(nex, ney, Blizzard.Any)) exploreNext[(nex, ney)] = true;
					}
				}
				(map, nextMap) = (nextMap, map);
				(explore, exploreNext) = (exploreNext, explore);
				time++;
				if (explore.Read((width - 2, height - 1)) && state == 0)
				{
					part1 = time;
					explore.Clear();
					explore[(width - 2, height - 1)] = true;
					state++;
				}
				if (explore.Read((1, 0)) && state == 1)
				{
					explore.Clear();
					explore[(1, 0)] = true;
					state++;
				}
				if (explore.Read((width - 2, height - 1)) && state == 2)
				{
					part2 = time;
					state++;
				}
			}
			WriteLine($"Part 1: {part1}");
			WriteLine($"Part 2: {part2}");
		}
	}
}
