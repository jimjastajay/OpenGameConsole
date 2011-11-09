/// These defines control compilation of each command - it is recommended to undefine any commands that are not enabled.
#define NYAN_CAT
#define CLEAR
#define CONSOLE_SETTINGS
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
#define GET
#define RUN
using UnityEngine;
using ThinksquirrelSoftware.OpenGameConsole;
using ThinksquirrelSoftware.OpenGameConsole.Utility;
using NDesk.Options;
using System;
using System.IO;
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
			GameObject go = null;
			if (GameObject.Find("nyan.lock"))
			{
				go = GameObject.Find("nyan.lock");
			}
			else
			{
				go = new GameObject("nyan.lock");
			}
			go.transform.position = position;
			AudioSource source = go.AddComponent<AudioSource>();
			source.clip = clip;
			source.loop = true;
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
					UnityEngine.Object.DestroyImmediate(nyan.gameObject);
					return "Awww...";
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(nyan);
					nyan = PlayNyan(Resources.Load("nyan") as AudioClip, Vector3.zero);
					return 
					"Yes son. Now we are a family again.\n" +
					",*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`\n" +
					".,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,\n" +
					"*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^         ,---/V\\\n" +
					"`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.    ~|__(o.o)\n" +
					"^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'  UU  UU";
				}
			}
			else
			{
				nyan = PlayNyan(Resources.Load("nyan") as AudioClip, Vector3.zero);
				return 
				"Yes son. Now we are a family again.\n" +
				",*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`\n" +
				".,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,\n" +
				"*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^         ,---/V\\\n" +
				"`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.    ~|__(o.o)\n" +
				"^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'^`*.,*'  UU  UU";
			}
#else
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		public static string Clear(params string[] args)
		{
#if CLEAR
			GameConsole.instance.ClearStream();
			return string.Empty;
#else
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		public static string ConsoleSettings(params string[] args)
		{
#if CONSOLE_SETTINGS
			if (args.Length == 0)
			{
				return ConsoleErrors.OptionExceptionError("ogc", "Invalid option(s) or no options specified");
			}
			
			bool noOptions = true;
			bool showHelp = false;
			bool showVersion = false;
			bool showList = false;		

			var options = new OptionSet() {
            { "l|list", "list the current console settings.", o => { showList = o != null; noOptions = false; } },
			{ "s|linespace=", "the amount of {LINES} of whitespace between console lines.\nthis must be an integer.", (int o) => { GameConsole.instance.streamSpacing = o; noOptions = false; } },
            { "H|history=", "the size, in {LINES} of the command buffer.\nthis must be an integer.", (int o) => { GameConsole.instance.historySize = o; noOptions = false; } },
			{ "B|buffer=", "the size, in {LINES} of the line buffer.\nthis must be an integer.", (int o) => { GameConsole.instance.bufferSize = o; noOptions = false; } },
            { "V|verbose", "send console commands to the Unity log.\nonly useful if logging is disabled.", o => { GameConsole.instance.verbose = o != null; noOptions = false; } },
			{ "S|silent", "don't send console commands to the Unity log.\nonly useful if logging is enabled.", o => { if (o != null) GameConsole.instance.verbose = false; noOptions = false; } },
            { "E|throwerrors",  "throw errors to the Unity log.\nonly useful if throwing errors is disabled.", o => { GameConsole.instance.throwErrors = o != null; noOptions = false; } },
			{ "N|noerrors",  "don't throw errors to the Unity log.\nonly useful if throwing errors is enabled.", o => { if (o != null) GameConsole.instance.throwErrors = false; noOptions = false; } },
			{ "version",  "show version information and exit", o => { showVersion = o != null; noOptions = false; } },
			{ "h|help",  "show this message and exit", o => { showHelp = o != null; noOptions = false; } },};
		
			try
			{
				options.Parse(args);
			}
			catch (OptionException e)
			{
				return ConsoleErrors.OptionExceptionError("ogc", e.Message);
			}
			
			if (noOptions)
			{
				return ConsoleErrors.OptionExceptionError("ogc", "Invalid option(s) or no options specified");
			}
			if (showVersion)
			{
				return OGCParse.ExpatLicense("OpenGameConsole Settings", "1.0", "2011", "Thinksquirrel Software, LLC");
			}
			if (showHelp)
			{
				TextWriter helpText = new StringWriter();
				options.WriteOptionDescriptions(helpText);
				return
					"Usage: ogc [OPTIONS]\n" + 
					"Change the console settings.\n" +
					"Options:\n" +
					helpText.ToString() + "\n" +
					"Report bugs to: support@thinksquirrel.com";
			}
			
			if (showList)
			{
				return
					"Console settings:\n" +
					"Linespace - " + GameConsole.instance.streamSpacing + "\n" +
					"history - " + GameConsole.instance.historySize + "\n" +
					"buffer - " + GameConsole.instance.bufferSize + "\n" +
					"verbose - " + GameConsole.instance.verbose + "\n" +
					"throwerrors - " + GameConsole.instance.throwErrors;
			}			
			return string.Empty;
#else
			ConsoleErrors.CommandExecutionError;
#endif
		}
		
		public static string Help(params string[] args)
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
					"$THIS (the currently selected object) can be used in place of \"GameObject Name\"\n" +
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
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		public static string ListObjects(params string[] args)
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
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		public static string MemUsage(params string[] args)
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
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		public static string GarbageCollect(params string[] args)
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
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		/// <summary>
		/// Changes the physics gravity vector.
		/// </summary>
		public static string Gravity(params string[] args)
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
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		
		/// <summary>
		/// Changes the physics time scale.
		/// </summary>
		public static string TimeScale(params string[] args)
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
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		public static string Move(params string[] args)
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
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		public static string MoveRB(params string[] args)
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
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		
		/// <summary>
		/// Send a message to a Game Object.
		/// </summary>
		public static string SendMessage(params string[] args)
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
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		/// <summary>
		/// Finds the location of a GameObject.
		/// </summary>
		public static string Loc(params string[] args)
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
		public static string Print(params string[] args)
		{
#if PRINT
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
				
			return args[0];
#else
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		/// <summary>
		/// Log a message to the Unity Console.
		/// </summary>
		public static string Log(params string[] args)
		{	
#if LOG
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			Debug.Log(args[0]);
			return args[0];
#else
			return ConsoleErrors.CommandExecutionError;
#endif
		}

		/// <summary>
		/// Get a file and print it.
		/// </summary>
		public static string Get(params string[] args)
		{	
#if GET
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			if (GameObject.Find("ogc-dl.lock"))
			{
				return ConsoleErrors.LockFileError("ogc-dl.lock");
			}
			OGCDownloader downloader = new GameObject("ogc-dl.lock").AddComponent<OGCDownloader>();
			downloader.StartDownload(args[0], true);
			return "Downloading: " + args[0];
#else
			return ConsoleErrors.CommandExecutionError;
#endif
		}
		
		/// <summary>
		/// Run a console script.
		/// </summary>
		public static string Run(params string[] args)
		{	
#if RUN
			if (args.Length != 1)
			{
				return ConsoleErrors.InvalidArgumentError;
			}
			
			if (GameObject.Find("ogc-dl.lock"))
			{
				return ConsoleErrors.LockFileError("ogc-dl.lock");
			}
			OGCDownloader downloader = new GameObject("ogc-dl.lock").AddComponent<OGCDownloader>();
			downloader.StartDownload(args[0], false);
			return "Downloading: " + args[0];
#else
			return ConsoleErrors.CommandExecutionError;
#endif
		}
	}
}