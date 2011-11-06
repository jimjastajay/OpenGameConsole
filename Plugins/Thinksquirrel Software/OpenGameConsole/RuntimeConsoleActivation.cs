using UnityEngine;
using System.Collections;

public class RuntimeConsoleActivation : MonoBehaviour
{
	
	public KeyCode activationKey = KeyCode.BackQuote;
	public RuntimeConsoleDisplay consoleDisplay;

	void Update()
	{
		if (Input.GetKeyUp(activationKey))
		{
			if (!consoleDisplay.enabled)
			{
				consoleDisplay.enabled = true;
			}
		}
	}
}
