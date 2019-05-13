using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnemyBody
{
    None = 0,
    Head,
    Check,
    Heart,
    UpperArmRight,
    ForeArmRight,
    UpperArmLeft,
    ForeArmLeft,
    ThighLegRight,
    ShinLegRight,
    FootLegRight,
    ThighLegLeft,
    ShinLegLeft,
    FootLegLeft
}

public abstract class Enemy : MonoBehaviour
{
    #region Variable.
    protected bool isDeath = false;
    protected bool isTakeDown = false;
    protected int stateIdle = 0;
    protected int enemyBody = 0;
    protected int partOfHead = 0;

    protected float nextTimeToFire = 0.0f;
    protected float fireRate = 6.0f;

    
    [Header("Head outside and inside")]
    [SerializeField] protected GameObject headOutside;
    [SerializeField] protected GameObject headInside;
    [SerializeField] protected GameObject zombieTooth;

    [SerializeField] protected GameObject headCollision;

    [Header("Head explosion center")]
    [SerializeField] protected Transform explosionCenter;
    [SerializeField] protected Transform ground;

    [Header("Blood Head Explosion Effect")]
    [SerializeField] protected GameObject[] bulletHoleBlood;
    [SerializeField] protected GameObject bloodExplosion;
    protected GameObject currentBulletHole;

    [SerializeField] protected Animator enemyAnimator;
    [SerializeField] protected Shooter shooter;
    [SerializeField] protected GrimAnimator grimAnimator;

    [Header("Audio")]
    [SerializeField] protected AudioClip[] soundBulletHoleBlood;
    [SerializeField] protected AudioClip soundHeadShot;
    [SerializeField] protected AudioSource audioSource;

    protected Rigidbody[] headRigs;
    #endregion

    #region Functions
    protected void TimeToCreateBulletHole()
    {
        if ((grimAnimator.GetIsCanReload() == false) || (grimAnimator.GetIsCanSwitch() == false)) return;

        if ((grimAnimator.GetIsPistol() && grimAnimator.GetIsFire() && grimAnimator.GetIsCanFirePistol() == true) && Time.time >= nextTimeToFire)
        {
            fireRate = 1.1f;
            nextTimeToFire = Time.time + 1.0f / fireRate;
            CreatBulletHole();
        }
        else if ((!grimAnimator.GetIsPistol() && grimAnimator.GetIsFire()) && Time.time >= nextTimeToFire)
        {
            fireRate = 6.0f;
            nextTimeToFire = Time.time + 1.0f / fireRate;
            CreatBulletHole();
        }
    }

    private void CreatBulletHole()
    {
        currentBulletHole = bulletHoleBlood[UnityEngine.Random.Range(0, bulletHoleBlood.Length)];
        audioSource.PlayOneShot(soundBulletHoleBlood[UnityEngine.Random.Range(0, soundBulletHoleBlood.Length)]);

        GameObject bulletHole = Instantiate(currentBulletHole, shooter.GetRayHitPoint(), Quaternion.LookRotation(shooter.GetRayHitNormal())) as GameObject;
        Destroy(bulletHole, UnityEngine.Random.Range(3.0f, 5.0f));
    }

    public virtual void TakeDamage(EEnemyBody enemyBody)
    {

    }
    #endregion
}
