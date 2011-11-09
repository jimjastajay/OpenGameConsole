using UnityEngine;
using UnityEditor;
using ThinksquirrelSoftware.OpenGameConsole;
using ThinksquirrelSoftware.OpenGameConsole.Utility;
using System.Collections;

public class EditorConsoleDisplay : EditorWindow
{
	private Rect windowRect;
	private Vector2 scrollPosition;
	private string currentText = "";
	private int commandPointer = - 1;
	private Color bgColor = Color.black;
	private Color fgColor = Color.white;
	private Font font;
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
	}
	
	void OnGUI()
	{	
		newSkin = GUI.skin;
		
		fieldStyle = newSkin.GetStyle("TextField");
		
		fieldStyle.font = font;
		fieldStyle.wordWrap = true;
		
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
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		
		scrollPosition = EditorGUILayout.BeginScrollView(
        scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 20));
		
		GUI.backgroundColor = Color.clear;
		
		GUIContent stream = new GUIContent(GameConsole.instance.stream);
		float height = fieldStyle.CalcHeight(stream, position.width);
		
		GUI.SetNextControlName("Console Area");
		EditorGUILayout.SelectableLabel(GameConsole.instance.stream, fieldStyle, GUILayout.Height(height));
		
		
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
		/*
		GUILayout.Label(
			scene +
			"@" + Application.platform.ToString() +
			":" + GameConsole.instance.contextString +
			"# >", fieldStyle, GUILayout.Width(sceneSize.x), GUILayout.Height(sceneSize.y));		
		*/
		fieldStyle.wordWrap = true;
		
		GUIContent current = new GUIContent(currentText);
		height = fieldStyle.CalcHeight(current, position.width - sceneSize.x);
		
		GUI.SetNextControlName("Current Text");
		string consoleText = EditorGUILayout.TextField(scene + currentText, fieldStyle, GUILayout.Height(height));
		Rect r = GUILayoutUtility.GetLastRect();
		
		if (consoleText.Length < scene.Length || !consoleText.StartsWith(scene))
		{
			consoleText = scene + currentText;
			GUI.FocusControl("Console Area");
		}
		
		currentText = consoleText.Remove(0, Mathf.Min(consoleText.Length, scene.Length));
		
		fieldStyle.wordWrap = false;
		GUI.SetNextControlName("Cursor");
		
		GUIContent cText = new GUIContent(consoleText);
		Vector2 cTextSize = fieldStyle.CalcSize(cText);
		
		if (GUI.GetNameOfFocusedControl() == "Current Text")
		{
			GUI.Label(new Rect(r.x + cTextSize.x, r.y, 30, sceneSize.y), "_", fieldStyle);
		}
		
		GUI.backgroundColor = bgColor;
		
		fieldStyle.font = null;
		
		GUI.skin = null;
		
		GUILayout.EndHorizontal();
		
		GUILayout.EndScrollView();
		
		if (Event.current != null)
		{	
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
				GUI.FocusControl("Current Text");
			}
		}	
	}
}
