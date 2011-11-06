using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OpenGameConsole
{
	public static class OGCSerialization
	{
		public static void SaveData()
		{
			try
			{
				DictionaryWrapper activeWrapper = new DictionaryWrapper(GameConsole.instance.activeConsoleCommandsEDITOR);
				DictionaryWrapper inactiveWrapper = new DictionaryWrapper(GameConsole.instance.inactiveConsoleCommandsEDITOR);
				
				DictionaryWrapper activeAliasWrapper = new DictionaryWrapper(GameConsole.instance.activeAliases);
				DictionaryWrapper inactiveAliasWrapper = new DictionaryWrapper(GameConsole.instance.inactiveAliases);
				
				XmlSerializer serializer = new XmlSerializer(typeof(DictionaryWrapper));
				
				TextWriter textWriterActive = new StreamWriter(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/active-commands.xml");
				serializer.Serialize(textWriterActive, activeWrapper);
				textWriterActive.Close();
				
				TextWriter textWriterInactive = new StreamWriter(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/inactive-commands.xml");
				serializer.Serialize(textWriterInactive, inactiveWrapper);
				textWriterInactive.Close();
				
				TextWriter textWriterActiveAlias = new StreamWriter(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/active-aliases.xml");
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
		}
		
		public static void LoadData()
		{
			try
			{
				XmlSerializer deserializer = new XmlSerializer(typeof(DictionaryWrapper));
				
				TextReader textReaderActive = new StreamReader(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/active-commands.xml");
				DictionaryWrapper activeWrapper = (DictionaryWrapper)deserializer.Deserialize(textReaderActive);
				textReaderActive.Close();
				
				TextReader textReaderInactive = new StreamReader(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/inactive-commands.xml");
				DictionaryWrapper inactiveWrapper = (DictionaryWrapper)deserializer.Deserialize(textReaderInactive);
				textReaderInactive.Close();
				
				TextReader textReaderActiveAlias = new StreamReader(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/active-aliases.xml");
				DictionaryWrapper activeAliasWrapper = (DictionaryWrapper)deserializer.Deserialize(textReaderActiveAlias);
				textReaderActiveAlias.Close();
				
				TextReader textReaderInactiveAlias = new StreamReader(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/inactive-aliases.xml");
				DictionaryWrapper inactiveAliasWrapper = (DictionaryWrapper)deserializer.Deserialize(textReaderInactiveAlias);
				textReaderInactiveAlias.Close();
				
				Dictionary<string, string> active = activeWrapper.GetMap();
				Dictionary<string, string> inactive = inactiveWrapper.GetMap();
				
				Dictionary<string, string> activeAlias = activeAliasWrapper.GetMap();
				Dictionary<string, string> inactiveAlias = inactiveAliasWrapper.GetMap();
				
				foreach (KeyValuePair<string, string> kvp in active)
				{
					GameConsole.instance.AddCommand(kvp.Key, kvp.Value);
				}
				foreach (KeyValuePair<string, string> kvp in inactive)
				{
					GameConsole.instance.AddCommand(kvp.Key, kvp.Value);
					GameConsole.instance.ToggleCommand(kvp.Key);
				}
				foreach (KeyValuePair<string, string> kvp in activeAlias)
				{
					GameConsole.instance.AddAlias(kvp.Key, kvp.Value);
				}
				foreach (KeyValuePair<string, string> kvp in inactiveAlias)
				{
					GameConsole.instance.AddAlias(kvp.Key, kvp.Value);
				}
			}
			catch
			{
				Debug.LogError("OpenGameConsole: Unable to load data!");
			}
		}
	}
}