using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimMoments : MonoBehaviour
{
    #region Variable.
    private float vertical = 0.0f, horizontal = 0.0f;
    private float speed = 2.0f;

    float damping = 0.5f;
    float sensivity = 1.0f;

    private Vector2 V2MouseMove = Vector2.zero;
    private Vector2 V2CurrentMouseMove = Vector2.zero;
    private Vector2 V2DirectionMove = Vector2.zero;

    private Vector3 V3PlayerRotationYaw = Vector2.zero;
    private Vector3 V3Direction = Vector2.zero;

    [SerializeField] private GrimAnimator grimAnimator;

    #endregion

    #region Get Functions.
    public float GetSensivity()
    {
        return sensivity;
    }
    #endregion

    #region Functions.
    private void Update()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        V2MouseMove = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (grimAnimator.GetIsTakeDown() == false)
        {
            if (!isAnimationRunning("BittingReaction", 0))
            {
                Moving();
                Turning();
            }

        }

    }
    private void FixedUpdate()
    {
        //vertical = Input.GetAxis("Vertical");
        //horizontal = Input.GetAxis("Horizontal");

        //V2MouseMove = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        //if (grimAnimator.GetIsTakeDown() == false)
        //{
        //    if (!isAnimationRunning("BittingReaction", 0))
        //    {
        //        Moving();
        //        Turning();
        //    }

        //}
    }
    public bool isAnimationRunning(string name, int indexlayer)
    {

         return grimAnimator.GetGrimAnimator().GetCurrentAnimatorStateInfo(indexlayer).IsName(name);
    }
    private void Moving()
    {
        if (grimAnimator.GetIsRun() && grimAnimator.GetVertical() > 0.0f)
        {
            speed = Mathf.SmoothStep(5.0f, 5.0f, 0.5f);
        }

        if (grimAnimator.GetIsAim())
        {
            speed = Mathf.SmoothStep(1.0f, 1.0f, 0.5f);
        }

        if ((!grimAnimator.GetIsRun() && !grimAnimator.GetIsAim()) ||
            (grimAnimator.GetIsRun() && grimAnimator.GetVertical() < 0.0f))
        {
            speed = Mathf.SmoothStep(2.0f, 2.0f, 0.5f);
        }

        V2DirectionMove = new Vector2(vertical * speed, horizontal * speed);

        if (grimAnimator.GetIsRun())
        {
            V3Direction = transform.forward * V2DirectionMove.x * Time.deltaTime;

        }
        else
        {
            V3Direction = transform.forward * V2DirectionMove.x * Time.deltaTime + transform.right * V2DirectionMove.y * Time.deltaTime;

        }
        GetComponent<CharacterController>().Move(V3Direction); //Toan edit To step over Platform
        //transform.position += V3Direction;  // old way
    }

    private void Turning()
    {
        V2CurrentMouseMove.x = Mathf.Lerp(V2CurrentMouseMove.x, V2MouseMove.x, 1.0f / damping);
        V3PlayerRotationYaw = Vector3.up * V2CurrentMouseMove.x * sensivity;
        transform.Rotate(V3PlayerRotationYaw);
    }
    #endregion
}
