using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	AudioSource src;

    public float songVolumeMax = 0.60f;
    public float fxVolumeMax = 0.75f;

    public List<string> songPaths = new List<string>
    {
        "Songs\\Hubris",
        "Songs\\Doom",
        "Songs\\Pathing",
        "Songs\\Drive",
        "Songs\\Flow",
        "Songs\\Unavoidable",
        "Songs\\Well",
        "Songs\\SlowMotion"
    };

    List<string> songMemory = new List<string>();
    int songMemoryNum = 5;


    public string fxPathGuitar = "SoundEffects\\shrimpwrecked_guitar";
    public string fxPathDrop = "SoundEffects\\shrimpwrecked_dynamite_drop";
    public string fxPathExplosion = "SoundEffects\\shrimpwrecked_explosion";
    public string fxPathHit = "SoundEffects\\shrimpwrecked_struck";
    public string fxPathDie = "SoundEffects\\shrimpwrecked_die";
    public string fxPathSpike = "SoundEffects\\shrimpwrecked_spike";
    public string fxPathDisco = "SoundEffects\\shrimpwrecked_disco";
    public string fxPathBeam = "SoundEffects\\shrimpwrecked_beam";
    public string fxPathInk = "SoundEffects\\shrimpwrecked_ink";
    public string fxPathElectric = "SoundEffects\\shrimpwrecked_electric";

    bool fading = false;

    bool changingSong = false;

    List<AudioSource> soundEffectsAvail;
    List<AudioSource> soundEffectsInUse;
    int soundEffectMax = 20;

	public void Awake(){
		src = GetComponent<AudioSource> ();
        soundEffectsAvail = new List<AudioSource>();
        soundEffectsInUse = new List<AudioSource>();
        AudioSource s = null;
        for (int i = 0; i < soundEffectMax; i++)
        {
            s = gameObject.AddComponent<AudioSource>();
            soundEffectsAvail.Add(s);
        }
        float prefSongMax, prefFxMax;
        prefSongMax = PlayerPrefs.GetInt("shrimpwrecked_song_max", 100);
        if (prefSongMax != 0)
            prefSongMax /= 100f;
        prefFxMax = PlayerPrefs.GetInt("shrimpwrecked_fx_max", 100);
        if (prefFxMax != 0)
            prefFxMax /= 100f;
        songVolumeMax *= prefSongMax;
        fxVolumeMax *= prefFxMax;
	}

    public void PlayCatawampusJingle()
    {
        src.loop = false;
        src.volume = songVolumeMax;
        src.clip = (AudioClip)Resources.Load("Songs\\CometDog_Jingle");
        src.Play();
    }

    IEnumerator Fade(float goal)
    {
        fading = true;
        float start = src.volume;
        while (fading)
        {
            if (goal < start)
                VolumeDown(0.004f);
            else if (goal > start)
                VolumeUp(0.004f);
            if (goal > start)
            {
                if (src.volume >= goal)
                    fading = false;
            }
            else
            {
                if (src.volume <= goal)
                {
                    fading = false;
                }
            }
            yield return null;
        }

        if (goal < start)
            src.Stop();
    }

    public void PlaySong(AudioClip clip, bool loop, bool smooth)
    {
        StartCoroutine(PlayOrChangeSong(clip, loop, smooth));
    }

    IEnumerator PlayOrChangeSong(AudioClip clip, bool loop, bool smooth)
    {
        while(changingSong)
        {
            yield return new WaitForSeconds(1);
        }
        changingSong = true;
        if(smooth)
            yield return Fade(0.01f);
        src.clip = clip;
        src.loop = loop;
        src.Play();
        if(smooth)
            yield return Fade(songVolumeMax);
    }

    public void PlaySongsRandom()
    {
        StartCoroutine(ProcessRandomSongs());
    }

    IEnumerator ProcessRandomSongs()
    {
        while (changingSong)
        {
            yield return new WaitForSeconds(1);
        }
        changingSong = true;
        bool fadeIn = true;
        src.volume = 0.1f;
        src.loop = false;
        while (changingSong)
        {
            if(songMemory.Count == songMemoryNum)
            {
                songPaths.Add(songMemory[0]);
                songMemory.RemoveAt(0);
            }
            int random = Random.Range(0, songPaths.Count);
            src.clip = (AudioClip)Resources.Load((string)songPaths[random]);
            songMemory.Add(songPaths[random]);
            songPaths.RemoveAt(random);
            src.Play();
            if (fadeIn)
            {
                StartCoroutine(Fade(songVolumeMax));
                fadeIn = false;
            }
            yield return new WaitForSeconds(src.clip.length);
        }
    }

    //if loop is chosen, using class must catch audiosource and stop manually
    public AudioSource PlaySoundEffect(AudioClip clip, bool loop = false)
    {
        if (soundEffectsAvail.Count == 0)
            return null;
        AudioSource s = soundEffectsAvail[0];
        soundEffectsAvail.RemoveAt(0);
        s.volume = fxVolumeMax;
        s.clip = clip;
        s.loop = loop;
        soundEffectsInUse.Add(s);
        StartCoroutine(ProcessSoundEffect(s));
        return s;
    }

    //if loop is chosen, using class must catch audiosource and stop manually
    public AudioSource PlaySoundEffect(string path, bool loop = false)
    {
        if (soundEffectsAvail.Count == 0)
            return null;
        AudioSource s = soundEffectsAvail[0];
        soundEffectsAvail.RemoveAt(0);
        s.clip = (AudioClip)Resources.Load(path);
        s.volume = fxVolumeMax;
        s.loop = loop;
        soundEffectsInUse.Add(s);
        StartCoroutine(ProcessSoundEffect(s));
        return s;
    }

    IEnumerator ProcessSoundEffect(AudioSource src)
    {
        float maxLen = 4;
        src.Play();
        while (src.isPlaying)
        {
            yield return new WaitForSeconds(0.2f);
            maxLen -= .2f;
            if (maxLen <= 0) break;
        }
        src.loop = false;
        soundEffectsInUse.Remove(src);
        soundEffectsAvail.Insert(0, src);
    }

    public void WarpTime(float t)
    {
        src.pitch = t;
        int sizeUse = soundEffectsInUse.Count,
            sizeAvail = Mathf.Min(soundEffectsAvail.Count, 5);
        for(int i = 0; i < sizeUse; i++)
        {
            soundEffectsInUse[i].pitch = t;
        }
        for(int i = 0; i < sizeAvail; i++)  //a couple in case sounds are started during timeshift
        {
            soundEffectsAvail[i].pitch = t;
        }
    }

	void VolumeUp(float i){
		src.volume = Mathf.Min (1f, src.volume + i);
	}

	void VolumeDown(float i){
		src.volume = Mathf.Max (0f, src.volume - i);
	}

}