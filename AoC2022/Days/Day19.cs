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
		struct Blueprint
		{
			public int Ore, Clay;
			public int ObsidianOre, ObsidianClay;
			public int GeodeOre, GeodeObsidian;

			public Blueprint(int ore, int clay, int oOre, int oClay, int geOre, int geObsidian)
			{
				Ore = ore;
				Clay = clay;
				ObsidianOre = oOre;
				ObsidianClay = oClay;
				GeodeOre = geOre;
				GeodeObsidian = geObsidian;
			}
		}

		// [UseSRL] // Uncomment if you wanna use SuperReadLine
		// [NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		static void Day19(List<string> input)
		{
			var blueprints = new List<Blueprint>();
			for (int i = 0; i < input.Count; i++)
			{
				if (input[i] == "") break;

				string[] sub;
				if (input[i].Contains("Each"))
				{
					sub = input[i].Split('.');
					sub[0] = sub[0].Substring(sub[0].IndexOf(":") + 1);
				}
				else
				{
					i++;
					sub = input.Skip(i).Take(4).ToArray();
					i += 4;
				}
				var ore = sub[0].SplitToStringArray(" ", true);
				var clay = sub[1].SplitToStringArray(" ", true);
				var obsidian = sub[2].SplitToStringArray(" ", true);
				var geode = sub[3].SplitToStringArray(" ", true);

				blueprints.Add(new Blueprint(int.Parse(ore[4]), int.Parse(clay[4]), int.Parse(obsidian[4]), int.Parse(obsidian[7]), int.Parse(geode[4]), int.Parse(geode[7])));

			}
			
			int maxOre;
			int maxClay;
			int maxObs;
			int doBlueprint(
				Blueprint blueprint,
				int ore,
				int oreBots,
				int clay,
				int clayBots,
				int obsidian,
				int obsidianBots,
				int geode,
				int geodeBots,
				int round,
				int maxRounds)
			{
				bool prevCanOre = false;
				bool prevCanClay = false;
				bool prevCanObs = false;
				bool prevCanGeo = false;

				bool canOre = false;
				bool canClay = false;
				bool canObs = false;
				bool canGeo = false;

				int score = geode;
				int subscore = 0;
				
				for (int i = round; i < maxRounds; i++)
				{
					canOre = blueprint.Ore <= ore;
					canClay = blueprint.Clay <= ore;
					canObs = blueprint.ObsidianOre <= ore && blueprint.ObsidianClay <= clay;
					canGeo = blueprint.GeodeOre <= ore && blueprint.GeodeObsidian <= obsidian;

					ore += oreBots;
					clay += clayBots;
					obsidian += obsidianBots;
					geode += geodeBots;

					if (canGeo && !prevCanGeo)
					{
						subscore = doBlueprint(blueprint, ore - blueprint.GeodeOre, oreBots, clay, clayBots, obsidian - blueprint.GeodeObsidian, obsidianBots, geode, geodeBots + 1, i + 1, maxRounds);
						score = Math.Max(score, subscore);
						prevCanGeo = canGeo;
					}
					if (canObs && !prevCanObs && obsidianBots < maxObs)
					{
						subscore = doBlueprint(blueprint, ore - blueprint.ObsidianOre, oreBots, clay - blueprint.ObsidianClay, clayBots, obsidian, obsidianBots + 1, geode, geodeBots, i + 1, maxRounds);
						score = Math.Max(score, subscore);
						prevCanObs = canObs;
					}
					if (canClay && !prevCanClay && clayBots < maxClay)
					{
						subscore = doBlueprint(blueprint, ore - blueprint.Clay, oreBots, clay, clayBots + 1, obsidian, obsidianBots, geode, geodeBots, i + 1, maxRounds);
						score = Math.Max(score, subscore);
						prevCanClay = canClay;
					}
					if (canOre && !prevCanOre && oreBots < maxOre)
					{
						subscore = doBlueprint(blueprint, ore - blueprint.Ore, oreBots + 1, clay, clayBots, obsidian, obsidianBots, geode, geodeBots, i + 1, maxRounds);
						score = Math.Max(score, subscore);
						prevCanOre = canOre;
					}

					score = Math.Max(score, geode);
				}
				return score;
			}
			
			int qualities = 0;

			//blueprints.RemoveAt(1);
			for (int i = 0; i < blueprints.Count; i++)
			{
				int ore = 0;
				int oreBots = 1;
				int clay = 0;
				int clayBots = 0;
				int obsidian = 0;
				int obsidianBots = 0;
				int geode = 0;
				int geodeBots = 0;
			
				int round = 0;

				var blueprint = blueprints[i];

				maxOre = new[] { blueprint.Ore, blueprint.Clay, blueprint.ObsidianOre, blueprint.GeodeOre }.Max();
				maxClay = blueprint.ObsidianClay;
				maxObs = blueprint.GeodeObsidian;
				
				int newScore = doBlueprint(blueprint, ore, oreBots, clay, clayBots, obsidian, obsidianBots, geode, geodeBots, round, 24);
				qualities += newScore * (i + 1);
			}
			WriteLine($"Part 1: {qualities}");

			qualities = 1;
			for (int i = 0; i < Math.Min(3, blueprints.Count); i++)
			{
				int ore = 0;
				int oreBots = 1;
				int clay = 0;
				int clayBots = 0;
				int obsidian = 0;
				int obsidianBots = 0;
				int geode = 0;
				int geodeBots = 0;

				int round = 0;

				var blueprint = blueprints[i];

				maxOre = new[] { blueprint.Ore, blueprint.Clay, blueprint.ObsidianOre, blueprint.GeodeOre }.Max();
				maxClay = blueprint.ObsidianClay;
				maxObs = blueprint.GeodeObsidian;

				int newScore = doBlueprint(blueprint, ore, oreBots, clay, clayBots, obsidian, obsidianBots, geode, geodeBots, round, 32);
				qualities *= newScore;
			}
			WriteLine($"Part 2: {qualities}");
		}
	}
}
