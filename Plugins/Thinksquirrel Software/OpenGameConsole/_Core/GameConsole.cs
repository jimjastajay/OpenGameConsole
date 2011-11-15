using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using ThinksquirrelSoftware.OpenGameConsole.Utility;

namespace ThinksquirrelSoftware.OpenGameConsole
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
		// Alias Dictionaries
		private Dictionary<string, string> _activeAliases = new Dictionary<string, string>();

#if UNITY_EDITOR
		private Dictionary<string, string> _inactiveAliases = new Dictionary<string, string>();
#endif		
		public Dictionary<string, string> activeAliases
		{
			get
			{
				return this._activeAliases;
			}
		}
		
		// Dictionaries
		private Dictionary<string, Func<string[], string>> _activeConsoleCommands = new Dictionary<string, Func<string[], string>>();

#if UNITY_EDITOR
		private Dictionary<string, Func<string[], string>> _inactiveConsoleCommands = new Dictionary<string, Func<string[], string>>();
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
#if UNITY_EDITOR				
				return this._inactiveConsoleCommands;
#else
				return null;
#endif
			}
		}
		
#if UNITY_EDITOR
		
		// Dictionaries in string format - for security reasons this should only be visible in the editor.
		private Dictionary<string, string> _activeConsoleCommandsEDITOR = new Dictionary<string, string>();
		private Dictionary<string, string> _inactiveConsoleCommandsEDITOR = new Dictionary<string, string>();
#endif			
		public Dictionary<string, string> activeConsoleCommandsEDITOR
		{
			get
			{
#if UNITY_EDITOR				
				return this._activeConsoleCommandsEDITOR;
#else
				return null;
#endif	
			}
		}
		
		public Dictionary<string, string> inactiveConsoleCommandsEDITOR
		{
			get
			{
#if UNITY_EDITOR				
				return this._inactiveConsoleCommandsEDITOR;
#else
				return null;
#endif	
			}
		}

		public Dictionary<string, string> inactiveAliases
		{
			get
			{
#if UNITY_EDITOR				
				return this._inactiveAliases;
#else
				return null;
#endif	
			}
		}
		
		// Constructor
		GameConsole()
		{
			_instance = this;
			OGCSerialization.LoadData();
			OGCSerialization.LoadConsolePrefs();
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
			Echo("$!error$" + message, false);
		}
			
		// Echo (also records to stream)
		public void Echo(string message, bool input)
		{
			if (!string.IsNullOrEmpty(message))
			{
				bool error = false;
				if (message.StartsWith("$!error$"))
				{
					error = true;
					message = message.Remove(0, 8);
				}
				string separator = "";
				string date = "";
				
				if (input)
				{
					separator = "  > ";
					date = DateTime.Now.ToString();
				}
			
				if (error)
					separator = "[Error] ";
			
				string newMessage = date + separator + message;
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
		/// Adds a console command.
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
#if UNITY_EDITOR				
				_inactiveConsoleCommands.ContainsKey(commandName) ||
#endif
				_activeAliases.ContainsKey(commandName) ||
#if UNITY_EDITOR
				_inactiveAliases.ContainsKey(commandName) ||
#endif
				commandName.Contains("#") ||
				commandName.Contains("cd") ||
				commandName.Contains("$THIS"))
			{
				throw new ArgumentException("GameConsole: Command error/already exists");
			}
			_activeConsoleCommands.Add(commandName, GetCommand(command));
#if UNITY_EDITOR
			_activeConsoleCommandsEDITOR.Add(commandName, command);
#endif
		}
		
