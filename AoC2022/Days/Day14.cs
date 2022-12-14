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
		static void Day14(List<string> input)
		{
			var map = new Dictionary<(int X, int Y), int>();
			int spawnX = 500;
			int spawnY = 0;

			Regex path = new Regex(@"(?:(\d+),(\d+)(?: ->)?)", RegexOptions.Compiled);

			foreach (var line in input)
			{
				var matches = path.Matches(line);
				int px = 0, py = 0;
				for (int i = 0; i < matches.Count; i++)
				{
					int x = int.Parse(matches[i].Groups[1].Value);
					int y = int.Parse(matches[i].Groups[2].Value);
					if (i > 0)
					{
						if (x == px)
						{
							bool mag = py - y < 0;
							for (int j = py; mag ? j <= y : j >= y; j += mag ? 1 : -1)
							{
								map[(x, j)] = 1;
							}
						}
						else
						{
							bool mag = px - x < 0;
							for (int j = px; mag ? j <= x : j >= x; j += mag ? 1 : -1)
							{
								map[(j, y)] = 1;
							}
						}
					}
					(px, py) = (x, y);
				}
			}

			int sands = 0;
			int maxLevel = map.Max(i => i.Key.Y);
			bool abyss = false;

			while (!abyss)
			{
				int sx = spawnX;
				int sy = spawnY;

				while (!abyss)
				{
					if (map.Read((sx, sy + 1)) == 0)
					{
						sy++;
					}
					else if (map.Read((sx - 1, sy + 1)) == 0)
					{
						sx--;
						sy++;
					}
					else if (map.Read((sx + 1, sy + 1)) == 0)
					{
						sx++;
						sy++;
					}
					else
					{
						break;
					}
					if (sy >= maxLevel) abyss = true;
				}

				if (!abyss)
				{
					map[(sx, sy)] = 2;
					sands++;
				}
			}
			WriteLine($"Part 1: {sands}");

			bool blocked = false;

			while (!blocked)
			{
				int sx = spawnX;
				int sy = spawnY;

				while (true)
				{
					if (map.Read((sx, sy + 1)) == 0)
					{
						sy++;
					}
					else if (map.Read((sx - 1, sy + 1)) == 0)
					{
						sx--;
						sy++;
					}
					else if (map.Read((sx + 1, sy + 1)) == 0)
					{
						sx++;
						sy++;
					}
					else
					{
						if (sx == spawnX && sy == spawnY)
						{
							blocked = true;
						}
						break;
					}
					if (sy >= maxLevel + 1) break;
				}
				
				map[(sx, sy)] = 2;
				sands++;
			}
			WriteLine($"Part 2: {sands}");
		}
	}
}
