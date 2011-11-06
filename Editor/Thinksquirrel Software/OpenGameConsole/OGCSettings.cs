using UnityEngine;
using UnityEditor;
using OpenGameConsole;
using System.Collections;
using System.Collections.Generic;

public class OGCSettings : EditorWindow
{
	private Vector2 scrollPosition;
	private string newKey = "New Command";
	private string newValue = "Method Name";
	
	[MenuItem ("Window/OpenGameConsole Settings")]
    static void Init()
	{
		EditorWindow.GetWindow(typeof(OGCSettings), false, "OGC Settings");
	}
	
	void OnGUI()
	{
		if (Event.current != null)
		{	
			Repaint();
		}
		
		scrollPosition = GUILayout.BeginScrollView(
        scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
		
		bool toggleCommand = false;
		bool removeCommand = false;
		string key = "";
		
		GUILayout.Space(4);
		GUILayout.Label("Active Console Commands:");
		GUILayout.Space(4);
		foreach (KeyValuePair<string, string> kvp in GameConsole.instance.activeConsoleCommandsEDITOR)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(kvp.Key, GUILayout.Width(100));
			GUILayout.Label(kvp.Value);
			GUI.color = Color.yellow;
			if (GUILayout.Button("-", GUILayout.Width(25)))
			{
				toggleCommand = true;
				key = kvp.Key;
			}
			GUI.color = Color.red;
			if (GUILayout.Button("x", GUILayout.Width(25)))
			{
				removeCommand = true;
				key = kvp.Key;
			}
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
		}
		
		GUILayout.BeginHorizontal();
		
		newKey = GUILayout.TextField(newKey, GUILayout.Width(100));
		newValue = GUILayout.TextField(newValue);
		GUI.color = Color.green;
		if (GUILayout.Button("++", GUILayout.Width(54)))
		{
			GameConsole.instance.AddCommand(newKey, newValue);
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
		
		GUILayout.Space(4);
		GUILayout.Label("Inactive Console Commands:");
		GUILayout.Space(4);
		foreach (KeyValuePair<string, string> kvp in GameConsole.instance.inactiveConsoleCommandsEDITOR)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(kvp.Key, GUILayout.Width(100));
			GUILayout.Label(kvp.Value);
			GUI.color = Color.green;
			if (GUILayout.Button("+", GUILayout.Width(25)))
			{
				toggleCommand = true;
				key = kvp.Key;
			}
			GUI.color = Color.red;
			if (GUILayout.Button("x", GUILayout.Width(25)))
			{
				removeCommand = true;
				key = kvp.Key;
			}
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
		}
		
		if (toggleCommand)
		{
			GameConsole.instance.ToggleCommand(key);
			OGCSerialization.SaveData();
			AssetDatabase.Refresh();
		}
		else if (removeCommand)
		{
			GameConsole.instance.RemoveCommand(key);
			OGCSerialization.SaveData();
			AssetDatabase.Refresh();
		}
		
		if (GUILayout.Button("Load Defaults"))
		{
			GameConsole.instance.LoadDefaults();
			OGCSerialization.SaveData();
			AssetDatabase.Refresh();
		}
		GUILayout.EndScrollView();
	}
}
