using UnityEngine;
using System.Collections;

namespace ThinksquirrelSoftware.OpenGameConsole.Utility
{
	public static class ConsoleErrors
	{
		public const string InvalidArgumentError = "$!error$Invalid argument(s)";
		public const string VectorParsingError = "$!error$Vector parsing error";
		public const string CommandExecutionError = "$!error$Command execution error";
		
		public static string InvalidCommandError(string command)
		{
			return "$!error$Invalid command: <" + command + ">";
		}

		public static string ResourceNotFoundError(string resource)
		{
			return "$!error$Resource not found: <" + resource + ">";
		}
		
		public static string GameObjectNotFoundError(string gameObj)
		{
			return "$!error$GameObject not found: <" + gameObj + ">";
		}
		
		public static string SendMessageError(string message, string target)
		{
			return "$!error$Unable to send message <" + message + "> to <" + target + ">";
		}
	}
}
