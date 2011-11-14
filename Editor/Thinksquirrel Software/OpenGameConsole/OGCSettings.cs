using UnityEngine;
using UnityEditor;
using ThinksquirrelSoftware.OpenGameConsole;
using ThinksquirrelSoftware.OpenGameConsole.Utility;
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
	private string manPage = "";
	private string oldManPage = "";
	private Rect windowRect = new Rect(100, 100, 300, 200);
	private Rect areaRect;
	private Vector2 aScrollPosition;
	private bool activeFoldout = true;
	private bool inactiveFoldout = true;
	
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
		this.minSize = new Vector2(500, 350);
		if (Event.current != null)
		{	
			Repaint();
		}
		
		scrollPosition = GUILayout.BeginScrollView(
        scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
		
		if (aliasWindow)
			GUI.enabled = false;
			
		bool toggleCommand = false;
		bool removeCommand = false;
		string key = "";
		
		GUILayout.Space(4);
		activeFoldout = EditorGUILayout.Foldout(activeFoldout, "Active Console Commands:");
		if (activeFoldout)
		{
			GUILayout.Space(4);
			int i = 0;
			foreach (KeyValuePair<string, string> kvp in GameConsole.instance.activeConsoleCommandsEDITOR)
			{
				GUILayout.BeginHorizontal();
				
				if (i % 2 == 0)
					GUI.backgroundColor = Color.gray;
				else
					GUI.backgroundColor = Color.white;
				
				EditorGUILayout.SelectableLabel(kvp.Key, EditorStyles.textField, GUILayout.Width(100), GUILayout.Height(20));
				EditorGUILayout.SelectableLabel(kvp.Value, EditorStyles.textField, GUILayout.Height(20), GUILayout.ExpandWidth(true));
				if (GameConsole.instance.activeAliases.ContainsValue(kvp.Key) ||
					GameConsole.instance.inactiveAliases.ContainsValue(kvp.Key))
				{
					GUI.color = Color.cyan;
				}
				
				GUI.backgroundColor = Color.white;
				
				if (GUILayout.Button("i", EditorStyles.miniButton, GUILayout.Width(25), GUILayout.Height(20)))
				{
					aliasWindow = true;
					aliasKey = kvp.Key;
					manPage = OGCSerialization.LoadManPage(aliasKey);
					oldManPage = manPage;
					AssetDatabase.Refresh();
				}
				GUI.color = Color.white;
				if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(25), GUILayout.Height(20)))
				{
					toggleCommand = true;
					key = kvp.Key;
				}
				if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(25), GUILayout.Height(20)))
				{
					removeCommand = true;
					key = kvp.Key;
				}
				GUILayout.EndHorizontal();
				i++;
			}
			GUI.backgroundColor = Color.green;
			GUILayout.Space(16);
			GUILayout.BeginHorizontal();
			newKey = EditorGUILayout.TextField(newKey, GUILayout.Width(100), GUILayout.Height(20));
			newValue = EditorGUILayout.TextField(newValue, GUILayout.ExpandWidth(true), GUILayout.Height(20));
			GUI.color = Color.green;
			if (GUILayout.Button("Add", GUILayout.Width(84)))
			{
				GameConsole.instance.AddCommand(newKey, newValue);
				newKey = "New Command";
				newValue = "Method Name";
			}
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
		}
		
		GUILayout.Space(32);
		if (GameConsole.instance.inactiveConsoleCommands.Count > 0)
		{
			inactiveFoldout = EditorGUILayout.Foldout(inactiveFoldout, "Inactive Console Commands:");
		}
		if (inactiveFoldout)
		{
			GUILayout.Space(4);
			int i = 0;
			foreach (KeyValuePair<string, string> kvp in GameConsole.instance.inactiveConsoleCommandsEDITOR)
			{
				if (i % 2 == 0)
					GUI.backgroundColor = Color.gray;
				else
					GUI.backgroundColor = Color.white;
					
				GUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(kvp.Key, EditorStyles.textField, GUILayout.Width(100), GUILayout.Height(20));
				EditorGUILayout.SelectableLabel(kvp.Value, EditorStyles.textField, GUILayout.Height(20), GUILayout.ExpandWidth(true));
				if (GameConsole.instance.activeAliases.ContainsValue(kvp.Key) ||
					GameConsole.instance.inactiveAliases.ContainsValue(kvp.Key))
				{
					GUI.color = Color.cyan;
				}
				
				GUI.backgroundColor = Color.white;
				
				if (GUILayout.Button("i", EditorStyles.miniButton, GUILayout.Width(25), GUILayout.Height(20)))
				{
					aliasWindow = true;
					aliasKey = kvp.Key;
					manPage = OGCSerialization.LoadManPage(aliasKey);
					oldManPage = manPage;
					AssetDatabase.Refresh();
				}
				GUI.color = Color.white;
				if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(25), GUILayout.Height(20)))
				{
					toggleCommand = true;
					key = kvp.Key;
				}
				if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(25), GUILayout.Height(20)))
				{
					removeCommand = true;
					key = kvp.Key;
				}
				i++;
				GUILayout.EndHorizontal();
			}
		}
		GUI.color = Color.white;
		GUI.backgroundColor = Color.white;
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
				Repaint();
			}
		}
		
		if (aliasWindow)
			GUI.enabled = true;
			
		GUILayout.EndScrollView();
		
		if (aliasWindow)
		{
			GUI.depth = -100;
			// Begin Window
			BeginWindows();
			
			// All GUI.Window or GUILayout.Window must come inside here
			windowRect = ClampToParentWindow(GUILayout.Window(1, windowRect, DoWindow, aliasKey), position);        
			
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
		
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Label("Aliases:");
		GUILayout.Space(4);
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
			newAliasKey = "New Alias";
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Help Text:");
		GUILayout.Space(4);
		manPage = GUILayout.TextArea(manPage, GUILayout.MinWidth(300), GUILayout.MinHeight(200));
		GUILayout.BeginHorizontal();
		if (manPage == oldManPage)
		{
			GUI.enabled = false;
		}
		if (GUILayout.Button("Apply"))
		{
			OGCSerialization.SaveManPage(aliasKey, manPage);
			oldManPage = manPage;
			AssetDatabase.Refresh();
		}
		if (GUILayout.Button("Revert"))
		{
			manPage = OGCSerialization.LoadManPage(aliasKey);
		}
		GUI.enabled = true;
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
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
