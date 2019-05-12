using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMaterialsMode
{
    None = 0,
    Brick,
    Rock,
    Dirt,
    Glass,
    Water,
    Metal,
    Wood,
    Grass
}

public class Shooter : MonoBehaviour
{
    #region Variable.
    private int countEnemy = 0;

    private float maxX = 0.05f, maxY = 0.05f;
    private float minX = 0.0f, minY = 0.0f;
    private float nextTimeToFire = 0.0f;

    private float distance = 150.0f;
    private float damage = 10.0f;
    private float impaceForce = 0.0f;
    private float fireRate = 6.0f;

    private bool isCrosshairActive = false;

    private Vector3 V3Euler = Vector3.zero;
    private Vector3 V3Direction = Vector3.zero;

    private Quaternion QLookRotation;

    [Header("Bullet Spawn")]
    [SerializeField] private Transform bulletSpawnPistol;
    [SerializeField] private Transform bulletSpawnRifle;
    private Transform currentBulletSpawn;

    [Header("Lazer")]
    [SerializeField] private LineRenderer lazerPistol;
    [SerializeField] private LineRenderer lazerRifle;


    private GameObject currentBulletHole;
    private GameObject checkBulletHole;

    private RaycastHit rayHit;
    private Ray ray;

    [Header("Other")]
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GrimAnimator grimAnimator;
    [SerializeField] private Ground ground;
    [SerializeField] private Transform gameGround;

    private Enemy enemy;

    private EMaterialsMode eMaterialsMode = EMaterialsMode.None;
    private EEnemyBody eEnemyBody = EEnemyBody.None;
    public LayerMask shootingMask;
    #endregion

    #region Get Functions.
    public Vector3 GetRayHitPoint()
    {
        return rayHit.point;
    }

    public Vector3 GetRayHitNormal()
    {
        return rayHit.normal;
    }
    #endregion

    #region Functions.
    private void Awake()
    {
        crosshair.SetActive(isCrosshairActive);

        lazerPistol.gameObject.SetActive(isCrosshairActive);
        lazerRifle.gameObject.SetActive(isCrosshairActive);

        minX = maxX * -1;
        minY = maxY * -1;
        //shootingMask = LayerMask.GetMask("Zombie", "Obstable", "Ground");
    }

    private void Update()
    {
        if (grimAnimator.GetIsAim())
        {
            isCrosshairActive = true;

            if (grimAnimator.GetIsPistol())
            {
                lazerPistol.gameObject.SetActive(isCrosshairActive);
                lazerRifle.gameObject.SetActive(!isCrosshairActive);
            }
            else
            {
                lazerPistol.gameObject.SetActive(!isCrosshairActive);
                lazerRifle.gameObject.SetActive(isCrosshairActive);
            }
        }
        else
        {
            isCrosshairActive = false;

            lazerPistol.gameObject.SetActive(isCrosshairActive);
            lazerRifle.gameObject.SetActive(isCrosshairActive);
        }

        crosshair.SetActive(isCrosshairActive);
    }

    private void LateUpdate()
    {
        if (grimAnimator.GetIsAim())
        {
            AimLine();
        }
    }

    private void AimLine()
    {
        RandomPostionAnim();

        if (Physics.Raycast(ray, out rayHit, distance, shootingMask))
        {
            GetTagObject();

            crosshair.transform.position = rayHit.point;
            crosshair.transform.rotation = Quaternion.LookRotation(rayHit.normal);

            TimeToFireGun();
            LazerLine();
            ShooterEnemy();
        }
        else
        {
            crosshair.transform.position = ray.GetPoint(distance);
            LazerLine();
        }
    }

    private void TimeToFireGun()
    {
        if ((grimAnimator.GetIsCanReload() == false) || (grimAnimator.GetIsCanSwitch() == false)) return;

        if ((grimAnimator.GetIsPistol() && grimAnimator.GetIsFire() && grimAnimator.GetIsCanFirePistol() == true) && Time.time >= nextTimeToFire)
        {
            fireRate = 1.1f;
            nextTimeToFire = Time.time + 1.0f / fireRate;
            SpawnBulletOrHitShooterOnObject();
        }
        else if ((!grimAnimator.GetIsPistol() && grimAnimator.GetIsFire()) && Time.time >= nextTimeToFire)
        {
            fireRate = 6.0f;
            nextTimeToFire = Time.time + 1.0f / fireRate;
            SpawnBulletOrHitShooterOnObject();
        }
    }

    private void LazerLine()
    {
        if (rayHit.collider)
        {
            if (grimAnimator.GetIsPistol())
            {
                lazerPistol.SetPosition(1, new Vector3(0.0f, 0.0f, rayHit.distance));
            }
            else
            {
                lazerRifle.SetPosition(1, new Vector3(0.0f, 0.0f, rayHit.distance));
            }
        }
        else
        {
            lazerPistol.SetPosition(1, Vector3.forward * 150.0f);
            lazerRifle.SetPosition(1, Vector3.forward * 150.0f);
        }
    }

    private void ShooterEnemy()
    {
        if (grimAnimator.GetIsFire())
        {
            enemy = rayHit.collider.GetComponentInParent<Enemy>();
            //var enemy = rayHit.collider.GetComponent<CharacterStats>();
               enemy?.TakeDamage(eEnemyBody);
        }
    }

