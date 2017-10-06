﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LightGive
{
	[System.Serializable]
	public class SoundEffectPlayer : MonoBehaviour
	{
		[SerializeField]
		public AudioSource audioSource;
		[SerializeField]
		public GameObject parentObj;
		[SerializeField]
		public AnimationCurve animationCurve;
		[SerializeField]
		public float delay;
		[SerializeField]
		public float volume;
		[SerializeField]
		public int loopCnt;
		[SerializeField]
		public bool isActive;
		[SerializeField]
		public bool isFade;

		[SerializeField]
		public UnityAction callbackOnComplete;
		[SerializeField]
		public UnityAction callbackOnStart;

		public void Play()
		{
			isActive = true;
			this.gameObject.SetActive(true);
			audioSource.volume = AudioManager.Instance.TotalVolume * volume;
			audioSource.PlayDelayed(delay);			
			Invoke("AudioPlayCheck", (audioSource.clip.length / audioSource.pitch) + delay);
		}

		public void Stop()
		{
			audioSource.Stop();
			this.gameObject.SetActive(false);
			CancelInvoke();
			isActive = false;
			loopCnt = 0;
		}


		public void Pause()
		{
			audioSource.Pause();
		}

		void AudioPlayCheck()
		{
			loopCnt--;

			if (loopCnt > 0)
			{
				audioSource.Play();
				Invoke("AudioPlayCheck", (audioSource.clip.length / audioSource.pitch));
				return;
			}

			if (callbackOnComplete != null)
			{
				callbackOnComplete.Invoke();
			}
			this.gameObject.SetActive(false);
		}

		public SoundEffectPlayer()
		{
			isActive = false;
			audioSource = null;
			parentObj = null;
			delay = 0.0f;
			loopCnt = 0;
		}

		public void PlayerUpdate()
		{
			if (parentObj != null)
				transform.position = parentObj.transform.position;

			if (isFade)
			{
				audioSource.volume =
					AudioManager.Instance.TotalVolume *
					animationCurve.Evaluate(audioSource.time) *
					volume;
			}
		}

		public SoundEffectPlayer(AudioClip _audioClip)
		{
			audioSource = new AudioSource();
			audioSource.clip = _audioClip;
		}

		public void ChangeTotalVolume(float _val)
		{
			audioSource.volume = AudioManager.Instance.TotalVolume * volume;
		}
	}
}