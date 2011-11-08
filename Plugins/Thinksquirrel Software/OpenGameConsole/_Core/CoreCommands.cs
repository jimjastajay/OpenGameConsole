/// These defines control compilation of each command - it is recommended to undefine any commands that are not enabled.
#define NYAN_CAT
#define CLEAR
#define THROW_ERRORS
#define BUFFER
#define VERBOSE
#define HISTORY
#define LINE_SPACE
#define HELP
#define LIST_OBJECTS
#define MEM_USAGE
#define GARBAGE_COLLECT
#define GRAVITY
#define TIME_SCALE
#define MOVE
#define MOVE_RB
#define SEND_MESSAGE
#define LOC
#define PRINT
#define LOG
using UnityEngine;
using ThinksquirrelSoftware.OpenGameConsole;
using ThinksquirrelSoftware.OpenGameConsole.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ThinksquirrelSoftware.OpenGameConsole
{
	public static class CoreCommands
	{
#if NYAN_CAT
		private static AudioSource PlayNyan(AudioClip clip, Vector3 position)
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
#endif
		
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
#if NYAN_CAT
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
#else
			return string.Empty;
#endif
		}
		
		public static string Clear(string[] args)
		{
#if CLEAR
			GameConsole.instance.ClearStream();
#endif
			return string.Empty;
		}
		
		public static string ThrowErrors(string[] args)
		{	
#if THROW_ERRORS			
			if (args.Length == 0)
			{
				return "Throw errors currently set to: " + GameConsole.instance.throwErrors;
			}
			
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			try
			{
				GameConsole.instance.throwErrors = System.Convert.ToBoolean(args[0]);
				return "Throw errors set to: " + GameConsole.instance.throwErrors;
			}
			catch
			{
				return ConsoleErrors.InvalidArgumentError;
			}
#else
			return string.Empty;
#endif
		}
		
		public static string Buffer(string[] args)
		{
#if BUFFER			
			if (args.Length == 0)
			{
				return "Line buffer size currently set to: " + GameConsole.instance.bufferSize;
			}
			
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			try
			{
				GameConsole.instance.bufferSize = System.Convert.ToInt32(args[0]);
				return "Line buffer size set to: " + GameConsole.instance.bufferSize;
			}
			catch
			{
				return ConsoleErrors.InvalidArgumentError;
			}
#else
			return string.Empty;
#endif
		}
		
		public static string Verbose(string[] args)
		{
#if VERBOSE
			if (args.Length == 0)
			{
				return "Verbose mode currently set to: " + GameConsole.instance.verbose;
			}
			
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			try
			{
				GameConsole.instance.verbose = System.Convert.ToBoolean(args[0]);
				return "Verbose mode set to: " + GameConsole.instance.verbose;
			}
			catch
			{
				return ConsoleErrors.InvalidArgumentError;
			}
#else
			return string.Empty;
#endif
		}
		
		public static string History(string[] args)
		{
#if HISTORY			
			if (args.Length == 0)
			{
				return "Line history size currently set to: " + GameConsole.instance.historySize;
			}
			
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			try
			{
				GameConsole.instance.historySize = System.Convert.ToInt32(args[0]);
				return "Line history size set to: " + GameConsole.instance.historySize;
			}
			catch
			{
				return ConsoleErrors.InvalidArgumentError;
			}
#else
			return string.Empty;
#endif
		}
		
		public static string LineSpace(string[] args)
		{
#if LINE_SPACE			
			if (args.Length == 0)
			{
				return "Line spacing currently set to: " + GameConsole.instance.streamSpacing;
			}
			
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			try
			{
				GameConsole.instance.streamSpacing = System.Convert.ToInt32(args[0]);
				return "Line spacing set to: " + GameConsole.instance.streamSpacing;
			}
			catch
			{
				return ConsoleErrors.InvalidArgumentError;
			}
#else
			return string.Empty;
#endif			
		}
		
		public static string Help(string[] args)
		{
#if HELP
			if (args.Length > 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			if (args.Length == 0)
			{
				string commands = "List of commands:\n#\n";
				foreach (string key in GameConsole.instance.activeConsoleCommands.Keys)
				{
					commands += key;
					if (GameConsole.instance.activeAliases.ContainsValue(key))
					{
						foreach (KeyValuePair<string, string> kvp in GameConsole.instance.activeAliases)
						{
							if (kvp.Value == key)
							{
								commands += " | " + kvp.Key;
							}
						}
					}	
					commands += "\n";
				}
				commands += "\nType help/man <command name> for more information.\n" +
					"$this (the currently selected object) can be used in place of \"GameObject Name\"\n" +
					"Semicolons (;) denote a new line.";
				return commands;
			}
			
			try
			{
				if (args[0].Replace(" ", "") == "#")
					return "Help<#>\n\nDoes nothing - used as a comment.";
				
				string cmd = args[0].Replace(" ", "");
				if (GameConsole.instance.activeAliases.ContainsKey(cmd))
				{
					cmd = GameConsole.instance.activeAliases[cmd];
				}
				TextAsset helpFile = Resources.Load("man_" + cmd) as TextAsset;
				return "Help<" + args[0].Replace(" ", "") + ">\n\n" + helpFile.text;
			}
			catch
			{
				return ConsoleErrors.ResourceNotFoundError(args[0]);
			}
#else
			return string.Empty;
#endif
		}
		
		public static string ListObjects(string[] args)
		{
#if LIST_OBJECTS	
			if (args.Length > 0)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			if (args.Length == 0)
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
					return ConsoleErrors.GameObjectNotFoundError(args[0]);
				}
			}
			
			return ConsoleErrors.InvalidArgumentError;
#else
			return string.Empty;
#endif
		}
		
		public static string MemUsage(string[] args)
		{
#if MEM_USAGE
			if (args.Length > 0)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			if (args.Length == 0)
			{
				return "Current GC total memory:\n" + System.GC.GetTotalMemory(false).ToString() + " bytes";				
			}
			
			return ConsoleErrors.InvalidArgumentError;
#else
			return string.Empty;
#endif
		}
		
		public static string GarbageCollect(string[] args)
		{
#if GARBAGE_COLLECT
			if (args.Length > 0)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			if (args.Length == 0)
			{
				return "Garbage Collected - Current GC total memory:\n" + System.GC.GetTotalMemory(true).ToString() + " bytes";				
			}
			
			return ConsoleErrors.InvalidArgumentError;
#else
			return string.Empty;
#endif
		}
		
		/// <summary>
		/// Changes the physics gravity vector.
		/// </summary>
		public static string Gravity(string[] args)
		{
#if GRAVITY
			if (args.Length > 3)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			if (args.Length == 0)
			{
				return "Gravity currently set to: " +
					"X: " + Physics.gravity.x +
					" | Y: " + Physics.gravity.y +
					" | Z: " + Physics.gravity.z;
			}
			
			try
			{
				Physics.gravity = OGCParse.ParseVector3(args[0], args[1], args[2], Physics.gravity);
				
				return "Gravity set to: " +
					"X: " + Physics.gravity.x +
					" | Y: " + Physics.gravity.y +
					" | Z: " + Physics.gravity.z;
			}
			catch
			{
				return ConsoleErrors.InvalidArgumentError;
			}
#else
			return string.Empty;
#endif
		}
		
		
		/// <summary>
		/// Changes the physics time scale.
		/// </summary>
		public static string TimeScale(string[] args)
		{
#if TIME_SCALE
			if (args.Length > 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			if (args.Length == 0)
			{
				return "Time scale currently set to: " + Time.timeScale;
			}
			
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			try
			{
				Time.timeScale = System.Convert.ToSingle(args[0]);
				return "Time scale set to: " + Time.timeScale;
			}
			catch
			{
				return ConsoleErrors.InvalidArgumentError;
			}
#else
			return string.Empty;
#endif
		}
		
		public static string Move(string[] args)
		{
#if MOVE
			if (args.Length != 4)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			Vector3 a = Vector3.zero;
			
			try
			{
				a = OGCParse.ParseVector3(args[1], args[2], args[3], GameObject.Find(args[0]).transform.position);
			}
			catch
			{
				return ConsoleErrors.VectorParsingError;
			}
			try
			{
				GameObject.Find(args[0]).transform.position = a;
			}
			catch
			{
				return ConsoleErrors.GameObjectNotFoundError(args[0]);
			}
				
			return "Moved GameObject: " + args[0] + " to " + 
					"X: " + a.x +
					" | Y: " + a.y +
					" | Z: " + a.z;
#else
			return string.Empty;
#endif
		}
		
		public static string MoveRB(string[] args)
		{	
#if MOVE_RB
			if (args.Length != 4)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			Vector3 a = Vector3.zero;
			
			try
			{
				a = OGCParse.ParseVector3(args[1], args[2], args[3], GameObject.Find(args[0]).rigidbody.position);
			}
			catch
			{
				return ConsoleErrors.VectorParsingError;
			}
			try
			{
				GameObject.Find(args[0]).rigidbody.position = a;
			}
			catch
			{
				return ConsoleErrors.GameObjectNotFoundError(args[0]);
			}
				
			return "Moved Rigidbody: " + args[0] + " to " + 
					"X: " + a.x +
					" | Y: " + a.y +
					" | Z: " + a.z;
#else
			return string.Empty;
#endif
		}
		
		
		/// <summary>
		/// Send a message to a Game Object.
		/// </summary>
		public static string SendMessage(string[] args)
		{	
#if SEND_MESSAGE
			if (args.Length != 2)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
				
			try
			{
				GameObject.Find(args[0]).SendMessage(args[1], SendMessageOptions.RequireReceiver);
			}
			catch
			{
				return ConsoleErrors.SendMessageError(args[1], args[0]);
			}
				
			return "Sent message: " + args[1] + " to " + args[0];
#else
			return string.Empty;
#endif
		}
		
		/// <summary>
		/// Finds the location of a GameObject.
		/// </summary>
		public static string Loc(string[] args)
		{	
#if LOC
			if (args.Length != 2)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			Vector3 position = Vector3.zero;
			
			try
			{
				position = GameObject.Find(args[0]).transform.position;
			}
			catch
			{
				return ConsoleErrors.GameObjectNotFoundError(args[0]);
			}
			
			return args[0] + " is located at " + 
					"X: " + position.x +
					" | Y: " + position.y +
					" | Z: " + position.z;
#else
			return string.Empty;
#endif
		}
		
		/// <summary>
		/// Print a message to the console.
		/// </summary>
		public static string Print(string[] args)
		{
#if PRINT
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
				
			return args[0];
#else
			return string.Empty;
#endif
		}
		
		/// <summary>
		/// Log a message to the Unity Console.
		/// </summary>
		public static string Log(string[] args)
		{	
#if LOG
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			Debug.Log(args[0]);
			return args[0];
#else
			return string.Empty;
#endif
		}
	}
}