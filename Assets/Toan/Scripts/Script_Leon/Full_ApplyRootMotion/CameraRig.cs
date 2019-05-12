using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraRig : MonoBehaviour {

    public GrimAnimator grimAnimator;
    public Transform target;
    public bool autoTargetPlayer;
    public LayerMask wallLayer;

    public enum Shoulder
    {
        Right,Left
    }
    public Shoulder shoulder;

    [System.Serializable]
    public class CameraSettings
    {
        [Header("-Positioning vs Euler-")]
        public Vector3 camPositionOffsetLeft;
        public Vector3 camPositionOffsetRight;
        public Vector3 camEulerOffset;


        [Header("-Camera Option-")]
        public Camera UICamera;
        public float mouseXSensitivy = 5.0f;
        public float mouseYSensitivy = 5.0f;
        public float minAngle = -40f;
        public float maxAngle = 50f;
        public float rotationSpeed = 5.0f;

        [Header("-Zoom-")]
        public float fieldOfview = 70.0f;
        public float zoomFieldOfview = 25.0f;
        public float zoomSpeed = 3.0f;
        public Transform viewAim_Tranform;

        [Header("-Visual Option-")]
        public float hidMeshWhenDistance = 0.5f;
        
    }

    [SerializeField]
    public CameraSettings cameraSettings;

    [System.Serializable]
    public class InputSettings
    {
        public string MouseXAxis = "Mouse X";
        public string MouseYAxis = "Mouse Y";
        public string aimViewButton = "Fire2";
        public string zoomCamButton = "Zoom";
        public string switchShoulderButton = "SwitchShoulderCam";

    }
    [SerializeField]
    public InputSettings input;


    [System.Serializable]
    public class MovementSettings
    {
        public float movenmentLerpSpeed = 5.0f;
    }
    [SerializeField]
    public MovementSettings movement;

    Transform pivot;
    Camera mainCamera;
    float newX = 0.0f;
    float newY = 0.0f;
    // Use this for initialization
    void Start () {
        mainCamera = Camera.main;
        pivot = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {
        if (target)
        {
            if (Application.isPlaying)
            {
                RotateCamera();
                CheckWall();
                CheckMeshRenderer();
                //Zoom(Input.GetButton(input.zoomCamButton));
                if (target.GetComponent<WeaponHandler>())
                {
                    if (target.GetComponent<WeaponHandler>().currentWeapon)
                    {
                        AimView_toShot(Input.GetButton(input.aimViewButton));
                    }
                }
                if (Input.GetButtonDown(input.switchShoulderButton))
                {
                    
                    SwitchShoulders();

                }


            }
        }
       

    }
    void LateUpdate()
    {
        if (!target)
        {
            TargetPlayer();
        }
        else
        {
            Vector3 targetPosition = target.position;
            Quaternion targetRotation = target.rotation;

            FollowTarget(targetPosition, targetRotation);
        }
    }
    void TargetPlayer()
    {
        if (autoTargetPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player)
            {
                Transform playerT = player.transform;
                target = playerT;

            }

        }
    }

    void FollowTarget(Vector3 targetPosotion , Quaternion targetRotation)
    {
        if (!Application.isPlaying)
        {
            transform.position = targetPosotion;
            transform.rotation = targetRotation;
        }
        else
        {
            Vector3 newpos = Vector3.Lerp(transform.position, targetPosotion, Time.deltaTime * movement.movenmentLerpSpeed);
            transform.position = newpos;
        }
    }

    void RotateCamera()
    {
        if (!pivot)
        {
            return;
        }
        newX += cameraSettings.mouseXSensitivy * Input.GetAxis(input.MouseXAxis);
        newY += cameraSettings.mouseYSensitivy * -Input.GetAxis(input.MouseYAxis);

        Vector3 eulerAangleAxis = new Vector3();
        eulerAangleAxis.x = newY;
        eulerAangleAxis.y = newX;

        newX = Mathf.Repeat(newX, 360);
        newY = Mathf.Clamp(newY, cameraSettings.minAngle, cameraSettings.maxAngle);

        Quaternion newRotation = Quaternion.Slerp(pivot.localRotation, Quaternion.Euler(eulerAangleAxis), Time.deltaTime * cameraSettings.rotationSpeed);
        pivot.localRotation = newRotation;

    }

    void CheckWall()
    {
        if (!pivot || !mainCamera)
        {
            return;
        }

        RaycastHit hit;

        Transform mainCamT = mainCamera.transform;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 pivotPos = pivot.position;

        Vector3 start = pivotPos;
        Vector3 dir = mainCamPos - pivotPos;

        float dist = Mathf.Abs(shoulder == Shoulder.Left ? cameraSettings.camPositionOffsetLeft.z : cameraSettings.camPositionOffsetRight.z);

        //Debug.DrawRay(start, dir, Color.red, dist);
        if (Physics.SphereCast(start, mainCamera.nearClipPlane, dir, out hit, dist, wallLayer
          /*Physics.Raycast(start,dir,out hit,dist,wallLayer*/) )
        {
            MoveCamUp(hit, pivotPos, dir, mainCamT);
            //mainCamT.position = hit.point;

        }
        else
        {
            if (!Input.GetButton(input.aimViewButton))
            {
                switch (shoulder)
                {
                    case Shoulder.Left:
                        PostionCamera(cameraSettings.camPositionOffsetLeft);
                        break;
                    case Shoulder.Right:
                        PostionCamera(cameraSettings.camPositionOffsetRight);
                        break;
                }
            }
          
        }

    }

    void MoveCamUp(RaycastHit hit,Vector3 pivotPos,Vector3 dir,Transform cameraT)
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
        Vector3 newPos = Vector3.Lerp(mainCamPos, cameraPos, Time.deltaTime * movement.movenmentLerpSpeed);
        mainCamt.localPosition = newPos;

    }
    

    void CheckMeshRenderer()
    {
        if (!mainCamera || target)
        {
            return;
        }

        SkinnedMeshRenderer[] meshes = target.GetComponentsInChildren<SkinnedMeshRenderer>();
        Transform mainCamT = mainCamera.transform;
        Vector3 mainCampos = mainCamT.position;
        Vector3 targetPos = target.position;
        float dist = Vector3.Distance(mainCampos, targetPos);

        if (meshes.Length > 0)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                if (dist <= cameraSettings.hidMeshWhenDistance)
                {
                    meshes[i].enabled = false;

                }
                else
                {
                    meshes[i].enabled = true;
                }
            }
        }
    }

    void Zoom(bool isZooming)
    {
        if (!mainCamera)
        {
            return;
        }

        if (isZooming)
        {
            float newFieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraSettings.zoomFieldOfview, Time.deltaTime * cameraSettings.zoomSpeed);
            mainCamera.fieldOfView = newFieldOfView;

            if (cameraSettings.UICamera != null)
            {
                cameraSettings.UICamera.fieldOfView = newFieldOfView;
            }
        }
        else
        {
            float originalFieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraSettings.fieldOfview, Time.deltaTime * cameraSettings.zoomSpeed);
            mainCamera.fieldOfView = originalFieldOfView;

            if (cameraSettings.UICamera != null)
            {
                cameraSettings.UICamera.fieldOfView = originalFieldOfView;
            }


        }
    }
    void AimView_toShot(bool isZooming)
    {
        if (!mainCamera)
        {
            return;
        }

        //if (isZooming)
        //{
        //    float newFieldOfView = Mathf.Lerp(maincamera2.fieldOfView, cameraSettings.zoomFieldOfview, Time.deltaTime * cameraSettings.zoomSpeed);
        //    maincamera2.fieldOfView = newFieldOfView;
        //}
        //else
        //{
        //    float originalFieldOfView = Mathf.Lerp(maincamera2.fieldOfView, cameraSettings.fieldOfview, Time.deltaTime * cameraSettings.zoomSpeed);
        //    maincamera2.fieldOfView = originalFieldOfView;

        //}

        if (isZooming)
        {

            Vector3 newpos = Vector3.Lerp(mainCamera.transform.localPosition, cameraSettings.viewAim_Tranform.localPosition, Time.deltaTime * movement.movenmentLerpSpeed);
            mainCamera.transform.localPosition = newpos;
            mainCamera.transform.localRotation = Quaternion.Lerp(mainCamera.transform.localRotation, cameraSettings.viewAim_Tranform.localRotation, Time.deltaTime * movement.movenmentLerpSpeed);
            if (cameraSettings.UICamera != null)
            {
                cameraSettings.UICamera.fieldOfView = mainCamera.fieldOfView;
            }
        }
        else
        {
            if (shoulder == Shoulder.Right)
            {
                PostionCamera(cameraSettings.camPositionOffsetRight);
            }
            else
            {
                PostionCamera(cameraSettings.camPositionOffsetLeft);
            }
            //Vector3 newpos = Vector3.Lerp(maincamera2.transform.position, shoulder==Shoulder.Right? cameraSettings.camPositionOffsetRight: cameraSettings.camPositionOffsetLeft, Time.deltaTime * movement.movenmentLerpSpeed);
            //transform.position = newpos;
            mainCamera.transform.localRotation = Quaternion.Lerp(mainCamera.transform.localRotation, Quaternion.Euler(cameraSettings.camEulerOffset), Time.deltaTime * movement.movenmentLerpSpeed);
            if (cameraSettings.UICamera != null)
            {
                cameraSettings.UICamera.fieldOfView = mainCamera.fieldOfView;
            }
        }
    }
    public void SwitchShoulders()
    {
        switch (shoulder)
        {
            case Shoulder.Left:
                shoulder = Shoulder.Right;
                break;
            case Shoulder.Right:
                shoulder = Shoulder.Left;
                break;
            default:
                break;
        }
    }


    //[System.Serializable]
    //public class OtherSettings
    //{
    //    public float lookSpeed = 5.0f;
    //    public float lookDistance = 30.0f;
    //    public bool requireInputForTurn = true;
    //    //public LayerMask aimDetectionLayers;
    //}
    //[SerializeField]
    //public OtherSettings other;
    //void CameraLookLogic()
    //{


    //    other.requireInputForTurn = !grimAnimator.GetIsAim();

    //    if (other.requireInputForTurn)
    //    {
    //        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
    //        {
    //            PlayerLook();
    //            //characterMove.CharacterLook();
    //        }
    //    }
    //    else
    //    {
    //        PlayerLook();
    //        //characterMove.CharacterLook();

    //    }
    //}
    //void PlayerLook()
    //{
    //    Transform mainCamT = mainCamera.transform;
    //    Transform pivotT = mainCamT.parent;
    //    Vector3 pivotPos = pivotT.position;
    //    Vector3 lookTarget = pivotPos + (pivotT.forward * other.lookDistance);
    //    Vector3 thisPos = target.position;
    //    Vector3 lookDir = lookTarget - thisPos;
    //    Quaternion lookRot = Quaternion.LookRotation(lookDir);
    //    lookRot.x = 0;
    //    lookRot.z = 0;

    //    Quaternion newRotation = Quaternion.Lerp(target.rotation, lookRot, Time.deltaTime * other.lookSpeed);
    //    target.rotation = newRotation;
    //}

    //bool isShooting = false;
    //float lastAngleX;
    //float newY2 = 0.0f; // variable for recoil when firing
    //void SpinCameraAsWeapon()
    //{
    //    Ray aimRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
    //    if (grimAnimator.GetIsHoldingRifle())
    //    {
    //        if (grimAnimator.GetIsAim() && grimAnimator.GetIsFire())
    //        {
    //            if (!isShooting)
    //            {
    //                isShooting = true;

    //                lastAngleX = mainCamera.transform.localRotation.eulerAngles.x;
    //                if (lastAngleX > 50)
    //                {
    //                    lastAngleX = lastAngleX - 360;
    //                }
    //                newY2 = lastAngleX;
    //            }
    //            newY2 -= 1;

    //            Vector3 eulerAangleAxis = new Vector3();
    //            eulerAangleAxis.x = newY2;
    //            //eulerAangleAxis.x = pivotCameraRig.localRotation.eulerAngles.x - 1;
    //            eulerAangleAxis.y = mainCamera.transform.localRotation.eulerAngles.y;
    //            eulerAangleAxis.z = mainCamera.transform.localRotation.eulerAngles.z;
    //            newY2 = Mathf.Clamp(newY2, -40, 50);
    //            //eulerAangleAxis.x= Mathf.Clamp(newY, -30, 30);
    //            Quaternion newRotation = Quaternion.Slerp(mainCamera.transform.localRotation, Quaternion.Euler(eulerAangleAxis), Time.deltaTime * 5);
    //            mainCamera.transform.localRotation = newRotation;
    //        }
    //        if (grimAnimator.GetIsAim() && !grimAnimator.GetIsFire())
    //        {
    //            if (isShooting == true)
    //            {
    //                isShooting = false;
    //            }
    //            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
    //            {
    //                newY2 = mainCamera.transform.localRotation.eulerAngles.x;
    //                lastAngleX = newY2;
    //                //lastAngleX = pivotCameraRig.localRotation.eulerAngles.x;
    //            }
    //            else if (lastAngleX != newY2)
    //            {
    //                newY2 += 1;
    //                Vector3 eulerAangleAxis = new Vector3();
    //                eulerAangleAxis.x = newY2;
    //                eulerAangleAxis.y = mainCamera.transform.localRotation.eulerAngles.y;
    //                eulerAangleAxis.z = mainCamera.transform.localRotation.eulerAngles.z;
    //                newY2 = Mathf.Clamp(newY2, -40, lastAngleX);
    //                //eulerAangleAxis.x = Mathf.Clamp(newY2, -30, 30);
    //                Quaternion newRotation = Quaternion.Slerp(mainCamera.transform.localRotation, Quaternion.Euler(eulerAangleAxis), Time.deltaTime * 5);
    //                mainCamera.transform.localRotation = newRotation;
    //                //lastAngleX = pivotCameraRig.localRotation.eulerAngles.x;
    //            }
    //        }
    //    }
    //}
}
