using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ThinksquirrelSoftware.OpenGameConsole.Utility
{
	public static class OGCSerialization
	{	
		public static void SaveData()
		{
#if UNITY_EDITOR
			try
			{
				DictionaryWrapper activeWrapper = new DictionaryWrapper(GameConsole.instance.activeConsoleCommandsEDITOR);
				DictionaryWrapper inactiveWrapper = new DictionaryWrapper(GameConsole.instance.inactiveConsoleCommandsEDITOR);
				
				DictionaryWrapper activeAliasWrapper = new DictionaryWrapper(GameConsole.instance.activeAliases);
				DictionaryWrapper inactiveAliasWrapper = new DictionaryWrapper(GameConsole.instance.inactiveAliases);
				
				XmlSerializer serializer = new XmlSerializer(typeof(DictionaryWrapper));
				
				TextWriter textWriterActive = new StreamWriter(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/Resources/active-commands.xml");
				serializer.Serialize(textWriterActive, activeWrapper);
				textWriterActive.Close();
				
				TextWriter textWriterInactive = new StreamWriter(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/inactive-commands.xml");
				serializer.Serialize(textWriterInactive, inactiveWrapper);
				textWriterInactive.Close();
				
				TextWriter textWriterActiveAlias = new StreamWriter(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/Resources/active-aliases.xml");
				serializer.Serialize(textWriterActiveAlias, activeAliasWrapper);
				textWriterActiveAlias.Close();
				
				TextWriter textWriterInactiveAlias = new StreamWriter(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/inactive-aliases.xml");
				serializer.Serialize(textWriterInactiveAlias, inactiveAliasWrapper);
				textWriterInactiveAlias.Close();
			}
			catch
			{
				Debug.LogError("OpenGameConsole: Unable to save data!");
			}
#endif
		}
		
		public static void LoadData()
		{
			try
			{
				XmlSerializer deserializer = new XmlSerializer(typeof(DictionaryWrapper));
				
				TextAsset activeText = Resources.Load("active-commands", typeof(TextAsset)) as TextAsset;
				TextReader textReaderActive = new StringReader(activeText.text);
				DictionaryWrapper activeWrapper = (DictionaryWrapper)deserializer.Deserialize(textReaderActive);
				textReaderActive.Close();
#if UNITY_EDITOR
				TextReader textReaderInactive = new StreamReader(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/inactive-commands.xml");
				DictionaryWrapper inactiveWrapper = (DictionaryWrapper)deserializer.Deserialize(textReaderInactive);
				textReaderInactive.Close();
#endif			
				TextAsset activeTextAlias = Resources.Load("active-aliases", typeof(TextAsset)) as TextAsset;
				TextReader textReaderActiveAlias = new StringReader(activeTextAlias.text);
				DictionaryWrapper activeAliasWrapper = (DictionaryWrapper)deserializer.Deserialize(textReaderActiveAlias);
				textReaderActiveAlias.Close();
#if UNITY_EDITOR
				TextReader textReaderInactiveAlias = new StreamReader(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/inactive-aliases.xml");
				DictionaryWrapper inactiveAliasWrapper = (DictionaryWrapper)deserializer.Deserialize(textReaderInactiveAlias);
				textReaderInactiveAlias.Close();
#endif				
				Dictionary<string, string> active = activeWrapper.GetMap();
#if UNITY_EDITOR
				Dictionary<string, string> inactive = inactiveWrapper.GetMap();
#endif				
				Dictionary<string, string> activeAlias = activeAliasWrapper.GetMap();
#if UNITY_EDITOR
				Dictionary<string, string> inactiveAlias = inactiveAliasWrapper.GetMap();
#endif				
				foreach (KeyValuePair<string, string> kvp in active)
				{
					GameConsole.instance.AddCommand(kvp.Key, kvp.Value);
				}
#if UNITY_EDITOR
				foreach (KeyValuePair<string, string> kvp in inactive)
				{
					GameConsole.instance.AddCommand(kvp.Key, kvp.Value);
					GameConsole.instance.ToggleCommand(kvp.Key);
				}
#endif
				foreach (KeyValuePair<string, string> kvp in activeAlias)
				{
					GameConsole.instance.AddAlias(kvp.Key, kvp.Value);
				}
#if UNITY_EDITOR
				foreach (KeyValuePair<string, string> kvp in inactiveAlias)
				{
					GameConsole.instance.AddAlias(kvp.Key, kvp.Value);
				}
#endif
			}
			catch
			{
				Debug.LogError("OpenGameConsole: Unable to load data!");
			}
		}
	
		public static void SaveManPage(string command, string text)
		{
			try
			{
				StreamWriter sw = new StreamWriter(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/Resources/man_" + command + ".txt");
				sw.Write(text);
				sw.Close();
			}
			catch
			{
				Debug.LogError("OpenGameConsole: Unable to save/create manual page!");
			}
		}
		
		public static string LoadManPage(string command)
		{
			StreamReader sr;
			try
			{
				sr = new StreamReader(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/Resources/man_" + command + ".txt");
				string s = sr.ReadToEnd();
				sr.Close();
				return s;
			}
			catch
			{
				SaveManPage(command, "");
				try
				{
					sr = new StreamReader(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/Resources/man_" + command + ".txt");
					string s = sr.ReadToEnd();
					sr.Close();
					return s;
				}
				catch
				{
					Debug.LogError("OpenGameConsole: Unable to read manual page!");
					return "";
				}
			}
		}
	
		public static void SaveConsolePrefs()
		{
			PlayerPrefs.SetInt("OGC_bufferSize", GameConsole.instance.bufferSize);
			PlayerPrefs.SetInt("OGC_historySize", GameConsole.instance.historySize);
			PlayerPrefs.SetInt("OGC_streamSpacing", GameConsole.instance.streamSpacing);
			
			PlayerPrefs.SetInt("OGC_throwErrors", System.Convert.ToInt32(GameConsole.instance.throwErrors));
			PlayerPrefs.SetInt("OGC_verbose", System.Convert.ToInt32(GameConsole.instance.verbose));
			
			PlayerPrefs.SetInt("OGC_savedPrefs", 1);
		}
		
		public static void LoadConsolePrefs()
		{
			if (!PlayerPrefs.HasKey("OGC_savedPrefs"))
			{
				SaveConsolePrefs();
			}
			
			GameConsole.instance.bufferSize = PlayerPrefs.GetInt("OGC_bufferSize");
			GameConsole.instance.historySize = PlayerPrefs.GetInt("OGC_historySize");
			GameConsole.instance.streamSpacing = PlayerPrefs.GetInt("OGC_streamSpacing");
			
			GameConsole.instance.throwErrors = System.Convert.ToBoolean(PlayerPrefs.GetInt("OGC_throwErrors"));
			GameConsole.instance.verbose = System.Convert.ToBoolean(PlayerPrefs.GetInt("OGC_verbose"));
		}
	}
}