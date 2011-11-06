using UnityEngine;
using ThinksquirrelSoftware.OpenGameConsole;
using System.Collections;

public class RuntimeConsoleDisplay : MonoBehaviour
{
	public bool draggable = true;
	public float consoleWidth = 500;
	public float consoleHeight = 200;
	public GUISkin guiSkin;
	private Rect windowRect;
	private Vector2 scrollPosition;
	private string currentText = "";
	private int commandPointer = - 1;
	private bool awake = false;
	
	private Rect ClampToScreen(Rect r)
	{
		r.x = Mathf.Clamp(r.x, 0, Screen.width - r.width);
		r.y = Mathf.Clamp(r.y, 0, Screen.height - r.height);
		return r;
	}
	
	void OnEnable()
	{
		awake = true;
	}

	void OnGUI()
	{
		GUISkin lastSkin = GUI.skin;
		
		GUI.skin = guiSkin;
		
		windowRect = ClampToScreen(GUILayout.Window(
        0, windowRect, ConsoleWindow, "Console", GUILayout.Width(consoleWidth), GUILayout.Height(consoleHeight)));
	
		GUI.skin = lastSkin;
	}
	
	void ConsoleWindow(int windowID)
	{
		scrollPosition = GUILayout.BeginScrollView(
        scrollPosition, GUILayout.Width(consoleWidth), GUILayout.Height(consoleHeight));
		
		GUILayout.Label(GameConsole.instance.stream);
		
		GUILayout.EndScrollView();
		
		GUI.SetNextControlName("Current Text");
		
		GUILayout.BeginHorizontal();
		GUILayout.Label(
			Application.loadedLevelName +
			"@" + Application.platform.ToString() +
			":" + GameConsole.instance.contextString +
			"# >", GUILayout.ExpandWidth(false));
		currentText = GUILayout.TextField(currentText);
		GUILayout.EndHorizontal();
		if (Event.current != null)
		{	
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
			if (Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyUp)
				this.enabled = false;
		}
		
		if (awake)
		{
			GUI.FocusWindow(0);
			GUI.FocusControl("Current Text");
			awake = false;
		}
			
		if (draggable)
			GUI.DragWindow();
	}
}
