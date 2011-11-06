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
			DictionaryWrapper activeWrapper = new DictionaryWrapper(GameConsole.instance.activeConsoleCommandsEDITOR);
			DictionaryWrapper inactiveWrapper = new DictionaryWrapper(GameConsole.instance.inactiveConsoleCommandsEDITOR);
			
			XmlSerializer serializer = new XmlSerializer(typeof(DictionaryWrapper));
			TextWriter textWriterActive = new StreamWriter(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/active-commands.xml");
			serializer.Serialize(textWriterActive, activeWrapper);
			textWriterActive.Close();
			
			TextWriter textWriterInactive = new StreamWriter(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/inactive-commands.xml");
			serializer.Serialize(textWriterInactive, inactiveWrapper);
			textWriterInactive.Close();
		}
		
		public static void LoadData()
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(DictionaryWrapper));
			
			TextReader textReaderActive = new StreamReader(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/active-commands.xml");
			DictionaryWrapper activeWrapper = (DictionaryWrapper)deserializer.Deserialize(textReaderActive);
			textReaderActive.Close();
			
			TextReader textReaderInactive = new StreamReader(Application.dataPath + "/Plugins/Thinksquirrel Software/OpenGameConsole/DB/inactive-commands.xml");
			DictionaryWrapper inactiveWrapper = (DictionaryWrapper)deserializer.Deserialize(textReaderInactive);
			textReaderInactive.Close();
			
			Dictionary<string, string> active = activeWrapper.GetMap();
			Dictionary<string, string> inactive = inactiveWrapper.GetMap();
			
			foreach (KeyValuePair<string, string> kvp in active)
			{
				GameConsole.instance.AddCommand(kvp.Key, kvp.Value);
			}
			foreach (KeyValuePair<string, string> kvp in inactive)
			{
				GameConsole.instance.AddCommand(kvp.Key, kvp.Value);
				GameConsole.instance.ToggleCommand(kvp.Key);
			}
		}
	}
}