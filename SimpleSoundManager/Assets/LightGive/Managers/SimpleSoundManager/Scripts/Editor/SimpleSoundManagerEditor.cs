﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleSoundManager))]
public class SimpleSoundManagerEditor : Editor
{
	private SerializedObject m_serializedObj;
	private SerializedProperty m_audioClipListSeProp;
	private SerializedProperty m_audioClipListBgmProp;
	private SerializedProperty m_editorIsFoldSeListProp;
	private SerializedProperty m_editorIsFoldBgmListProp;
	private SerializedProperty m_editorIsFoldSoundLIstProp;
	private SerializedProperty m_soundEffectPlayersProp;
	private SerializedProperty m_sePlayerNumProp;
	private SerializedProperty m_isChangeToSaveProp;
	private SerializedProperty m_volumeTotalProp;
	private SerializedProperty m_volumeSeProp;
	private SerializedProperty m_volumeBgmProp;

	private float currentWidth = 0.0f;

	private void OnEnable()
	{
		m_serializedObj = new SerializedObject(target);
		m_audioClipListSeProp = m_serializedObj.FindProperty("audioClipListSe");
		m_audioClipListBgmProp = m_serializedObj.FindProperty("audioClipListBgm");
		m_editorIsFoldSeListProp = m_serializedObj.FindProperty("m_editorIsFoldSeList");
		m_editorIsFoldBgmListProp = m_serializedObj.FindProperty("m_editorIsFoldBgmList");
		m_soundEffectPlayersProp = m_serializedObj.FindProperty("m_soundEffectPlayers");
		m_sePlayerNumProp = m_serializedObj.FindProperty("m_sePlayerNum");
		m_volumeTotalProp = m_serializedObj.FindProperty("m_volumeTotal");
		m_volumeSeProp = m_serializedObj.FindProperty("m_volumeSe");
		m_volumeBgmProp = m_serializedObj.FindProperty("m_volumeBgm");
		m_isChangeToSaveProp = m_serializedObj.FindProperty("m_isChangeToSave");


		List<AudioClip> bgmClipList = new List<AudioClip>();
		List<AudioClip> seClipList = new List<AudioClip>();
		bgmClipList = SimpleSoundManagerSetting.GetAudioClipListBgm();
		seClipList = SimpleSoundManagerSetting.GetAudioClipListSe();

		m_audioClipListSeProp.arraySize = seClipList.Count;
		m_audioClipListBgmProp.arraySize = bgmClipList.Count;


		foreach (SimpleSoundManager t in targets)
		{
			t.audioClipListSe.Clear();
			t.audioClipListBgm.Clear();
		}

		for (int i = 0; i < bgmClipList.Count; i++)
		{
			foreach (SimpleSoundManager t in targets)
			{
				t.audioClipListBgm.Add(bgmClipList[i]);
				serializedObject.ApplyModifiedProperties();
			}
		}

		for (int i = 0; i < seClipList.Count; i++)
		{
			foreach (SimpleSoundManager t in targets)
			{
				t.audioClipListSe.Add(seClipList[i]);
				serializedObject.ApplyModifiedProperties();
			}
		}

		m_serializedObj.Update();
		EditorUtility.SetDirty(target);
		serializedObject.ApplyModifiedProperties();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("--------------------");
		EditorGUILayout.LabelField("ここから下がCustomEditor");

		currentWidth = EditorGUIUtility.currentViewWidth;


		EditorGUILayout.LabelField("【Volume】");
		EditorGUILayout.Slider(m_volumeTotalProp, 0.0f, 1.0f, "Total");
		EditorGUILayout.Slider(m_volumeSeProp, 0.0f, 1.0f, "SE");
		EditorGUILayout.Slider(m_volumeBgmProp, 0.0f, 1.0f, "BGM");
		EditorGUILayout.Space();


		EditorGUILayout.LabelField("【Other】");
		EditorGUILayout.PropertyField(m_isChangeToSaveProp);
		m_sePlayerNumProp.intValue = EditorGUILayout.IntField("SE PlayerCount", m_sePlayerNumProp.intValue);


		EditorGUILayout.Space();
		EditorGUILayout.LabelField("【SoundList】");
		m_editorIsFoldSeListProp.boolValue = EditorGUILayout.Foldout(m_editorIsFoldSeListProp.boolValue, " SE", true);
		if (!m_editorIsFoldSeListProp.boolValue)
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);
			if (m_audioClipListSeProp.arraySize == 0)
			{
				EditorGUILayout.LabelField("None");
			}
			else
			{
				for (int i = 0; i < m_audioClipListSeProp.arraySize; i++)
				{
					var p = m_audioClipListSeProp.GetArrayElementAtIndex(i);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField((i + 1).ToString("00") + ".", GUILayout.Width(20));
					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.ObjectField(p.objectReferenceValue, typeof(AudioClip), false);
					EditorGUI.EndDisabledGroup();

					var clip = (AudioClip)p.objectReferenceValue;
					var timeSpan = System.TimeSpan.FromSeconds(clip.length);
					EditorGUILayout.LabelField(timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00"), GUILayout.Width(40));

					//Editor上で再生できる様に修正
					if (GUILayout.Button("Play"))
					{
						AudioUtility.StopAllClips();
						AudioUtility.PlayClip((AudioClip)p.objectReferenceValue);
					}

					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}

		m_editorIsFoldBgmListProp.boolValue = EditorGUILayout.Foldout(m_editorIsFoldBgmListProp.boolValue, " BGM", true);
		if (!m_editorIsFoldBgmListProp.boolValue)
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);
			if (m_audioClipListBgmProp.arraySize == 0)
			{
				EditorGUILayout.LabelField("None");
			}
			else
			{
				for (int i = 0; i < m_audioClipListBgmProp.arraySize; i++)
				{
					var p = m_audioClipListBgmProp.GetArrayElementAtIndex(i);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField((i + 1).ToString("00") + ".", GUILayout.Width(20));
					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.ObjectField(p.objectReferenceValue, typeof(AudioClip), false);
					EditorGUI.EndDisabledGroup();

					var clip = (AudioClip)p.objectReferenceValue;
					var timeSpan = System.TimeSpan.FromSeconds(clip.length);
					EditorGUILayout.LabelField(timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00"), GUILayout.Width(40));

					//Editor上で再生できる様に修正
					if (GUILayout.Button("Play"))
					{
						AudioUtility.StopAllClips();
						AudioUtility.PlayClip((AudioClip)p.objectReferenceValue);
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}


		if (Application.isPlaying)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("【Debug】");

			//プレイ中に表示する情報
			EditorGUILayout.BeginVertical(GUI.skin.box);

			Debug.Log(m_soundEffectPlayersProp.arraySize);
			for (int i = 0; i < m_soundEffectPlayersProp.arraySize; i++)
			{
				var playerProp = m_soundEffectPlayersProp.GetArrayElementAtIndex(i);
				var player = (SoundEffectPlayer)playerProp.objectReferenceValue;
				//エフェクトのプレイヤーを使用しているかどうかのチェック
				if (!player.isActive)
					continue;

				EditorGUILayout.BeginVertical(GUI.skin.box);

				float[] spectrum = new float[256];
				player.source.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
				float max = 0.0f;
				for (int j = 0; j < spectrum.Length; j++)
				{
					if (max < spectrum[j]) { max = spectrum[j]; }
				}

				Rect rect = GUILayoutUtility.GetRect(10, 20);
				EditorGUI.ProgressBar(rect, Mathf.Clamp01(max / 1.0f), "Player_" + i.ToString("0"));

				//EditorGUILayout.PropertyField(player);
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndVertical();
		}

		EditorUtility.SetDirty(target);
		m_serializedObj.ApplyModifiedProperties();
	}
}