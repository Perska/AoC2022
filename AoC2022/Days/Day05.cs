using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static System.Console;

namespace AoC2022
{
	partial class Program
	{
		// [UseSRL] // Uncomment if you wanna use SuperReadLine
		[NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		[HasVisual]
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

			frameSpeed = 2000;

			const int BOXSIZE = 24;
			const int BOXGAPX = BOXSIZE + 8;
			const int BOXGAPY = BOXSIZE + 2;

			


			void DrawBox(char c, int x, int y)
			{
				visual.DrawBox(new Rectangle(x - BOXSIZE / 2, y - BOXSIZE / 2, BOXSIZE, BOXSIZE), Color.Gray);
				visual.DrawBox(new Rectangle(x - BOXSIZE / 2 + 1, y - BOXSIZE/2 + 1, BOXSIZE - 2, BOXSIZE - 2), Color.DarkSlateGray);
				visual.Font.Draw(visual.SpriteBatch, c.ToString(), new Vector2(x, y) - visual.Font.GetChar(c).Size.ToVector2(), Color.White, 2);
			}

			var lift = new Stack<char>();

			void Draw(float column, float complete, float liftCount)
			{
				StartDraw(true);
				string disp = $"Advent of Code 2022 Day 5: Supply Stacks";//\n{i}/{input.Count-1} {frameSpeed}";
				visual.Font.Draw(visual.SpriteBatch, disp, new Vector2(Visual.WindowWidth / 2, 16) + visual.Font.CenteredOffsetW(disp) * 2, Color.White, 2);
				/*for (int x = 0; x < stacks.Count; x++)
				{
					for (int y = 0; y < stacks[x].Count; y++)
					{
						DrawBox(stacks[x].ElementAt(stacks[x].Count - y - 1), Visual.WindowWidth / 4 - stacks.Count * 10 + x * BOXGAPX, Visual.WindowHeight - 32 - y * BOXGAPY);
					}
				}*/
				for (int x = 0; x < stacks2.Count; x++)
				{
					int yoff = (int)((Visual.WindowHeight / 2 - stacks2[x].Count * BOXGAPY) * complete);
					for (int y = 0; y < stacks2[x].Count; y++)
					{
						DrawBox(stacks2[x].ElementAt(stacks2[x].Count - y - 1), Visual.WindowWidth / 2 - stacks2.Count * BOXGAPX / 2 + x * BOXGAPX + BOXGAPX / 2, Visual.WindowHeight - 32 - y * BOXGAPY - yoff);
					}
				}
				if (lift.Count > 0)
				{
					//float y = MathHelper.Lerp(stacks2[(int)column].Count, , liftCount);
					float y = liftCount;
					for (int y2 = 0; y2 < lift.Count; y2++)
					{
						DrawBox(lift.ElementAt(y2), Visual.WindowWidth / 2 - stacks2.Count * BOXGAPX / 2 + (int)(column * BOXGAPX) + BOXGAPX / 2, Visual.WindowHeight - 32 - (int)((y + y2) * BOXGAPY));
					}
				}
				StopDraw();
			}


			frameSpeed = 100;
			Draw(0, 0, 0);
			Vsync();
			ReadLine();

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
					//Draw(moveFrom, 0, 0, moveFrom, moveTo);
					//Vsync();
				}
				frameSpeed = 20;
				if (i > 20) frameSpeed = Math.Max(0, 40 - i);
				if (i > input.Count - 20) frameSpeed = Math.Min(20, 20 + (i - input.Count));

				(int from, int to) = (Math.Min(moveFrom, moveTo), Math.Max(moveFrom, moveTo) - Math.Min(moveFrom, moveTo) + 1);
				//goto skip;
				int raise = stacks2.GetRange(from, to).Max(s => s.Count) + 1;
				for (int anim = stacks2[moveFrom].Count*2; anim < raise*2; anim++)
				{
					if (anim % 2 == 0 && frameSpeed == 0) continue;
					if (anim > Visual.WindowHeight) continue;
					Draw(moveFrom, 0, anim / 2f);
					Vsync();
				}
				for (int anim = moveFrom * 4; anim != moveTo * 4; anim += moveFrom < moveTo ? 1 : -1)
				{
					Draw(anim / 4f, 0, raise);
					Vsync();
				}
				for (int anim = raise*2; anim > stacks2[moveTo].Count*2; anim--)
				{
					if (anim % 2 == 0 && frameSpeed == 0) continue;
					if (anim > Visual.WindowHeight) continue;
					Draw(moveTo, 0, anim / 2f);
					Vsync();
				}
				for (int j = 0; j < moveCount; j++)
				{
					stacks2[moveTo].Push(lift.Pop());
				}
				
				Draw(moveFrom, 0, 0);
				Vsync();
			}
			frameSpeed = 20;
			Vsync();
			for (int anim = 0; anim <= 10; anim++)
			{
				Draw(0, anim / 10f, 0);
				Vsync();
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
