using UnityEngine;
using UnityEditor;
using OpenGameConsole;
using System.Collections;

public class EditorConsoleDisplay : EditorWindow
{
	private Rect windowRect;
	private Vector2 scrollPosition;
	private string currentText = "";
	private int commandPointer = - 1;
	
	[MenuItem ("Window/OpenGameConsole")]
    static void Init()
	{
		EditorWindow.GetWindow(typeof(EditorConsoleDisplay), false, "OGC Terminal");
	}
	
	void OnGUI()
	{
		scrollPosition = GUILayout.BeginScrollView(
        scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 25));
		
		GUILayout.Label(GameConsole.instance.stream);
		
		GUILayout.EndScrollView();
		
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
		currentText = GUILayout.TextField(currentText);
		GUILayout.EndHorizontal();
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
