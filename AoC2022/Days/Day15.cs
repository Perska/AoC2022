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
		struct BeaconLine
		{
			public int X1;
			public int X2;

			public BeaconLine(int x1, int x2)
			{
				X1 = x1;
				X2 = x2;
			}

			public bool Valid
			{
				get
				{
					return (X1 <= X2);
				}
			}

			public int Width
			{
				get
				{
					return X2 - X1 + 1;
				}
			}

			public override string ToString()
			{
				return $"{X1}..{X2}";
			}

			public static BeaconLine Intersection(BeaconLine a, BeaconLine b)
			{
				return new BeaconLine(Math.Max(a.X1, b.X1), Math.Min(a.X2, b.X2));
			}

			public static void Abjunction(List<BeaconLine> lines, BeaconLine a, BeaconLine b)
			{
				BeaconLine intersection = Intersection(a, b);
				if (intersection.Valid)
				{
					//List<BeaconLine> lines = new List<BeaconLine>();
					if (intersection.X1 > a.X1)
					{
						lines.Add(new BeaconLine(a.X1, intersection.X1 - 1));
					}
					if (intersection.X2 < a.X2)
					{
						lines.Add(new BeaconLine(intersection.X2 + 1, a.X2));
					}
					//return lines.ToArray();
				}
				else
				{
					lines.Add(a);
					//return new BeaconLine[] { a };
				}
			}
		}

		// [UseSRL] // Uncomment if you wanna use SuperReadLine
		[NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		static void Day15(List<string> input)
		{
			var beacons = new List<(int X, int Y, int X2, int Y2, int Size)>();
			var beaconMap = new Dictionary<(int X, int Y), bool>();

			Regex coords = new Regex(@"x=(-?\d+), y=(-?\d+)", RegexOptions.Compiled);

			int maxX = 0;

			foreach (var line in input)
			{
				var match = coords.Matches(line);

				(int x, int y) = (int.Parse(match[0].Groups[1].Value), int.Parse(match[0].Groups[2].Value));
				(int x2, int y2) = (int.Parse(match[1].Groups[1].Value), int.Parse(match[1].Groups[2].Value));
				
				int size = Math.Abs(x - x2) + Math.Abs(y - y2);
				maxX = Math.Max(Math.Max(maxX, x + size), x2);

				beacons.Add((x, y, x2, y2, size));
				beaconMap[(x2, y2)] = true;
			}

			bool test = maxX < 100;
			int onRow = test ? 10 : 2000000;
			int scanMax = test ? 20 : 4000000;

			int rowBlocked = 0;
			{
				int y = onRow;
				var lines = new List<BeaconLine>();
				var lines2 = new List<BeaconLine>();
				for (int i = 0; i < beacons.Count; i++)
				{
					var beacon = beacons[i];
					int distance = Math.Abs(y - beacon.Y);
					if (distance <= beacon.Size)
					{
						int size = beacon.Size - distance;
						BeaconLine newLine = new BeaconLine(beacon.X - size, beacon.X + size);
						for (int j = 0; j < lines.Count; j++)
						{
							BeaconLine.Abjunction(lines2, lines[j], newLine);
						}
						lines2.Add(newLine);
						(lines, lines2) = (lines2, lines);
						lines2.Clear();
					}
				}
				rowBlocked = lines.Sum(line => line.Width) - beaconMap.Count(line => line.Key.Y == y);
			}
			WriteLine($"Part 1: {rowBlocked}");

			long tuning = 0;
			for (int y = 0; y <= scanMax; y++)
			{
				var lines = new List<BeaconLine>();
				for (int i = 0; i < beacons.Count; i++)
				{
					var beacon = beacons[i];
					int distance = Math.Abs(y - beacon.Y);
					if (distance <= beacon.Size)
					{
						int size = beacon.Size - distance;
						BeaconLine newLine = new BeaconLine(beacon.X - size, beacon.X + size);
						lines.Add(newLine);
					}
				}

				bool first = false;
				int prevX = int.MinValue;
				foreach (var line in lines.OrderBy(line => line.X1))
				{
					if (first && line.X1 > prevX && line.X1 - prevX != 1)
					{
						int x = prevX + 1;
						if (0 <= x && x <= scanMax && 0 <= y && y <= scanMax)
						{
							tuning = x * 4000000L + y;
						}
					}
					first = true;
					prevX = Math.Max(prevX, line.X2);
				}

				if (tuning != 0) break;
			}
			WriteLine($"Part 2: {tuning}");
		}
	}
}
