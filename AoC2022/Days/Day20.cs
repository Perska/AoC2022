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
		static void Day20(List<string> input)
		{
			var list = new LinkedList<int>();
			var list2 = new LinkedList<long>();
			var nodes = new List<LinkedListNode<int>>();
			var nodes2 = new List<LinkedListNode<long>>();

			const long key = 811589153;

			for (int i = 0; i < input.Count; i++)
			{
				nodes.Add(list.AddLast(int.Parse(input[i])));
				nodes2.Add(list2.AddLast(long.Parse(input[i]) * key));
			}

			for (int i = 0; i < nodes.Count; i++)
			{
				var node = nodes[i];
				int moves = node.Value;
				if (moves < 0)
				{
					for (int j = 0; j < -moves; j++)
					{
						var swap = node.Previous ?? node.List.Last;
						list.Remove(node);
						list.AddBefore(swap, node);
					}
				}
				else if (moves > 0)
				{
					for (int j = 0; j < moves; j++)
					{
						var swap = node.Next ?? node.List.First;
						list.Remove(node);
						list.AddAfter(swap, node);
					}
				}
			}

			var search = nodes.First(node => node.Value == 0);
			int nums = 0;
			for (int i = 0; i <= 3000; i++)
			{
				if (i == 1000)
				{
					nums += search.Value;
				}
				else if (i == 2000)
				{
					nums += search.Value;
				}
				else if (i == 3000)
				{
					nums += search.Value;
					break;
				}
				search = search.Next ?? search.List.First;
			}

			WriteLine($"Part 1: {nums}");

			for (int r = 0; r < 10; r++)
			{
				for (int i = 0; i < nodes2.Count; i++)
				{
					var node = nodes2[i];
					long moves = node.Value % (list2.Count - 1);
					if (moves < 0)
					{
						for (int j = 0; j < -moves; j++)
						{
							var swap = node.Previous ?? node.List.Last;
							list2.Remove(node);
							list2.AddBefore(swap, node);
						}
					}
					else if (moves > 0)
					{
						for (int j = 0; j < moves; j++)
						{
							var swap = node.Next ?? node.List.First;
							list2.Remove(node);
							list2.AddAfter(swap, node);
						}
					}
				}
			}

			var search2 = nodes2.First(node => node.Value == 0);
			long nums2 = 0;
			for (int i = 0; i <= 3000; i++)
			{
				if (i == 1000)
				{
					nums2 += search2.Value;
				}
				else if (i == 2000)
				{
					nums2 += search2.Value;
				}
				else if (i == 3000)
				{
					nums2 += search2.Value;
					break;
				}
				search2 = search2.Next ?? search2.List.First;
			}

			WriteLine($"Part 2: {nums2}");
		}
	}
}
