using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UserInput : MonoBehaviour
{
	public CharacterMovement characterMove { get; protected set; }
	public WeaponHandler weaponHandler { get; protected set; }
    Animator animator;

    [System.Serializable]
    public class InputSettings
    {
        public string verticalAxis = "Vertical";
        public string horizontalAxis = "Horizontal";
        public string jumpButton = "Jump";
        public string reloadButton = "Reload";
        public string aimButton = "Fire2";
        public string fireButton = "Fire1";
        public string dropWeaponButton = "DropWeapon";
        public string switchWeaponButton = "SwitchWeapon";
        public string MouseXAxis = "Mouse X";
        public string MouseYAxis = "Mouse Y";
    }
    [SerializeField]
    public InputSettings input;

    [System.Serializable]
    public class OtherSettings
    {
        public float lookSpeed = 5.0f;
        public float lookDistance = 30.0f;
        public bool requireInputForTurn = true;
        //public LayerMask aimDetectionLayers;
    }
    [SerializeField]
    public OtherSettings other;

    float forward;
    float strafe;
    public Camera mainCamera;
    
    public bool debugAim;
    public Transform spine;
    bool aiming;
    float newY = 0.0f; // variable for recoil when firing
    public Transform pivotCameraRig;
    bool isShooting =false;
    float lastAngleX;

    bool isMaxForward;
    bool isMinForward;

    bool isMaxStrafe;
    bool isMinStrafe;

    //System.Func<bool> isRunningCombo2;
    //System.Func<bool> isRunningCombo3;

    Dictionary<Weapon, GameObject> crosshairPrefabMap = new Dictionary<Weapon, GameObject>();

    // Use this for initialization
    void Start()
    {
        characterMove = GetComponent<CharacterMovement>();
        weaponHandler = GetComponent<WeaponHandler>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        SetupCrosshairs ();
        //isRunningCombo2 += IsRunningCombo2;
        //isRunningCombo3 += IsRunningCombo3;
    }

    private bool IsRunningCombo2()
    {
        return weaponHandler.isAnimationRunning("RightPunching",1);
    }
    private bool IsRunningCombo3()
    {
        return weaponHandler.isAnimationRunning("Mma Kick",0);
    }
    void SetupCrosshairs () {
		if (weaponHandler.weaponsList.Count > 0)
        {
			foreach (Weapon wep in weaponHandler.weaponsList)
            {
				GameObject prefab = wep.weaponSettings.crosshairPrefab;
				if (prefab != null)
                {
					GameObject clone = (GameObject)Instantiate (prefab);
					crosshairPrefabMap.Add (wep, clone);
					ToggleCrosshair (false, wep);
				}
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
        CharacterLogic();
        CameraLookLogic();
        WeaponLogic();
        KungFuLogic();

    }

    void LateUpdate()
    {
        if (weaponHandler)
        {
            if (weaponHandler.currentWeapon)
            {
                if (aiming)
                    RotateSpine();
            }
        }
    }
    void KungFuLogic()
    {
        if (!weaponHandler.currentWeapon)
        {
            weaponHandler.Defense(Input.GetButton(input.aimButton));
            if (Input.GetButtonDown(input.fireButton))
            {
                if (weaponHandler.GetCombo() == 0)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (animator.GetFloat("Forward") == 0)
                        {
                            weaponHandler.SetCombo(4);

                        }
                        else if (animator.GetFloat("Forward") > 0 && animator.GetFloat("Forward") < 1)
                        {
                            weaponHandler.SetCombo(5);

                        }
                        else if (animator.GetFloat("Forward") == 1f)
                        {
                            weaponHandler.SetCombo(6);

                        }

                    }
                    else
                    {
                        weaponHandler.SetCombo(1);
                        //StartCoroutine("ResetCombo1");

                    }


                }
                else if (weaponHandler.GetCombo() == 1)
                {
                    if (weaponHandler.isAnimationRunning("LeftPunching",1))
                    {
                        weaponHandler.SetCombo(2);

                    }
                   
                    //weaponHandler.SetCombo(2);
                    //StartCoroutine("ResetCombo2");



                }
                else if (weaponHandler.GetCombo() == 2)
                {
                    if (weaponHandler.isAnimationRunning("RightPunching",1))
                    {
                        weaponHandler.SetCombo(3);
                        //StartCoroutine("ResetCombo3");

                    }
                   
                }

            }
            else
            {
                if (weaponHandler.GetCombo() == 1 && !weaponHandler.isAnimationRunning("LeftPunching",1))
                {
                    weaponHandler.SetCombo(0);

                }
                if (weaponHandler.GetCombo() == 2 && !weaponHandler.isAnimationRunning("RightPunching",1) && !weaponHandler.isAnimationRunning("LeftPunching",1))
                {
                    weaponHandler.SetCombo(0);

                }
                if (weaponHandler.GetCombo() == 3 && !weaponHandler.isAnimationRunning("Mma Kick",0) && !weaponHandler.isAnimationRunning("RightPunching",0))
                {
                    weaponHandler.SetCombo(0);

                }
                if ( weaponHandler.GetCombo() == 4|| weaponHandler.GetCombo() == 5 || weaponHandler.GetCombo() == 6)
                {
                    weaponHandler.SetCombo(0);

                }


            }
           
        }

    }

    //IEnumerator ResetCombo1()
    //{
    //    yield return new WaitForSeconds (0.5f);
    //    if (weaponHandler.GetCombo()==1)
    //    {
    //        weaponHandler.SetCombo(0);
    //    }
        

    //}
    //IEnumerator ResetCombo2()
    //{
    //    yield return new WaitUntil(isRunningCombo2);
    //    Debug.Log("Bat dau reset combo222222222");
    //    yield return new WaitForSeconds(0.46f);
    //    if (weaponHandler.GetCombo() == 2)
    //    {
    //        weaponHandler.SetCombo(0);
    //    }
        
        

    //}
    //IEnumerator ResetCombo3()
    //{
    //    Debug.Log("vao reset com bo 3");
    //    yield return new WaitUntil(isRunningCombo3);
    //    Debug.Log("Bat dau reset combo333333333");
    //    weaponHandler.SetCombo(0);
        



    //}
    //Handles character logic
    void CharacterLogic()
    {
        if (!characterMove)
            return;

        // INPUT W OR S
        if (Input.GetKey(KeyCode.W) == true)
        {
            if (isMaxForward == false)
            {
                forward += Time.deltaTime * 3;
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    if (forward >= 0.5f)
                    {
                        if (forward >= 1)
                        {
                            isMaxForward = true;
                        }
                        else
                        {
                            forward = 0.5f;

                        }
                    }

                }
                else
                {
                    if (forward >= 1.0f)
                    {
                        forward = 1.0f;
                    }
                }
            }
            else
            {
                forward -= Time.deltaTime * 3;
                if (forward <= 0.5)
                {
                    forward = 0.5f;
                    isMaxForward = false;
                }
            }

    

        }
        else if (Input.GetKey(KeyCode.S) == true)
        {
            if (isMinForward == false)
            {
                forward -= Time.deltaTime * 3;
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    if (forward <= -0.5f)
                    {
                        if (forward <= -1)
                        {
                            isMinForward = true;
                        }
                        else
                        {
                            forward = -0.5f;

                        }
                    }

                }
                else
                {

                    if (forward <= -1.0f)
                    {
                        forward = -1.0f;
                    }
                }
            }
            else
            {
                forward += Time.deltaTime * 3;
                if (forward >= -0.5f)
                {
                    forward = -0.5f;
                    isMinForward = false;
                }
            }

            //forward -= Time.deltaTime * 3;
            //if (forward <= -0.5f)
            //{
            //    forward = -0.5f;
            //}

           
        }
        else
        {
            if (forward>0)
            {
                forward -= Time.deltaTime * 3;
                if (forward <= 0)
                {
                    forward = 0;
                }
            }
            if (forward <0)
            {
                forward += Time.deltaTime * 3;
                if (forward >= 0)
                {
                    forward = 0;
                }
            }
            
        }

        // INPUT A || D 
        if (Input.GetKey(KeyCode.D) == true)
        {
            if (isMaxStrafe == false)
            {
                strafe += Time.deltaTime * 3;
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    if (strafe >= 0.5f)
                    {
                        if (strafe >= 1)
                        {
                            isMaxStrafe = true;
                        }
                        else
                        {
                            strafe = 0.5f;

                        }
                    }

                }
                else
                {
                    if (strafe >= 1.0f)
                    {
                        strafe = 1.0f;
                    }
                }
            }
            else
            {
                strafe -= Time.deltaTime * 3;
                if (strafe <= 0.5)
                {
                    strafe = 0.5f;
                    isMaxStrafe = false;
                }
            }



        }
        else if (Input.GetKey(KeyCode.A) == true)
        {
            if (isMinStrafe == false)
            {
                strafe -= Time.deltaTime * 3;
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    if (strafe <= -0.5f)
                    {
                        if (strafe <= -1)
                        {
                            isMinStrafe = true;
                        }
                        else
                        {
                            strafe = -0.5f;

                        }
                    }

                }
                else
                {

                    if (strafe <= -1.0f)
                    {
                        strafe = -1.0f;
                    }
                }
            }
            else
            {
                strafe += Time.deltaTime * 3;
                if (strafe >= -0.5f)
                {
                    strafe = -0.5f;
                    isMinStrafe = false;
                }
            }
        }
        else
        {
            if (strafe>0)
            {
                strafe -= Time.deltaTime * 3;
                if (strafe <= 0)
                {
                    strafe = 0;
                }
            }
            if (strafe<0)
            {
                strafe += Time.deltaTime * 3;
                if (strafe >= 0)
                {
                    strafe = 0;
                }
            }
            
        }
        characterMove.Animate(forward,strafe);

        if (Input.GetButtonDown(input.jumpButton) && (weaponHandler.GetCombo()!=4 && weaponHandler.GetCombo() != 5 && weaponHandler.GetCombo() != 6))
            characterMove.Jump();
        
        //characterMove.AirControl(Input.GetAxis(input.verticalAxis), Input.GetAxis(input.horizontalAxis));



    }

    //Handles camera logic
    void CameraLookLogic()
    {
        if (!mainCamera)
            return;
		
		other.requireInputForTurn = !aiming;

		if (other.requireInputForTurn) {
			if (Input.GetAxis (input.horizontalAxis) != 0 || Input.GetAxis (input.verticalAxis) != 0) {
                PlayerLook();
                //characterMove.CharacterLook();
			}
		}
		else {
            PlayerLook();
            //characterMove.CharacterLook();

        }
    }
    //IEnumerator ResetAngle()
    //{
    //    //yield return new WaitForEndOfFrame();
    //    while (newY!= lastAngleX && Input.GetAxis(input.MouseXAxis) == 0 && Input.GetAxis(input.MouseYAxis) == 0 )
    //    {
    //        newY += 2;
    //        Vector3 eulerAangleAxis = new Vector3();
    //        eulerAangleAxis.x = newY;
    //        newY = Mathf.Clamp(newY, lastAngleX, 70);
    //        Quaternion newRotation = Quaternion.Slerp(pivotCameraRig.localRotation, Quaternion.Euler(eulerAangleAxis), Time.deltaTime * 5);
    //        pivotCameraRig.localRotation = newRotation;
    //        lastAngleX = pivotCameraRig.localRotation.eulerAngles.x;
    //    }
    //    yield return null;
    //}
    //Handles all weapon logic
    void WeaponLogic()
    {
        if (!weaponHandler)
            return;

		aiming = Input.GetButton (input.aimButton) || debugAim;

		weaponHandler.Aim (aiming);

		if (Input.GetButtonDown (input.switchWeaponButton))
        {
			weaponHandler.SwitchWeapons ();
			UpdateCrosshairs ();
		}
		
		if (weaponHandler.currentWeapon)
        {
			
			Ray aimRay = new Ray (mainCamera.transform.position, mainCamera.transform.forward);

            if (weaponHandler.currentWeapon.weaponType==Weapon.WeaponType.Pistol)
            {
                if (Input.GetButtonDown(input.fireButton) && aiming)
                {
                    weaponHandler.FireCurrentWeapon(aimRay);

                }
            }
            else if (weaponHandler.currentWeapon.weaponType == Weapon.WeaponType.Rifle)
            {
                if (Input.GetButton(input.fireButton) && aiming)
                {
                    if (!isShooting)
                    {
                        isShooting = true;

                        lastAngleX = pivotCameraRig.localRotation.eulerAngles.x;
                        if (lastAngleX >50  )
                        {
                            lastAngleX = lastAngleX - 360;
                        }
                        newY = lastAngleX;
                    }
                    weaponHandler.FireCurrentWeapon(aimRay);
                    
                    newY -= 1;
                  
                    Vector3 eulerAangleAxis = new Vector3();
                    eulerAangleAxis.x = newY;
                    //eulerAangleAxis.x = pivotCameraRig.localRotation.eulerAngles.x - 1;
                    eulerAangleAxis.y = pivotCameraRig.localRotation.eulerAngles.y;
                    eulerAangleAxis.z = pivotCameraRig.localRotation.eulerAngles.z;
                    newY = Mathf.Clamp(newY, -40, 50);
                    //eulerAangleAxis.x= Mathf.Clamp(newY, -30, 30);
                    Quaternion newRotation = Quaternion.Slerp(pivotCameraRig.localRotation, Quaternion.Euler(eulerAangleAxis), Time.deltaTime * 5);
                    pivotCameraRig.localRotation = newRotation;
                }
                if (aiming && !Input.GetButton(input.fireButton))
                {
                 
                    if (isShooting == true)
                    {
                        isShooting = false;
                    }
                  
                    if (Input.GetAxis(input.MouseXAxis) != 0 || Input.GetAxis(input.MouseYAxis) != 0)
                    {
                        newY = pivotCameraRig.localRotation.eulerAngles.x;
                        lastAngleX = newY;
                        //lastAngleX = pivotCameraRig.localRotation.eulerAngles.x;
                    }
                    else if (lastAngleX != newY)
                    {

                        newY += 1;
                        Vector3 eulerAangleAxis = new Vector3();
                        eulerAangleAxis.x = newY;
                        eulerAangleAxis.y = pivotCameraRig.localRotation.eulerAngles.y;
                        eulerAangleAxis.z = pivotCameraRig.localRotation.eulerAngles.z;
                        newY = Mathf.Clamp(newY, -40, lastAngleX);
                        //eulerAangleAxis.x = Mathf.Clamp(newY, -30, 30);

                        Quaternion newRotation = Quaternion.Slerp(pivotCameraRig.localRotation, Quaternion.Euler(eulerAangleAxis), Time.deltaTime * 5);
                        pivotCameraRig.localRotation = newRotation;
                        //lastAngleX = pivotCameraRig.localRotation.eulerAngles.x;
                    }

                }
            }
            Debug.Log("NewY : " + newY);
            Debug.Log("lastangle x : " + lastAngleX);

            if (Input.GetButtonDown (input.reloadButton))
				weaponHandler.Reload ();
			if (Input.GetButtonDown (input.dropWeaponButton))
            {
                DeleteCrosshair(weaponHandler.currentWeapon);
                weaponHandler.DropCurWeapon();
			}

            if (weaponHandler.currentWeapon)
            {
                if (aiming)
                {
                    ToggleCrosshair(true, weaponHandler.currentWeapon);
                    PositionCrosshair(aimRay, weaponHandler.currentWeapon);

                }
                else
                    ToggleCrosshair(false, weaponHandler.currentWeapon);
            }
			
		} else
			TurnOffAllCrosshairs ();
    }

	public void TurnOffAllCrosshairs () {
       
            foreach (Weapon wep in crosshairPrefabMap.Keys)
            {
                ToggleCrosshair(false, wep);
            }
            
		
	}

	void CreateCrosshair (Weapon wep)
    {
		GameObject prefab = wep.weaponSettings.crosshairPrefab;
		if (prefab != null) {
			prefab = Instantiate (prefab);
			ToggleCrosshair (false, wep);
		}
	}
    
	void DeleteCrosshair (Weapon wep)
    {
		if (!crosshairPrefabMap.ContainsKey (wep))
			return;

		Destroy (crosshairPrefabMap [wep]);
		crosshairPrefabMap.Remove (wep);
	}

	// Position the crosshair to the point that we are aiming
	void PositionCrosshair (Ray ray, Weapon wep)
	{
		Weapon curWeapon = weaponHandler.currentWeapon;
		if (curWeapon == null)
			return;
		if (!crosshairPrefabMap.ContainsKey (wep))
			return;

		GameObject crosshairPrefab = crosshairPrefabMap [wep];
		RaycastHit hit;
		Transform bSpawn = curWeapon.weaponSettings.bulletSpawn;
		Vector3 bSpawnPoint = bSpawn.position;
		//Vector3 dir = ray.GetPoint(curWeapon.weaponSettings.range) - bSpawnPoint; //  khong con dung ray tu camera roi tra ve diem de - bulletspawn lam dir
        Vector3 dir = curWeapon.weaponSettings.bulletSpawn.forward;
        Debug.DrawRay(bSpawnPoint, dir);

        if (Physics.Raycast (bSpawnPoint, dir, out hit, curWeapon.weaponSettings.range, 
			curWeapon.weaponSettings.bulletLayers)) {
			if (crosshairPrefab != null) {
				ToggleCrosshair (true, curWeapon);
				crosshairPrefab.transform.position = hit.point;
                crosshairPrefab.transform.LookAt(Camera.main.transform);
            }
		} else {
			ToggleCrosshair (false, curWeapon);
		}
	}

	// Toggle on and off the crosshair prefab
	void ToggleCrosshair(bool enabled, Weapon wep)
	{
		if (!crosshairPrefabMap.ContainsKey(wep))
			return;

		crosshairPrefabMap [wep].SetActive (enabled);
	}

	void UpdateCrosshairs () {
		if (weaponHandler.weaponsList.Count == 0)
			return;

		foreach (Weapon wep in weaponHandler.weaponsList) {
			if (wep != weaponHandler.currentWeapon) {
				ToggleCrosshair (false, wep);
			}
		}
	}

    //Postions the spine when aiming
    void RotateSpine()
    {
        if (!spine || !weaponHandler.currentWeapon || !mainCamera)
            return;

        Transform mainCamT = mainCamera.transform;
        Vector3 mainCamPos = mainCamT.position;
        Vector3 dir = mainCamT.forward;
        Ray ray = new Ray(mainCamPos, dir);

        spine.LookAt(ray.GetPoint(50));

        Vector3 eulerAngleOffset = weaponHandler.currentWeapon.userSettings.spineRotation;
        spine.Rotate(eulerAngleOffset);
    }

    //Make the character look at a forward point from the camera
    void PlayerLook()
    {
        Transform mainCamT = mainCamera.transform;
        Transform pivotT = mainCamT.parent;
        Vector3 pivotPos = pivotT.position;
        Vector3 lookTarget = pivotPos + (pivotT.forward * other.lookDistance);
        Vector3 thisPos = transform.position;
        Vector3 lookDir = lookTarget - thisPos;
        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        lookRot.x = 0;
        lookRot.z = 0;

        Quaternion newRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * other.lookSpeed);
        transform.rotation = newRotation;
    }
}
