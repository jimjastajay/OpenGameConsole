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
		
		public static Vector3 ParseVector3(string xString, string yString, string zString, Vector3 currentVector)
		{
			float x = 0;
			float y = 0;
			float z = 0;
			
			if (xString == "x")
			{
				x = currentVector.x;
			}
			else
			{
				x = System.Convert.ToSingle(xString);
			}
			if (yString == "y")
			{
				y = currentVector.y;
			}
			else
			{
				y = System.Convert.ToSingle(yString);
			}
			if (zString == "z")
			{
				z = currentVector.z;
			}
			else
			{
				z = System.Convert.ToSingle(zString);
			}
			
			return new Vector3(x, y, z);
		}
	}
}
