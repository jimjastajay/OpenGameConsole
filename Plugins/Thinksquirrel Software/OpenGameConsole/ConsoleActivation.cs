using UnityEngine;
using System.Collections;

public class ConsoleActivation : MonoBehaviour
{
	
	public KeyCode activationKey = KeyCode.BackQuote;
	public ConsoleDisplay consoleDisplay;

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
