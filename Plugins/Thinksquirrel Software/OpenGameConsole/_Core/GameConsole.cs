//UnityEngine only for Debug.Log and GameObject
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;

namespace OpenGameConsole
{
	public class GameConsole
	{
		private static GameConsole _instance;
		private static readonly object lockObj = new object();
		private GameObject _context;
		private string _contextString;
		private string _stream;
		private List<string> _commandHistory = new List<string>();
		private int _streamSpacing = 1;
		private bool _verbose = false;
		private int _bufferSize = 200;
		private int _historySize = 10;
		private bool _throwErrors = false;
		
		// Dictionaries
		private Dictionary<string, Func<string[], string>> _activeConsoleCommands = new Dictionary<string, Func<string[], string>>();
		private Dictionary<string, Func<string[], string>> _inactiveConsoleCommands = new Dictionary<string, Func<string[], string>>();

#if UNITY_EDITOR
		
		// In string format - for security reasons this should only be visible in the editor.
		private Dictionary<string, string> _activeConsoleCommandsEDITOR = new Dictionary<string, string>();
		private Dictionary<string, string> _inactiveConsoleCommandsEDITOR = new Dictionary<string, string>();
		
		public Dictionary<string, string> activeConsoleCommandsEDITOR
		{
			get
			{
				return this._activeConsoleCommandsEDITOR;
			}
		}
		
		public Dictionary<string, string> inactiveConsoleCommandsEDITOR
		{
			get
			{
				return this._inactiveConsoleCommandsEDITOR;
			}
		}
#endif		
		public Dictionary<string, Func<string[], string>> activeConsoleCommands
		{
			get
			{
				return this._activeConsoleCommands;
			}
		}

		public Dictionary<string, Func<string[], string>> inactiveConsoleCommands
		{
			get
			{
				return this._inactiveConsoleCommands;
			}
		}		
		
		// Constructor
		GameConsole()
		{
			_instance = this;
			OGCSerialization.LoadData();
			context = null;
		}
		
		public GameObject target
		{
			get
			{
				return this._context;
			}
		}

		private GameObject context
		{
			get
			{
				return this._context;
			}
			set
			{
				if (value == null)
				{
					_contextString = "/";
				}
				else
				{
					// value is a child of current transform
					foreach (Transform child in context.transform)
					{
						if (context.transform != child && value.transform == child)
						{
							_contextString += "/" + value.name;
							break;
						}
					}
				}
				this._context = value;
			}
		}

		public string contextString
		{
			get
			{
				return this._contextString;
			}
		}
		
		public static GameConsole instance
		{
			get
			{
				lock (lockObj)
				{
					if (_instance == null)
					{
						_instance = new GameConsole();
					}
					return _instance;
				}
			}
			set
			{
				_instance = value;
			}
		}
		
		public string stream
		{
			get
			{
				return this._stream;
			}
		}

		public int bufferSize
		{
			get
			{
				return this._bufferSize;
			}
			set
			{
				if (value > 0)
					_bufferSize = value;
			}
		}
		
		public List<string> commandHistory
		{
			get
			{
				return this._commandHistory;
			}
		}
		
		public int historySize
		{
			get
			{
				return this._historySize;
			}
			set
			{
				if (value >= 0)
				{
					if (_commandHistory.Count > value)
					{
						_commandHistory.RemoveRange(0, _commandHistory.Count - value);
					}
					_historySize = value;
				}
			}
		}

		public int streamSpacing
		{
			get
			{
				return this._streamSpacing;
			}
			set
			{
				if (value > 0)
					_streamSpacing = value;
			}
		}

		public bool throwErrors
		{
			get
			{
				return this._throwErrors;
			}
			set
			{
				_throwErrors = value;
			}
		}

		public bool verbose
		{
			get
			{
				return this._verbose;
			}
			set
			{
				_verbose = value;
			}
		}
		
		public void ClearStream()
		{
			_stream = "";
		}
		
		/// <summary>
		/// Throw an error to the console.
		/// </summary>
		public void Throw(string message)
		{
			Echo("$!error" + message, false);
		}
			
		// Echo (also records to stream)
		private void Echo(string message, bool input)
		{
			if (!string.IsNullOrEmpty(message))
			{
				bool error = false;
				if (message.Contains("$!error"))
				{
					error = true;
					message = message.Remove(0, 7);
				}
				string seperator = "--> ";
				string date = "";
				
				if (input)
				{
					seperator = "  > ";
					date = DateTime.Now.ToString();
				}
			
				if (error)
					seperator = "--> [Error] ";
			
				string newMessage = date + seperator + message;
				for (int i = 0; i < streamSpacing; i++)
				{
					newMessage += "\n";	
				}
			
				if (!input && !error)
				{
					_stream += newMessage;
				}
				else if (input && !error)
				{
					_stream += newMessage;
				}
				
				if (verbose && !error)
				{
					Debug.Log(newMessage);
				}
				if (error)
				{
					_stream += newMessage;
					if (throwErrors)
					{
						Debug.LogError(newMessage);	
					}
				}
				
				string[] streams = stream.Split('\n');
				string[] newStreams = new string[bufferSize];
				if (streams.Length > bufferSize)
				{
					Array.Copy(streams, streams.Length - bufferSize, newStreams, 0, bufferSize);
					_stream = string.Join("\n", newStreams);
				}
			}
		}
		
