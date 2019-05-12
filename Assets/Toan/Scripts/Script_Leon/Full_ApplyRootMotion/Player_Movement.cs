using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour {

    Animator animator;
    CharacterController characterController;
    Camera maincamera;

    [System.Serializable]
    public class AnimationSetting
    {
        public string vertiaclVelocityFloat = "Forward";
        public string horizontalVelocityFloat = "Strafe";
        public string jumpBool = "IsJumping";
        public string groundedBool = "IsGrounded";
    }

    [SerializeField]
    public AnimationSetting animations;

    [System.Serializable]
    public class PhysicSettings
    {
        public float gravityModifier = 9.81f;
        public float baseGravity = 50.0f;
        public float resetGravityValue = 0;
        public LayerMask ground;
        
    }
    [SerializeField]
    public PhysicSettings physics;

    [System.Serializable]
    public class MovementSettings
    {
        public float jumpSpeed = 4;
        public float jumpTime = 0.25f;
        public float airSpeed = 2.5f;

    }
    [SerializeField]
    public MovementSettings movement;

    [System.Serializable]
    public class RotatePlayer
    {
        public float lookSpeed = 5.0f;
        public float lookDistance = 30.0f;
    }
    [SerializeField]
    public RotatePlayer rotatePlayer;

    bool Jumping;
    bool resetGravity;
    float gravity;
    Vector3 airControl;

    public float height_character = 0.7f;
    void Awake()
    {
        animator = GetComponent<Animator>();
        SetupAnimator();
    }

    // Use this for initialization
    void Start () {
        characterController = GetComponent<CharacterController>();
        maincamera = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {

        ApplyGravity();
      
    }
    public void Animate(float forward, float strafe)
    {
        animator.SetFloat(animations.vertiaclVelocityFloat, forward);
        animator.SetFloat(animations.horizontalVelocityFloat, strafe);
        animator.SetBool(animations.groundedBool, isGrounded());
        animator.SetBool(animations.jumpBool, Jumping);
    }
    public void Jump()
    {
        if (Jumping)
        {
            return;
        }

        if (isGrounded())
        {
            Jumping = true;
            StartCoroutine(StopJump());
        }
    }

    IEnumerator StopJump()
    {
        yield return new WaitForSeconds(movement.jumpTime);
        Jumping = false;
    }
    public void AirControl(float forward, float strafe)
    {
        if (isGrounded() == false)
        {
            airControl.x = strafe;
            airControl.z = forward;
            airControl = transform.TransformDirection(airControl);
            airControl *= movement.airSpeed;

            characterController.Move(airControl * Time.deltaTime);
        }
    }
    void ApplyGravity()
    {

        if (!isGrounded())
        {
            if (!resetGravity)
            {
                gravity = physics.resetGravityValue;
                resetGravity = true;
            }
            gravity += Time.deltaTime * physics.gravityModifier;
        }
        else
        {
            gravity = physics.baseGravity;
            resetGravity = false;
            //applyforce = 0.0f;
        }

        Vector3 S = new Vector3();  // S=at2 . 
        if (!Jumping)
        {
            S.y -= gravity * Time.deltaTime;
        }
        else
        {
            S.y = movement.jumpSpeed * Time.deltaTime;
        }
        //S.y = -gravity * Time.deltaTime + applyforce * Time.deltaTime;
        characterController.Move( S );
       
    }
    bool isGrounded()
    {
        RaycastHit hit;
        Vector3 start = transform.position + transform.up*height_character;

        //Vector3 start = transform.position ;

        Vector3 dir = Vector3.down;
        float radius = characterController.radius;
        Debug.DrawRay(start, dir, Color.yellow, 0.1f);
        if (Physics.SphereCast(start, radius, dir, out hit, transform.up.magnitude, physics.ground))
        {
            return true;
        }
        
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(test_start + Vector3.down * characterController.height / 2, characterController.radius);

    }
    void SetupAnimator()
    {
        Animator wantedAnim = GetComponentsInChildren<Animator>()[1];
        Avatar wantedAvatar = wantedAnim.avatar;
        animator.avatar = wantedAvatar;
        Destroy(wantedAnim);
        //Animator[] animators = GetComponentsInChildren<Animator>();
        //if (animators.Length > 0)
        //{
        //    for (int i = 0; i < animators.Length; i++)
        //    {
        //        Animator anim = animators[i];
        //        Avatar av = anim.avatar;
        //        if (anim!=animator)
        //        {
        //            animator.avatar = av;
        //            Destroy(anim);
        //        }
        //    }
        //}
    }
    public void CharacterLook()
    {
        Transform mainCamT = maincamera.transform;
        Transform pivotT = mainCamT.parent;
        Vector3 pivotPos = pivotT.position;
        Vector3 lookTarget = pivotPos + (pivotT.forward * rotatePlayer.lookDistance);

        //Debug.DrawLine(transform.position, pivotPos + (pivotT.forward * rotatePlayer.lookDistance), Color.red);
        //Debug.DrawLine(transform.position, lookTarget , Color.green);

        Vector3 thispos = transform.position;
        Vector3 lookDir = lookTarget - thispos;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        lookRot.x = 0;
        lookRot.z = 0;
        
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * rotatePlayer.lookSpeed);
        //Quaternion b = Quaternion.FromToRotation(transform.forward, maincamera.transform.forward);
        //b.x = 0;
        //b.z = 0;
        //Quaternion looktarget = Quaternion.Lerp(transform.rotation, b, Time.deltaTime * rotatePlayer.lookSpeed);

        //transform.rotation = looktarget;
    }
}
