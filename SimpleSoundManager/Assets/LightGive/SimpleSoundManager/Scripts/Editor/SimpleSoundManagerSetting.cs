﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SimpleSoundManagerSetting
{
	private static ConfigAsset m_configAsset;

	public static ConfigAsset configAsset
	{
		get
		{
			if (m_configAsset != null)
				return m_configAsset;

			m_configAsset = (ConfigAsset)AssetDatabase.LoadAssetAtPath(SimpleSoundManagerDefine.PathConfigData, typeof(ConfigAsset));
			return (m_configAsset != null) ? m_configAsset : CreateConfigData();
		}
	}

	public static ConfigAsset CreateConfigData()
	{
		return ConfigAsset.CreateAsset();
	}

	public static void CreateSoundName()
	{
		string[] fileEntriesBgm = Directory.GetFiles(SourceFolderPathBGM, "*", SearchOption.AllDirectories);
		string[] fileEntriesSe = Directory.GetFiles(SourceFolderPathSE, "*", SearchOption.AllDirectories);

		List<Object> bgmObjList = new List<Object>();
		List<Object> seObjList = new List<Object>();

		List<AudioClipInfo> bgmClipList = new List<AudioClipInfo>();
		List<AudioClipInfo> seClipList = new List<AudioClipInfo>();

		int idx = 0;

		//Search files in BGM folder one by one
		for (int i = 0; i < fileEntriesBgm.Length; i++)
		{
			var filePath = fileEntriesBgm[i];
			filePath = ConvertSystemPathToUnityPath(filePath);
			var obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(object));
			if (obj != null)
			{
				if (obj.GetType() != typeof(AudioClip))
					continue;
				idx++;
				AudioClip audio = (AudioClip)obj;
				bgmObjList.Add(obj);

				var bgmInfo = new AudioClipInfo(idx, audio);
				bgmClipList.Add(bgmInfo);
			}
		}

		//Init
		idx = 0;


		//Find "SE" folder file
		for (int i = 0; i < fileEntriesSe.Length; i++)
		{
			var filePath = fileEntriesSe[i];
			filePath = ConvertSystemPathToUnityPath(filePath);
			var obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(object));
			if (obj != null)
			{
				if (obj.GetType() != typeof(AudioClip))
					continue;
				idx++;
				AudioClip audio = (AudioClip)obj;
				var seInfo = new AudioClipInfo(idx, audio);
				seObjList.Add(obj);
				seClipList.Add(seInfo);
			}
		}

		//CreateClipListAsset
		var bgmClipListInstance = Editor.CreateInstance<AudioClipList>();
		var seClipListInstance = Editor.CreateInstance<AudioClipList>();
		bgmClipListInstance = new AudioClipList(bgmClipList);
		seClipListInstance = new AudioClipList(seClipList);


		var bgmClipListOld = (AudioClipList)AssetDatabase.LoadAssetAtPath(CliplistDataPathBGM, typeof(AudioClipList));
		var seClipListOld = (AudioClipList)AssetDatabase.LoadAssetAtPath(CliplistDataPathSE, typeof(AudioClipList));
		AssetDatabase.CreateAsset(bgmClipListInstance, CliplistDataPathBGM);
		AssetDatabase.CreateAsset(seClipListInstance, CliplistDataPathSE);
		AssetDatabase.Refresh();


		//Get script path
		var audioNameScriptPath = ManagerRootFolderPath + "/Scripts/" + AUDIO_SCRIPT_NAME;

		//Create "AudioName.cs"
		string audioFileNameExtension = Path.GetFileNameWithoutExtension(audioNameScriptPath);
		StringBuilder strBuilder = new StringBuilder();
		strBuilder.AppendFormat("public static class {0}", audioFileNameExtension).AppendLine();
		strBuilder.AppendLine("{");
		foreach (AudioClip bgm in bgmObjList)
			strBuilder.Append("\t").AppendFormat(@"public const string BGM_{0} = ""{1}"";", bgm.name, bgm.name).AppendLine();
		strBuilder.AppendLine("\t");
		foreach (AudioClip se in seObjList)
			strBuilder.Append("\t").AppendFormat(@"public const string SE_{0} = ""{1}"";", se.name, se.name).AppendLine();
		strBuilder.AppendLine("}");
		strBuilder.AppendLine("\t");

		strBuilder.AppendFormat("public enum AudioNameBGM").AppendLine();
		strBuilder.AppendLine("{");
		strBuilder.Append("\t").AppendFormat(@"None,").AppendLine();
		foreach (AudioClip bgm in bgmObjList)
			strBuilder.Append("\t").AppendFormat(@"{0},", bgm.name).AppendLine();
		strBuilder.AppendLine("}");
		strBuilder.AppendLine("\t");

		strBuilder.AppendFormat("public enum AudioNameSE").AppendLine();
		strBuilder.AppendLine("{");
		strBuilder.Append("\t").AppendFormat(@"None,").AppendLine();
		foreach (AudioClip se in seObjList)
			strBuilder.Append("\t").AppendFormat(@"{0},", se.name).AppendLine();
		strBuilder.AppendLine("}");

		string directoryName = Path.GetDirectoryName(audioNameScriptPath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		File.WriteAllText(audioNameScriptPath, strBuilder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
	}
}