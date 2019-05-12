using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimCamera : MonoBehaviour
{
    #region Variable.
    private bool isReturnAngle = false;
    private bool pitchLock = false;

    private float mouseSensitivity = 0.0f;
    private float distanceFromTarget = 1.4f;
    private float rotationSmoothTime = 8.0f;
    private float pitchTarget = 0.0f;
    private float pitch, yaw = 0.0f;

    private float moveSpeed = 5.0f;
    private float returnSpeed = 9.0f;
    private float wallPush = 0.7f;

    private Vector2 V2PicthMinMax = new Vector2(-80.0f, 80.0f);

    private Vector3 V3PitchYaw = Vector3.zero;
    private Vector3 V3EndAngleTarget = Vector3.zero;
    private Vector3 V3RotationSmoothVelocity = Vector3.zero;
    private Vector3 CurrentRotation = Vector3.zero;
    private Vector3 V3TargetPosition = Vector3.zero;
    private Vector3 V3Normal = Vector3.zero;
    private Vector3 V3Push = Vector3.zero;

    [Header("Class Grim Animator")]
    [SerializeField] private GrimAnimator grimAnimator;
    [SerializeField] private GrimMoments grimMoments;
    [SerializeField] private Transform target;

    [Header("Mask")]
    [SerializeField] private LayerMask collisionMask;

    private RaycastHit raycastHit;
    private Ray ray;
    Camera mainCamera;
    #endregion

    #region Functions.
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mainCamera = Camera.main;
        mouseSensitivity = grimMoments.GetSensivity();
    }

    private void LateUpdate()
    {
        TargetSmoothPosition();

        CollisionCheck(target.position - transform.forward * distanceFromTarget);
        //WallCheck();
        CheckWall();
        if (grimAnimator.GetIsTakeDown() == false)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!pitchLock)
            {
                StartCoroutine(ReturnAngle());
            }
            else
            {
                if (grimAnimator.GetIsAim())
                {
                    StartCoroutine(ReturnAngle());
                }
                //else
                //{
                //    pitch = V2PicthMinMax.y;
                //}
            }
        }
        else
        {
            if (yaw > 0.0f)
            {
                yaw = 180.0f;
            }
            else
            {
                yaw = -180.0f;
            }

            pitch = 10.0f;
            distanceFromTarget = 1.4f;
        }

        V3PitchYaw = new Vector2(pitch, yaw);

        CurrentRotation = Vector3.Lerp(CurrentRotation, V3PitchYaw, rotationSmoothTime * Time.deltaTime);

        V3EndAngleTarget = transform.eulerAngles;
        V3EndAngleTarget.x = 0.0f;
        target.eulerAngles = V3EndAngleTarget;

        transform.eulerAngles = CurrentRotation;
    }

    private IEnumerator ReturnAngle()
    {
        if (isReturnAngle == true)
        {
            pitch = 10.0f;

            isReturnAngle = !isReturnAngle;
        }

        yield return new WaitForSeconds(0.2f);

        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, V2PicthMinMax.x, V2PicthMinMax.y);
    }

    private void TargetSmoothPosition()
    {
        pitchTarget = target.position.y;

        if (grimAnimator.GetIsAim())
        {
            distanceFromTarget = 0.75f;
            //pitchTarget = Mathf.Lerp(1.6f, 1.65f, Time.deltaTime * 10);
        }
        else
        {
            distanceFromTarget = 1.4f;
            //pitchTarget = Mathf.Lerp(1.65f, 1.6f, Time.deltaTime * 10);
        }

        V3TargetPosition = new Vector3(target.position.x, pitchTarget, target.position.z);
        target.position = Vector3.SmoothDamp(target.position, V3TargetPosition, ref V3RotationSmoothVelocity, 0.1f * Time.deltaTime);
    }

    private void CollisionCheck(Vector3 V3ReturnPoint)
    {
        if (Physics.Linecast(target.position, V3ReturnPoint, out raycastHit, collisionMask))
        {
            V3Normal = raycastHit.normal * wallPush;
            V3Push = raycastHit.point + V3Normal;

            transform.position = Vector3.Lerp(transform.position, V3Push, moveSpeed * Time.deltaTime);

            return;
        }
        transform.position = Vector3.Lerp(transform.position, V3ReturnPoint, returnSpeed * Time.deltaTime);
        pitchLock = false;
    }

    private void WallCheck()
    {
        ray = new Ray(target.position, -target.forward);

        if (Physics.SphereCast(ray, 0.5f, out raycastHit, 0.7f, collisionMask))
        {
            pitchLock = true;
            isReturnAngle = true;
        }
        else
        {
            pitchLock = false;
        }
    }
    #endregion

    void CheckWall()
    {
        RaycastHit hit;

        //Transform mainCamT = mainCamera.transform;
        //Vector3 mainCamPos = mainCamT.position;
        //Vector3 pivotPos = pivot.position;

        Vector3 start = target.transform.position;
        Vector3 dir = transform.position - start;

        //float dist = Mathf.Abs(shoulder == Shoulder.Left ? cameraSettings.camPositionOffsetLeft.z : cameraSettings.camPositionOffsetRight.z);

        //Debug.DrawRay(start, dir, Color.red, dist);
        if (Physics.SphereCast(start, mainCamera.nearClipPlane, dir, out hit, dir.magnitude, collisionMask
          /*Physics.Raycast(start,dir,out hit,dist,wallLayer*/))
        {
            MoveCamUp(hit, start, dir, transform);
            //transform.position = hit.point;

        }
        else
        {
            //if (!Input.GetButton(input.aimViewButton))
            //{
            //    switch (shoulder)
            //    {
            //        case Shoulder.Left:
            //            PostionCamera(cameraSettings.camPositionOffsetLeft);
            //            break;
            //        case Shoulder.Right:
            //            PostionCamera(cameraSettings.camPositionOffsetRight);
            //            break;
            //    }
            //}

        }

    }

    void MoveCamUp(RaycastHit hit, Vector3 pivotPos, Vector3 dir, Transform cameraT)
    {
        float hitDist = hit.distance;
        Vector3 sphereCastCenter = pivotPos + (dir.normalized * hitDist);
        cameraT.position = sphereCastCenter;
    }

    void PostionCamera(Vector3 cameraPos)
    {
        if (!mainCamera)
            return;
        Transform mainCamt = mainCamera.transform;
        Vector3 mainCamPos = mainCamt.localPosition;
        Vector3 newPos = Vector3.Lerp(mainCamPos, cameraPos, Time.deltaTime /** movement.movenmentLerpSpeed*/);
        mainCamt.localPosition = newPos;

    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GrimCamera : MonoBehaviour
//{
//    #region Variable.
//    private bool isReturnAngle = false;
//    private bool pitchLock = false;

//    private float mouseSensitivity = 0.0f;
//    private float distanceFromTarget = 1.4f;
//    private float rotationSmoothTime = 8.0f;
//    private float pitchTarget = 0.0f;
//    private float pitch, yaw = 0.0f;

//    private float moveSpeed = 5.0f;
//    private float returnSpeed = 9.0f;
//    private float wallPush = 0.7f;

//    private Vector2 V2PicthMinMax = new Vector2(-80.0f, 80.0f);

//    private Vector3 V3PitchYaw = Vector3.zero;
//    private Vector3 V3EndAngleTarget = Vector3.zero;
//    private Vector3 V3RotationSmoothVelocity = Vector3.zero;
//    private Vector3 CurrentRotation = Vector3.zero;
//    private Vector3 V3TargetPosition = Vector3.zero;
//    private Vector3 V3Normal = Vector3.zero;
//    private Vector3 V3Push = Vector3.zero;

//    [Header("Class Grim Animator")]
//    [SerializeField] private GrimAnimator grimAnimator;
//    [SerializeField] private GrimMoments grimMoments;
//    [SerializeField] private Transform target;

//    [Header("Mask")]
//    [SerializeField] private LayerMask collisionMask;

//    private RaycastHit raycastHit;
//    private Ray ray;
//    #endregion

//    #region Functions.
//    private void Awake()
//    {
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;

//        mouseSensitivity = grimMoments.GetSensivity();
//    }

//    private void LateUpdate()
//    {
//        TargetSmoothPosition();

//        CollisionCheck(target.position - transform.forward * distanceFromTarget);
//        WallCheck();

//        if (grimAnimator.GetIsTakeDown() == false)
//        {
//            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

//            if (!pitchLock)
//            {
//                StartCoroutine(ReturnAngle());
//            }
//            else
//            {
//                if (grimAnimator.GetIsAim())
//                {
//                    StartCoroutine(ReturnAngle());
//                }
//                //else
//                //{
//                //    pitch = V2PicthMinMax.y;
//                //}
//            }
//        }
//        else
//        {
//            if (yaw > 0.0f)
//            {
//                yaw = 180.0f;
//            }
//            else
//            {
//                yaw = -180.0f;
//            }

//            pitch = 10.0f;
//            distanceFromTarget = 1.4f;
//        }

//        V3PitchYaw = new Vector2(pitch, yaw);

//        CurrentRotation = Vector3.Lerp(CurrentRotation, V3PitchYaw, rotationSmoothTime * Time.deltaTime);

//        V3EndAngleTarget = transform.eulerAngles;
//        V3EndAngleTarget.x = 0.0f;
//        target.eulerAngles = V3EndAngleTarget;

//        transform.eulerAngles = CurrentRotation;
//    }

//    private IEnumerator ReturnAngle()
//    {
//        if (isReturnAngle == true)
//        {
//            pitch = 10.0f;

//            isReturnAngle = !isReturnAngle;
//        }

//        yield return new WaitForSeconds(0.2f);

//        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
//        pitch = Mathf.Clamp(pitch, V2PicthMinMax.x, V2PicthMinMax.y);
//    }

//    private void TargetSmoothPosition()
//    {
//        pitchTarget = target.position.y;

//        if (grimAnimator.GetIsAim())
//        {
//            distanceFromTarget = 0.75f;
//            //pitchTarget = Mathf.Lerp(1.6f, 1.65f, Time.deltaTime * 10);
//        }
//        else
//        {
//            distanceFromTarget = 1.4f;
//            //pitchTarget = Mathf.Lerp(1.65f, 1.6f, Time.deltaTime * 10);
//        }

//        V3TargetPosition = new Vector3(target.position.x, pitchTarget, target.position.z);
//        target.position = Vector3.SmoothDamp(target.position, V3TargetPosition, ref V3RotationSmoothVelocity, 0.1f * Time.deltaTime);
//    }

//    private void CollisionCheck(Vector3 V3ReturnPoint)
//    {
//        if (Physics.Linecast(target.position, V3ReturnPoint, out raycastHit, collisionMask))
//        {
//            V3Normal = raycastHit.normal * wallPush;
//            V3Push = raycastHit.point + V3Normal;

//            transform.position = Vector3.Lerp(transform.position, V3Push, moveSpeed * Time.deltaTime);

//            return;
//        }
//        transform.position = Vector3.Lerp(transform.position, V3ReturnPoint, returnSpeed * Time.deltaTime);
//        pitchLock = false;
//    }

//    private void WallCheck()
//    {
//        ray = new Ray(target.position, -target.forward);

//        if (Physics.SphereCast(ray, 0.5f, out raycastHit, 0.7f, collisionMask))
//        {
//            pitchLock = true;
//            isReturnAngle = true;
//        }
//        else
//        {
//            pitchLock = false;
//        }
//    }
//    #endregion
//}
