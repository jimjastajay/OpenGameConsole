using UnityEngine;
using System;
using System.Collections;
using ThinksquirrelSoftware.OpenGameConsole;
using ThinksquirrelSoftware.OpenGameConsole.Utility;

public class OGCDownloader : MonoBehaviour
{
	public WWW www;
	public bool prt;
	private int percent;

#if UNITY_EDITOR
	[NonSerialized]
	public OGCDLEditor downloader_EDITOR;
#endif
	
	public void StartDownload(string url, bool print)
	{
#if UNITY_EDITOR
		www = new WWW(url);
		this.prt = print;
		downloader_EDITOR = new OGCDLEditor(this);	
#else
		StartCoroutine(Download(url, print));
#endif
	}

#if UNITY_EDITOR	
	void OnDestroy()
	{
		downloader_EDITOR = null;
	}
#endif
	
	public IEnumerator Download(string url, bool print)
	{
		www = new WWW(url);
        
		yield return www;
		
		if (www.error != null)
		{
			GameConsole.instance.Echo(ConsoleErrors.DownloadError(www.error), true);
			Destroy(this.gameObject);
		}
		else
		{
			GameConsole.instance.Echo("Download complete.", true);
			if (print)
			{
				GameConsole.instance.Echo(www.text, true);
			}
			else
			{
				GameConsole.instance.Input(www.text);
			}
			Destroy(this.gameObject);
		}
	}
	
	void Update()
	{
		if (www != null)
		{
			int oldPercent = percent;
			percent = (int)(www.progress * 100f);
			if (percent != 0 && percent % 10 == 0 && percent <= 100 && percent != oldPercent)
			{
				GameConsole.instance.Echo(percent + "%", true);
			}
		}
	}
	
}