#if UNITY_EDITOR		
		/// <summary>
		/// Activates/Deactivates a console command. Editor only.
		/// </summary>
		public void ToggleCommand(string commandName)
		{
			if (_activeConsoleCommands.ContainsKey(commandName))
			{
				// Check for aliases
				if (_activeAliases.ContainsValue(commandName))
				{
					List<string> aliasesToRemove = new List<string>();
					foreach (KeyValuePair<string, string> kvp in _activeAliases)
					{
						if (kvp.Value == commandName)
						{
							aliasesToRemove.Add(kvp.Key);
						}
					}
					foreach (string s in aliasesToRemove)
					{
						_inactiveAliases.Add(s, commandName);
						_activeAliases.Remove(s);
					}
				}
				_inactiveConsoleCommands.Add(commandName, _activeConsoleCommands[commandName]);
				_activeConsoleCommands.Remove(commandName);
				_inactiveConsoleCommandsEDITOR.Add(commandName, _activeConsoleCommandsEDITOR[commandName]);
				_activeConsoleCommandsEDITOR.Remove(commandName);	
			}
			else if (_inactiveConsoleCommands.ContainsKey(commandName))
			{
				// Check for aliases
				if (_inactiveAliases.ContainsValue(commandName))
				{
					List<string> aliasesToRemove = new List<string>();
					foreach (KeyValuePair<string, string> kvp in _inactiveAliases)
					{
						if (kvp.Value == commandName)
						{
							aliasesToRemove.Add(kvp.Key);
						}
					}
					foreach (string s in aliasesToRemove)
					{
						_activeAliases.Add(s, commandName);
						_inactiveAliases.Remove(s);
					}
				}
				_activeConsoleCommands.Add(commandName, _inactiveConsoleCommands[commandName]);
				_inactiveConsoleCommands.Remove(commandName);
				_activeConsoleCommandsEDITOR.Add(commandName, _inactiveConsoleCommandsEDITOR[commandName]);
				_inactiveConsoleCommandsEDITOR.Remove(commandName);
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
				// Check for aliases
				if (_activeAliases.ContainsValue(commandName))
				{
					List<string> aliasesToRemove = new List<string>();
					foreach (KeyValuePair<string, string> kvp in _activeAliases)
					{
						if (kvp.Value == commandName)
						{
							aliasesToRemove.Add(kvp.Key);
						}
					}
					foreach (string s in aliasesToRemove)
					{
						_activeAliases.Remove(s);
					}
				}
				_activeConsoleCommands.Remove(commandName);
				_activeConsoleCommandsEDITOR.Remove(commandName);
			}
			else if (_inactiveConsoleCommands.ContainsKey(commandName))
			{
				// Check for aliases
				if (_inactiveAliases.ContainsValue(commandName))
				{
					List<string> aliasesToRemove = new List<string>();
					foreach (KeyValuePair<string, string> kvp in _inactiveAliases)
					{
						if (kvp.Value == commandName)
						{
							aliasesToRemove.Add(kvp.Key);
						}
					}
					foreach (string s in aliasesToRemove)
					{
						_inactiveAliases.Remove(s);
					}
				}
				_inactiveConsoleCommands.Remove(commandName);
				_inactiveConsoleCommandsEDITOR.Remove(commandName);
			}
			else
			{
				throw new ArgumentException("GameConsole: Command not found");
			}		
		}
		
		/// <summary>
		/// Load the default commands. Editor only.
		/// </summary>
		public void LoadDefaults()
		{
			_activeConsoleCommands = new Dictionary<string, Func<string[], string>>();
			_inactiveConsoleCommands = new Dictionary<string, Func<string[], string>>();
			
			_activeConsoleCommandsEDITOR = new Dictionary<string, string>();
			_inactiveConsoleCommandsEDITOR = new Dictionary<string, string>();
			
			_activeAliases = new Dictionary<string, string>();
			_inactiveAliases = new Dictionary<string, string>();
			
			// Version
			AddCommand("version", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.Version");
			
			// Help
			AddCommand("man", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.Help");
			
			// Console settings
			AddCommand("ogc", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.ConsoleSettings");
			
			// Clearing
			AddCommand("clear", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.Clear");
			AddAlias("clr", "clear");
			AddAlias("cls", "clear");
			
			// Debugging
			AddCommand("echo", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.Echo");
			AddCommand("gc", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.GarbageCollect");
		
			// GameObject stuff
			AddCommand("listobjects", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.ListObjects");
			AddAlias("list", "listobjects");
			AddAlias("ls", "listobjects");
			AddAlias("li", "listobjects");
			AddCommand("send", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.SendMessage");
			AddCommand("edit", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.Edit");
			
			// Get and Run
			AddCommand("get", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.Get");
			AddAlias("download", "get");
			AddAlias("dl", "get");
			AddCommand("get2", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.Get2");
			
			// Physics
			AddCommand("physics", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.PhysicsSettings");
			
			// Time
			AddCommand("time", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.TimeSettings");
			
			// Loc
			AddCommand("loc", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.Loc");
			
			// Move
			AddCommand("mv", "ThinksquirrelSoftware.OpenGameConsole.CoreCommands.Move");
		}
#endif
		
		/// <summary>
		/// Adds an alias.
		/// </summary>
		public void AddAlias(string aliasName, string commandName)
		{
			if (_activeAliases.ContainsKey(aliasName) ||
#if UNITY_EDITOR
				_inactiveAliases.ContainsKey(aliasName) ||
#endif
				_activeConsoleCommands.ContainsKey(aliasName) ||
#if UNITY_EDITOR
				_inactiveConsoleCommands.ContainsKey(aliasName) ||
#endif
				aliasName.Contains("#") ||
				aliasName.StartsWith("cd") ||
				aliasName.Contains("$THIS") ||
				commandName.Contains("#") ||
				commandName.StartsWith("cd") ||
				commandName.Contains("$THIS"))
			{
				throw new ArgumentException("GameConsole: Alias error/already exists");
			}
			
			// Make sure command exists
			if (_activeConsoleCommands.ContainsKey(commandName))
			{
				_activeAliases.Add(aliasName, commandName);
			}
#if UNITY_EDITOR
			else if (_inactiveConsoleCommands.ContainsKey(commandName))
			{
				_inactiveAliases.Add(aliasName, commandName);
			}
#endif
			else
			{
				throw new ArgumentException("GameConsole: Alias error - command not found");
			}
		}

		/// <summary>
		/// Removes an alias.
		/// </summary>
		public void RemoveAlias(string aliasName)
		{
			if (_activeAliases.ContainsKey(aliasName))
			{
				_activeAliases.Remove(aliasName);
			}
#if UNITY_EDITOR
			else if (_inactiveAliases.ContainsKey(aliasName))
			{
				_inactiveAliases.Remove(aliasName);
			}
#endif
			else
			{
				throw new ArgumentException("GameConsole: Alias not found");
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
			
			inputString = inputString.Sanitize();
			
			string[] consoleInput = inputString.ParseLines();
			
			foreach (string s in consoleInput)
			{
				string command = s.ParseCommand();
				string[] args = s.ParseArguments();
				
				if (!s.IsBlankCommand() && !command.StartsWith("#") && !command.StartsWith("cd"))
				{
					if (_activeAliases.ContainsKey(command))
					{
						Echo(s, true);
						
						if (context)
						{
							args.ParseTokens("THIS", context.name);
						}
						else
						{
							args.ParseTokens("THIS", null);
						}
						
						Echo(_activeConsoleCommands[_activeAliases[command]](args), false);
					}
					else if (_activeConsoleCommands.ContainsKey(command))
					{
						Echo(s, true);
						if (context)
						{
							args.ParseTokens("THIS", context.name);
						}
						else
						{
							args.ParseTokens("THIS", null);
						}
						Echo(_activeConsoleCommands[command](args), false);
					}
					else
					{
						Echo(s, true);
						Echo(ConsoleErrors.InvalidCommandError(command), false);
					}
					
				}
				else if (command.StartsWith("#"))
				{
					Echo(s, true);
				}
				else if (command.StartsWith("cd.."))
				{
					Echo(s, true);
					ChangeContext("..");
				}
				else if (command.StartsWith("cd"))
				{
					Echo(s, true);
					if (context)
					{
						args.ParseTokens("THIS", context.name);
					}
					else
					{
						args.ParseTokens("THIS", null);
					}
					ChangeContext(args);
				}
			}
		}
				
		void ChangeContext(params string[] args)
		{
			if (args.Length != 1)
			{
				Echo(ConsoleErrors.InvalidArgumentError, true);
				return;
			}
			
			if (args[0] == "..")
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
			else if (args[0] == "/")
			{
				// Root
				context = null;
				return;
			}
			
			ChangeContext_Process(args[0]);
			return;
		}
		
		void ChangeContext_Process(string result)
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
						_contextString = result;
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
				Throw("Unable to find GameObject " + "<" + result + ">");
				return;
			}
		}
	}
}