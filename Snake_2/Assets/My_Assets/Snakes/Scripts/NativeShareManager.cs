using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NativeShareManager
{
	private static AndroidJavaClass m_ajc;

	private static AndroidJavaObject m_context;

	private string subject = string.Empty;

	private string text = string.Empty;

	private string title = string.Empty;

	private string targetPackage = string.Empty;

	private string targetClass = string.Empty;

	private List<string> files;

	private List<string> mimes;

	private static AndroidJavaClass AJC
	{
		get
		{
			if (m_ajc == null)
			{
				m_ajc = new AndroidJavaClass("com.yasirkula.unity.NativeShare");
			}
			return m_ajc;
		}
	}

	private static AndroidJavaObject Context
	{
		get
		{
			if (m_context == null)
			{
				using (AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					m_context = androidJavaObject.GetStatic<AndroidJavaObject>("currentActivity");
				}
			}
			return m_context;
		}
	}

	public NativeShareManager()
	{
		files = new List<string>(0);
		mimes = new List<string>(0);
	}

	public NativeShareManager SetSubject(string subject)
	{
		if (subject != null)
		{
			this.subject = subject;
		}
		return this;
	}

	public NativeShareManager SetText(string text)
	{
		if (text != null)
		{
			this.text = text;
		}
		return this;
	}

	public NativeShareManager AddFile(string filePath, string mime = null)
	{
		if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
		{
			files.Add(filePath);
			mimes.Add(mime ?? string.Empty);
		}
		else
		{
			UnityEngine.Debug.LogError("File does not exist at path or permission denied: " + filePath);
		}
		return this;
	}

	public void Share()
	{
		if (files.Count == 0 && subject.Length == 0 && text.Length == 0)
		{
			UnityEngine.Debug.LogWarning("Share Error: attempting to share nothing!");
		}
		else
		{
			AJC.CallStatic("Share", Context, targetPackage, targetClass, files.ToArray(), mimes.ToArray(), subject, text, title);
		}
	}
}
