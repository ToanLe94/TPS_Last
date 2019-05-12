using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimFootStep : MonoBehaviour
{
    #region Variable.
    [Header("Audio")]
    [SerializeField] private AudioClip[] audioClipsGlass;
    [SerializeField] private AudioClip[] audioClipsGrass;
    [SerializeField] private AudioClip[] audioClipsWood;
    [SerializeField] private AudioClip[] audioClipsRock;
    [SerializeField] private AudioClip[] audioClipsWater;
    [SerializeField] private AudioClip[] audioClipsDirt;
    [SerializeField] private AudioClip[] audioClipsMetal;
    private AudioClip currentAudioClips;
    [SerializeField] private AudioSource audioSource;

    [Header("Effect water footstep")]
    [SerializeField] private ParticleSystem water1;
    [SerializeField] private ParticleSystem water2;

    [SerializeField] private GrimAnimator grimAnimator;

    #endregion

    #region Functions.
    private void Step()
    {
        currentAudioClips = GetRamdomAudioClip();
        audioSource.PlayOneShot(currentAudioClips);
    }

    private AudioClip GetRamdomAudioClip()
    {
        if (grimAnimator.GetObjectFootStep() == (int)EMaterialsMode.Grass)
        {
            return audioClipsGrass[UnityEngine.Random.Range(0, audioClipsGrass.Length)];
        }

        if (grimAnimator.GetObjectFootStep() == (int)EMaterialsMode.Wood)
        {
            return audioClipsWood[UnityEngine.Random.Range(0, audioClipsWood.Length)];
        }

        if (grimAnimator.GetObjectFootStep() == (int)EMaterialsMode.Water)
        {
            water1.transform.position = transform.position;
            water2.transform.position = transform.position;

            water1.Play();
            water2.Play();

            return audioClipsWater[UnityEngine.Random.Range(0, audioClipsWater.Length)];
        }

        if (grimAnimator.GetObjectFootStep() == (int)EMaterialsMode.Dirt)
        {
            return audioClipsDirt[UnityEngine.Random.Range(0, audioClipsDirt.Length)];
        }

        if (grimAnimator.GetObjectFootStep() == (int)EMaterialsMode.Rock)
        {
            return audioClipsRock[UnityEngine.Random.Range(0, audioClipsRock.Length)];
        }

        if (grimAnimator.GetObjectFootStep() == (int)EMaterialsMode.Metal)
        {
            return audioClipsMetal[UnityEngine.Random.Range(0, audioClipsMetal.Length)];
        }

        if (grimAnimator.GetObjectFootStep() == (int)EMaterialsMode.Glass)
        {
            return audioClipsGlass[UnityEngine.Random.Range(0, audioClipsGlass.Length)];
        }

        return currentAudioClips;
    }
    #endregion
}
