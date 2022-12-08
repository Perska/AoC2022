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
		static void Day08(List<string> input)
		{
			int width = input[0].Length;
			int height = input.Count;
			int[,] map = new int[width, height];

			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					map[j, i] = input[i][j] - '0';
				}
			}

			var move = new (int x, int y)[] { (1, 0), (0, 1), (-1, 0), (0, -1) };
			long visible = width * 2 + (height - 2) * 2;
			long hiScore = 0;
			for (int i = 1; i < height - 1; i++)
			{
				for (int j = 1; j < width - 1; j++)
				{
					bool OK = false;
					int y = map[j, i];
					long[] score = new long[4];
					for (int k = 0; k < 4; k++)
					{
						bool dirOK = true;
						int mx = j, my = i;
						int moved = 0;
						mx += move[k].x;
						my += move[k].y;
						while (mx != -1 && mx != width && my != -1 && my != height)
						{
							moved++;
							if (y <= map[mx, my])
							{
								dirOK = false;
								break;
							}
							mx += move[k].x;
							my += move[k].y;
						}
						if (dirOK) OK = true;
						score[k] = moved;
					}
					if (OK) visible++;
					hiScore = Math.Max(hiScore, score[0] * score[1] * score[2] * score[3]);
				}
			}

			WriteLine($"Part 1: {visible}");
			WriteLine($"Part 2: {hiScore}");
		}
	}
}
