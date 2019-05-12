using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tyrant : Enemy
{
    #region Functions
    private void Start()
    {
        headInside.SetActive(false);
    }

    public override void TakeDamage(EEnemyBody enemyBody)
    {
        //if (isDeath == true) return;

        switch (enemyBody)
        {
            case EEnemyBody.None:
                break;
            case EEnemyBody.Head:
                enemyAnimator.Play("damage head");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.Check:
                enemyAnimator.Play("damage center");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.Heart:
                GetComponent<CharacterStats>().Damage(100);
                TimeToCreateBulletHole();

                StartCoroutine(HeartExplosion());
                break;
            case EEnemyBody.UpperArmRight:
                enemyAnimator.Play("damage right");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ForeArmRight:
                enemyAnimator.Play("damage right");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.UpperArmLeft:
                enemyAnimator.Play("damage left");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ForeArmLeft:
                enemyAnimator.Play("damage left");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ThighLegRight:
                enemyAnimator.Play("damage right");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ShinLegRight:
                enemyAnimator.Play("damage right");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.FootLegRight:
                enemyAnimator.Play("damage right");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ThighLegLeft:
                enemyAnimator.Play("damage left");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.ShinLegLeft:
                enemyAnimator.Play("damage left");

                TimeToCreateBulletHole();
                break;
            case EEnemyBody.FootLegLeft:
                enemyAnimator.Play("damage left");

                TimeToCreateBulletHole();
                break;
        }
    }

    private IEnumerator HeartExplosion()
    {
        yield return new WaitForFixedUpdate();

        headCollision.SetActive(false);
        headInside.SetActive(true);
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
}
