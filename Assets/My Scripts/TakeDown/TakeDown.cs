using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDown : MonoBehaviour
{
    #region Variable.
    [Header("Blood Explosion")]
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private Transform positionHead;
    [SerializeField] private Transform positionNeck;

    [Header("Audio")]
    [SerializeField] private AudioClip audioExplosion;
    [SerializeField] private AudioSource audioSource;

    [Header("Other")]
    [SerializeField] private Transform lookAtToEnemy;
    [SerializeField] private Transform transformToTakeDown;
    [SerializeField] private GrimAnimator grimAnimator;
    [SerializeField] private Zombie zombie;
    #endregion
   
    #region Functions.
    public void HitHead()
    {
        GameObject hit = Instantiate(bloodEffect, positionHead) as GameObject;
        audioSource.PlayOneShot(audioExplosion);
    }

    public void HitNeck()
    {
        GameObject hit = Instantiate(bloodEffect, positionHead) as GameObject;
        audioSource.PlayOneShot(audioExplosion);
    }

    private void OnTriggerStay(Collider other)
    {
        if (zombie.GetIsDeath() == true || zombie.GetIsTakeDown() == true || grimAnimator.GetIsTakeDown() == true)
        {
            return;
        }
        else
        {
            if (other.CompareTag("Grim"))
            {
                Debug.Log("Press E to take down enemy");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    grimAnimator.SetPositionGrim(transformToTakeDown.position);
                    grimAnimator.SetLookAtGrim(lookAtToEnemy);
                    grimAnimator.GetGrimAnimator().CrossFadeInFixedTime("GrimTakeDown", 0.5f);
                    grimAnimator.GetGrimAnimator().CrossFadeInFixedTime("GrimTakeDown", 0.5f, grimAnimator.GetLayerUpperBody());
                    zombie.GetEnemyAnimator().CrossFadeInFixedTime("EnemyTakeDown", 0.5f);
                    zombie.SetIsTakeDown(true);
                    zombie.GetEnemyAnimator().SetBool("Take Down", zombie.GetIsTakeDown());
                }

            }
        }
    }
    #endregion
}
