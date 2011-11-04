using UnityEngine;
using OpenGameConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenGameConsole
{
	public static class CoreCommands
	{
		static AudioSource PlayNyan(AudioClip clip, Vector3 position)
		{
			GameObject go = new GameObject("nyan");
			go.transform.position = position;
			AudioSource source = go.AddComponent<AudioSource>();
			source.clip = clip;
			source.volume = 1;
			source.Play();
			return source;
		}
		
		private static AudioSource nyan;
		
		/// <summary>
		/// Nyans the cat.
		/// </summary>
		/// <returns>
		/// The cat.
		/// </returns>
		/// <param name='args'>
		/// What is this I don't even.
		/// </param>
		public static string NyanCat(params string[] args)
		{
			if (nyan)
			{
				if (nyan.isPlaying)
				{
					nyan.Stop();
					return "Awww...";
				}
				else
				{
					nyan.Play();
					return 
					"Yes son. Now we are a family again.\n" +
					"+        o        +                 o\n" + 
					"     +               o       +       +\n" +
					"         o                +\n" +
					"      o       +          +           +\n" +
					"+        o        o          +      o\n" +
					"-_-_-_-_-_-_-_,------,      o \n" +
					"_-_-_-_-_-_-_-|      /\\_/\\  \n" +
					"-_-_-_-_-_-_-~|__( ^ .^)  + \n" +
					"_-_-_-_-_-_-_-\"\"  \"\"      \n" +
					"+        o        +                 o\n" + 
					"     +               o       +       +\n" +
					"         o                +\n" +
					"      o       +          +           +\n" +
					"+        o        o          +      o\n";
					
				}
			}
			else
			{
				nyan = PlayNyan(Resources.Load("nyan") as AudioClip, Vector3.zero);
				return 
				"Yes son. Now we are a family again.\n" +
				"+        o        +                 o\n" + 
				"     +               o       +       +\n" +
				"         o                +\n" +
				"      o       +          +           +\n" +
				"+        o        o          +      o\n" +
				"-_-_-_-_-_-_-_,------,      o \n" +
				"_-_-_-_-_-_-_-|      /\\_/\\  \n" +
				"-_-_-_-_-_-_-~|__( ^ .^)  + \n" +
				"_-_-_-_-_-_-_-\"\"  \"\"      \n" +
				"+        o        +                 o\n" + 
				"     +               o       +       +\n" +
				"         o                +\n" +
				"      o       +          +           +\n" +
				"+        o        o          +      o\n";
			}
		}
		
		public static string Clear(string[] args)
		{
			GameConsole.instance.ClearStream();
			return null;
		}
		
		public static string ThrowErrors(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (string.IsNullOrEmpty(arg))
			{
				return "Throw errors currently set to: " + GameConsole.instance.throwErrors;
			}
			
			if (args.Length != 1)
			{
				return "$!errorThrowErrors: Invalid argument " + "<" + arg + ">";
			}
			
			try
			{
				GameConsole.instance.throwErrors = System.Convert.ToBoolean(args[0]);
				return "Throw errors set to: " + GameConsole.instance.throwErrors;
			}
			catch
			{
				return "$!errorThrowErrors: Invalid argument " + "<" + arg + ">";
			}	
		}
		
		public static string Buffer(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (string.IsNullOrEmpty(arg))
			{
				return "Line buffer size currently set to: " + GameConsole.instance.bufferSize;
			}
			
			if (args.Length != 1)
			{
				return "$!errorBuffer: Invalid argument " + "<" + arg + ">";
			}
			
			try
			{
				GameConsole.instance.bufferSize = System.Convert.ToInt32(args[0]);
				return "Line buffer size set to: " + GameConsole.instance.bufferSize;
			}
			catch
			{
				return "$!errorBuffer: Invalid argument " + "<" + arg + ">";
			}
		}
		
		public static string Verbose(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (string.IsNullOrEmpty(arg))
			{
				return "Verbose mode currently set to: " + GameConsole.instance.verbose;
			}
			
			if (args.Length != 1)
			{
				return "$!errorVerbose: Invalid argument " + "<" + arg + ">";
			}
			
			try
			{
				GameConsole.instance.verbose = System.Convert.ToBoolean(args[0]);
				return "Verbose mode set to: " + GameConsole.instance.verbose;
			}
			catch
			{
				return "$!errorVerbose: Invalid argument " + "<" + arg + ">";
			}	
		}
		
		public static string History(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (string.IsNullOrEmpty(arg))
			{
				return "Line history size currently set to: " + GameConsole.instance.historySize;
			}
			
			if (args.Length != 1)
			{
				return "$!errorHistory: Invalid argument " + "<" + arg + ">";
			}
			
			try
			{
				GameConsole.instance.historySize = System.Convert.ToInt32(args[0]);
				return "Line history size set to: " + GameConsole.instance.historySize;
			}
			catch
			{
				return "$!errorHistory: Invalid argument " + "<" + arg + ">";
			}
		}
		
		public static string LineSpace(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (string.IsNullOrEmpty(arg))
			{
				return "Line spacing currently set to: " + GameConsole.instance.streamSpacing;
			}
			
			if (args.Length != 1)
			{
				return "$!errorLineSpace: Invalid argument " + "<" + arg + ">";
			}
			
			try
			{
				GameConsole.instance.streamSpacing = System.Convert.ToInt32(args[0]);
				return "Line spacing set to: " + GameConsole.instance.streamSpacing;
			}
			catch
			{
				return "$!errorLineSpace: Invalid argument " + "<" + arg + ">";
			}
		}
		
		public static string Help(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);

			if (args.Length > 1)
			{
				return "$!errorHelp: Invalid argument " + "<" + arg + ">";
			}
			
			if (string.IsNullOrEmpty(arg))
			{
				string commands = "List of commands:\n#\n";
				foreach (string key in GameConsole.instance.activeConsoleCommands.Keys)
				{
					commands += key + "\n";	
				}
				commands += "\nType help/man <command name> for more information.\n" +
					"$this (the currently selected object) can be used in place of 'GameObject Name'\n" +
					"Semicolons (;) denote a new line.";
				return commands;
			}
			try
			{
				if (args[0].Replace(" ", "") == "#")
					return "Help<#>\n\nDoes nothing - used as a comment.";
				
				TextAsset helpFile = Resources.Load("man_" + args[0].Replace(" ", "")) as TextAsset;
				return "Help<" + args[0].Replace(" ", "") + ">\n\n" + helpFile.text;
			}
			catch
			{
				return "$!errorHelp: Command or manual entry not found " + "<" + arg + ">";
			}
		}
		
		public static string ListObjects(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (string.IsNullOrEmpty(arg))
			{
				try
				{
					string objects = "\n";
					
					if (GameConsole.instance.target == null)
					{
						GameObject[] objs = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
						foreach (GameObject obj in objs)
						{
							if (obj.transform.parent == null)
							{
								objects += obj.name + "\n";
							}
						}
					}
					else
					{
						foreach (Transform t in GameConsole.instance.target.transform)
						{
							if (t == GameConsole.instance.target.transform)
								continue;
							
							objects += t.gameObject.name + "\n";
						}
					}
					return objects;
				}
				catch
				{
					return "$!errorListObjects: Command or manual entry not found " + "<" + arg + ">";
				}
			}
			
			return "$!errorListObjects: Invalid argument " + "<" + arg + ">";
		}
		
		public static string MemUsage(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (string.IsNullOrEmpty(arg))
			{
				return "Current GC total memory:\n" + System.GC.GetTotalMemory(false).ToString() + " bytes";				
			}
			
			return "$!errorMemUsage: Invalid argument " + "<" + arg + ">";
		}
		
		public static string GarbageCollect(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (string.IsNullOrEmpty(arg))
			{
				return "Garbage Collected - Current GC total memory:\n" + System.GC.GetTotalMemory(true).ToString() + " bytes";				
			}
			
			return "$!errorMemUsage: Invalid argument " + "<" + arg + ">";
		}
		
		/// <summary>
		/// Changes the physics gravity vector.
		/// </summary>
		public static string Gravity(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (string.IsNullOrEmpty(arg))
			{
				return "Gravity currently set to: " +
					"X: " + Physics.gravity.x +
					" | Y: " + Physics.gravity.y +
					" | Z: " + Physics.gravity.z;
			}
			
			if (args.Length != 3)
			{
				return "$!errorGravity: Invalid argument " + "<" + arg + ">";
			}
			
			float x = 0;
			float y = 0;
			float z = 0;
			
			try
			{
				x = System.Convert.ToSingle(args[0]);
				y = System.Convert.ToSingle(args[1]);
				z = System.Convert.ToSingle(args[2]);
				
				Physics.gravity = new Vector3(x, y, z);
				return "Gravity set to: " +
					"X: " + x +
					" | Y: " + y +
					" | Z: " + z;
			}
			catch
			{
				return "$!errorGravity: Invalid argument " + "<" + arg + ">";
			}
		}
		
		
		/// <summary>
		/// Changes the physics time scale.
		/// </summary>
		public static string TimeScale(string[] args)
		{
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (string.IsNullOrEmpty(arg))
			{
				return "Time scale currently set to: " + Time.timeScale;
			}
			
			if (args.Length != 1)
			{
				return "$!errorTimeScale: Invalid argument " + "<" + arg + ">";
			}
			
			try
			{
				Time.timeScale = System.Convert.ToSingle(args[0]);
				return "Time scale set to: " + Time.timeScale;
			}
			catch
			{
				return "$!errorTimeScale: Invalid argument " + "<" + arg + ">";
			}
		}
		
		public static string Move(string[] args)
		{	
			// Parse the string and test for malformed strings
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (args.Length != 4)
			{
				return "$!errorMove: Invalid argument " + "<" + arg + ">";
			}
			
			
			if (arg.IndexOf("'") == -1)
			{
				return "$!errorMove: Invalid argument " + "<" + arg + ">";
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(0, arg.IndexOf("'")).Replace(" ", "")))
			{
				return "$!errorMove: Invalid argument " + "<" + arg + ">";
			}
			
			Regex regName = new Regex("'(.*)'");
			Match match = regName.Match(arg);
			string result = "";
			float x = 0;
			float y = 0;
			float z = 0;
			
			if (match.Success)
			{
				
				result = match.Groups[1].Value;
				try
				{
					x = System.Convert.ToSingle(args[1]);
					y = System.Convert.ToSingle(args[2]);
					z = System.Convert.ToSingle(args[3]);
					GameObject.Find(result).transform.position = new Vector3(x, y, z);
				}
				catch
				{
					return "$!errorMove: Unable to move GameObject " + "<" + arg + ">";
				}
			}
			else
			{
				return "$!errorMove: Invalid argument " + "<" + arg + ">";
				
			}
				
			return "Moved GameObject: " + result + " to " + 
					"X: " + x +
					" | Y: " + y +
					" | Z: " + z;
		}
		
		public static string MoveRB(string[] args)
		{	
			// Parse the string and test for malformed strings
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (args.Length != 4)
			{
				return "$!errorMoveRB: Invalid argument " + "<" + arg + ">";
			}
			
			
			if (arg.IndexOf("'") == -1)
			{
				return "$!errorMoveRB: Invalid argument " + "<" + arg + ">";
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(0, arg.IndexOf("'")).Replace(" ", "")))
			{
				return "$!errorMove: Invalid argument " + "<" + arg + ">";
			}
			
			Regex regName = new Regex("'(.*)'");
			Match match = regName.Match(arg);
			string result = "";
			float x = 0;
			float y = 0;
			float z = 0;
			if (match.Success)
			{
				result = match.Groups[1].Value;
				try
				{
					x = System.Convert.ToSingle(args[1]);
					y = System.Convert.ToSingle(args[2]);
					z = System.Convert.ToSingle(args[3]);
					GameObject.Find(result).rigidbody.position = new Vector3(x, y, z);
				}
				catch
				{
					return "$!errorMoveRB: Unable to move Rigidbody " + "<" + arg + ">";
				}
			}
			else
			{
				return "$!errorMoveRB: Invalid argument " + "<" + arg + ">";
				
			}
				
			return "Moved Rigidbody: " + result + " to " + 
					"X: " + x +
					" | Y: " + y +
					" | Z: " + z;
		}
		
		
		/// <summary>
		/// Send a message to a Game Object.
		/// </summary>
		public static string SendMessage(string[] args)
		{	
			// Parse the string and test for malformed strings
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (args.Length != 2)
			{
				return "$!errorSendMessage: Invalid argument " + "<" + arg + ">";
			}
			
			
			if (arg.IndexOf("'") == -1)
			{
				return "$!errorSendMessage: Invalid argument " + "<" + arg + ">";
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(0, arg.IndexOf("'")).Replace(" ", "")))
			{
				return "$!errorSendMessage: Invalid argument " + "<" + arg + ">";
			}
			
			Regex regName = new Regex("'(.*)'");
			Match match = regName.Match(arg);
			string result = "";
			
			if (match.Success)
			{
				result = match.Groups[1].Value;
				try
				{
					GameObject.Find(result).SendMessage(args[1], SendMessageOptions.RequireReceiver);
				}
				catch
				{
					return "$!errorSendMessage: Unable to send message " + "<" + arg + ">";
				}
			}
			else
			{
				return "$!errorSendMessage: Invalid argument " + "<" + arg + ">";
				
			}
				
			return "Sent message: " + args[1] + " to " + result;
		}
		
		/// <summary>
		/// Finds the location of a GameObject.
		/// </summary>
		public static string Loc(string[] args)
		{	
			// Parse the string and test for malformed strings
			string arg = String.Join(",", args);
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (arg.IndexOf("'") == -1)
			{
				return "$!errorLoc: Invalid argument " + "<" + arg + ">";
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(0, arg.IndexOf("'")).Replace(" ", "")))
			{
				return "$!errorLoc: Invalid argument " + "<" + arg + ">";
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(arg.LastIndexOf("'") + 1, arg.Length - 1 - arg.LastIndexOf("'")).Replace(" ", "")))
			{
				return "$!errorLoc: Invalid argument " + "<" + arg + ">";
			}
			
			Regex regName = new Regex("'(.*)'");
			Match match = regName.Match(arg);
			string result = "";
			Vector3 position = Vector3.zero;
			
			if (match.Success)
			{
				result = match.Groups[1].Value;
				try
				{
					position = GameObject.Find(result).transform.position;
				}
				catch
				{
					return "$!errorLoc: Unable to find GameObject " + "<" + arg + ">";
				}
			}
			else
			{
				return "$!errorLoc: Invalid argument " + "<" + arg + ">";
				
			}
				
			return result + " is located at " + 
					"X: " + position.x +
					" | Y: " + position.y +
					" | Z: " + position.z;
		}
		
		/// <summary>
		/// Print a message to the console.
		/// </summary>
		public static string Print(string[] args)
		{	
			// Parse the string and test for malformed strings
			string arg = String.Join(",", args);
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (arg.IndexOf("'") == -1)
			{
				return "$!errorPrint: Invalid argument " + "<" + arg + ">";
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(0, arg.IndexOf("'")).Replace(" ", "")))
			{
				return "$!errorPrint: Invalid argument " + "<" + arg + ">";
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(arg.LastIndexOf("'") + 1, arg.Length - 1 - arg.LastIndexOf("'")).Replace(" ", "")))
			{
				return "$!errorPrint: Invalid argument " + "<" + arg + ">";
			}
			
			Regex regName = new Regex("'(.*)'");
			Match match = regName.Match(arg);
			string result = "";
			
			if (match.Success)
			{
				result = match.Groups[1].Value;
			}
			else
			{
				return "$!errorPrint: Invalid argument " + "<" + arg + ">";
				
			}
				
			return result;
		}
		
		/// <summary>
		/// Log a message to the Unity Console.
		/// </summary>
		public static string Log(string[] args)
		{	
			// Parse the string and test for malformed strings
			string arg = String.Join(",", args);
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (arg.IndexOf("'") == -1)
			{
				return "$!errorLog: Invalid argument " + "<" + arg + ">";
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(0, arg.IndexOf("'")).Replace(" ", "")))
			{
				return "$!errorLog: Invalid argument " + "<" + arg + ">";
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(arg.LastIndexOf("'") + 1, arg.Length - 1 - arg.LastIndexOf("'")).Replace(" ", "")))
			{
				return "$!errorLog: Invalid argument " + "<" + arg + ">";
			}
			
			Regex regName = new Regex("'(.*)'");
			Match match = regName.Match(arg);
			string result = "";
			
			if (match.Success)
			{
				result = match.Groups[1].Value;
				Debug.Log(result);
			}
			else
			{
				return "$!errorLog: Invalid argument " + "<" + arg + ">";
				
			}
				
			return result;
		}
	}
}