﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SimpleSoundManager : SingletonMonoBehaviour<SimpleSoundManager>
{
	[SerializeField]
	private List<SoundEffectPlayer> m_soundEffectPlayers = new List<SoundEffectPlayer>();
	[SerializeField]
	private List<AudioClip> m_audioClipListSe = new List<AudioClip>();
	[SerializeField]
	private List<AudioClip> m_audioClipListBgm = new List<AudioClip>();
	[SerializeField]
	private int m_sePlayerNum = 10;

	private Dictionary<string, AudioClip> m_audioClipDirectorSe = new Dictionary<string, AudioClip>();
	private Dictionary<string, AudioClip> m_audioClipDirectorBgm = new Dictionary<string, AudioClip>();


	protected override void Init()
	{
		base.Init();
		for (int i = 0; i < m_sePlayerNum;i++)
		{
			GameObject soundPlayerObj = new GameObject("SoundPlayer" + i.ToString("0"));
			soundPlayerObj.transform.SetParent(transform);
			SoundEffectPlayer player = soundPlayerObj.AddComponent<SoundEffectPlayer>();
			player.Init();
			m_soundEffectPlayers.Add(player);
		}

		//Dictionaryに追加
		for (int i = 0; i < m_audioClipListSe.Count;i++)
		{
			m_audioClipDirectorSe.Add(m_audioClipListSe[i].name, m_audioClipListSe[i]);
		}
		for (int i = 0; i < m_audioClipListBgm.Count; i++)
		{
			m_audioClipDirectorBgm.Add(m_audioClipListBgm[i].name, m_audioClipListBgm[i]);
		}
	}

	public void Play()
	{
		var player = GetSoundEffectPlayer();
		StartCoroutine(Play(0.0f));
 	}

	private IEnumerator Play(float _delay)
	{
		yield return new WaitForSeconds(_delay);

	}

	private SoundEffectPlayer GetSoundEffectPlayer()
	{
		for (int i = 0; i < m_soundEffectPlayers.Count;i++)
		{
			if (m_soundEffectPlayers[i].isActive)
				continue;

			return m_soundEffectPlayers[i];
		}

		int idx = 0;
		for (int i = 1; i < m_soundEffectPlayers.Count; i++)
		{
			if (m_soundEffectPlayers[i].Length > m_soundEffectPlayers[idx].Length)
			{
				idx = i;
			}
		}
		return m_soundEffectPlayers[idx];
	}
}
