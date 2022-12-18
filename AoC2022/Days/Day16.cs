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
		static void Day16(List<string> input)
		{
			var valves = new Dictionary<string, (int Rate, string[] Leads)>();
			var valves2 = new Dictionary<int, (int Rate, (int Destination, int Cost)[] Leads)>();

			foreach (var line in input)
			{
				var split = line.SplitToStringArray(new char[] { ' ', ',', ';', '=' }, true);
				string valve = split[1];
				int rate = int.Parse(split[5]);
				int count = split.Length - 10;
				string[] leads = new string[count];
				Array.Copy(split, 10, leads, 0, count);
				valves[valve] = (rate, leads);
			}

			int maxRate = valves.Sum(v => v.Value.Rate);
			int maxValves = valves.Count(v => v.Value.Rate > 0);
			
			foreach (var valve in valves.Where(v => v.Value.Rate > 0 || v.Key == "AA"))
			{
				var destinations = new List<(int Destination, int Cost)>();
				
				foreach (var dest in valves.Where(v => v.Value.Rate > 0 && v.Key != valve.Key))
				{
					var visited = new Dictionary<string, int>();
					visited[valve.Key] = 0;
					
					var find = new Queue<(string Node, int Cost)>();
					var findNext = new Queue<(string Node, int Cost)>();
					find.Enqueue((valve.Key, 0));
					while (find.Count > 0)
					{
						(string node, int cost) = find.Dequeue();
						foreach (var lead in valves[node].Leads)
						{
							tryGo(node, cost, lead);
						}
						if (find.Count == 0) (find, findNext) = (findNext, find);
					}

					void tryGo(string from, int price, string to)
					{
						int check = visited.Read(to, int.MaxValue);
						if (price + 1 >= check) return;
						visited[to] = price + 1;
						findNext.Enqueue((to, price + 1));
					}
					int key2 = (dest.Key[0] - 'A') | (dest.Key[1] - 'A') << 16;
					destinations.Add((key2, visited[dest.Key]));
				}
				int key = (valve.Key[0] - 'A') | (valve.Key[1] - 'A') << 16;
				valves2[key] = (valve.Value.Rate, destinations.ToArray());
			}
			
			var valveMask = new Dictionary<int, uint>();
			uint mask = 1;
			foreach (var item in valves2)
			{
				valveMask[item.Key] = mask;
				mask <<= 1;
			}
			
			var cache = new Dictionary<(int Minute, int Cooldown, int Location, uint Path), int>();
			var x = dfs(0, 0, 0, 0, 0, 0, 0, 0);
			WriteLine($"Part 1: {x}");
			int dfs(int location, uint path, int cooldown, uint enableMask, int rate, int total, int moves, int enabled)
			{
				int premod = cooldown;
				int cached = cache.Read((moves, cooldown, location, path), -1);
				if (cached != -1) return rate + cached;

				var exploreables = new List<(int location, int boost, int cost)>();
				var exploreablesP2 = new List<(int location, int boost, int cost)>();

				int best = 0;

				if (moves < 29)
				{
					if (cooldown > 0)
					{
						cooldown--;
					}
					if (cooldown <= 0)
					{
						if (valves2[location].Rate != 0 && (enableMask & valveMask[location]) == 0)
						{
							rate += valves2[location].Rate;
							enableMask |= valveMask[location];
						}
						foreach (var (Destination, Cost) in valves2[location].Leads)
						{
							if ((path & valveMask[Destination]) != 0) continue;
							exploreables.Add((Destination, 0, Cost + 1));
						}
					}
					if (exploreables.Count == 0)
					{
						exploreables.Add((location, 0, 0));
					}
					foreach (var act in exploreables)
					{
						if (moves == 0)
						{
							//WriteLine($"{act.path}: {watch.Elapsed}");
						}
						int turned = Math.Sign(act.boost);
						best = Math.Max(best, dfs(act.location, path | valveMask[act.location], cooldown + act.cost, enableMask | (act.boost > 0 ? valveMask[location] : 0), rate + act.boost, total, moves + 1, enabled + turned));
					}
				}

				cache[(moves, premod, location, path)] = best; // Math.Max(best, cache.Read((moves, location, cooldown, locationP2, cooldownP2, enableMask)));
				return rate + best;
			}
			cache.Clear();
			int bestScore = 0;
			var cache2 = new Dictionary<(int Minute, int Cooldown, int Location, int CooldownP2, int LocationP2, uint Path), bool>();
			dfs2(0, 0, 0, 0, 0, 0, 0, 0, 0);
			WriteLine($"Part 2: {bestScore}");
			void dfs2(int location, uint path, int cooldown, int locationP2, uint pathP2, int cooldownP2, uint enableMask, int total, int moves)
			{
				bool cached = cache2.Read((moves, cooldown, location, cooldownP2, locationP2, path));
				if (cached) return;
				bestScore = Math.Max(bestScore, total);
				var exploreables = new List<(int location, int path, int boost, int cost)>();
				var exploreablesP2 = new List<(int location, int path, int boost, int cost)>();

				int best = 0;
				int remainingTurns = 26 - moves;

				if (moves < 25)
				{
					if (cooldown > 0)
					{
						cooldown--;
					}
					if (cooldown <= 0)
					{
						if (valves2[location].Rate != 0 && (enableMask & valveMask[location]) == 0)
						{
							enableMask |= valveMask[location];
						}
						foreach (var (Destination, Cost) in valves2[location].Leads)
						{
							if (((path | pathP2) & valveMask[Destination]) != 0) continue;
							exploreables.Add((Destination, Destination, valves2[Destination].Rate, Cost + 1));
						}
					}
					if (cooldownP2 > 0)
					{
						cooldownP2--;
					}
					if (cooldownP2 <= 0)
					{
						if (valves2[locationP2].Rate != 0 && (enableMask & valveMask[locationP2]) == 0)
						{
							enableMask |= valveMask[locationP2];
						}
						foreach (var (Destination, Cost) in valves2[locationP2].Leads)
						{
							if (((path | pathP2) & valveMask[Destination]) != 0) continue;
							exploreablesP2.Add((Destination, Destination, valves2[Destination].Rate, Cost + 1));
						}
					}
					
					if (exploreables.Count == 0)
					{
						exploreables.Add((location, location, 0, 0));
					}
					if (exploreablesP2.Count == 0)
					{
						exploreablesP2.Add((locationP2, locationP2, 0, 0));
					}
					foreach (var act in exploreables)
					{
						if (moves == 0)
						{
							cache2.Clear();
						}

						foreach (var actP2 in exploreablesP2)
						{
							if (act.path == actP2.path) continue;
							int turned = Math.Sign(act.boost) + Math.Sign(actP2.boost);

							int newTotal = total + act.boost * Math.Max(0, remainingTurns - act.cost) + actP2.boost * Math.Max(0, remainingTurns - actP2.cost);

							dfs2(act.location, path | valveMask[act.path], cooldown + act.cost, actP2.location, pathP2 | valveMask[actP2.path], cooldownP2 + actP2.cost, enableMask | (act.boost > 0 ? valveMask[location] : 0) | (actP2.boost > 0 ? valveMask[locationP2] : 0), newTotal, moves + 1);
						}
					}
				}
				
				// I've no idea what the heck is wrong with this cache. It's useless. As such, I'm only using it to stop early here.
				cache2[(moves + 1, cooldown, location, cooldownP2, locationP2, path)] = true;
			}
		}
	}
}
