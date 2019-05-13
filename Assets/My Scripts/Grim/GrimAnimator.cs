using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GrimAnimator : MonoBehaviour
{
    #region Variable.
    private int layerUpperBody = 0;

    private float X, Y = 0.0f;
    private float mouseX = 0.0f;
    private float aimingAngle = 0.0f;
    private float timeToLayerUpperBodyWeight = 0.0f;

    private bool isPistol = false;
    private bool isRun = false;
    private bool isAim = false;
    private bool isFire = false;
    private bool isReload = false;
    private bool isCanFirePistol = true;
    private bool isCanReload = true;
    private bool isCanSwitch = true;
    private bool isSlotAllWeaponts = false;
    private bool isHoldingPistol = false;
    private bool isHoldingRifle = false;
    private bool isHoldingKnife = false;
    private bool isStrafe = false;
    private bool isTakeDown = false;
    [SerializeField] private Animator grimAnim;

    private int objectFootStep;
    #endregion

    #region GetFunctions.
    public Animator GetGrimAnimator()
    {
        return grimAnim;
    }

    public float GetHorizontal()
    {
        return X;
    }

    public float GetVertical()
    {
        return Y;
    }

    public float GetMouseX()
    {
        return mouseX;
    }

    public bool GetIsPistol()
    {
        return isPistol;
    }

    public bool GetIsRun()
    {
        return isRun;
    }

    public bool GetIsAim()
    {
        return isAim;
    }

    public bool GetIsFire()
    {
        return isFire;
    }

    public bool GetIsReload()
    {
        return isReload;
    }

    public bool GetIsCanFirePistol()
    {
        return isCanFirePistol;
    }

    public bool GetIsCanReload()
    {
        return isCanReload;
    }

    public bool GetIsCanSwitch()
    {
        return isCanSwitch;
    }

    public bool GetIsStrafe()
    {
        return isStrafe;
    }

    public bool GetIsSlotAllWeaponts()
    {
        return isSlotAllWeaponts;
    }

    public bool GetIsHoldingPistol()
    {
        return isHoldingPistol;
    }

    public bool GetIsHoldingRifle()
    {
        return isHoldingRifle;
    }

    public bool GetIsHoldingKnife()
    {
        return isHoldingKnife;
    }

    public bool GetIsTakeDown()
    {
        return isTakeDown;
    }

    public int GetObjectFootStep()
    {
        return objectFootStep;
    }

    public int GetLayerUpperBody()
    {
        return layerUpperBody;
    }

    public void SetPositionGrim(Vector3 V3PositionGrim)
    {
        transform.position = V3PositionGrim;
    }

    public void SetLookAtGrim(Transform lookAtGrim)
    {
        transform.LookAt(lookAtGrim);
    }
    #endregion

    #region Functions.
    private void Awake()
    {
        layerUpperBody = grimAnim.GetLayerIndex("Upper Body");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene(0);
        }

        if (isTakeDown == false)
        {
            if (grimAnim == null) return;

            X = Input.GetAxis("Horizontal");
            Y = Input.GetAxis("Vertical");

            grimAnim.SetFloat("Horizontal", X);
            grimAnim.SetFloat("Vertical", Y);

            Moving();
        }
        else
        {
            grimAnim.SetLayerWeight(layerUpperBody, Mathf.Lerp(0.01f, 1.0f, 1.0f));
        }
    }

    private void Moving()
    {
        // input key or console to action animation.
        Sprint();
        Aiming();
        Firing();
        Reload();
        Switch();
        Turning();
        AimingAngle();
    }

    private void Sprint()
    {
        isRun = Input.GetKey(KeyCode.LeftShift);

        grimAnim.SetBool("IsRun", isRun);
    }
    
    private void Aiming()
    {
        isAim = Input.GetKey(KeyCode.Mouse1);

        if (isAim)
        {
            timeToLayerUpperBodyWeight = Mathf.Lerp(timeToLayerUpperBodyWeight, 1.0f, 1.0f);
        }
        else
        {
            if (isCanSwitch == false || isCanReload == false)
            {
                timeToLayerUpperBodyWeight = Mathf.Lerp(timeToLayerUpperBodyWeight, 1.0f, 1.0f);
            }
            else
            {
                timeToLayerUpperBodyWeight = Mathf.Lerp(timeToLayerUpperBodyWeight, 0.01f, 1.0f);
            }
            
        }

        grimAnim.SetLayerWeight(layerUpperBody, timeToLayerUpperBodyWeight);

        grimAnim.SetBool("IsAim", isAim);
    }

    private void Firing()
    {
        if (isPistol)
        {
            if (isAim && isCanFirePistol == true)
            {
                if ((isCanReload == false) || (isCanSwitch == false)) return;

                isFire = Input.GetKeyDown(KeyCode.Mouse0);
                grimAnim.SetBool("IsFire", isFire);
            }
        }
        else
        {
            if (isAim)
            {
                isFire = Input.GetKey(KeyCode.Mouse0);

                if ((isCanReload == false) || (isCanSwitch == false)) return;
                grimAnim.SetBool("IsFire", isFire);
            }
        }
    }

    private void Reload()
    {
        if (isFire) return;

        isReload = Input.GetKeyDown(KeyCode.R);

        if (isReload)
        {
            timeToLayerUpperBodyWeight = Mathf.SmoothStep(timeToLayerUpperBodyWeight, 1.0f, 1.0f);
            grimAnim.SetLayerWeight(layerUpperBody, timeToLayerUpperBodyWeight);

            if (isCanReload == true)
            {
                if (isCanFirePistol == false || isCanSwitch == false) return;
            }
        }

        grimAnim.SetBool("IsReload", isReload);
    }

    private void Switch()
    {
        if (isFire) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            timeToLayerUpperBodyWeight = Mathf.Lerp(timeToLayerUpperBodyWeight, 1.0f, 1.0f);
            grimAnim.SetLayerWeight(layerUpperBody, timeToLayerUpperBodyWeight);

            if (isCanSwitch == true)
            {
                if (isCanFirePistol == false || isCanReload == false) return;

                isPistol = !isPistol;
                grimAnim.SetBool("IsPistol", isPistol);
            }
        }
    }

    private void Turning()
    {
        mouseX = Input.GetAxis("Mouse X");

        if (X == 0.0f && Y == 0.0f && (mouseX >= 0.9f || mouseX <= -0.9f))
        {
            isStrafe = true;
        }
        else
        {
            isStrafe = false;
            mouseX = 0.0f;
        }

        grimAnim.SetBool("IsStrafe", isStrafe);
        grimAnim.SetFloat("Turning", mouseX, 0.5f, Time.deltaTime);
    }

    private void AimingAngle()
    {
        aimingAngle = Camera.main.transform.eulerAngles.x;

        if (isAim)
        {
            aimingAngle = CheckAngle(aimingAngle) * -1;
        }
        else
        {
            aimingAngle = 0.0f;
        }

        grimAnim.SetFloat("Aiming Angle", aimingAngle);
    }

    private float CheckAngle(float value)
    {
        float angle = value - 180.0f;
        if (angle > 0)
        {
            return angle - 180.0f;
        }

        return angle + 180.0f;
    }

    private void OnTriggerEnter(Collider collider)
    {
        FootStepCollision(collider);
    }

    private void FootStepCollision(Collider collider)
    {
        if (collider.transform.tag == "GROUND")
        {
            objectFootStep = (int)EMaterialsMode.Ground;
        }
        if (collider.transform.tag == "Glass")
        {
            objectFootStep = (int)EMaterialsMode.Glass;
        }

        if (collider.transform.tag == "Grass")
        {
            objectFootStep = (int)EMaterialsMode.Grass;
        }

        if (collider.transform.tag == "Wood")
        {
            objectFootStep = (int)EMaterialsMode.Wood;
        }

        if (collider.transform.tag == "Rock")
        {
            objectFootStep = (int)EMaterialsMode.Rock;
        }

        if (collider.transform.tag == "Water")
        {
            objectFootStep = (int)EMaterialsMode.Water;
        }

        if (collider.transform.tag == "Dirt")
        {
            objectFootStep = (int)EMaterialsMode.Dirt;
        }

        if (collider.transform.tag == "Metal")
        {
            objectFootStep = (int)EMaterialsMode.Metal;
        }
    }

    private void StartFirePistol()
    {
        isCanFirePistol = false;
    }

    private void EndFirePistol()
    {
        isCanFirePistol = true;
    }

    private void StartReload()
    {
        isCanReload = false;
    }

    private void EndReload()
    {
        isCanReload = true;
    }

    private void StartSwitch()
    {
        isCanSwitch = false;
    }

    private void EndSwitch()
    {
        isCanSwitch = true;
    }

    private void StartSlotAllWeaponts()
    {
        isSlotAllWeaponts = true;
        isHoldingPistol = false;
        isHoldingRifle = false;
    }

    private void StartHoldingPistol()
    {
        isSlotAllWeaponts = false;
        isHoldingPistol = true;
        isHoldingRifle = false;
    }

    private void StartHoldingRifle()
    {
        isSlotAllWeaponts = false;
        isHoldingPistol = false;
        isHoldingRifle = true;
    }

    private void StartTakeDown()
    {
        isTakeDown = true;
        isHoldingKnife = true;
    }

    private void EndTakeDown()
    {
        isTakeDown = false;
        isHoldingKnife = false;
        
    }
    public bool isRunningAnimation(String nameAnimation , int layerindex)
    {
       return grimAnim.GetCurrentAnimatorStateInfo(layerindex).IsName(nameAnimation);
    }
    #endregion
}
