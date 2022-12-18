using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using static System.Console;

namespace AoC2022
{
	partial class Program
	{
		// [UseSRL] // Uncomment if you wanna use SuperReadLine
		[NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		[HasVisual]
		static void Day17(List<string> input)
		{
			var map = new Dictionary<(int X, int Y), int>();

			string[][] blocks = {
				new[] {
					"3445"
				},
				new[] {
					" 6 ",
					"666",
					" 6 "
				},
				new[] {
					"777",
					"  7",
					"  7"
				},
				new[] {
					"2",
					"1",
					"1",
					"0"
				},
				new[] {
					"88",
					"88"
				}
			};

			string tape = string.Join("", input);
			int index = 0;
			long repeats = 0;

			int maxY = -1;
			int dispMax = 0;
			int prevMax = 0;

			bool inPlay = false;

			var (blockX, blockY) = (0, 0);

			long nth = 0;
			int block = 0;
			string[] current = blocks[block];

			var revisit = new Dictionary<(int Tape, int Shape), (long Rocks, long Height)>();

			bool checkCollision(int bx, int by)
			{
				bool ok = true;
				for (int y = by; y < by + current.Length; y++)
				{
					for (int x = bx; x < bx + current[y - by].Length; x++)
					{
						int read = map.Read((x, y), -1);
						if (by <= y && y <= by + current.Length - 1 && bx <= x && x <= bx + current[y - by].Length - 1)
						{
							if (current[y - by][x - bx] != ' ' && read != -1)
							{
								ok = false;
							}
						}
					}
				}
				return ok;
			}

			long found1 = 0;
			long found2 = 0;

			frameSpeed = 2000;
			Vsync();

			frameSpeed = 20;
			int gameSpeed = 20;

			visual.Resize(136 * 4, 144 * 4);

			//Texture2D wall = visual.EmbeddedTexture("wall.png", visual.GraphicsDevice);
			Texture2D tile = visual.EmbeddedTexture("block.png", visual.GraphicsDevice);
			Texture2D counter = visual.EmbeddedTexture("numbers.png", visual.GraphicsDevice);
			Texture2D bg = visual.EmbeddedTexture("bg.png", visual.GraphicsDevice);
			SoundEffect stack = visual.EmbeddedSound("stack.wav");
			SoundEffect move = visual.EmbeddedSound("move.wav");

			//Draw();
			//ReadLine();

			void Num(long num, int x, int y)
			{
				const int SIZE = 32;
				Rectangle pos = new Rectangle(x * SIZE, y * SIZE, SIZE, SIZE);
				string draw = num.ToString().PadLeft(5, ' ');
				for (int i = 0; i < draw.Length; i++)
				{
					char c = draw[i];
					int t = c - '0';
					if (c != ' ') visual.SpriteBatch.Draw(counter, pos, new Rectangle(t * 8, 0, 8, 8), Color.White);
					pos.X += SIZE;
				}
			}

			int time = 0;

			void Draw()
			{
				const int SIZE = 32;
				const int SCALE = SIZE / 8;
				int newMin = Math.Max(0, dispMax - Visual.GraphicsHeight / SIZE - 1);

				for (int i = 0; i < gameSpeed; i++)
				{
					StartDraw(false, false, true);
					visual.GraphicsDevice.Clear(Color.White);
					visual.SpriteBatch.Draw(bg, Vector2.Zero, null, Color.White, 0, Vector2.Zero, SCALE, 0, 0);
					//visual.SpriteBatch.Draw(wall, new Vector2(0, 0), new Rectangle(0, 0, 8, Visual.GraphicsHeight / SCALE), Color.White, 0, Vector2.Zero, new Vector2(SCALE, SCALE), 0, 0);
					//visual.SpriteBatch.Draw(wall, new Vector2(8 * SIZE, 0), new Rectangle(0, 0, 8, Visual.GraphicsHeight / SCALE), Color.White, 0, Vector2.Zero, new Vector2(SCALE, SCALE), 0, 0);
					if (prevMax < newMin * 8) prevMax = Math.Min(prevMax + Math.Max(1, 4 - gameSpeed - 1), newMin * 8);
					for (int y = dispMax; y >= Math.Max(0, newMin - 30); y--)
					{
						for (int x = 0; x < 7; x++)
						{
							int read = map.Read((x, y), -1);
							if (inPlay && blockY <= y && y <= blockY + current.Length - 1 && blockX <= x && x <= blockX + current[y - blockY].Length - 1)
							{
								if (current[y - blockY][x - blockX] != ' ')
								{
									read = current[y - blockY][x - blockX] - '0';
									//visual.DrawBox(new Rectangle(x * SIZE, y * SIZE, SIZE, SIZE), Color.White);
									//ok = false;
								}
							}
							if (read != -1)
							{
								int drawY = Visual.GraphicsHeight - (y * SIZE + SIZE) + prevMax * SCALE;
								//visual.DrawBox(new Rectangle(x * SIZE + SIZE, drawY, SIZE, SIZE), Color.White);
								visual.SpriteBatch.Draw(tile, new Vector2(x * SIZE + SIZE * 2, drawY), new Rectangle((read % 3) * 8, (read / 3) * 8, 8, 8), Color.White, 0, Vector2.Zero, SCALE, 0, 0);
							}
						}
					}

					var next = blocks[(block + 1) % blocks.Length];
					for (int y = 0; y < next.Length; y++)
					{
						for (int x = 0; x < next[y].Length; x++)
						{
							if (next[y][x] != ' ')
							{
								int read = next[y][x] - '0';
								int drawY = 15 * SIZE - (y * SIZE);
								if (next.Length == 4) drawY += SIZE;
								visual.SpriteBatch.Draw(tile, new Vector2(12 * SIZE + x * SIZE + (next.Length % 2 == 0 ? SIZE : 0), drawY), new Rectangle((read % 3) * 8, (read / 3) * 8, 8, 8), Color.White, 0, Vector2.Zero, SCALE, 0, 0);
							}
						}
					}

					Num(maxY + 1, 11, 3);
					Num(nth - 1, 11, 7);
					Num(time, 11, 10);
					StopDraw();
					Vsync();
				}
				FrameworkDispatcher.Update();
			}


			while (found1 == 0 || found2 == 0)
			{
				if (!inPlay)
				{
					blockX = 2;
					blockY = (map.Count > 1 ? maxY : -1) + 4;
					dispMax = blockY + blocks[block].Length;
					current = blocks[block];
					inPlay = true;
					nth++;
					if (nth == 2023)
					{
						found1 = maxY + 1;
					}
				}
				Draw();

				int next = blockX;
				if (tape[index] == '<')
				{
					next--;
				}
				else
				{
					next++;
				}
				if (0 <= next && next + current[0].Length <= 7 && checkCollision(next, blockY))
				{
					blockX = next;
					if (gameSpeed > 1) move.Play();
				}
				Draw();

				index = (index + 1) % tape.Length;
				if (index == 0) repeats++;
				next = blockY - 1;
				if (0 <= next && checkCollision(blockX, next))
				{
					blockY = next;
				}
				else
				{
					for (int y = 0; y < current.Length; y++)
					{
						for (int x = 0; x < current[y].Length; x++)
						{
							bool place = current[y][x] != ' ';
							if (place) map[(blockX + x, blockY + y)] = current[y][x] - '0';
							maxY = Math.Max(maxY, blockY + y);
						}
					}
					if (frameSpeed > 3)
					{
						stack.Play();
					}
					block = (block + 1) % blocks.Length;
					inPlay = false;
					if (nth > 4 && gameSpeed > 1) gameSpeed--;
					if (gameSpeed == 1 && (nth % 4 == 0) && frameSpeed > 0) frameSpeed--;

					if (revisit.ContainsKey((index, block)) && found2 == 0)
					{
						var last = revisit[(index, block)];
						long cycle = nth - last.Rocks;
						long adds = maxY + 1 - last.Height;
						long remaining = 1000000000000 - nth - 1;
						long combo = (remaining / (cycle) + 1);
						if (nth + combo * cycle == 1000000000000)
						found2 = maxY + 1 + combo * adds;
					}
					else
					{
						revisit[(index, block)] = (nth, maxY + 1);
					}
				}
				if (nth > 2000)
				{
					frameSpeed = 20;
					gameSpeed = 1;
				}
				//Draw();
				time++;
			}
			frameSpeed = 20;
			gameSpeed = 20;
			for (int i = 0; i < 50; i++)
			{
				Draw();
			}
			WriteLine($"Part 1: {found1}");
			WriteLine($"Part 2: {found2}");

			//wall.Dispose();
			tile.Dispose();
			counter.Dispose();
			bg.Dispose();
		}
	}
}
