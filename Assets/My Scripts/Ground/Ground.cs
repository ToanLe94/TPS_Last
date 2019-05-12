using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    #region Variable
    [Header("Bullet Hole")]
    [SerializeField] private GameObject bulletHoleBrick;
    [SerializeField] private GameObject bulletHoleRock;
    [SerializeField] private GameObject bulletHoleDirt;
    [SerializeField] private GameObject bulletHoleGlass;
    [SerializeField] private GameObject bulletHoleWater;
    [SerializeField] private GameObject bulletHoleMetal;
    [SerializeField] private GameObject bulletHoleWood;
    [SerializeField] private GameObject bulletHoleFoliage;

    [Header("Audio")]
    [SerializeField] private AudioClip[] soundBulletFlyby;
    [SerializeField] private AudioClip[] soundBulletHoleBrick;
    [SerializeField] private AudioClip[] soundBulletHoleRock;
    [SerializeField] private AudioClip[] soundBulletHoleDirt;
    [SerializeField] private AudioClip[] soundBulletHoleGlass;
    [SerializeField] private AudioClip[] soundBulletHoleWater;
    [SerializeField] private AudioClip[] soundBulletHoleMetal;
    [SerializeField] private AudioClip[] soundBulletHoleWood;
    [SerializeField] private AudioClip[] soundBulletHoleFoliage;
    [SerializeField] private AudioSource audioSource;

    EMaterialsMode eMaterialsMode = EMaterialsMode.None;
    #endregion

    #region Functions.
    public GameObject GetBulletHole(GameObject currentBulletHole, EMaterialsMode eMaterialsMode)
    {
        if (eMaterialsMode == EMaterialsMode.Brick)
        {
            currentBulletHole = bulletHoleBrick;
            audioSource.PlayOneShot(soundBulletHoleBrick[UnityEngine.Random.Range(0, soundBulletHoleBrick.Length)]);
        }
        else if (eMaterialsMode == EMaterialsMode.Rock)
        {
            currentBulletHole = bulletHoleRock;
            audioSource.PlayOneShot(soundBulletHoleRock[UnityEngine.Random.Range(0, soundBulletHoleRock.Length)]);
        }
        else if (eMaterialsMode == EMaterialsMode.Dirt)
        {
            currentBulletHole = bulletHoleDirt;
            audioSource.PlayOneShot(soundBulletHoleDirt[UnityEngine.Random.Range(0, soundBulletHoleDirt.Length)]);
        }
        else if (eMaterialsMode == EMaterialsMode.Glass)
        {
            currentBulletHole = bulletHoleGlass;
            audioSource.PlayOneShot(soundBulletHoleGlass[UnityEngine.Random.Range(0, soundBulletHoleGlass.Length)]);
        }
        else if (eMaterialsMode == EMaterialsMode.Water)
        {
            currentBulletHole = bulletHoleWater;
            audioSource.PlayOneShot(soundBulletHoleWater[UnityEngine.Random.Range(0, soundBulletHoleWater.Length)]);
        }
        else if (eMaterialsMode == EMaterialsMode.Metal)
        {
            currentBulletHole = bulletHoleMetal;
            audioSource.PlayOneShot(soundBulletHoleMetal[UnityEngine.Random.Range(0, soundBulletHoleMetal.Length)]);
        }
        else if (eMaterialsMode == EMaterialsMode.Wood)
        {
            currentBulletHole = bulletHoleWood;
            audioSource.PlayOneShot(soundBulletHoleWood[UnityEngine.Random.Range(0, soundBulletHoleWood.Length)]);
        }
        else if (eMaterialsMode == EMaterialsMode.Grass)
        {
            currentBulletHole = bulletHoleFoliage;
            audioSource.PlayOneShot(soundBulletHoleFoliage[UnityEngine.Random.Range(0, soundBulletHoleFoliage.Length)]);
        }
        else
        {
            currentBulletHole = null;
        }

        return currentBulletHole;
    }
    #endregion
}
