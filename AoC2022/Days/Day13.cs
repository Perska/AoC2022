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
		static void Day13(List<string> input)
		{
			var lists = new List<(List<object>, List<object>)>();
			var allLists = new List<List<object>>();
			
			List<object> parseList(string text)
			{
				var a = new List<object>();
				var stack = new Stack<List<object>>();
				a.Add(null);
				stack.Push(a);
				int parseNum = 0;

				for (int i = 1; i < text.Length; i++)
				{
					char c = text[i];
					if ('0' <= c && c <= '9')
					{
						a[a.Count - 1] = parseNum = parseNum * 10 + (c - '0');
					}
					else if (c == '[')
					{
						var b = new List<object>();
						b.Add(null);
						a[a.Count - 1] = b;
						stack.Push(a);
						a = b;
					}
					else if (c == ']')
					{
						a.Remove(null);
						a = stack.Pop();
					}
					else if (c == ',')
					{
						parseNum = 0;
						a.Add(null);
					}
				}
				return a;
			}

			for (int i = 0; i < input.Count; i += 3)
			{
				(var a, var b) = (parseList(input[i]), parseList(input[i + 1]));
				lists.Add((a, b));
				allLists.Add(a);
				allLists.Add(b);
			}
			var keyA = new List<object> { new List<object> { 2 } };
			var keyB = new List<object> { new List<object> { 6 } };
			allLists.Add(keyA);
			allLists.Add(keyB);

			int compareItems(object a, object b)
			{
				if (a is int && b is int)
				{
					(int numA, int numB) = ((int)a, (int)b);

					if (numA < numB) return 1;
					if (numA > numB) return -1;
				}
				else if (a is List<object> && b is List<object>)
				{
					int index = 0;
					(var listA, var listB) = (a as List<object>, b as List<object>);
					while (index < listA.Count)
					{
						if (index >= listB.Count) return -1;
						int ret = compareItems(listA[index], listB[index]);
						if (ret != 0) return ret;
						index++;
					}
					if (listA.Count < listB.Count) return 1;
				}
				else
				{
					object itemA, itemB;
					if (a is int)
					{
						(itemA, itemB) = (new List<object>() { a }, b as List<object>);
					}
					else
					{
						(itemA, itemB) = (a as List<object>, new List<object>() { b });
					}
					return compareItems(itemA, itemB);
				}
				return 0;
			}

			int sum = 0;
			for (int i = 0; i < lists.Count; i++)
			{
				int ok = compareItems(lists[i].Item1, lists[i].Item2);
				if (ok == 1) sum += i + 1;
			}
			WriteLine($"Part 1: {sum}");

			allLists.Sort((a, b) => -compareItems(a, b));
			int posA = allLists.IndexOf(keyA) + 1;
			int posB = allLists.IndexOf(keyB) + 1;
			WriteLine($"Part 2: {posA * posB}");
		}
	}
}
