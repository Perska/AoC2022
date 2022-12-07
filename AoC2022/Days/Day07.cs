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
		static void Day07(List<string> input)
		{
			Stack<string> cd = new Stack<string>();
			var files = new Dictionary<string, List<(long size, string name, bool folder)>>();
			var dirSize = new Dictionary<string, long>();

			for (int i = 0; i < input.Count; i++)
			{
				string str = input[i];
				if (str.StartsWith("$"))
				{
					string[] cmd = str.Split(' ');
					if (cmd[1] == "cd")
					{
						if (cmd[2] == "/") cd.Clear();
						else if (cmd[2] == "..") cd.Pop();
						else cd.Push(cmd[2]);
					}
					else if (cmd[1] == "ls")
					{
						string path = string.Join("/", cd.Reverse());
						dirSize[path] = 0;
						files[path] = new List<(long size, string name, bool folder)>();
						for (i = i + 1; i < input.Count && !input[i].StartsWith("$"); i++)
						{
							str = input[i];
							cmd = str.Split(' ');
							bool fold = cmd[0] == "dir";
							long size = fold ? 0 : long.Parse(cmd[0]);
							files[path].Add((size, cmd[1], fold));
							dirSize[path] += size;
						}
						i--;
					}
				}
			}

			long total = 0;
			foreach (var folder in files.OrderByDescending(x => x.Key.Count(y => y == '/') - (x.Key.Length == 0 ? 1 : 0))) // Subtract if path length is 0 (cheap hack), otherwise "" and "a" would be the same level
			{
				long size = dirSize[folder.Key];
				foreach (var file in folder.Value)
				{
					if (file.folder)
					{
						long add = dirSize.Read(folder.Key != "" ? $"{folder.Key}/{file.name}" : file.name);
						size += add;
						dirSize[folder.Key] += add;
					}
				}
				//WriteLine($"/{folder.Key}: {size}");
				if (size <= 100000)
				{
					total += size;
				}
			}

			const long max = 70000000;
			const long required = 30000000;
			long takenSpace = dirSize[""];
			long unused = max - takenSpace;
			long needFree = required - unused;

			var deleteThis = dirSize.Where(x => x.Value >= needFree).Min(x => x.Value);
			WriteLine($"Part 1: {total}");
			WriteLine($"Part 2: {deleteThis}");
		}
	}
}
