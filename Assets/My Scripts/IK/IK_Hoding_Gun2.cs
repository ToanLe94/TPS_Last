using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using System;

public class IK_Hoding_Gun2 : MonoBehaviour
{
    #region Variable.
    [SerializeField] private GrimAnimator grimAnimator;
    [SerializeField] private BipedIK bipedIK;

    [Header("Left hand to holding gun")]
    [SerializeField] private Transform HandL_HoldDesert;
    [SerializeField] private Transform HandL_HoldM416;

    [Header("Aim position and crosshair")]
    [SerializeField] private Transform aimTransfromPistol;
    [SerializeField] private Transform aimTransfromRifle;

    [SerializeField] private Transform crosshair;

    [SerializeField] public Transform targetIK_IsBitted;

    private Transform HeadLookAt;
    #endregion

    #region Functions.
    private void LateUpdate()
    {
        IKFootLeg();
        LeftHandToHoldingGun();
        Aimming();
        ReloadingAndFirePistol();
        SetIKWhenIsBitted();
    }

    private void IKFootLeg()
    {
        if (grimAnimator.GetHorizontal() == 0.0f && grimAnimator.GetVertical() == 0.0f && (grimAnimator.GetMouseX() >= 0.9f || grimAnimator.GetMouseX() <= -0.9f))
        {
            bipedIK.solvers.leftFoot.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
            bipedIK.solvers.leftFoot.SetIKRotationWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
            bipedIK.solvers.rightFoot.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
            bipedIK.solvers.rightFoot.SetIKRotationWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
        }
        else if (grimAnimator.GetHorizontal() == 0.0f && grimAnimator.GetVertical() == 0.0f)
        {
            bipedIK.solvers.leftFoot.SetIKPositionWeight(Mathf.Lerp(0.0f, 1.0f, 1.0f));
            bipedIK.solvers.leftFoot.SetIKRotationWeight(Mathf.Lerp(0.0f, 1.0f, 1.0f));
            bipedIK.solvers.rightFoot.SetIKPositionWeight(Mathf.Lerp(0.0f, 1.0f, 1.0f));
            bipedIK.solvers.rightFoot.SetIKRotationWeight(Mathf.Lerp(0.0f, 1.0f, 1.0f));
        }
        else
        {
            bipedIK.solvers.leftFoot.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
            bipedIK.solvers.leftFoot.SetIKRotationWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
            bipedIK.solvers.rightFoot.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
            bipedIK.solvers.rightFoot.SetIKRotationWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
        }
    }

    private void LeftHandToHoldingGun()
    {
        if (grimAnimator.GetIsPistol())
        {
            bipedIK.solvers.leftHand.SetIKPosition(HandL_HoldDesert.position);
            bipedIK.solvers.leftHand.SetIKRotation(HandL_HoldDesert.rotation);

            bipedIK.solvers.aim.transform = aimTransfromPistol;
        }
        else
        {
            bipedIK.solvers.leftHand.SetIKPosition(HandL_HoldM416.position);
            bipedIK.solvers.leftHand.SetIKRotation(HandL_HoldM416.rotation);

            bipedIK.solvers.aim.transform = aimTransfromRifle;
        }

        bipedIK.solvers.leftHand.SetIKRotationWeight(Mathf.Lerp(0.0f, 1.0f, 1.0f));
    }

    private void Aimming()
    {
        if (grimAnimator.GetIsAim())
        {
            bipedIK.solvers.leftHand.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
            bipedIK.solvers.leftHand.SetIKRotationWeight(Mathf.Lerp(0.0f, 0.5f, 1.0f));

            if (grimAnimator.GetIsCanSwitch() == false)
            {
                bipedIK.solvers.aim.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
                return;
            }

            bipedIK.solvers.aim.SetIKPositionWeight(Mathf.Lerp(0.0f, 1.0f, 1.0f));
        }
        else
        {
            bipedIK.solvers.aim.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));

            if (grimAnimator.GetIsCanSwitch() == false)
            {
                if (!grimAnimator.GetIsPistol())
                {
                    bipedIK.solvers.leftHand.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
                    return;
                }
            }

            bipedIK.solvers.leftHand.SetIKPositionWeight(Mathf.Lerp(0.0f, 1.0f, 1.0f));
            bipedIK.solvers.leftHand.SetIKRotationWeight(Mathf.Lerp(0.0f, 1.0f, 1.0f));
        }

        bipedIK.solvers.aim.SetIKPosition(crosshair.position);
    }

    private void ReloadingAndFirePistol()
    {
        if (grimAnimator.GetIsCanReload() == false || grimAnimator.GetIsTakeDown() == true)
        {
            bipedIK.solvers.leftHand.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f)); 
            bipedIK.solvers.leftHand.SetIKRotationWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
            bipedIK.solvers.aim.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
        }

        if (grimAnimator.GetIsCanFirePistol() == false)
        {
            bipedIK.solvers.aim.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
            
        }

    }
    private void SetIKWhenIsBitted()
    {
        if (grimAnimator.isRunningAnimation("BittingReaction",0))
        {
            //bipedIK.solvers.leftHand.SetIKPositionWeight(Mathf.Lerp(1.0f, 0.0f, 1.0f));
            //bipedIK.solvers.leftHand.SetIKRotationWeight(1);
            //bipedIK.solvers.leftHand.SetIKPositionWeight(1);

            //bipedIK.solvers.leftHand.SetIKPosition(targetIK_IsBitted.position);
            //bipedIK.solvers.leftHand.SetIKRotation(targetIK_IsBitted.rotation);


        }

    }

   
    #endregion
}
