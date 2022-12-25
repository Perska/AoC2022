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
		static void Day25(List<string> input)
		{
			var digits = new Dictionary<char, long>() { { '0', 0 }, { '1', 1 }, { '2', 2 }, { '=', -2 }, { '-', -1 } };
			var reDigits = new List<char>() { '0', '1', '2', '=', '-' };

			long sum = 0;

			long toInt(string snafu)
			{
				long add = 0;
				long pow = 1;
				for (int j = snafu.Length - 1; j >= 0; j--)
				{
					add += digits[snafu[j]] * pow;
					pow *= 5;
				}
				return add;
			}

			long toIntB(char[] snafu)
			{
				long add = 0;
				long pow = 1;
				for (int j = snafu.Length - 1; j >= 0; j--)
				{
					add += digits[snafu[j]] * pow;
					pow *= 5;
				}
				return add;
			}

			for (int i = 0; i < input.Count; i++)
			{
				long add = toInt(input[i]);
				sum += add;
			}
			
			int log = (int)Math.Ceiling(Math.Log(sum, 5));
			char[] snaf = "=".PadRight(log, '=').ToArray();
			long val = 0;
			while (true)
			{
				val = toIntB(snaf);
				if (val == sum) break;
				long diff = Math.Abs(sum - val);
				int nlog = Math.Max((int)Math.Ceiling(Math.Log(diff, 5)), 1);
				
				int dig = (reDigits.IndexOf(snaf[log - nlog]) + 1) % 5;
				snaf[log - nlog] = reDigits[dig];
			}

			WriteLine($"Part 1: {new string(snaf)}");
		}
	}
}
