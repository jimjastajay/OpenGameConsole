#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using ThinksquirrelSoftware.OpenGameConsole;
using ThinksquirrelSoftware.OpenGameConsole.Utility;
using System.Collections;

public class OGCDLEditor
{
	private OGCDownloader downloader;
	private int percent;
	
	public OGCDLEditor(OGCDownloader downloader)
	{
		this.downloader = downloader;
		EditorApplication.update += DownloaderUpdate;
	}
	
	void DownloaderUpdate()
	{
		if (downloader.www != null)
		{
			int oldPercent = percent;
			percent = (int)(downloader.www.progress * 100f);
			if (percent != 0 && percent % 10 == 0 && percent < 100 && percent != oldPercent)
			{
				GameConsole.instance.Echo(percent + "%", true);
			}
		}
		
		if (downloader.www.error != null)
		{
			GameConsole.instance.Echo(ConsoleErrors.DownloadError(downloader.www.error), true);
			UnityEngine.Object.DestroyImmediate(downloader.gameObject);
			Finish();
		}
		else if (downloader.www.isDone)
		{
			GameConsole.instance.Echo("Download complete.", true);
			if (downloader.prt)
			{
				GameConsole.instance.Echo(downloader.www.text, true);
			}
			else
			{
				GameConsole.instance.Input(downloader.www.text);
			}
			UnityEngine.Object.DestroyImmediate(downloader.gameObject);
			Finish();
		}
	}
	
	void Finish()
	{
		EditorApplication.update -= DownloaderUpdate;
	}
	
}

#endif
