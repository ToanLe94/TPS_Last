using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    [Header("SFX")]
    [SerializeField] private AudioClip[] soundIdle;
    [SerializeField] private AudioClip[] soundMove;
    [SerializeField] private AudioClip[] soundAttack;
    [SerializeField] private AudioClip[] soundHurt;

    #region Functions.
    private void Start()
    {
        //Toand edit
        shooter = FindObjectOfType<Shooter>();
        grimAnimator = FindObjectOfType<GrimAnimator>();
        ///

        if (headInside)
        {
            headInside.SetActive(false);

        }
        if (headOutside)
        {
            headOutside.SetActive(true);

        }

        stateIdle = UnityEngine.Random.Range(1, 3);
        enemyAnimator.SetInteger("Int State", stateIdle);
    }

    #region Get Functions.
    public Animator GetEnemyAnimator()
    {
        return enemyAnimator;
    }

    public bool GetIsDeath()
    {
        return isDeath;
    }

    public void SetIsDeath(bool isDeath)
    {
        this.isDeath = isDeath;
    }

    public bool GetIsTakeDown()
    {
        return isTakeDown;
    }

    public void SetIsTakeDown(bool isTakeDown)
    {
        this.isTakeDown = isTakeDown;
    }
    #endregion

    public override void TakeDamage(EEnemyBody enemyBody)
    {
        if (isDeath == true || isTakeDown == true) return;

        switch (enemyBody)
        {
            case EEnemyBody.None:
                break;
            case EEnemyBody.Head:
                GetComponent<CharacterStats>().Damage(100);
                TimeToCreateBulletHole();

                StartCoroutine(HeadExplosion());
                break;
            case EEnemyBody.Check:

                enemyAnimator.CrossFadeInFixedTime("damage center", 1);
                GetComponent<CharacterStats>().Damage(40);

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.UpperArmRight:

                enemyAnimator.CrossFadeInFixedTime("zombie hit stand right", 1);
                GetComponent<CharacterStats>().Damage(40);

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ForeArmRight:

                enemyAnimator.CrossFadeInFixedTime("zombie hit stand right", 1);
                GetComponent<CharacterStats>().Damage(40);

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.UpperArmLeft:

                enemyAnimator.CrossFadeInFixedTime("zombie hit stand left", 1);
                GetComponent<CharacterStats>().Damage(40);

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ForeArmLeft:

                enemyAnimator.CrossFadeInFixedTime("zombie hit stand left", 1);
                GetComponent<CharacterStats>().Damage(40);

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ThighLegRight:

                enemyAnimator.CrossFadeInFixedTime("zombie hit leg backward right", 1);
                GetComponent<CharacterStats>().Damage(40);

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ShinLegRight:

                enemyAnimator.CrossFadeInFixedTime("zombie hit leg backward right", 1);
                GetComponent<CharacterStats>().Damage(40);

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.FootLegRight:

                enemyAnimator.CrossFadeInFixedTime("zombie hit leg backward right", 1);
                GetComponent<CharacterStats>().Damage(40);

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ThighLegLeft:
                enemyAnimator.CrossFadeInFixedTime("zombie hit leg backward left", 1);
                GetComponent<CharacterStats>().Damage(40);
                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ShinLegLeft:
                enemyAnimator.CrossFadeInFixedTime("zombie hit leg backward left", 1);
                GetComponent<CharacterStats>().Damage(40);
                TimeToCreateBulletHole();
                break;
            case EEnemyBody.FootLegLeft:
                enemyAnimator.CrossFadeInFixedTime("zombie hit leg backward left", 1);
                GetComponent<CharacterStats>().Damage(40);

                TimeToCreateBulletHole();
                break;
        }
    }

    private IEnumerator HeadExplosion()
    {
        yield return new WaitForFixedUpdate();

        headCollision.SetActive(false);
        zombieTooth.SetActive(false);
        headInside.SetActive(true);
        headOutside.SetActive(false);

        headInside.transform.SetParent(ground);

        GameObject playEffectBlood = Instantiate(bloodExplosion, explosionCenter);
        Destroy(playEffectBlood, UnityEngine.Random.Range(3.0f, 5.0f));
        audioSource.PlayOneShot(soundHeadShot);

        headRigs = headInside.GetComponentsInChildren<Rigidbody>();

        for (partOfHead = 0; partOfHead < headRigs.Length; partOfHead++)
        {
            headRigs[partOfHead].AddExplosionForce(2.0f, explosionCenter.position + 0.02f * UnityEngine.Random.insideUnitSphere, 2.0f, 0.1f, ForceMode.Impulse);
        }

        Destroy(headInside, UnityEngine.Random.Range(3.0f, 5.0f));

        yield return null;
    }
    #endregion
    public void PlaySFXIdle()
    {
        AudioClip clip = soundIdle[UnityEngine.Random.Range(0, soundIdle.Length)];
        EasyAudioUtility.PlayOptionalSound(audioSource, clip, 1, false, false);
        
    }
    public void PlaySFXMoving()
    {
        AudioClip clip = soundMove[UnityEngine.Random.Range(0, soundMove.Length)];
        EasyAudioUtility.PlayOptionalSound(audioSource, clip, 1, false, false);
    }
    public void PlaySFXHurt()
    {
        AudioClip clip = soundHurt[UnityEngine.Random.Range(0, soundHurt.Length)];
        EasyAudioUtility.PlayOptionalSound(audioSource, clip, 1, false, false);
    }
}
