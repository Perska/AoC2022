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
		static void Day23(List<string> input)
		{
			var map = new Dictionary<(int X, int Y), bool>();

			for (int y = 0; y < input.Count; y++)
			{
				for (int x = 0; x < input[y].Length; x++)
				{
					if (input[y][x] == '#')
					{
						map[(x, y)] = true;
					}
				}
			}


			bool simulateRound(int round)
			{
				var occupy = new Dictionary<(int X, int Y), int>();
				var propose = new List<((int X, int Y) Old, (int X, int Y) New)>();

				foreach (var (X, Y) in map.Keys)
				{
					bool consider = false;
					for (int x = -1; x <= 1; x++)
					{
						for (int y = -1; y <= 1; y++)
						{
							if (x == 0 && y == 0) continue;
							if (map.Read((X + x, Y + y))) consider = true;
						}
					}
					if (consider)
					{
						bool proposed = false;
						for (int i = 0; i < 4; i++)
						{
							if (((round + i) % 4) == 0 && !(map.Read((X, Y - 1)) || map.Read((X - 1, Y - 1)) || map.Read((X + 1, Y - 1))))
							{
								propose.Add(((X, Y), (X, Y - 1)));
								proposed = true;
							}
							if (((round + i) % 4) == 1 && !(map.Read((X, Y + 1)) || map.Read((X - 1, Y + 1)) || map.Read((X + 1, Y + 1))))
							{
								propose.Add(((X, Y), (X, Y + 1)));
								proposed = true;
							}
							if (((round + i) % 4) == 2 && !(map.Read((X - 1, Y)) || map.Read((X - 1, Y - 1)) || map.Read((X - 1, Y + 1))))
							{
								propose.Add(((X, Y), (X - 1, Y)));
								proposed = true;
							}
							if (((round + i) % 4) == 3 && !(map.Read((X + 1, Y)) || map.Read((X + 1, Y - 1)) || map.Read((X + 1, Y + 1))))
							{
								propose.Add(((X, Y), (X + 1, Y)));
								proposed = true;
							}
							if (proposed) break;
						}
					}
				}

				foreach (var (Old, New) in propose)
				{
					occupy[(New.X, New.Y)] = occupy.Read((New.X, New.Y)) + 1;
				}

				foreach (var (Old, New) in propose)
				{
					if (occupy.Read((New.X, New.Y)) == 1)
					{
						map.Remove(Old);
						map[New] = true;
					}
				}
				if (propose.Count == 0)
				{
					return false;
				}

				propose.Clear();
				occupy.Clear();

				return true;
			}

			int r = 0;

			for (; r < 10; r++)
			{
				simulateRound(r);
			}

			int mix, miy, max, may;
			mix = map.Min(c => c.Key.X);
			max = map.Max(c => c.Key.X);
			miy = map.Min(c => c.Key.Y);
			may = map.Max(c => c.Key.Y);

			WriteLine($"Part 1: {(max - mix + 1) * (may - miy + 1) - map.Count}");

			while (simulateRound(r))
			{
				r++;
			}

			WriteLine($"Part 2: {r + 1}");
		}
	}
}
