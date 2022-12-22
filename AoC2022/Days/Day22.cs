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
		// [NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		static void Day22(List<string> input)
		{
			var map = new Dictionary<(int X, int Y), char>();
			int index;
			for (index = 0; input[index] != ""; index++)
			{
				for (int x = 0; x < input[index].Length; x++)
				{
					char read;
					if ((read = input[index][x]) != ' ') map[(x, index)] = read;
				}
			}
			index++;

			int maxX = map.Max(c => c.Key.X) + 1;
			int maxY = map.Max(c => c.Key.Y) + 1;

			string _path = string.Join("", input.Skip(index));
			Regex regex = new Regex(@"(\d+|[LR])", RegexOptions.Compiled);

			var parsedpath = regex.Matches(_path);
			string[] path = new string[parsedpath.Count];
			for (int i = 0; i < path.Length; i++)
			{
				path[i] = parsedpath[i].Groups[1].Value;
			}

			;

			var move = new (int x, int y)[] { (1, 0), (0, 1), (-1, 0), (0, -1) };
			/*var moveBack = new Dictionary<(int x, int y), int>() {
				{ (1, 0), 0 },
				{ (0, 1), 0 },
				{ (-1, 0), 0 },
				{ (0, -1), 0 }
			};*/
			var moveChar = new[] { '>', 'v', '<', '^' };
			var rotChar = new[] { 'U', 'R', 'D', 'L' };

			{
				int x = map.Keys.First().X, y = 0;
				int direction = 0;

				var visits = new Dictionary<(int x, int y), char>();

				for (int i = 0; i < path.Length; i++)
				{
					int steps = 0;
					var m = move[direction];
					if (path[i] == "L")
					{
						direction = (direction + 3) % 4;
					}
					else if (path[i] == "R")
					{
						direction = (direction + 1) % 4;
					}
					else
					{
						steps = int.Parse(path[i]);
					}
					for (int j = 0; j < steps; j++)
					{
						int nx = x, ny = y;
						do
						{
							nx = (nx + m.x + maxX) % maxX;
							ny = (ny + m.y + maxY) % maxY;
						} while (map.Read((nx, ny)) == '\0');

						if (map.Read((nx, ny)) != '#')
						{
							x = nx;
							y = ny;
							visits[(x, y)] = moveChar[direction];
						}
						else
						{
							break;
						}
					}
				}

				WriteLine($"Part 1: {(y + 1) * 1000 + (x + 1) * 4 + direction}");
			}

			int cubeSize = Math.Abs(maxX - maxY);

			int[] cube = new int[16];
			(int f, int r)[,] connects = new (int Face, int Rotation)[7, 4];

			int face = 0;
			for (int sy = 0; sy < maxY; sy += cubeSize)
			{
				for (int sx = 0; sx < maxX; sx += cubeSize)
				{
					if (map.Read((sx, sy)) != '\0')
					{
						face++;
						cube[sx / cubeSize + (sy / cubeSize) * 4] = face;
					}
				}
			}

			for (int sy = 0; sy < 4; sy++)
			{
				for (int sx = 0; sx < 4; sx++)
				{
					Write(cube[sx + sy * 4]);
				}
				WriteLine();
			}

			for (int sy = 0; sy < 4; sy++)
			{
				for (int sx = 0; sx < 4; sx++)
				{
					face = cube[sx + sy * 4];
					if (face != 0)
					{
						for (int i = 0; i < 4; i++)
						{
							var m = move[i];
							int nx = (sx + m.x + 4) % 4;
							int ny = (sy + m.y + 4) % 4;
							int target = cube[nx + ny * 4];
							if (target != 0)
							{
								//WriteLine($"Face {face}'s {moveChar[i]} connects to face {target}");
								connects[face, i] = (target, 0);
							}
						}
					}
				}
			}

			var find = new (int d, (int f, int r)[] o)[] {
				(0, new[] { (1, -1), (3, 1) }),
				(1, new[] { (0, 1), (2, -1) }),
				(2, new[] { (1, 1), (3, -1) }),
				(3, new[] { (0, -1), (2, 1) }),
			};

			for (int _ = 0; _ < 3; _++)
			{
				for (int i = 1; i <= 6; i++)
				{
					foreach (var (d, o) in find)
					{
						var fc = connects[i, d];
						if (fc.f == 0) continue;
						face = fc.f;
						foreach (var (f, r) in o)
						{
							if (connects[i, f].f != 0) continue;
							var ta = connects[face, (f + fc.r) % 4];
							for (int s = 0; s < 4; s++)
							{
								if (connects[i, s].f == ta.f) goto skip;
							}
							if (ta.f != 0 && i != ta.f)
							{
								int rot = (fc.r + ta.r + r + 4) % 4;
								//WriteLine($"Face {i}'s {moveChar[f]} connects to face {ta.f} via {face}, through {moveChar[d]}");
								connects[i, f] = (ta.f, rot);
								//WriteLine();
							}
						skip:;
						}
					}
				}
				//WriteLine();
			}
			
			/*ReadLine();
			Clear();

			int[] cube2 = new int[64];
			for (int sy = 0; sy < 4; sy++)
			{
				for (int sx = 0; sx < 4; sx++)
				{
					face = cube[sx + sy * 4];
					if (face == 0) continue;
					SetCursorPosition(sx * 8 + 3, sy * 6);
					Write($"{moveChar[(connects[face, 3].r + 3) % 4]}");
					SetCursorPosition(sx * 8 + 3, sy * 6 + 1);
					Write($"{connects[face, 3].f}");
					SetCursorPosition(sx * 8, sy * 6 + 2);
					Write($"{moveChar[(connects[face, 2].r + 2) % 4]}{connects[face, 2].f}[{face}]{connects[face, 0].f}{moveChar[(connects[face, 0].r) % 4]}");
					SetCursorPosition(sx * 8 + 3, sy * 6 + 3);
					Write($"{connects[face, 1].f}");
					SetCursorPosition(sx * 8 + 3, sy * 6 + 4);
					Write($"{moveChar[(connects[face, 1].r + 1) % 4]}");
				}
				//WriteLine();
			}
			ReadLine();
			Clear();*/
			{
				int x = map.Keys.First().X, y = 0;
				int direction = 0;

				var visits = new Dictionary<(int x, int y), char>();
				visits[(x, y)] = moveChar[direction];

				for (int i = 0; i < path.Length; i++)
				//while (true)
				{
					int steps = 0;
					//var key = ReadKey(true);
					int cellX = x / cubeSize;
					int cellY = y / cubeSize;
					face = cube[cellX + cellY * 4];

					/*if (key.Key == ConsoleKey.LeftArrow)
					{
						direction = (direction + 3) % 4;
					}
					else if (key.Key == ConsoleKey.RightArrow)
					{
						direction = (direction + 1) % 4;
					}
					else if (key.Key == ConsoleKey.UpArrow)
					{
						steps = 1;
					}/**/

					if (path[i] == "L")
					{
						direction = (direction + 3) % 4;
					}
					else if (path[i] == "R")
					{
						direction = (direction + 1) % 4;
					}
					else
					{
						steps = int.Parse(path[i]);
					}/**/

					for (int j = 0; j < steps; j++)
					{
						var m = move[direction];
						int nx = x, ny = y;
						int nextdirection = direction;
						//do
						{
							nx = nx + m.x;
							ny = ny + m.y;
							int ncx = (int)Math.Floor(nx / (float)cubeSize);
							int ncy = (int)Math.Floor(ny / (float)cubeSize);
							int mindex = Array.IndexOf(move, (ncx - cellX, ncy - cellY));
							if (mindex != -1)
							{
								var (f, r) = connects[face, mindex];
								int cindex = Array.IndexOf(cube, f);
								int cx = cindex % 4;
								int cy = cindex / 4;
								int ox = (nx + cubeSize) % cubeSize - cubeSize / 2;
								int oy = (ny + cubeSize) % cubeSize - cubeSize / 2;
								Title = $"{nx}, {ny} | {ox}, {oy}";
								switch (r)
								{
									case 0:
										break;
									case 1:
										nextdirection = (direction + 1) % 4;
										(ox, oy) = (-oy - 1, ox);
										break;
									case 2:
										nextdirection = (direction + 2) % 4;
										(ox, oy) = (-ox - 1, -oy - 1);
										break;
									case 3:
										nextdirection = (direction + 3) % 4;
										(ox, oy) = (oy, -ox - 1);
										break;
								}
								ox += cubeSize / 2;
								oy += cubeSize / 2;
								nx = (cx * cubeSize + ox);
								ny = (cy * cubeSize + oy);
							}
						}

						if (map.Read((nx, ny)) != '#')
						{
							x = nx;
							y = ny;
							direction = nextdirection;
							visits[(x, y)] = moveChar[direction];
						}
						else
						{
							break;
						}
					}
					visits[(x, y)] = moveChar[direction];
					/*SetCursorPosition(0, 0);
					for (int sy = 0; sy < maxY; sy++)
					{
						for (int sx = 0; sx < maxX; sx++)
						{
							Write(visits.Read((sx, sy), map.Read((sx, sy))));
						}
						WriteLine();
					}
					WriteLine($"X/Y: {x}, {y} ({cellX}, {cellY}) \nCurrently on face {face}  ");
					SetCursorPosition(x, y);
					Write(rotChar[(direction + 1) % 4]);*/
				}


				/*Clear();
				SetCursorPosition(0, 0);
				for (int sy = 0; sy < maxY; sy++)
				{
					for (int sx = 0; sx < maxX; sx++)
					{
						Write(visits.Read((sx, sy), map.Read((sx, sy))));
					}
					WriteLine();
				}*/
				WriteLine($"Part 2: {(y + 1) * 1000 + (x + 1) * 4 + direction}");
			}


		}
	}
}
