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
			var valves2 = new Dictionary<string, (int Rate, (string Destination, int Cost)[] Leads)>();

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
			;

			/*var explore = new Queue<(string location, string path, int rate, int total)>();
			var exploreNext = new Queue<(string location, string path, int rate, int total)>();
			var terminated = new Dictionary<(int rate, int total), bool>();
			int bestScore = 0;
			int bestRate = 0;

			void process(string location, string path, int rate, int total)
			{
				total += rate;
				bool actions = false;
				if (location != null)
				{
					if (path.Length / 2 > 10)
					{
						// Cut down on paths that show no promise
						if (total < bestScore * 0.9 || rate < bestRate * 0.5) return;
					}
					if (valves[location].Rate != 0 && !path.Contains($"¤{location}"))
					{
						exploreNext.Enqueue((location, path + $"¤{location}", rate + valves[location].Rate, total));
						actions = true;
					}
					foreach (string valve in valves[location].Leads)
					{
						if (path.Length == 3)
						{
							if (valve == "AA") continue;
						}
						else if (path.Length >= 6)
						{
							string[] history = path.Split('¤');
							//WriteLine(history[history.Length - 1]);
							if (history[history.Length - 1].Contains(valve)) continue;
						}
						actions = true;
						exploreNext.Enqueue((valve, path + $">{valve}", rate, total));
					}
				}
				if (total > bestScore)
				{
					bestScore = total;
				}
				if (rate > bestRate)
				{
					bestRate = rate;
				}
				if (!actions)
				{
					terminated[(rate, total)] = true;
				}
			}
			
			process("AA", "", 0, 0);
			for (int i = 0; i < 30; i++)
			{
				//WriteLine(i);
				while (explore.Count > 0)
				{
					var exp = explore.Dequeue();
					process(exp.location, exp.path, exp.rate, exp.total);
				}
				(explore, exploreNext) = (exploreNext, explore);
				foreach (var path in terminated)
				{
					explore.Enqueue((null, null, path.Key.rate, path.Key.total));
				}
			}
			WriteLine($"Part 1: {bestScore}");
			explore.Clear();*/

			/*var exploreCoop = new Queue<(string location, uint path, int cooldown, string locationP2, uint pathP2, int cooldownP2, uint mask, int rate, int total, int moves, int enabled)>();
			var exploreNextCoop = new Queue<(string location, uint path, int cooldown, string locationP2, uint pathP2, int cooldownP2, uint mask, int rate, int total, int moves, int enabled)>();
			var terminatedCoop = new Dictionary<(int rate, int total), bool>();
			int bestScoreCoop = 0;
			int bestProjectedCoop = 0;
			int bestRateCoop = 0;*/

			int maxRate = valves.Sum(v => v.Value.Rate);
			int maxValves = valves.Count(v => v.Value.Rate > 0);
			
			foreach (var valve in valves.Where(v => v.Value.Rate > 0 || v.Key == "AA"))
			{
				var destinations = new List<(string Destination, int Cost)>();
				//WriteLine($"From {valve.Key}");
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
					//WriteLine(best.Where(item => map[item.Key.X, item.Key.Y] == 0).Min(item => item.Value));

					void tryGo(string from, int price, string to)
					{
						int check = visited.Read(to, int.MaxValue);
						if (price + 1 >= check) return;
						visited[to] = price + 1;
						findNext.Enqueue((to, price + 1));
					}
					//WriteLine($" To {dest.Key}: {visited[dest.Key]}");
					destinations.Add((dest.Key, visited[dest.Key]));
					//valves2[valve]
				}
				valves2[valve.Key] = (valve.Value.Rate, destinations.ToArray());
			}
			
			var valveMask = new Dictionary<string, uint>();
			uint mask = 1;
			foreach (var item in valves2)
			{
				valveMask[item.Key] = mask;
				mask <<= 1;
			}

			int cacheHit = 0;
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();
			
			var cache = new Dictionary<(int Minute, int Cooldown, string Location, uint Path), int>();
			var x = dfs("AA", 0, 0, 0, 0, 0, 0, 0);
			watch.Stop();
			WriteLine($"Part 1: {x}\nThat took {watch.ElapsedMilliseconds}ms (or {watch.Elapsed})");
			;
			int dfs(string location, uint path, int cooldown, uint enableMask, int rate, int total, int moves, int enabled)
			{
				int premod = cooldown;
				int cached = cache.Read((moves, cooldown, location, path), -1);
				if (cached != -1)
				{
					cacheHit++;
					return rate + cached;
				}

				var exploreables = new List<(string location, string path, int boost, int cost)>();
				var exploreablesP2 = new List<(string location, string path, int boost, int cost)>();

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
							exploreables.Add((Destination, $"{Destination}", 0, Cost + 1));
						}
					}
					if (exploreables.Count == 0)
					{
						exploreables.Add((location, $"{location}", 0, 0));
					}
					foreach (var act in exploreables)
					{
						if (moves == 0)
						{
							//WriteLine($"{act.path}: {watch.Elapsed}");
						}
						int turned = Math.Sign(act.boost);
						best = Math.Max(best, dfs(act.location, path | valveMask[act.path], cooldown + act.cost, enableMask | (act.boost > 0 ? valveMask[location] : 0), rate + act.boost, total, moves + 1, enabled + turned));
					}
				}

				cache[(moves, premod, location, path)] = best; // Math.Max(best, cache.Read((moves, location, cooldown, locationP2, cooldownP2, enableMask)));
				return rate + best;
			}
			cache.Clear();
			watch.Restart();
			int bestScore = 0;
			var cache2 = new Dictionary<(int Minute, int Cooldown, string Location, int CooldownP2, string LocationP2, uint Path), bool>();
			dfs2("AA", 0, 0, "AA", 0, 0, 0, 0, 0);
			watch.Stop();
			WriteLine($"Part 2: {bestScore}\nThat took {watch.ElapsedMilliseconds}ms (or {watch.Elapsed})");
			void dfs2(string location, uint path, int cooldown, string locationP2, uint pathP2, int cooldownP2, uint enableMask, int total, int moves)
			{
				//(int premod, int premodP2) = (cooldown, cooldownP2);
				bool cached = cache2.Read((moves, cooldown, location, cooldownP2, locationP2, path));
				if (cached) return;
				bestScore = Math.Max(bestScore, total);
				var exploreables = new List<(string location, string path, int boost, int cost)>();
				var exploreablesP2 = new List<(string location, string path, int boost, int cost)>();

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
							//best = valves2[locationP2].Rate;
							//rate += valves2[location].Rate;
							enableMask |= valveMask[location];
						}
						foreach (var valve in valves2[location].Leads)
						{
							if (((path | pathP2) & valveMask[valve.Destination]) != 0) continue;
							exploreables.Add((valve.Destination, $"{valve.Destination}", valves2[valve.Destination].Rate, valve.Cost + 1));
						}
					}
					// Player 2 (elephant)
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
							exploreablesP2.Add((Destination, $"{Destination}", valves2[Destination].Rate, Cost + 1));
						}
					}
					
					if (exploreables.Count == 0)
					{
						exploreables.Add((location, $"{location}", 0, 0));
					}
					if (exploreablesP2.Count == 0)
					{
						exploreablesP2.Add((locationP2, $"{locationP2}", 0, 0));
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
							if (moves == 0)
							{
								WriteLine($"{act.path}/{actP2.path}: {watch.Elapsed}");
							}
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
