using UnityEngine;
using UnityEditor;
using System;
using ThinksquirrelSoftware.OpenGameConsole;
using ThinksquirrelSoftware.OpenGameConsole.Utility;
using System.Collections;

public class EditorConsoleDisplay : EditorWindow
{
	private Rect windowRect;
	private Vector2 scrollPosition;
	private string currentText = "";
	private int commandPointer = - 1;
	private static Color bgColor = Color.black;
	private static Color fgColor = Color.white;
	private static Font font;
	private GUISkin newSkin;
	private GUIStyle fieldStyle;
	
	[MenuItem ("Window/OpenGameConsole")]
    static void Init()
	{
		EditorWindow.GetWindow(typeof(EditorConsoleDisplay), false, "OGC Terminal");
	}
	
	void OnEnable()
	{
		font = EditorGUIUtility.Load("Thinksquirrel Software/OpenGameConsole/Font/ConsolaMono.ttf") as Font;
		LoadEditorPrefs();
	}
	
	void OnGUI()
	{	
		newSkin = GUI.skin;
		
		fieldStyle = newSkin.GetStyle("TextField");
		
		fieldStyle.font = font;
		fieldStyle.wordWrap = false;
		
		GUI.backgroundColor = bgColor;
		GUI.contentColor = fgColor;
		GUI.SetNextControlName("Dummy Button");
		GUI.depth = -100;
		GUI.color = new Color(1, 1, 1, 2);
		GUI.enabled = false;
		GUI.Button(new Rect(0, 14, position.width, position.height - 14), "");
		GUI.enabled = true;
		GUI.color = Color.white;
		GUI.depth = 100;
		if (GUI.GetNameOfFocusedControl() == "Dummy Button")
		{
			GUI.FocusControl("Current Text");
		}
		GUI.backgroundColor = Color.white;

		GUILayout.BeginVertical("Toolbar");

		GUILayout.BeginHorizontal();
		GUILayout.Space(4);

		GUI.backgroundColor = Color.white;
		GUILayout.Label("Background ", EditorStyles.toolbarButton);
		bgColor = EditorGUILayout.ColorField(bgColor, GUILayout.Width(40));
		GUILayout.Label("Foreground ", EditorStyles.toolbarButton);
		fgColor = EditorGUILayout.ColorField(fgColor, GUILayout.Width(40));
		font = EditorGUILayout.ObjectField(font, typeof(Font), false) as Font;
		if (GUILayout.Button("Save Prefs", EditorStyles.toolbarButton))
		{
			SaveEditorPrefs();
		}
		if (GUILayout.Button("Revert", EditorStyles.toolbarButton))
		{
			LoadEditorPrefs();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		
		scrollPosition = EditorGUILayout.BeginScrollView(
        scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 16));
		
		GUI.backgroundColor = Color.clear;
		
		GUIContent stream = new GUIContent(GameConsole.instance.stream);
		float height = fieldStyle.CalcHeight(stream, position.width);
		
		fieldStyle.wordWrap = true;
		GUI.SetNextControlName("Console Area");
		if (string.IsNullOrEmpty(GameConsole.instance.stream))
		{
			EditorGUILayout.SelectableLabel(" ", fieldStyle, GUILayout.Height(height));
		}
		else
		{
			EditorGUILayout.SelectableLabel(GameConsole.instance.stream, fieldStyle, GUILayout.Height(height));
		}
		
		GUILayout.Space(20);
		GUILayout.BeginHorizontal();
		
		string[] path = EditorApplication.currentScene.Split('/');
		string scene = path[path.Length - 1];
		
		if (scene.Contains(".unity"))
		{
			scene = scene.Remove(scene.Length - 6);
		}
		else
		{
			scene = "Untitled";
		}
		
		scene = scene +
			"@" + Application.platform.ToString() +
			":" + GameConsole.instance.contextString +
			"# > ";
		
		
		fieldStyle.wordWrap = false;	
		GUIContent sceneText = new GUIContent(scene);
		Vector2 sceneSize = fieldStyle.CalcSize(sceneText);
		fieldStyle.wordWrap = true;
		
		GUIContent current = new GUIContent(currentText);
		height = fieldStyle.CalcHeight(current, position.width - sceneSize.x);
		
		GUI.SetNextControlName("Current Text");
		string consoleText = GUILayout.TextField(scene + currentText, fieldStyle, GUILayout.Height(height));
		Rect consoleTextRect = GUILayoutUtility.GetLastRect();
		
		if (!consoleText.StartsWith(scene))
		{
			consoleText = scene + currentText;
			GUI.FocusControl("Console Area");
			Event.current.Use();
		}
		
		currentText = consoleText.Remove(0, Mathf.Min(consoleText.Length, scene.Length));
		
		if (GUI.GetNameOfFocusedControl() == "Current Text")
		{
			GUI.color = new Color(1,1,1,.75f);
			GUI.backgroundColor = Color.white;
			GUI.contentColor = Color.white;
			GUI.depth = 100;
			if (string.IsNullOrEmpty(currentText)) GUI.enabled = false;
			if (GUI.Button(new Rect(consoleTextRect.x, consoleTextRect.y - 20, 60, 20),"Cut"))
			{
				EditorGUIUtility.systemCopyBuffer = currentText;
				currentText = "";
			}
			if (GUI.Button(new Rect(consoleTextRect.x + 60, consoleTextRect.y - 20, 60, 20),"Copy"))
			{
				EditorGUIUtility.systemCopyBuffer = currentText;
			}
			if (GUI.Button(new Rect(consoleTextRect.x + 180, consoleTextRect.y - 20, 60, 20),"Clear"))
			{
				currentText = "";
			}
			GUI.enabled = true;
			if (string.IsNullOrEmpty(EditorGUIUtility.systemCopyBuffer)) GUI.enabled = false;
			if (GUI.Button(new Rect(consoleTextRect.x + 120, consoleTextRect.y - 20, 60, 20),"Paste"))
			{
				currentText = EditorGUIUtility.systemCopyBuffer;
			}
			GUI.enabled = true;
			GUI.contentColor = fgColor;
			GUI.backgroundColor = bgColor;
			GUI.color = Color.white;
		}
		
		fieldStyle.wordWrap = false;
		
		GUI.contentColor = fgColor;
		GUI.backgroundColor = bgColor;
		
		fieldStyle.font = null;
		
		GUI.skin = null;
		
		GUILayout.EndHorizontal();
		
		GUILayout.EndScrollView();
		
		if (Event.current != null)
		{	
			if (Event.current.type == EventType.KeyUp && GUI.GetNameOfFocusedControl() == "Console Area")
			{
				consoleText = scene + currentText;
				currentText = consoleText.Remove(0, Mathf.Min(consoleText.Length, scene.Length));
				GUI.FocusControl("Current Text");
			}
		
			Repaint();	
			
			if (Event.current.keyCode == KeyCode.UpArrow && Event.current.type == EventType.KeyUp)
			{
				if (GameConsole.instance.commandHistory.Count > 0)
				{
					if (commandPointer == -1)
					{
						commandPointer = GameConsole.instance.commandHistory.Count - 1;
					}
					currentText = GameConsole.instance.commandHistory[commandPointer];
					commandPointer--;
					if (commandPointer < 0)
					{
						commandPointer = 0;
					}
				}
			}
			else if (Event.current.keyCode == KeyCode.DownArrow && Event.current.type == EventType.KeyUp)
			{
				if (GameConsole.instance.commandHistory.Count > 0 && commandPointer != -1)
				{
					currentText = GameConsole.instance.commandHistory[commandPointer];
					commandPointer++;
					if (commandPointer > GameConsole.instance.commandHistory.Count - 1)
					{
						commandPointer = GameConsole.instance.commandHistory.Count - 1;
					}
				}
			}
			else if (Event.current.type == EventType.KeyUp && Event.current.keyCode != KeyCode.LeftArrow && Event.current.keyCode != KeyCode.RightArrow)
			{
				commandPointer = -1;
			}
				
			if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp)
			{
				GameConsole.instance.Input(currentText);
				scrollPosition += Vector2.up * 5000f;
				currentText = string.Empty;
				consoleText = scene + currentText;
				Event.current.Use();
				GUI.FocusControl("Current Text");
			}
		}

	}
	
	public static void SaveEditorPrefs()
	{
		EditorPrefs.SetFloat("OGC_Editor_bgColor_R", EditorConsoleDisplay.bgColor.r);
		EditorPrefs.SetFloat("OGC_Editor_bgColor_G", EditorConsoleDisplay.bgColor.g);
		EditorPrefs.SetFloat("OGC_Editor_bgColor_B", EditorConsoleDisplay.bgColor.b);
		EditorPrefs.SetFloat("OGC_Editor_bgColor_A", EditorConsoleDisplay.bgColor.a);

		EditorPrefs.SetFloat("OGC_Editor_fgColor_R", EditorConsoleDisplay.fgColor.r);
		EditorPrefs.SetFloat("OGC_Editor_fgColor_G", EditorConsoleDisplay.fgColor.g);
		EditorPrefs.SetFloat("OGC_Editor_fgColor_B", EditorConsoleDisplay.fgColor.b);
		EditorPrefs.SetFloat("OGC_Editor_fgColor_A", EditorConsoleDisplay.fgColor.a);

		EditorPrefs.SetInt("OGC_Editor_savedPrefs", 1);
	}

	public static void LoadEditorPrefs()
	{
		if (!EditorPrefs.HasKey("OGC_savedPrefs"))
		{
			SaveEditorPrefs();
		}

		EditorConsoleDisplay.bgColor = new Color(
			EditorPrefs.GetFloat("OGC_Editor_bgColor_R"),
			EditorPrefs.GetFloat("OGC_Editor_bgColor_G"),
			EditorPrefs.GetFloat("OGC_Editor_bgColor_B"),
			EditorPrefs.GetFloat("OGC_Editor_bgColor_A")
			);

		EditorConsoleDisplay.fgColor = new Color(
			EditorPrefs.GetFloat("OGC_Editor_fgColor_R"),
			EditorPrefs.GetFloat("OGC_Editor_fgColor_G"),
			EditorPrefs.GetFloat("OGC_Editor_fgColor_B"),
			EditorPrefs.GetFloat("OGC_Editor_fgColor_A")
			);
	}
}
