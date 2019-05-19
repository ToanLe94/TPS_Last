using UnityEngine.Audio;
using System;
using UnityEngine;

public class EasyAudioUtility : MonoBehaviour
{
    //Static reference
    public static EasyAudioUtility instance;
    //Master Audio Mixer
    public AudioMixerGroup mixerGroup;

    //Helper Class
    public EasyAudioUtility_Helper[] helper;

    public EasyAudioUtility_SceneManager soundSceneManager;
    void Awake()
    {
        //creating static instance so we don't need any physical reference
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //Adding audio source in all helpers
        foreach (EasyAudioUtility_Helper h in helper)
        {
            h.source = gameObject.AddComponent<AudioSource>();
            h.source.clip = h.clip;
            h.source.loop = h.loop;

            h.source.outputAudioMixerGroup = mixerGroup;
        }

        //playing BG Audio Clip on Main Menu
        if (FindObjectOfType<MainMenuController>())
        {
            Play("BG");
            if (FindObjectOfType<OptionsController_Game>())
            {
                foreach (EasyAudioUtility_Helper h in helper)
                {
                    if (h.name == "BG")
                        FindObjectOfType<OptionsController_Game>().musicSource = h.source;
                }

                
            }
        }
        
    }

    /// <summary>
    /// Play an Audio Clip defined in the inspector
    /// </summary>
    /// <param name="sound"></param>
    public void Play(string sound)
    {
        EasyAudioUtility_Helper h = Array.Find(helper, item => item.name == sound);
        //randomizing volume by variation
         h.source.volume = h.volume * (1f + UnityEngine.Random.Range(-h.volumeVariance / 2f, h.volumeVariance / 2f));
        //randomizing pitch by variation
        h.source.pitch = h.pitch * (1f + UnityEngine.Random.Range(-h.pitchVariance / 2f, h.pitchVariance / 2f));

        //playing it after setting all variations
        h.source.Play();
    }

    /// <summary>
    /// Stops an Audio which is being played
    /// </summary>
    /// <param name="sound"></param>
    public void Stop(string sound)
    {
        EasyAudioUtility_Helper h = Array.Find(helper, item => item.name == sound);
        //Stopping
        h.source.Stop();
    }
    public static void InstantiateClip(Vector3 pos, AudioClip clip, float time = 2f, bool randomizePitch = false, float randomPitchMin = 1, float randomPitchMax = 1)
    {
        GameObject clone = new GameObject("one shot audio");
        clone.transform.position = pos;
        AudioSource audio = clone.AddComponent<AudioSource>();
        audio.spatialBlend = 1;
        audio.clip = clip;
        audio.Play();

        Destroy(clone, time);
    }
    public static void PlaySound(AudioSource audioS, AudioClip clip, bool randomizePitch = false, float randomPitchMin = 1, float randomPitchMax = 1)
    {

        audioS.clip = clip;
        audioS.loop = true;
        if (randomizePitch == true)
        {
            audioS.pitch = UnityEngine.Random.Range(randomPitchMin, randomPitchMax);
        }
        audioS.Play();
    }
    public static void PlayOptionalSound(AudioSource audioS, AudioClip clip, int spatialBlend, bool isLoop,bool isPlayOnShot , bool randomizePitch = false, float randomPitchMin = 1, float randomPitchMax = 1)
    {
        audioS.clip = clip;
        audioS.loop = isLoop;
        audioS.spatialBlend = spatialBlend;
        if (randomizePitch == true)
        {
            audioS.pitch = UnityEngine.Random.Range(randomPitchMin, randomPitchMax);
        }
        if (isPlayOnShot)
        {
            audioS.PlayOneShot(clip);
        }
        else
        {
            audioS.Play();
        }
    }
}
