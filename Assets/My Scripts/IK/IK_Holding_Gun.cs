using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using System;

public class IK_Holding_Gun : MonoBehaviour
{
    #region Variable.
    private float positionWeight = 0.0f;
    private float rotationWeight = 0.0f;
    private int bone = 0;

    private FullBodyBipedIK fullBodyBipedIK;
    private AimIK aimIK;

    [SerializeField] private Transform HandL_HoldDesert;
    [SerializeField] private Transform HandL_HoldM416;
    [SerializeField] private Transform LowerArmL_IdleM416;

    [SerializeField] private Transform aimTransfromPistol;
    [SerializeField] private Transform aimTransfromRifle;
    [SerializeField] private Transform spawnMuzzlePistol;
    [SerializeField] private Transform spawnMuzzleRife;
    [SerializeField] private Transform crosshair;

    private IKEffector leftHand { get { return fullBodyBipedIK.solver.leftHandEffector; } }
    private IKEffector rightHand { get { return fullBodyBipedIK.solver.rightHandEffector; } }

    private Vector3 toLeftHand = Vector3.zero;
    private Vector3 toLeftHandRelative = Vector3.zero;
    private Vector3 rightHandBone = Vector3.zero;

    private Quaternion leftHandRotationRelative;

    private GrimAnimator grimAnimator;

    #endregion

    #region Functions.
    private void Awake()
    {
        fullBodyBipedIK = GetComponent<FullBodyBipedIK>();
        aimIK = GetComponent<AimIK>();
        grimAnimator = GetComponent<GrimAnimator>();

        aimIK.solver.transform = aimTransfromRifle;

        aimIK.Disable();
        fullBodyBipedIK.solver.OnPostUpdate += OnPostFBBIK;
    }

    private void LateUpdate()
    {
        FullBoydyToHoldGun();

        AimingIK();
    }

    private void FullBoydyToHoldGun()
    {
        if (grimAnimator.GetIsPistol())
        {
            fullBodyBipedIK.solver.leftHandEffector.position = HandL_HoldDesert.position;
            fullBodyBipedIK.solver.leftHandEffector.rotation = HandL_HoldDesert.rotation;
        }
        else
        {
            fullBodyBipedIK.solver.leftHandEffector.position = HandL_HoldM416.position;
            fullBodyBipedIK.solver.leftHandEffector.rotation = HandL_HoldM416.rotation;
        }

        //if (grimAnimator.GetIsSwitchWeapont() || grimAnimator.GetIsReload() || grimAnimator.GetIsAim())
        //{
        //    fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0.0f;
        //    fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 0.0f;

        //    fullBodyBipedIK.solver.leftArmMapping.weight = 0.0f;
        //    fullBodyBipedIK.solver.leftArmMapping.maintainRotationWeight = 0.0f;
        //}
        //else
        //{
        //    fullBodyBipedIK.solver.leftHandEffector.positionWeight = 1.0f;
        //    fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 0.5f;

        //    fullBodyBipedIK.solver.leftArmMapping.weight = 1.0f;
        //    fullBodyBipedIK.solver.leftArmMapping.maintainRotationWeight = 0.5f;
        //}
    }

    private void AimingIK()
    {
        aimIK.solver.IKPosition = crosshair.position;

        if (grimAnimator.GetIsPistol())
        {
            aimIK.solver.transform = aimTransfromPistol;
        }
        else
        {
            aimIK.solver.transform = aimTransfromRifle;
        }

        if (grimAnimator.GetIsAim())
        {
            SetLeftHandAndRightHandPosition();

            aimIK.solver.IKPositionWeight = 1.0f;
            for (bone = 0; bone < aimIK.solver.bones.Length; bone++)
            {
                aimIK.solver.bones[bone].weight = 1.0f;
            }
        }
        else
        {
            aimIK.solver.IKPositionWeight = 0.0f;
            for (bone = 0; bone < aimIK.solver.bones.Length; bone++)
            {
                aimIK.solver.bones[bone].weight = 0.0f;
            }
        }
    }

    private void SetLeftHandAndRightHandPosition()
    {
        // Find out how left hand is positioned relative to the right hand rotation.
        toLeftHand = leftHand.bone.position - rightHand.bone.position;
        toLeftHandRelative = rightHand.bone.InverseTransformDirection(toLeftHand);

        leftHandRotationRelative = Quaternion.Inverse(rightHand.bone.rotation) * leftHand.bone.rotation;

        aimIK.solver.Update();

        // Position the left Hand on the gun.
        rightHandBone = new Vector3(rightHand.bone.position.x, rightHand.bone.position.y, rightHand.bone.position.z);
        leftHand.position = rightHandBone + rightHand.bone.TransformDirection(toLeftHandRelative);
        leftHand.positionWeight = 1.0f;

        rightHand.position = rightHand.bone.position;
        rightHand.positionWeight = 1.0f;
        fullBodyBipedIK.solver.GetLimbMapping(FullBodyBipedChain.RightArm).maintainRotationWeight = 1.0f;

        fullBodyBipedIK.solver.Update();
    }

    private void OnPostFBBIK()
    {
        leftHand.bone.rotation = rightHand.bone.rotation * leftHandRotationRelative;
    }
    #endregion
}
