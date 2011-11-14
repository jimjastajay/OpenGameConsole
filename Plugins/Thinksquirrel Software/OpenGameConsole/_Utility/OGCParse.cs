using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace ThinksquirrelSoftware.OpenGameConsole.Utility
{
	public static class OGCParse
	{
		public static string ExpatLicense(string product, string version, string year, string author)
		{
			return	
				product + " " + version + "\n" +
				"Copyright (C) " + year + " by " + author + "\n" +
				"License: MIT (Expat)\n" +
				"\n" +
				"Permission is hereby granted, free of charge, to any person obtaining a copy\n" +
				"of this software and associated documentation files (the \"Software\"), to deal\n" +
				"in the Software without restriction, including without limitation the rights\n" +
				"to use, copy, modify, merge, publish, distribute, sublicense, and/or sell\n" +
				"copies of the Software, and to permit persons to whom the Software is\n" +
				"furnished to do so, subject to the following conditions:\n" +
				"\n" +
				"The above copyright notice and this permission notice shall be included in\n" +
				"all copies or substantial portions of the Software.\n" +
				"\n" +
				"THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\n" +
				"IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,\n" +
				"FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE\n" +
				"AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER\n" +
				"LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,\n" +
				"OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN\n" +
				"THE SOFTWARE.";
		}

		public static bool IsBlankCommand(this string input)
		{
			return string.IsNullOrEmpty(input.Replace(" ", ""));
		}
		
		public static string Sanitize(this string input)
		{
			return input.Replace("$!error$", "");
		}

		public static string[] ParseLines(this string input)
		{
			string[] results = input.Split(';');
			for (int i = 0; i < results.Length; i++)
			{
				results[i] = results[i].Trim();
			}
			return results;
		}
		
		public static string ReplaceFirst(this string text, string search, string replace)
		{
			int pos = text.IndexOf(search);
			if (pos < 0)
			{
				return text;
			}
			return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
		}
			
		public static string ParseCommand(this string input)
		{
			if (input.Length < 3)
			{
				return input.Replace(" ", "");
			}
			
			try
			{
				// Number of words we still want to display.
				int words = 1;
				
				// Loop through entire summary.
				for (int i = 0; i < input.Length; i++)
				{
					// Increment words on a space.
					if (input[i] == ' ')
					{
						words--;
					}
					
					// If we have no more words to display, return the substring.
					if (words == 0)
					{
						return input.Substring(0, i).Replace(" ", "");
					}
					
					// At the end!
					if (i == input.Length - 1)
					{
						return input.Replace(" ", "");
					}
				}
			}
			catch (Exception)
			{
				GameConsole.instance.Throw("Command parsing error");
			}
			return string.Empty;
		}
	
		public static string[] ParseArguments(this string input)
		{
			var results = Regex
				.Matches(input.Substring(input.ParseCommand().Length), @"(?<match>[^""\s]+)|\""(?<match>[^""]*)""")
				.Cast<Match>()
				.Select(m => m.Groups["match"].Value)
				.ToList();
			
			return results.ToArray();
		}
		
		public static string ParseTokens(this string input, string token, string replacement)
		{
			if (replacement == null)
				replacement = "/dev/null";
					
			return input.Replace("$" + token, replacement);
		}
		
		public static string[] ParseTokens(this string[] input, string token, string replacement)
		{
			if (replacement == null)
				replacement = "/dev/null";
			
			for (int i = 0; i < input.Length; i++)
			{
				input[i] = input[i].Replace("$" + token, replacement);
			}
			
			return input;
		}
		
		public static Vector3 ParseVector3(string xString, string yString, string zString)
		{
			return new Vector3(System.Convert.ToSingle(xString), System.Convert.ToSingle(yString), System.Convert.ToSingle(zString));
		}
	}
}
