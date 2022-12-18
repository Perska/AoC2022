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
		static void Day18(List<string> input)
		{
			var lava = new Dictionary<(int X, int Y, int Z), int>();
			(int X, int Y, int Z)[] sides = { (1, 0, 0), (0, 1, 0), (0, 0, 1), (-1, 0, 0), (0, -1, 0), (0, 0, -1) };

			var (minX, maxX) = (int.MaxValue, int.MinValue);
			var (minY, maxY) = (int.MaxValue, int.MinValue);
			var (minZ, maxZ) = (int.MaxValue, int.MinValue);

			foreach (var line in input)
			{
				var nums = line.SplitToIntArray(',');
				minX = Math.Min(minX, nums[0]);
				minY = Math.Min(minY, nums[1]);
				minZ = Math.Min(minZ, nums[2]);

				maxX = Math.Max(maxX, nums[0]);
				maxY = Math.Max(maxY, nums[1]);
				maxZ = Math.Max(maxZ, nums[2]);
				lava[(nums[0], nums[1], nums[2])] = 1;
			}
			
			minX--;
			minY--;
			minZ--;
			maxX++;
			maxY++;
			maxZ++;

			var flood = new Queue<(int X, int Y, int Z)>();
			var floodNext = new Queue<(int X, int Y, int Z)>();
			flood.Enqueue((minX, minY, minZ));

			void spread(int X, int Y, int Z)
			{
				if (lava.Read((X, Y, Z)) == 2) return;
				lava[(X, Y, Z)] = 2;
				foreach (var (OX, OY, OZ) in sides)
				{
					if (minX <= X + OX && X + OX <= maxX &&
						minY <= Y + OY && Y + OY <= maxY &&
						minZ <= Z + OZ && Z + OZ <= maxZ && 
						lava.Read((X + OX, Y + OY, Z + OZ)) == 0) floodNext.Enqueue((X + OX, Y + OY, Z + OZ));
				}
			}

			while (flood.Count > 0)
			{
				var (X, Y, Z) = flood.Dequeue();
				spread(X, Y, Z);
				if (flood.Count == 0) (flood, floodNext) = (floodNext, flood);
			}

			int visibleP1 = 0;
			int visibleP2 = 0;

			foreach (var spot in lava)
			{
				if (spot.Value != 1) continue;
				var (X, Y, Z) = spot.Key;
				foreach (var (OX, OY, OZ) in sides)
				{
					if (lava.Read((X + OX, Y + OY, Z + OZ)) != 1) visibleP1++;
					if (lava.Read((X + OX, Y + OY, Z + OZ)) == 2) visibleP2++;
				}
			}

			WriteLine(visibleP1);
			WriteLine(visibleP2);
		}
	}
}
