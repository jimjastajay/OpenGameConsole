using UnityEngine;
using UnityEditor;
using OpenGameConsole;
using System.Collections;
using System.Collections.Generic;

public class OGCSettings : EditorWindow
{
	private Vector2 scrollPosition;
	private bool aliasWindow = false;
	private string aliasKey = "";
	private string newKey = "New Command";
	private string newValue = "Method Name";
	private string newAliasKey = "New Alias";
	private Rect windowRect = new Rect(100, 100, 300, 200);
	private Rect areaRect;
	private Vector2 aScrollPosition;
	
	[MenuItem ("Window/OpenGameConsole Settings")]
    static void Init()
	{
		EditorWindow.GetWindow(typeof(OGCSettings), false, "OGC Settings");
	}
	
	private Rect ClampToParentWindow(Rect r, Rect parent)
	{
		r.x = Mathf.Clamp(r.x, 0, parent.width - r.width);
		r.y = Mathf.Clamp(r.y, 0, parent.height - r.height);
		return r;
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
			if (GameConsole.instance.activeAliases.ContainsValue(kvp.Key) ||
				GameConsole.instance.inactiveAliases.ContainsValue(kvp.Key))
			{
				GUI.color = Color.cyan;
			}
			if (GUILayout.Button("A", GUILayout.Width(25)))
			{
				aliasWindow = true;
				aliasKey = kvp.Key;
			}
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
		if (GUILayout.Button("++", GUILayout.Width(84)))
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
			if (GameConsole.instance.activeAliases.ContainsValue(kvp.Key) ||
				GameConsole.instance.inactiveAliases.ContainsValue(kvp.Key))
			{
				GUI.color = Color.cyan;
			}
			if (GUILayout.Button("A", GUILayout.Width(25)))
			{
				aliasWindow = true;
				aliasKey = kvp.Key;
			}
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
			if (EditorUtility.DisplayDialog("Load Default Commands?", "Are you sure you want to load the default commands? This will delete ALL current commands.", "OK", "Cancel"))
			{
				GameConsole.instance.LoadDefaults();
				OGCSerialization.SaveData();
				AssetDatabase.Refresh();
			}
		}
		
		GUILayout.EndScrollView();
		
		if (aliasWindow)
		{
			// Begin Window
			BeginWindows();
			
			// All GUI.Window or GUILayout.Window must come inside here
			windowRect = ClampToParentWindow(GUILayout.Window(1, windowRect, DoWindow, "Aliases: " + aliasKey), position);        
			
			// Collect all the windows between the two.
			EndWindows();
			
			windowRect = new Rect(windowRect.x, windowRect.y, areaRect.width, areaRect.height);
		}
	}
	
	void DoWindow(int windowID)
	{
		areaRect = EditorGUILayout.BeginVertical();
		
		bool removeAlias = false;
		string key = "";
		
		foreach (KeyValuePair<string, string> kvp in GameConsole.instance.activeAliases)
		{
			if (kvp.Value == aliasKey)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(kvp.Key, GUILayout.Width(100));
				GUI.color = Color.red;
				if (GUILayout.Button("x", GUILayout.Width(25)))
				{
					removeAlias = true;
					key = kvp.Key;
				}
				GUI.color = Color.white;
				GUILayout.EndHorizontal();
			}
		}
		foreach (KeyValuePair<string, string> kvp in GameConsole.instance.inactiveAliases)
		{
			if (kvp.Value == aliasKey)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(kvp.Key, GUILayout.Width(100));
				GUI.color = Color.red;
				if (GUILayout.Button("x", GUILayout.Width(25)))
				{
					removeAlias = true;
					key = kvp.Key;
				}
				GUI.color = Color.white;
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.BeginHorizontal();
		newAliasKey = GUILayout.TextField(newAliasKey, GUILayout.Width(100));
		GUI.color = Color.green;
		if (GUILayout.Button("+", GUILayout.Width(25)))
		{
			GameConsole.instance.AddAlias(newAliasKey, aliasKey);
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
		
		if (GUILayout.Button("Close"))
		{
			aliasWindow = false;
		}
		
		GUILayout.EndVertical();
		GUI.DragWindow(); 
		
		if (removeAlias)
		{
			GameConsole.instance.RemoveAlias(key);
		}
	}
}
