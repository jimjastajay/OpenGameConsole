using UnityEngine;
using UnityEditor;
using ThinksquirrelSoftware.OpenGameConsole;
using System.Collections;

public class EditorConsoleDisplay : EditorWindow
{
	private Rect windowRect;
	private Vector2 scrollPosition;
	private string currentText = "";
	private int commandPointer = - 1;
	private Color bgColor = Color.black;
	private Color fgColor = Color.white;
	
	[MenuItem ("Window/OpenGameConsole")]
    static void Init()
	{
		EditorWindow.GetWindow(typeof(EditorConsoleDisplay), false, "OGC Terminal");
	}
	
	void OnGUI()
	{
		GUI.backgroundColor = bgColor;
		GUI.contentColor = fgColor;
		GUI.SetNextControlName("Dummy Button");
		GUI.depth = 100;
		if (GUI.Button(new Rect(0, 14, position.width, position.height - 14), ""))
		{
			GUI.FocusControl("Current Text");
		}
		GUI.depth = 0;
		if (GUI.GetNameOfFocusedControl() == "Dummy Button")
		{
			GUI.FocusControl("Current Text");
		}
		GUI.backgroundColor = Color.white;

		GUILayout.BeginVertical("Toolbar");
		GUI.backgroundColor = bgColor;

		GUILayout.BeginHorizontal();
		GUILayout.Space(4);

		GUI.backgroundColor = Color.white;
		GUILayout.Label("Background ", EditorStyles.toolbarButton);
		bgColor = EditorGUILayout.ColorField(bgColor, GUILayout.Width(40));
		GUILayout.Label("Foreground ", EditorStyles.toolbarButton);
		fgColor = EditorGUILayout.ColorField(fgColor, GUILayout.Width(40));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		GUI.backgroundColor = bgColor;
		
		scrollPosition = GUILayout.BeginScrollView(
        scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 20));
		
		GUILayout.Label(GameConsole.instance.stream);
		
		GUI.SetNextControlName("Current Text");
		
		GUILayout.BeginHorizontal();
		
		string[] path = EditorApplication.currentScene.Split('/');
		string scene = path[path.Length - 1];
		scene = scene.Remove(scene.Length - 6);
		
		GUILayout.Label(
			scene +
			"@" + Application.platform.ToString() +
			":" + GameConsole.instance.contextString +
			"# >", GUILayout.ExpandWidth(false));
		GUI.backgroundColor = Color.clear;
		currentText = GUILayout.TextField(currentText);
		GUI.backgroundColor = bgColor;
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
				currentText = "";
			}
		}	
	}
}
