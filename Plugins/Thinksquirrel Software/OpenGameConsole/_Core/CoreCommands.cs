/// These defines control compilation of each command - it is recommended to undefine any commands that are not enabled.
#define NYAN_CAT
#define VERSION
#define CLEAR
#define CONSOLE_SETTINGS
#define HELP
#define LIST_OBJECTS
#define GARBAGE_COLLECT
#define PHYSICS
#define TIME
#define MOVE
#define MOVE_RB
#define SEND_MESSAGE
#define LOC
#define ECHO
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
		
		public static string Version(params string[] args)
		{
#if VERSION
			return OGCParse.ExpatLicense("OpenGameConsole", "1.0 | Environment: Unity Engine v. " + Application.unityVersion + " | Language: " + 
				Application.systemLanguage.ToString(), "2011", "Thinksquirrel Software, LLC");
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
            { "v|verbose", "send console commands to the Unity log.\nonly useful if logging is disabled.", o => { GameConsole.instance.verbose = o != null; noOptions = false; } },
			{ "S|silent", "don't send console commands to the Unity log.\nonly useful if logging is enabled.", o => { if (o != null) GameConsole.instance.verbose = false; noOptions = false; } },
            { "E|throwerrors",  "throw errors to the Unity log.\nonly useful if throwing errors is disabled.", o => { GameConsole.instance.throwErrors = o != null; noOptions = false; } },
			{ "N|noerrors",  "don't throw errors to the Unity log.\nonly useful if throwing errors is enabled.", o => { if (o != null) GameConsole.instance.throwErrors = false; noOptions = false; } },
			{ "V|version",  "show version information and exit", o => { showVersion = o != null; noOptions = false; } },
			{ "help",  "show this message and exit", o => { showHelp = o != null; noOptions = false; } },};
		
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
					"Usage: ogc <[-l] [-v | -S] [-E | -N] [-s=] [-H=] [-B=]>\n" + 
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
				commands += "\nType man <command name> for more information.\n";
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
	
		public static string GarbageCollect(params string[] args)
		{
#if GARBAGE_COLLECT
			
			string units = "KB";
			double unitMod = 0.0009765625;
			
			if (args.Length == 0)
			{
				return "Current GC total memory - " + (System.GC.GetTotalMemory(false) * unitMod).ToString() + " " + units;
			}
			
			bool showHelp = false;
			bool showVersion = false;
			bool changedOutput = false;
			bool changedOutputTwice = false;
			bool collect = false;

			var options = new OptionSet() {
            { "b|bytes", "display output in bytes.", o => { if (o != null) units = "bytes" ; changedOutputTwice = (changedOutput == true) ? true : false; changedOutput = true;} },
			{ "k|kb", "display output in kilobytes(KB).\nthis is the default.", o => { if (o != null) units = "KB" ; changedOutputTwice = (changedOutput == true) ? true : false; changedOutput = true;} },
            { "m|mb", "display output in megabytes(MB)", o => { if (o != null) units = "MB" ; changedOutputTwice = (changedOutput == true) ? true : false; changedOutput = true;} },
			{ "g|gb", "display output in gigabytes(GB)", o => { if (o != null) units = "GB" ; changedOutputTwice = (changedOutput == true) ? true : false; changedOutput = true;} },
            { "c|collect", "perform garbage collection", o => { if (o != null) collect = true; } },
			{ "V|version",  "show version information and exit", o => { showVersion = o != null; } },
			{ "help",  "show this message and exit", o => { showHelp = o != null; } },};
		
			try
			{
				options.Parse(args);
			}
			catch (OptionException e)
			{
				return ConsoleErrors.OptionExceptionError("gc", e.Message);
			}
			
			if (showVersion)
			{
				return OGCParse.ExpatLicense("OpenGameConsole Garbage Collection Utility", "1.0", "2011", "Thinksquirrel Software, LLC");
			}
			if (showHelp)
			{
				TextWriter helpText = new StringWriter();
				options.WriteOptionDescriptions(helpText);
				return
					"Usage: gc [-b | -k | -m | -g] [-c]>\n" + 
					"Check managed memory and perform garbage collection.\n" +
					"Options:\n" +
					helpText.ToString() + "\n" +
					"Report bugs to: support@thinksquirrel.com";
			}
			
			if (changedOutputTwice)
			{
				return ConsoleErrors.OptionExceptionError("gc", "Invalid option(s) or no options specified");
			}
			if (units == "bytes")
			{
				unitMod = 1;
			}
			else if (units == "MB")
			{
				unitMod = 9.53674316e-7;
			}
			else if (units == "GB")
			{
				unitMod = 9.31322575e-10;
			}
			
			GameConsole.instance.Echo("Current GC total memory - " + (System.GC.GetTotalMemory(false) * unitMod).ToString() + " " + units, false);
			
			if (collect)
			{
				GameConsole.instance.Echo("Performing Garbage Collection:", false);
				System.GC.Collect();
				GameConsole.instance.Echo("Garbage Collection Completed. Total memory - " + (System.GC.GetTotalMemory(true) * unitMod).ToString() + " " + units, false);
			}
			
			return string.Empty;
#else
			ConsoleErrors.CommandExecutionError;
#endif
		}

		public static string PhysicsSettings(params string[] args)
		{
#if PHYSICS
			if (args.Length == 0)
			{
				return ConsoleErrors.OptionExceptionError("physics", "Invalid option(s) or no options specified");
			}
			
			bool noOptions = true;
			bool showHelp = false;
			bool showVersion = false;
			bool showList = false;		

			var options = new OptionSet() {
            { "l|list", "list the current physics settings", o => { showList = o != null; noOptions = false; } },
			{ "g|gravity=", "the gravity {VECTOR} x,y,z applied to all rigid bodies in the scene", o => { if (o != null) { string[] v = o.ToString().Split(','); Physics.gravity = OGCParse.ParseVector3(v[0],v[1],v[2]); noOptions = false; } } },
            { "x=", "the x {VALUE} of the gravity vector", (float o) => { Physics.gravity = new Vector3(o, Physics.gravity.y, Physics.gravity.z); noOptions = false; } },
			{ "y=", "the y {VALUE} of the gravity vector", (float o) => { Physics.gravity = new Vector3(Physics.gravity.x, o, Physics.gravity.z); noOptions = false; } },
            { "z=", "the z {VALUE} of the gravity vector", (float o) => { Physics.gravity = new Vector3(Physics.gravity.x, Physics.gravity.y, o); noOptions = false; } },
			{ "p|penalty=", "the minimum contact penentration {VALUE} in order to apply a penalty force.\nmust be positive.", (float o) => { if (o >= 0) Physics.minPenetrationForPenalty = o; noOptions = false; } },
            { "b|bounce=",  "two colliding objects with a relative {VELOCITY} below this will not bounce.\nmust be positive.", (float o) => { if (o >= 0) Physics.bounceThreshold = o; noOptions = false; } },
			{ "v|sleepvelocity=",  "the minimum {VELOCITY} before objects sleep.\nmust be positive.", (float o) => { if (o >= 0) Physics.sleepVelocity = o; noOptions = false; } },
			{ "a|sleepangular=",  "the minimum angular {VELOCITY} before objects sleep.\nmust be positive.", (float o) => { if (o >= 0) Physics.sleepAngularVelocity = o; noOptions = false; } },
			{ "m|maxangular=",  "the maxinimum angular {VELOCITY} permitted.\nmust be positive.", (float o) => { if (o >= 0) Physics.maxAngularVelocity = o; noOptions = false; } },
			{ "s|solvers=",  "the default solver iteration {COUNT} for rigid bodies.\nmust be a positive integer.", (int o) => { if (o >= 0) Physics.solverIterationCount = o; noOptions = false; } },
			{ "V|version",  "show version information and exit", o => { showVersion = o != null; noOptions = false; } },
			{ "help",  "show this message and exit", o => { showHelp = o != null; noOptions = false; } },};
		
			try
			{
				options.Parse(args);
			}
			catch (Exception e)
			{
				return ConsoleErrors.OptionExceptionError("physics", e.Message);
			}
			
			if (noOptions)
			{
				return ConsoleErrors.OptionExceptionError("physics", "Invalid option(s) or no options specified");
			}
			if (showVersion)
			{
				return OGCParse.ExpatLicense("OpenGameConsole Physics Settings", "1.0", "2011", "Thinksquirrel Software, LLC");
			}
			if (showHelp)
			{
				TextWriter helpText = new StringWriter();
				options.WriteOptionDescriptions(helpText);
				return
					"Usage: physics <[-l] [[-g=] | [-x=] [-y=] [-z=]] [-p=] [-b=] [-v=] [-a=] [-m=] [-s=]>\n" + 
					"Change the physics environment settings.\n" +
					"Options:\n" +
					helpText.ToString() + "\n" +
					"Report bugs to: support@thinksquirrel.com";
			}
			
			if (showList)
			{
				return
					"Physics settings:\n" +
					"gravity - " + Physics.gravity.ToString() + "\n" +
					"penalty - " + Physics.minPenetrationForPenalty + "\n" +
					"bounce - " + Physics.bounceThreshold + "\n" +
					"sleepvelocity - " + Physics.sleepVelocity + "\n" +
					"sleepangular - " + Physics.sleepAngularVelocity + "\n" +
					"maxangular - " + Physics.maxAngularVelocity + "\n" +
					"solvers - " + Physics.solverIterationCount;
			}			
			return string.Empty;
#else
			ConsoleErrors.CommandExecutionError;
#endif
		}
	
		public static string TimeSettings(params string[] args)
		{
#if TIME
			if (args.Length == 0)
			{
				return ConsoleErrors.OptionExceptionError("time", "Invalid option(s) or no options specified");
			}
			
			bool noOptions = true;
			bool showHelp = false;
			bool showVersion = false;
			bool showList = false;		

			var options = new OptionSet() {
            { "l|list", "list all time settings", o => { showList = o != null; noOptions = false; } },
			{ "t|time", "the current time since the start of the game", o => { if (o != null) { GameConsole.instance.Echo("time - " + Time.time, false); noOptions = false; } } },
            { "T|scenetime", "the current time since the last level load", o => { if (o != null) { GameConsole.instance.Echo("scenetime - " + Time.timeSinceLevelLoad, false); noOptions = false; } } },
			{ "s|timescale=", "the {SCALE} at which the time is passing.\nmust be positive.", (float o) => { if (o >= 0) Time.timeScale = o; noOptions = false; } },
            { "f|framecount", "the number of frames that have passed",  o => { if (o != null) { GameConsole.instance.Echo("framecount - " + Time.frameCount, false); noOptions = false; } } },
			{ "r|realtime", "the real time in seconds since the game started",  o => { if (o != null) { GameConsole.instance.Echo("scenetime - " + Time.realtimeSinceStartup, false); noOptions = false; } } },
            { "m|maxfps=",  "try to render at the specified {FPS}.\nmust be positive.", (int o) => { if (o >= 0) Application.targetFrameRate = o; noOptions = false; } },
			{ "n|nomaxfps=",  "render at the fastest possible FPS.\noverrides maxfps.", o => { if (o != null) { Application.targetFrameRate = -1; noOptions = false; } } },
			{ "V|version",  "show version information and exit", o => { showVersion = o != null; noOptions = false; } },
			{ "help",  "show this message and exit", o => { showHelp = o != null; noOptions = false; } },};
		
			try
			{
				options.Parse(args);
			}
			catch (Exception e)
			{
				return ConsoleErrors.OptionExceptionError("time", e.Message);
			}
			
			if (noOptions)
			{
				return ConsoleErrors.OptionExceptionError("time", "Invalid option(s) or no options specified");
			}
			if (showVersion)
			{
				return OGCParse.ExpatLicense("OpenGameConsole Time Settings", "1.0", "2011", "Thinksquirrel Software, LLC");
			}
			if (showHelp)
			{
				TextWriter helpText = new StringWriter();
				options.WriteOptionDescriptions(helpText);
				return
					"Usage: time <[-l] [-t] [-T] [-s=] [-f] [-r] [-m=] [-n]>\n" + 
					"Change the time settings.\n" +
					"Options:\n" +
					helpText.ToString() + "\n" +
					"Report bugs to: support@thinksquirrel.com";
			}
			
			if (showList)
			{
				return
					"Time settings:\n" +
					"time - " + Time.time + "\n" +
					"scenetime - " + Time.timeSinceLevelLoad + "\n" +
					"timescale - " + Time.timeScale + "\n" +
					"framecount - " + Time.frameCount + "\n" +
					"realtime - " + Time.realtimeSinceStartup + "\n" +
					"maxfps - " + Application.targetFrameRate;
			}			
			return string.Empty;
#else
			ConsoleErrors.CommandExecutionError;
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

		public static string Echo(params string[] args)
		{
#if ECHO
			if (args.Length == 0)
			{
				return ConsoleErrors.OptionExceptionError("echo", "Invalid option(s) or no arguments specified");
			}
			
			bool showHelp = false;
			bool showVersion = false;
			bool log = false;		

			var options = new OptionSet() {
            { "l|log", "send the message to the log", o => { log = o != null; } },
			{ "V|version",  "show version information and exit", o => { showVersion = o != null; } },
			{ "help",  "show this message and exit", o => { showHelp = o != null; } },};
		
			List<string> messages;
			try
			{
				messages = options.Parse(args);
			}
			catch (OptionException e)
			{
				return ConsoleErrors.OptionExceptionError("ogc", e.Message);
			}
			
			if (showVersion)
			{
				return OGCParse.ExpatLicense("OpenGameConsole Echo", "1.0", "2011", "Thinksquirrel Software, LLC");
			}
			if (showHelp)
			{
				TextWriter helpText = new StringWriter();
				options.WriteOptionDescriptions(helpText);
				return
					"Usage: echo [-l] message1 [message2 message3...]\n" + 
					"Print text to the console.\n" +
					"Options:\n" +
					helpText.ToString() + "\n" +
					"Report bugs to: support@thinksquirrel.com";
			}
			
			foreach (string message in messages)
			{
				GameConsole.instance.Echo(message, false);
				if (log)
				{
					Debug.Log(message);
				}
			}
				
			return string.Empty;
#else
			ConsoleErrors.CommandExecutionError;
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