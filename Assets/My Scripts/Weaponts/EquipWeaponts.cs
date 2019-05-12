using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EquipWeaponts : MonoBehaviour
{
    #region Variable.
    private float nextTimeToFire = 0.0f;
    private float fireRate = 6.0f;
    private int randomNumberToPlay = 0;

    [SerializeField] private GrimAnimator grimAnimator;

    [Header("Pistol")]
    [SerializeField] private GameObject pistol;
    [SerializeField] private Transform pistolSlot;
    [SerializeField] private Transform pistolHolding;
    [SerializeField] private Transform pistolSpawnMuzzle;
    [SerializeField] private Transform pistolShellPoint;
    [SerializeField] private ParticleSystem pistolMuzzleFlash1;
    [SerializeField] private ParticleSystem pistolMuzzleFlash2;
    [SerializeField] private ParticleSystem pistolFlame;
    [SerializeField] private ParticleSystem pistolDistortion;
    [SerializeField] private ParticleSystem pistolSmoke;
    [SerializeField] private ParticleSystem pistolCartridgeEject;
    [SerializeField] private AudioClip pistolSound1;
    [SerializeField] private AudioClip pistolSound2;
    [SerializeField] private AudioClip pistolSound3;
    [SerializeField] private AudioClip pistolSound4;

    [Header("Rifle")]
    [SerializeField] private GameObject rifle;
    [SerializeField] private Transform rifleSlot;
    [SerializeField] private Transform rifleHolding;
    [SerializeField] private Transform rifleSpawnMuzzle;
    [SerializeField] private Transform rifleShellPoint;

    [SerializeField] private ParticleSystem rifleMuzzleFlash1;
    [SerializeField] private ParticleSystem rifleMuzzleFlash2;
    [SerializeField] private ParticleSystem rifleFlame;
    [SerializeField] private ParticleSystem rifleDistortion;
    [SerializeField] private ParticleSystem rifleSmoke;
    [SerializeField] private ParticleSystem rifleCartridgeEject;

    [SerializeField] private AudioClip rifleSound1;
    [SerializeField] private AudioClip rifleSound2;
    [SerializeField] private AudioClip rifleSound3;
    [SerializeField] private AudioClip rifleSound4;

    [Header("Knife")]
    [SerializeField] private GameObject knife;
    [SerializeField] private Transform knifeHolding;
    [SerializeField] private Transform knifeSlot;

    [Header("Audio Soure")]
    [SerializeField] private AudioSource audioSource;

    #endregion

    #region Functions.
    private void Awake()
    {
        GetHoldingRifle();
    }

    private void Update()
    {
        if (grimAnimator.GetIsSlotAllWeaponts())
        {
            SetAllSlotWeaponts();
        }

        if (grimAnimator.GetIsHoldingPistol())
        {
            GetHoldingPistol();
        }

        if (grimAnimator.GetIsHoldingRifle())
        {
            GetHoldingRifle();
        }

        if (grimAnimator.GetIsHoldingKnife())
        {
            GetHoldingKnife();
        }
    }

    private void LateUpdate()
    {
        PlayEffectAndSound();
    }

    private void SetAllSlotWeaponts()
    {
        pistol.transform.SetPositionAndRotation(pistolSlot.position, pistolSlot.rotation);
        pistol.transform.SetParent(pistolSlot);

        rifle.transform.SetPositionAndRotation(rifleSlot.position, rifleSlot.rotation);
        rifle.transform.SetParent(rifleSlot);

        knife.transform.SetPositionAndRotation(knifeSlot.position, knifeSlot.rotation);
        knife.transform.SetParent(knifeSlot);
    }

    private void GetHoldingPistol()
    {
        pistol.transform.SetPositionAndRotation(pistolHolding.position, pistolHolding.rotation);
        pistol.transform.SetParent(pistolHolding);

        rifle.transform.SetPositionAndRotation(rifleSlot.position, rifleSlot.rotation);
        rifle.transform.SetParent(rifleSlot);

        knife.transform.SetPositionAndRotation(knifeSlot.position, knifeSlot.rotation);
        knife.transform.SetParent(knifeSlot);
    }

    private void GetHoldingRifle()
    {
        rifle.transform.SetPositionAndRotation(rifleHolding.position, rifleHolding.rotation);
        rifle.transform.SetParent(rifleHolding);

        pistol.transform.SetPositionAndRotation(pistolSlot.position, pistolSlot.rotation);
        pistol.transform.SetParent(pistolSlot);

        knife.transform.SetPositionAndRotation(knifeSlot.position, knifeSlot.rotation);
        knife.transform.SetParent(knifeSlot);
    }

    private void GetHoldingKnife()
    {
        knife.transform.SetPositionAndRotation(knifeHolding.position, knifeHolding.rotation);
        knife.transform.SetParent(knifeHolding);

        pistol.transform.SetPositionAndRotation(pistolSlot.position, pistolSlot.rotation);
        pistol.transform.SetParent(pistolSlot);

        rifle.transform.SetPositionAndRotation(rifleSlot.position, rifleSlot.rotation);
        rifle.transform.SetParent(rifleSlot);
    }

    private void PlayEffectAndSound()
    {
        if (grimAnimator.GetIsAim())
        {
            if ((grimAnimator.GetIsCanReload() == false) || (grimAnimator.GetIsCanSwitch() == false)) return;

            randomNumberToPlay = UnityEngine.Random.Range(0, 4);

            if ((grimAnimator.GetIsPistol() && grimAnimator.GetIsFire() && grimAnimator.GetIsCanFirePistol() == true) && Time.time >= nextTimeToFire) 
            {
                fireRate = 1.1f;
                nextTimeToFire = Time.time + 1.0f / fireRate;

                pistolSmoke.transform.position = pistolSpawnMuzzle.transform.position;

                PlayEffectRandomRange(pistolMuzzleFlash1, pistolMuzzleFlash2, pistolFlame, pistolDistortion, pistolSmoke, randomNumberToPlay);

                pistolCartridgeEject.transform.position = pistolShellPoint.position;
                pistolCartridgeEject.Play();
            }
            else if ((!grimAnimator.GetIsPistol() && grimAnimator.GetIsFire()) && Time.time >= nextTimeToFire)
            {
                fireRate = 6.0f;
                nextTimeToFire = Time.time + 1.0f / fireRate;

                rifleSmoke.transform.position = rifleSpawnMuzzle.transform.position;

                PlayEffectRandomRange(rifleMuzzleFlash1, rifleMuzzleFlash2, rifleFlame, rifleDistortion, rifleSmoke, randomNumberToPlay);

                rifleCartridgeEject.transform.position = rifleShellPoint.position;
                rifleCartridgeEject.Play();
            }

        }

    }

    private void PlayEffectRandomRange(ParticleSystem muzzleFlash1, ParticleSystem muzzleFlash2, ParticleSystem flame, ParticleSystem distortion, ParticleSystem smoke, int numberRandom)
    {
        if (numberRandom == 0)
        {
            muzzleFlash1.Play();
            flame.Play();
            distortion.Play();
            smoke.Play();

            if (grimAnimator.GetIsPistol())
            {
                audioSource.PlayOneShot(pistolSound1);
            }
            else
            {
                audioSource.PlayOneShot(rifleSound1);
            }
        }

        if (numberRandom == 1)
        {
            muzzleFlash1.Play();
            flame.Play();
            smoke.Play();

            if (grimAnimator.GetIsPistol())
            {
                audioSource.PlayOneShot(pistolSound2);
            }
            else
            {
                audioSource.PlayOneShot(rifleSound2);
            }
        }

        if (numberRandom == 2)
        {
            muzzleFlash2.Play();
            flame.Play();
            smoke.Play();

            if (grimAnimator.GetIsPistol())
            {
                audioSource.PlayOneShot(pistolSound3);
            }
            else
            {
                audioSource.PlayOneShot(rifleSound3);
            }
        }

        if (numberRandom == 3)
        {
            muzzleFlash2.Play();
            flame.Play();
            distortion.Play();
            smoke.Play();

            if (grimAnimator.GetIsPistol())
            {
                audioSource.PlayOneShot(pistolSound4);
            }
            else
            {
                audioSource.PlayOneShot(rifleSound4);
            }
        }

        if (numberRandom == 4)
        {
            muzzleFlash2.Play();
            flame.Play();
            distortion.Play();
            smoke.Play();

            if (grimAnimator.GetIsPistol())
            {
                audioSource.PlayOneShot(pistolSound1);
            }
            else
            {
                audioSource.PlayOneShot(rifleSound1);
            }
        }
    }
    #endregion
}
