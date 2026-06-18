using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string MusicPref = "MusicPref";
    private static readonly string SFXPref = "SFXPref";
    private int fistPlayInt;

    public Slider musicSlider, sfxSlider;
    private float sfxSliderVolume, musicSliderVolume;

    public AudioSource music;
    public AudioSource sfx;
    public List<AudioClip> sfxClips = new List<AudioClip>();

    private void Start()
    {
        fistPlayInt = PlayerPrefs.GetInt(FirstPlay);
        
        if(fistPlayInt == 0)
        {
            musicSliderVolume = 0.25f;
            sfxSliderVolume = 0.75f;
            musicSlider.value = musicSliderVolume;
            sfxSlider.value = sfxSliderVolume;
            PlayerPrefs.SetFloat(MusicPref, musicSliderVolume);
            PlayerPrefs.SetFloat(SFXPref, sfxSliderVolume);
            PlayerPrefs.SetInt(FirstPlay, -1);
        }
        else
        {
            musicSliderVolume = PlayerPrefs.GetFloat(MusicPref);
            musicSlider.value = musicSliderVolume;
            sfxSliderVolume = PlayerPrefs.GetFloat(SFXPref);
            sfxSlider.value = sfxSliderVolume;
        }
    }

    public void UpdateMusic()
    {
        music.volume = musicSlider.value;
        PlayerPrefs.SetFloat(MusicPref, musicSlider.value);
    }

    public void UpdateSFX()
    {
        sfx.volume = sfxSlider.value;
        PlayerPrefs.SetFloat(SFXPref, sfxSlider.value);
    }

    public void Update()
    {
        if (musicSlider == null || sfxSlider == null)
        {
            if (GameObject.FindGameObjectWithTag("OptionsMenu"))
            {
                musicSlider = GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>();
                sfxSlider = GameObject.FindGameObjectWithTag("SFXSlider").GetComponent<Slider>();
            }
        }
    }

    public void PlaySFX(int clipNumber)
    {
        AudioClip clip = sfxClips[clipNumber];
        sfx.PlayOneShot(clip);
    }
}
