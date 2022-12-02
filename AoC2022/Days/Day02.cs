using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
	partial class Program
	{
		// [UseSRL] // Uncomment if you wanna use SuperReadLine
		[NoTrailingNewLine] // Uncomment to not include an extra blank line in the input at the end
		static void Day02(List<string> input)
		{
			var guide = new List<(char Foe, char You)>();
			int score = 0;

			string[] attacks = new string[] { "Rock", "Paper", "Scissors" };

			int RPS(int foe, int you)
			{
				int points = you + 1;
				//LogL($"Game: {attacks[foe]} vs {attacks[you]}: ");
				if (foe == you)
				{
					points += 3;
					//LogL("Draw");
				}
				else if ((foe + 1) % 3 == you)
				{
					points += 6;
					//LogL("Win");
				}
				else
				{
					//LogL("Lose");
				}
				//Log($" - Got {points} points.");
				return points;
			}

			for (int i = 0; i < input.Count; i++)
			{
				string str = input[i].ToUpperInvariant();
				if (str.Length == 3)
					guide.Add((str[0], str[2]));
				else Log("Warn: line with invalid input?");
			}

			for (int i = 0; i < guide.Count; i++)
			{
				int foe = guide[i].Foe - 'A';
				int you = guide[i].You - 'X';
				score += RPS(foe, you);
			}
			Log($"Part 1 Total Score: {score}");

			score = 0;
			for (int i = 0; i < guide.Count; i++)
			{
				int foe = guide[i].Foe - 'A';
				int you = 0;
				switch (guide[i].You)
				{
					case 'X':
						you = (foe+2) % 3;
						break;
					case 'Y':
						you = foe;
						break;
					case 'Z':
						you = (foe + 1) % 3;
						break;
				}
				score += RPS(foe, you);
			}
			Log($"Part 2 Total Score: {score}");
		}
	}
}
