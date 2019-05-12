using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolAnimation : MonoBehaviour
{
    #region Variable.
    private bool isFire;
    private int isNotAmmo;

    [SerializeField] private Animator pistolAnim;

    [SerializeField] private GrimAnimator grimAnimator;
    [SerializeField] private EquipWeaponts equipWeaponts;
    #endregion

    #region Functions.
    private void Update()
    {
        Fire();
    }

    private void Fire()
    {
        isFire = Input.GetKeyDown(KeyCode.Mouse0);

        if (grimAnimator.GetIsPistol() && grimAnimator.GetIsAim() && grimAnimator.GetIsCanFirePistol() == true)
        {
            pistolAnim.SetBool("IsFire", isFire);
        }
    }

    private void NotAmmo()
    {
        
    }
    #endregion
}
