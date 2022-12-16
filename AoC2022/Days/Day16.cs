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

			var exploreCoop = new Queue<(string location, uint path, int cooldown, string locationP2, uint pathP2, int cooldownP2, uint mask, int rate, int total, int moves, int enabled)>(20_000_000);
			var exploreNextCoop = new Queue<(string location, uint path, int cooldown, string locationP2, uint pathP2, int cooldownP2, uint mask, int rate, int total, int moves, int enabled)>(20_000_000);
			var exploreNextCoop2 = new Queue<(string location, uint path, int cooldown, string locationP2, uint pathP2, int cooldownP2, uint mask, int rate, int total, int moves, int enabled)>(20_000_000);
			var exploreNextCoop3 = new Queue<(string location, uint path, int cooldown, string locationP2, uint pathP2, int cooldownP2, uint mask, int rate, int total, int moves, int enabled)>(20_000_000);
			var exploreNextCoop4 = new Queue<(string location, uint path, int cooldown, string locationP2, uint pathP2, int cooldownP2, uint mask, int rate, int total, int moves, int enabled)>(20_000_000);
			var terminatedCoop = new Dictionary<(int rate, int total), bool>();
			int bestScoreCoop = 0;
			int bestProjectedCoop = 0;
			int bestRateCoop = 0;

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

			void processCoop(string location, uint path, int cooldown, string locationP2, uint pathP2, int cooldownP2, uint enableMask, int rate, int total, int moves, int enabled)
			{
				total += rate;
				bool actions = false;
				var exploreables = new List<(string location, string path, int boost, int cost)>();
				var exploreablesP2 = new List<(string location, string path, int boost, int cost)>();
				int projected = 0;
				bool cooled = cooldown + cooldownP2 > 0;
				if (location != null && moves < 26)
				{
					int currentTurn = moves + 1;
					int remainingTurns = 26 - currentTurn;
					projected = total + rate * remainingTurns;
					if (remainingTurns > 1 && rate != maxRate && enabled != maxValves)
					{
						if (moves > 8)
						{
							//float gradual = Math.Min((moves - 8) / 4f, 1);
							float gradual = 1;
							//if (path.Length / 3 > 16) gradual = 1.1f;
							float scoreThreshold = 0.75f * gradual;
							float rateThreshold = 0.0f * gradual;
							float projectedThreshold = 0.25f * gradual;
							if (total < bestScoreCoop * scoreThreshold || rate < bestRateCoop * rateThreshold || projected < bestProjectedCoop * projectedThreshold) return;
							if (enabled < 2 + moves / 6) return;
						}
						// Player 1 (you)
						if (cooldown > 0)
						{
							cooldown--;
						}
						if (cooldown <= 0)
						{
							if (valves2[location].Rate != 0 && (enableMask & valveMask[location]) == 0)
							{
								exploreables.Add((location, $"{location}", valves2[location].Rate, 1));
								actions = true;
							}
							foreach (var valve in valves2[location].Leads)
							{
								//string[] history = path.Split('¤');
								if (((path | pathP2) & valveMask[valve.Destination]) != 0) continue;
								exploreables.Add((valve.Destination, $"{valve.Destination}", 0, valve.Cost));
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
								exploreablesP2.Add((locationP2, $"{locationP2}", valves2[locationP2].Rate, 1));
							}
							foreach (var valve in valves2[locationP2].Leads)
							{
								//string[] history = pathP2.Split('¤');
								if (((path | pathP2) & valveMask[valve.Destination]) != 0) continue;
								exploreablesP2.Add((valve.Destination, $"{valve.Destination}", 0, valve.Cost));
							}
							//exploreablesP2.RemoveAll(act => exploreables.Any(act2 => act.path == act2.path));
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
							foreach (var actP2 in exploreablesP2)
							{
								if (act.path == actP2.path) continue;
								//WriteLine($"{act.path} and {actP2.path}");
								int turned = Math.Sign(act.boost) + Math.Sign(actP2.boost);
								exploreNextCoop.Enqueue((act.location, path | valveMask[act.path], cooldown + act.cost, actP2.location, pathP2 | valveMask[actP2.path], cooldownP2 + actP2.cost, enableMask | (act.boost>0 ? valveMask[location] : 0) | (actP2.boost > 0 ? valveMask[locationP2] : 0), rate + act.boost + actP2.boost, total, moves + 1, enabled + turned));
								if (act.cost + actP2.cost > 0) actions = true;
							}
						}

					}
					else
					{
						cooled = false;
						cooldown = cooldownP2 = 0;
					}
				}
				if (total > bestScoreCoop)
				{
					bestScoreCoop = total;
				}
				if (rate > bestRateCoop)
				{
					bestRateCoop = rate;
				}
				if (projected > bestProjectedCoop)
				{
					bestProjectedCoop = projected;
				}
				if (!actions && !cooled)
				{
					terminatedCoop[(rate, total)] = true;
				}
			}

			processCoop("AA", 0, 0, "AA", 0, 0, 0, 0, 0, 0, 0);
			for (int i = 0; i < 26; i++)
			{
				WriteLine($"Minute {i}, have {exploreCoop.Count} options [{bestRateCoop}/{maxRate} rate]");
				while (exploreCoop.Count > 0)
				{
					var exp = exploreCoop.Dequeue();
					processCoop(exp.location, exp.path, exp.cooldown, exp.locationP2, exp.pathP2, exp.cooldownP2, exp.mask, exp.rate, exp.total, exp.moves, exp.enabled);
				}
				(exploreCoop, exploreNextCoop) = (exploreNextCoop, exploreCoop);
				//terminated
				int termed = 100;
				foreach (var path in terminatedCoop.OrderByDescending(s => s.Key.total + s.Key.rate * (26 - i)))
				{
					exploreCoop.Enqueue((null, 0, 0, null, 0, 0, 0, path.Key.rate, path.Key.total, 0, 0));
					termed--;
					if (0 > termed) break;
				}
				WriteLine($"projected: {bestProjectedCoop}");
				//WriteLine(terminatedCoop.Count);
				terminatedCoop.Clear();
			}
			WriteLine($"Part 2: {bestScoreCoop}");
			exploreCoop.Clear();
			;
		}
	}
}
