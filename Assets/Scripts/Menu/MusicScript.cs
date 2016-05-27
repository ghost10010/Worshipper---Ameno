//Victor Adamse
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class MusicScript : MonoBehaviour
{
    public static MusicScript instance = null;

    [SerializeField] private AudioSource _AudioSourceMusic;
	[SerializeField] private List<AudioSource> _AudioSourcesSounds;

	[SerializeField] private AudioClip[] _Music;

	[SerializeField] private SliderController sldMaster;
	[SerializeField] private SliderController sldSounds;
	[SerializeField] private SliderController sldMusic;

    [SerializeField] private int menuAudioID = 0;

	public float SoundsMaxVolume = 0.5f;
	public float MusicMaxVolume = 1.0f;
	public float VocalMaxVolume = 4.0f;

	public bool MusicMuted = false;
	public bool SoundsMuted = false;

	public int MasterVolume = 100;
	public int MusicVolume = 100;
	public int SoundsVolume = 100;

    [SerializeField] private bool followCamera = true;

    void Update()
    {
        if (followCamera)
        {
            this.transform.position = Camera.main.transform.position;
        }
    }

    void Awake()
    {
		if (instance != null && instance != this)
        {
			if (Application.loadedLevelName == "Menu")
			{
				MusicScript.instance.PlayMenuMusic();
			}

			Destroy(this.gameObject);
            return;
        }
        else
        {
			instance = this;

			GetSavedVolumeSettings();
			GetAudioSources ();
			ToggleMusic();
			ToggleSounds();
			
			if (Application.loadedLevelName == "Menu")
			{
				PlayMenuMusic();
			}
        }

        DontDestroyOnLoad(this.gameObject);
    }

	public void GetAudioSources()
	{
		AudioSource[] AllAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		_AudioSourcesSounds = new List<AudioSource> ();
		_AudioSourcesSounds = AllAudioSources.ToList ();

		for (int index = 0; index < _AudioSourcesSounds.Count; index++)
		{
			if (_AudioSourcesSounds[index].name == "MusicObject")
			{
				_AudioSourceMusic.volume = GetMusicVolumeWithMax();
				_AudioSourceMusic.mute = MusicMuted;
				_AudioSourcesSounds.RemoveAt(index);
			}
			else
			{
				_AudioSourcesSounds[index].volume = GetSoundsVolumeWithMax();
				_AudioSourcesSounds[index].mute = SoundsMuted;
			}
		}
	}

	public void AddSoundSource(AudioSource Source)
	{
		Source.volume = GetSoundsVolumeWithMax ();

		_AudioSourcesSounds.Add (Source);
	}

	public void PlayRandomMusic()
	{
        //TODO exlude Menu music
		int RandomNumber = Random.Range(0, _Music.Length);
        PlayMusic(RandomNumber);
	}
    public void PlayMenuMusic()
    {
        PlayMusic(menuAudioID);
    }
    private void PlayMusic(int Id)
    {
        _AudioSourceMusic.clip = _Music[Id];
        _AudioSourceMusic.Play();
    }
	
	public void ToggleMusic()
	{
		_AudioSourceMusic.mute = MusicMuted;
	}
	public void ToggleSounds()
	{
		for (int index = 0; index < _AudioSourcesSounds.Count; index++)
		{
			_AudioSourcesSounds[index].mute = SoundsMuted;
		}
	}
	public void SetMusicVolume(bool SaveSettings)
	{
		_AudioSourceMusic.volume = GetMusicVolumeWithMax ();

		if (SaveSettings)
		{
			SaveVolumeSettings();
		}
	}
	public void SetSoundsVolume(bool SaveSettings)
	{
		for (int index = 0; index < _AudioSourcesSounds.Count; index++)
		{
			if (_AudioSourcesSounds[index] != null)
			{
				_AudioSourcesSounds[index].volume = GetSoundsVolumeWithMax();
			}
		}

		if (SaveSettings)
		{
			SaveVolumeSettings();
		}
	}
    
	public void PlayAudioClip(AudioClip Clip)
	{
		if (!SoundsMuted && Clip != null)
		{
			_AudioSourceMusic.PlayOneShot(Clip, GetVocalVolumeWithMax());
		}
	}

	void GetSavedVolumeSettings()
	{
        SoundsVolume = PlayerPrefs.GetInt("VolumeSounds", 100);
        MusicVolume = PlayerPrefs.GetInt("VolumeMusic", 100);
        MasterVolume = PlayerPrefs.GetInt("VolumeMaster", 100);

        if (sldMusic != null && sldSounds != null && sldMaster != null)
		{
            sldSounds.SetDefaultValue(SoundsVolume / 100.0f);
            sldMusic.SetDefaultValue(MusicVolume / 100.0f);
            sldMaster.SetDefaultValue(MasterVolume / 100.0f);
		}

		SetSoundsVolume (false);
		SetMusicVolume (false);
	}
	public void SaveVolumeSettings()
	{
		PlayerPrefs.SetInt ("VolumeSounds", SoundsVolume);
		PlayerPrefs.SetInt ("VolumeMusic", MusicVolume);
		PlayerPrefs.SetInt ("VolumeMaster", MasterVolume);
	}

	public float GetMusicVolumeWithMax()
	{
		int NewVolume = 100;
		
		if (MasterVolume < SoundsVolume) //if master is lower
		{
			NewVolume = MasterVolume;
		}
		else //if Music is lower
		{
			NewVolume = MusicVolume;
		}
		
		return (MusicMaxVolume / 100) * NewVolume;
	}
	public float GetSoundsVolumeWithMax()
	{
		int NewVolume = 100;

		if (MasterVolume < SoundsVolume) //if master is lower
		{
			NewVolume = MasterVolume;
		}
		else //if Music is lower
		{
			NewVolume = SoundsVolume;
		}

		return (SoundsMaxVolume / 100) * NewVolume;
	}
	public float GetVocalVolumeWithMax()
	{
		int NewVolume = 100;
		
		if (MasterVolume < SoundsVolume) //if master is lower
		{
			NewVolume = MasterVolume;
		}
		else //if Music is lower
		{
			NewVolume = MusicVolume;
		}
		
		return (VocalMaxVolume / 100) * NewVolume;
	}
	public float GetMusicVolume()
	{
		return (MusicVolume / 100.0f);
	}
	public float GetSoundsVolume()
	{
		return (SoundsVolume / 100.0f);
	}
	public float GetMasterVolume()
	{
		return (MasterVolume / 100.0f);
	}

    public void FollowCamera(bool follow)
    {
        followCamera = follow;
    }
}