    private void SpawnBulletOrHitShooterOnObject()
    {
        #region Spawn bullet with collision (but I don't use anymore - Code preview).
        //Rigidbody bulletInstance = Instantiate(bulletRigPrefabs, currentBulletSpawn) as Rigidbody;
        //if (grimAnimator.GetIsPistol())
        //{
        //    bulletInstance.AddForce(V3Direction * 100.0f, ForceMode.Impulse);
        //}
        //else
        //{
        //    bulletInstance.AddForce(V3Direction * 50.0f, ForceMode.Impulse);
        //}

        //bulletInstance.transform.SetParent(ground);
        #endregion

        #region Old Way.
        checkBulletHole = ground.GetBulletHole(currentBulletHole, eMaterialsMode);

        if (checkBulletHole == null) return;

        GameObject bulletHole = Instantiate(ground.GetBulletHole(currentBulletHole, eMaterialsMode), rayHit.point, Quaternion.LookRotation(rayHit.normal)) as GameObject;

        Destroy(bulletHole, UnityEngine.Random.Range(3.0f, 5.0f));

        if (grimAnimator.GetIsPistol())
        {
            impaceForce = 500.0f;
        }
        else
        {
            impaceForce = 150.0f;
        }

        rayHit.rigidbody?.AddForce(-rayHit.normal * impaceForce);
        #endregion
    }

    private void GetTagObject()
    {
        #region Get Material Object.
        if (rayHit.transform.CompareTag("Brick"))
        {
            eEnemyBody = EEnemyBody.None;
            eMaterialsMode = EMaterialsMode.Brick;
        }
        else if(rayHit.transform.CompareTag("Rock"))
        {
            eEnemyBody = EEnemyBody.None;
            eMaterialsMode = EMaterialsMode.Rock;
        }
        else if (rayHit.transform.CompareTag("Wood"))
        {
            eEnemyBody = EEnemyBody.None;
            eMaterialsMode = EMaterialsMode.Wood;
        }
        else if (rayHit.transform.CompareTag("Glass"))
        {
            eEnemyBody = EEnemyBody.None;
            eMaterialsMode = EMaterialsMode.Glass;
        }
        else if (rayHit.transform.CompareTag("Grass"))
        {
            eEnemyBody = EEnemyBody.None;
            eMaterialsMode = EMaterialsMode.Grass;
        }
        else if (rayHit.transform.CompareTag("Dirt"))
        {
            eEnemyBody = EEnemyBody.None;
            eMaterialsMode = EMaterialsMode.Dirt;
        }
        else if (rayHit.transform.CompareTag("Metal"))
        {
            eEnemyBody = EEnemyBody.None;
            eMaterialsMode = EMaterialsMode.Metal;
        }
        else if (rayHit.transform.CompareTag("Water"))
        {
            eEnemyBody = EEnemyBody.None;
            eMaterialsMode = EMaterialsMode.Water;
        }
        #endregion

        #region Get Enemy Body.
        if (rayHit.transform.CompareTag("Head"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.Head;
        }
        else if (rayHit.transform.CompareTag("Check"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.Check;
        }
        else if (rayHit.transform.CompareTag("Heart"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.Heart;
        }
        else if (rayHit.transform.CompareTag("Upper Arm Right"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.UpperArmRight;
        }
        else if (rayHit.transform.CompareTag("Fore Arm Right"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.ForeArmRight;
        }
        else if (rayHit.transform.CompareTag("Upper Arm Left"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.UpperArmLeft;
        }
        else if (rayHit.transform.CompareTag("Fore Arm Left"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.ForeArmLeft;
        }
        else if (rayHit.transform.CompareTag("Thigh Leg Right"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.ThighLegRight;
        }
        else if (rayHit.transform.CompareTag("Shin Leg Right"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.ShinLegRight;
        }
        else if (rayHit.transform.CompareTag("Foot Leg Right"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.FootLegRight;
        }
        else if (rayHit.transform.CompareTag("Thigh Leg Left"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.ThighLegLeft;
        }
        else if (rayHit.transform.CompareTag("Shin Leg Left"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.ShinLegLeft;
        }
        else if (rayHit.transform.CompareTag("Foot Leg Left"))
        {
            eMaterialsMode = EMaterialsMode.None;
            eEnemyBody = EEnemyBody.FootLegLeft;
        } 
        #endregion
    }

    private void RandomPostionAnim()
    {
        V3Euler = transform.eulerAngles;

        if (grimAnimator.GetIsPistol())
        {
            currentBulletSpawn = bulletSpawnPistol;

            V3Direction = Camera.main.transform.forward;
        }
        else
        {
            currentBulletSpawn = bulletSpawnRifle;

            V3Direction = Camera.main.transform.forward;
        }

        if (!grimAnimator.GetIsPistol() && grimAnimator.GetIsFire())
        {
            currentBulletSpawn = bulletSpawnRifle;

            V3Euler.x = UnityEngine.Random.Range(minX, maxX);
            V3Euler.y = UnityEngine.Random.Range(minY, maxY);
            V3Direction = new Vector3(Camera.main.transform.forward.x + V3Euler.x,
                                              Camera.main.transform.forward.y + V3Euler.y,
                                              Camera.main.transform.forward.z);
        }

        ray = new Ray(currentBulletSpawn.position, V3Direction);
    }
    #endregion
}