		/// <summary>
		/// Get the first word of a string.
		/// </summary>
		public string FirstWord(string input)
		{
			if (input.Length < 3)
			{
				return input;
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
						return input.Substring(0, i);
					}
					
					// At the end!
					if (i == input.Length - 1)
					{
						return input;
					}
				}
			}
			catch (Exception)
			{
				Throw("Command parsing error");
			}
			return string.Empty;
		}
		
//#if UNITY_EDITOR
		/// <summary>
		///  Convert string to a command (Func<object, string>). Calls both static and instance methods.
		/// </summary>
		private static Func<string[], string> GetCommand(string fullMethodName)
		{	
			ArgumentException ex = new ArgumentException("GameConsole: Invalid command - " + fullMethodName);
			ArgumentException ex2 = new ArgumentException("GameConsole: Command parsing error - " + fullMethodName);
			
			string methodName = fullMethodName.Substring(fullMethodName.LastIndexOf('.') + 1);
			
			// Instance Methods
			
			if (fullMethodName[0] == '{')
			{
				if (fullMethodName.Contains("}"))
				{
					string objectName = fullMethodName.Substring(1, fullMethodName.LastIndexOf('}') - 1);
					string componentName = fullMethodName.Substring(fullMethodName.LastIndexOf('}') + 1,
																	fullMethodName.IndexOf('.', fullMethodName.LastIndexOf('}')) - fullMethodName.LastIndexOf('}') - 1);
					
					try
					{
						var o = Expression.Constant(GameObject.Find(objectName).GetComponent(componentName));
						var cV = Expression.Convert(o, Type.GetType(componentName));
						var input_i = Expression.Parameter(typeof(string[]), "input");
						var methodInfo_i = Type.GetType(componentName).GetMethod(methodName);
						if (methodInfo_i == null ||
		            		methodInfo_i.ReturnType != typeof(string) ||
		            		methodInfo_i.GetParameters().Length != 1)
						{
							throw ex;
						}
						
						return Expression.Lambda<Func<string[], string>>(Expression.Call(cV, methodInfo_i, input_i), input_i).Compile();
					}
					catch
					{
						throw ex;
					}
				}
				else
				{
					throw ex2;
				}	
			}
			
			try
			{
				// Static Methods
				string typeName = fullMethodName.Remove(fullMethodName.LastIndexOf('.'));
				
				var input = Expression.Parameter(typeof(string[]), "input");
				var methodInfo = Type.GetType(typeName).GetMethod(methodName);
		
				if (methodInfo == null ||
		            methodInfo.ReturnType != typeof(string) ||
		            methodInfo.GetParameters().Length != 1)
				{
					throw ex2;
				}
				
				return Expression.Lambda<Func<string[], string>>(Expression.Call(null, methodInfo, input), input).Compile();
			}
			catch
			{
				throw ex2;
			}
			
		}
		
		/// <summary>
		/// Adds a console command. Editor only.
		/// </summary>
		/// <remarks>
		/// Console commands can call either static or instance methods.
		/// Syntax:
		/// Instance Method - {GameObject}Component.Method
		/// Static Method - Namespace.Class.StaticMethod
		/// </remarks>
		public void AddCommand(string commandName, string command)
		{
			if (_activeConsoleCommands.ContainsKey(commandName) ||
				_inactiveConsoleCommands.ContainsKey(commandName) ||
				commandName.Contains("#") ||
				commandName.Contains("cd") ||
				commandName.Contains("$this"))
			{
				throw new ArgumentException("GameConsole: Command error/already exists");
			}
			_activeConsoleCommands.Add(commandName, GetCommand(command));
#if UNITY_EDITOR
			_activeConsoleCommandsEDITOR.Add(commandName, command);
#endif
		}
		
		/// <summary>
		/// Activates/Deactivates a console command. Editor only.
		/// </summary>
		public void ToggleCommand(string commandName)
		{
			if (_activeConsoleCommands.ContainsKey(commandName))
			{
				_inactiveConsoleCommands.Add(commandName, _activeConsoleCommands[commandName]);
				_activeConsoleCommands.Remove(commandName);
#if UNITY_EDITOR
				_inactiveConsoleCommandsEDITOR.Add(commandName, _activeConsoleCommandsEDITOR[commandName]);
				_activeConsoleCommandsEDITOR.Remove(commandName);
#endif	
			}
			else if (_inactiveConsoleCommands.ContainsKey(commandName))
			{
				_activeConsoleCommands.Add(commandName, _inactiveConsoleCommands[commandName]);
				_inactiveConsoleCommands.Remove(commandName);
#if UNITY_EDITOR
				_activeConsoleCommandsEDITOR.Add(commandName, _inactiveConsoleCommandsEDITOR[commandName]);
				_inactiveConsoleCommandsEDITOR.Remove(commandName);
#endif
			}
			else
			{
				throw new ArgumentException("GameConsole: Command not found");
			}
		}
		
		/// <summary>
		/// Removes a console command. Editor only.
		/// </summary>
		public void RemoveCommand(string commandName)
		{
			if (_activeConsoleCommands.ContainsKey(commandName))
			{
				_activeConsoleCommands.Remove(commandName);
#if UNITY_EDITOR
				_activeConsoleCommandsEDITOR.Remove(commandName);
#endif
			}
			else if (_inactiveConsoleCommands.ContainsKey(commandName))
			{
				_inactiveConsoleCommands.Remove(commandName);
#if UNITY_EDITOR
				_inactiveConsoleCommandsEDITOR.Remove(commandName);
#endif
			}
			else
			{
				throw new ArgumentException("GameConsole: Command not found");
			}
				
		}
	
		/// <summary>
		/// Input the specified inputString to the console.
		/// </summary>
		public void Input(string inputString)
		{
			if (_commandHistory.Count > _historySize)
			{
				_commandHistory.RemoveRange(0, _commandHistory.Count - _historySize);
			}
			_commandHistory.Add(inputString);
			
			string inputStringSanitized = inputString.Replace("$!error", "");
			if (context)
			{
				inputStringSanitized = inputStringSanitized.Replace("$this", "'" + context.name + "'");
			}
			else
			{
				inputStringSanitized = inputStringSanitized.Replace("$this", "");
			}
			string[] consoleInput = inputStringSanitized.Split(';');
			
			foreach (string s in consoleInput)
			{
				string command = FirstWord(s);
				string[] args = s.Substring(command.Length).Split(',');
				
				if (!string.IsNullOrEmpty(s.Replace(" ", "")) && !command.Contains("#") && !command.Contains("cd"))
				{
					if (_activeConsoleCommands.ContainsKey(command))
					{
						Echo(s, true);
						Echo(_activeConsoleCommands[command](args), false);
					}
					else
					{
						Echo(s, true);
						Throw("GameConsole: Invalid command specified <" + command + ">");
					}
					
				}
				else if (command.Contains("#"))
				{
					Echo(s, true);
				}
				else if (command.Contains("cd"))
				{
					Echo(s, true);
					ChangeContext(args);
				}
			}
		}
				
		void ChangeContext(params string[] args)
		{
			// Parse the string and test for malformed strings
			string arg = String.Join(",", args);
			
			if (arg.Length > 1)
				arg = arg.Remove(0, 1);
			
			if (args.Length != 1)
			{
				Throw("Invalid argument " + "<" + arg + ">");
				return;
			}
			
			if (args[0].Replace(" ", "") == "..")
			{
				// Parent
				if (context == null)
				{
				}
				else if (context.transform.parent != null)
				{
					_context = context.transform.parent.gameObject;
					_contextString = _contextString.Remove(_contextString.LastIndexOf("/"));
				}
				else
				{
					context = null;
				}
				return;
			}
			else if (args[0].Replace(" ", "") == "/")
			{
				// Root
				context = null;
				return;
			}
			
			if (arg.IndexOf("'") == -1)
			{
				ChangeContext_Process(args[0].Replace(" ", ""), arg);
				return;
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(0, arg.IndexOf("'")).Replace(" ", "")))
			{
				Throw("Invalid argument " + "<" + arg + ">");
				return;
			}
			
			if (!string.IsNullOrEmpty(arg.Substring(arg.LastIndexOf("'") + 1, arg.Length - 1 - arg.LastIndexOf("'")).Replace(" ", "")))
			{
				Throw("Invalid argument " + "<" + arg + ">");
				return;
			}
			
			Regex regName = new Regex("'(.*)'");
			Match match = regName.Match(arg);
			
			if (match.Success)
			{
				ChangeContext_Process(match.Groups[1].Value, arg);
				return;
			}
			else
			{
				Throw("Invalid argument " + "<" + arg + ">");
				return;
			}
		}
		
		void ChangeContext_Process(string result, string arg)
		{
			try
			{
				if (context == null && !result.StartsWith("/"))
				{
					// Already at root
					_context = GameObject.Find("/" + result);
					if (context != null)
					{
						_contextString = "/" + result;
						return;
					}
					else
					{
						throw new NullReferenceException();
					}
				}
				else if (result.StartsWith("/"))
				{
					// Search from root (use field)
					_context = GameObject.Find(result);
					if (context != null)
					{
						_contextString = "/" + result;
						return;
					}
					else
					{
						throw new NullReferenceException();
					}
				}
				else
				{
					// Search children (use property)
					context = context.transform.Find(result).gameObject;
				}
				
				if (context == null)
					throw new NullReferenceException();
			}
			catch
			{
				Throw("Unable to find GameObject " + "<" + arg + ">");
				return;
			}
		}
		// Load default commands. Editor only.
		public void LoadDefaults()
		{
			// Help
			AddCommand("help", "OpenGameConsole.CoreCommands.Help");
			AddCommand("man", "OpenGameConsole.CoreCommands.Help");
			
			// Console settings
			AddCommand("c_linespace", "OpenGameConsole.CoreCommands.LineSpace");
			AddCommand("c_history", "OpenGameConsole.CoreCommands.History");
			AddCommand("c_verbose", "OpenGameConsole.CoreCommands.Verbose");
			AddCommand("c_buffer", "OpenGameConsole.CoreCommands.Buffer");
			AddCommand("c_throwerrors", "OpenGameConsole.CoreCommands.ThrowErrors");
			
			// Clearing
			AddCommand("clear", "OpenGameConsole.CoreCommands.Clear");
			AddCommand("clr", "OpenGameConsole.CoreCommands.Clear");
			AddCommand("cls", "OpenGameConsole.CoreCommands.Clear");
			
			// Debugging
			AddCommand("log", "OpenGameConsole.CoreCommands.Log");
			AddCommand("print", "OpenGameConsole.CoreCommands.Print");
			AddCommand("memusage", "OpenGameConsole.CoreCommands.MemUsage");
			AddCommand("mem", "OpenGameConsole.CoreCommands.MemUsage");
			AddCommand("garbagecollect", "OpenGameConsole.CoreCommands.GarbageCollect");
			AddCommand("gc", "OpenGameConsole.CoreCommands.GarbageCollect");
		
			// GameObject stuff
			AddCommand("listobjects", "OpenGameConsole.CoreCommands.ListObjects");
			AddCommand("list", "OpenGameConsole.CoreCommands.ListObjects");
			AddCommand("ls", "OpenGameConsole.CoreCommands.ListObjects");
			AddCommand("li", "OpenGameConsole.CoreCommands.ListObjects");
			AddCommand("sendmessage", "OpenGameConsole.CoreCommands.SendMessage");
			AddCommand("send", "OpenGameConsole.CoreCommands.SendMessage");
			
			// Physics
			AddCommand("timescale", "OpenGameConsole.CoreCommands.TimeScale");
			AddCommand("time", "OpenGameConsole.CoreCommands.TimeScale");
			AddCommand("gravity", "OpenGameConsole.CoreCommands.Gravity");
			AddCommand("grav", "OpenGameConsole.CoreCommands.Gravity");
			AddCommand("loc", "OpenGameConsole.CoreCommands.Loc");
			AddCommand("move", "OpenGameConsole.CoreCommands.Move");
			AddCommand("mv", "OpenGameConsole.CoreCommands.Move");
			AddCommand("move_rb", "OpenGameConsole.CoreCommands.MoveRB");
			AddCommand("mv_rb", "OpenGameConsole.CoreCommands.MoveRB");
			
			// Random
			AddCommand("nyan", "OpenGameConsole.CoreCommands.NyanCat");
		}
//#endif

		/// <summary>
		/// Enables a console command. Editor only.
		/// </summary>
		public void EnableCommand(string commandName)
		{
			if (_activeConsoleCommands.ContainsKey(commandName))
			{
				return;
			}
			if (!_inactiveConsoleCommands.ContainsKey(commandName))
			{
				throw new ArgumentException("GameConsole: Command not found");
			}

			_inactiveConsoleCommands.Remove(commandName);
			_activeConsoleCommands.Add(commandName, _activeConsoleCommands[commandName]);
		}
		
		/// <summary>
		/// Disables a console command.
		/// </summary>
		public void DisableCommand(string commandName)
		{
			if (_inactiveConsoleCommands.ContainsKey(commandName))
			{
				return;
			}
			if (!_activeConsoleCommands.ContainsKey(commandName))
			{
				throw new ArgumentException("GameConsole: Command not found:");
			}
			_activeConsoleCommands.Remove(commandName);
			_inactiveConsoleCommands.Add(commandName, _activeConsoleCommands[commandName]);
		}
	
		
	}
}