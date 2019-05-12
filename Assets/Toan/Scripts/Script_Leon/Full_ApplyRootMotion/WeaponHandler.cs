using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class WeaponHandler : MonoBehaviour
{
    Animator animator;
    SoundController sc;
    public bool isAI;
    Func<bool> finish;
    [System.Serializable]
    public class UserSettings
    {
        public Transform rightHand;
        public Transform pistolUnequipSpot;
        public Transform rifleUnequipSpot;
    }
    [SerializeField]
    public UserSettings userSettings;

    [System.Serializable]
    public class Animations
    {
        public string weaponTypeInt = "WeaponType";
        public string reloadingBool = "IsReloading";
        public string aimingBool = "Aiming";
        public string denfense = "Defense";
        public string combo = "Combo";
        public string isShooting = "IsShooting";
        public string TriggerEquip = "Equip";
        public string TriggerUnequip = "Unequip";

    }
    [SerializeField]
    public Animations animations;

    public Weapon currentWeapon;
    public List<Weapon> weaponsList = new List<Weapon>();
    public int maxWeapons = 2;
    public bool aim { get; protected set; }
    bool reload;
    int weaponType ;
    bool settingWeapon;
    bool isShooting;
    //variable for kungfu
    bool denfense;
    int combo = 0;

    public Text text;
    // Use this for initialization
   
    void OnEnable()
    {
        GameObject check = GameObject.FindGameObjectWithTag("Sound Controller");
        if (check != null)
            sc = check.GetComponent<SoundController>();
        animator = GetComponent<Animator>();
		SetupWeapons ();
    }

	void SetupWeapons () {
        Debug.Log("setup weaponnnnnnnnnnn");
		if (currentWeapon)
		{
			currentWeapon.SetEquipped(true);
			currentWeapon.SetOwner(this);
			AddWeaponToList(currentWeapon);

			if (currentWeapon.ammo.clipAmmo <= 0)
				Reload();

			if (reload)
            {
                if (settingWeapon)
                    reload = false;
            }
			    
		}

		if(weaponsList.Count > 0)
		{
			for(int i = 0; i < weaponsList.Count; i++)
			{
				if(weaponsList[i] != currentWeapon)
				{
					weaponsList[i].SetEquipped(false);
					weaponsList[i].SetOwner(this);
				}
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
        Animate();
    }

    //Animates the character
    void Animate()
    {
        if (!animator)
            return;

        animator.SetBool(animations.aimingBool, aim);
        animator.SetBool(animations.reloadingBool, reload);
        animator.SetInteger(animations.weaponTypeInt, weaponType);
        animator.SetBool(animations.isShooting, isShooting);
        if (transform.tag=="Player")
        {
            Debug.Log("combo =   weapon"+ combo);

            animator.SetBool(animations.denfense, denfense);
            animator.SetInteger(animations.combo, combo);
        }
     

        
        if (!currentWeapon)
        {
            weaponType = 0;
            return;
        }

        switch (currentWeapon.weaponType)
        {
            case Weapon.WeaponType.Pistol:
                weaponType = 1;
                break;
            case Weapon.WeaponType.Rifle:
                weaponType = 2;
                break;
        }
    }

    //Adds a weapon to the weaponsList
    void AddWeaponToList(Weapon weapon)
    {
        if (weaponsList.Contains(weapon))
            return;

        weaponsList.Add(weapon);
    }

    //Puts the finger on the trigger and asks if we pulled
	public void FireCurrentWeapon (Ray aimRay)
    {
		if (currentWeapon.ammo.clipAmmo == 0) {
			Reload ();
			return;
		}
        if (!isAnimationRunning("Pistol_ShootPowerful_RM",1))
        {
            currentWeapon.Fire(aimRay);
            animator.SetTrigger("IsFiring");
        }
		
    }

    //Reloads the current weapon
    public void Reload()
    {
        if (reload || !currentWeapon)
            return;

        if (currentWeapon.ammo.carryingAmmo <= 0 || currentWeapon.ammo.clipAmmo == currentWeapon.ammo.maxClipAmmo)
            return;

        if (sc != null)
        {
            if (currentWeapon.sounds.reloadSound != null)
            {
                if (currentWeapon.sounds.audioS != null)
                {
                    sc.PlaySound(currentWeapon.sounds.audioS, currentWeapon.sounds.reloadSound, true, currentWeapon.sounds.pitchMin, currentWeapon.sounds.pitchMax);
                }
            }
        }
     
        reload = true;
        StartCoroutine(StopReload());
    }

    //Stops the reloading of the weapon
    IEnumerator StopReload()
    {
        yield return new WaitForSeconds(currentWeapon.weaponSettings.reloadDuration);
        currentWeapon.LoadClip();
        reload = false;
    }
    public bool isAnimationRunning(string name,int indexlayer)
    {

        if (name == "RightPunching" || name == "LeftPunching")
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(name) || animator.GetCurrentAnimatorStateInfo(1).IsName(name);
        }
        else
        {
            return animator.GetCurrentAnimatorStateInfo(indexlayer).IsName(name);

        }

    }
    public void Defense(bool def)
    {
        this.denfense = def;
    }
    public void SetCombo(int tempcombo)
    {
        combo = tempcombo;
    }
    public int GetCombo()
    {
        return combo;
    }
    //Sets out aim bool to be what we pass it
    public void Aim(bool aiming)
    {
        aim = aiming;
    }
    public int getWeaponType()
    {
        return weaponType;
    }
    public bool getReload()
    {
        return reload;
    }
    public bool getSettingWeapon()
    {
        return settingWeapon;
    }
    //Drops the current weapon
    public void DropCurWeapon()
    {
        if (!currentWeapon)
            return;

        currentWeapon.SetEquipped(false);
        currentWeapon.SetOwner(null);
        weaponsList.Remove(currentWeapon);
        currentWeapon = null;
    }

    //Switches to the next weapon
    public void SwitchWeapons()
	{
        if (settingWeapon || weaponsList.Count == 0)
            return;

        StartCoroutine(EquipAndUnequip());

        //if (currentWeapon)
        //{
        //    StartCoroutine(EquipAndUnequip());
        //    //int currentWeaponIndex = weaponsList.IndexOf(currentWeapon);
        //    //int nextWeaponIndex = (currentWeaponIndex + 1) % weaponsList.Count;
        //    //if (nextWeaponIndex < currentWeaponIndex)
        //    //{
        //    //    currentWeapon = null;
        //    //}
        //    //else
        //    //{
        //    //    currentWeapon = weaponsList[nextWeaponIndex];

        //    //}

        //}
        //else
        //{
        //    currentWeapon = weaponsList[0];
        //}
        if (currentWeapon)
        {
            settingWeapon = true;
            StartCoroutine(StopSettingWeapon());
        }
       

		//SetupWeapons ();
    }

    //Stops swapping weapons
    IEnumerator StopSettingWeapon()
    {
        yield return new WaitForSeconds(0.7f);
        settingWeapon = false;
    }
    //void debug(string str)
    //{
    //    //text.text = Time.time + str;
    //}
    void UnequipCurrentWeapon()
    {
        Debug.Log("UnequipWeaponnnnn");
        currentWeapon.SetEquipped(false);
        currentWeapon.SetOwner(this);
    }
    //IEnumerator StartUnequip()
    //{
    //    if (currentWeapon != null)
    //    {
    //        //debug("start  unequip cua weapontype");

    //        animator.SetTrigger(animations.TriggerUnequip);
    //        yield return null;


    //    }
    //}
    
    IEnumerator EquipAndUnequip()
    {
        
        //StartCoroutine(StartUnequip());
        if (currentWeapon != null)
        {
            //debug("start  unequip cua weapontype");

            animator.SetTrigger(animations.TriggerUnequip);
            if (currentWeapon.weaponType==Weapon.WeaponType.Pistol)
            {
                yield return new WaitForSeconds(0.4f);

            }
            else if (currentWeapon.weaponType == Weapon.WeaponType.Rifle)
            {
                
                yield return new WaitForSeconds(1.05f);

            }


        }
        while (isAnimationRunning("Pistol_UnEquip", 1) || isAnimationRunning("UnEquip_rifle_01_x1d", 1))
        {
            //debug("dang unequip : " + weaponType);
            yield return null;
        }
        //debug("finish unequip");

        //Debug.Log("dang equip cua weapontype: " + currentWeapon.weaponType);
        if (currentWeapon)
        {

            int currentWeaponIndex = weaponsList.IndexOf(currentWeapon);
            int nextWeaponIndex = (currentWeaponIndex + 1) % weaponsList.Count;
            if (nextWeaponIndex < currentWeaponIndex)
            {
                currentWeapon = null;
            }
            else
            {
                
                currentWeapon = weaponsList[nextWeaponIndex];
                yield return null;

                animator.SetTrigger(animations.TriggerEquip);

            }
        }
        else
        {
            currentWeapon = weaponsList[0];
            animator.SetTrigger(animations.TriggerEquip);
            yield return null;
        }

        //while (isAnimationRunning("Pistol_Equip") || isAnimationRunning("Equip_Rifle_02_x2d"))
        //{
        //    debug("dang unequip cua weapontype");

        //    Debug.Log("dang equip cua weapontype: " + currentWeapon.weaponType);

        //    yield return null;
        //}

        //SetupWeapons();


    }
    void OnAnimatorIK()
    {
        if (!animator)
            return;

        if (currentWeapon && currentWeapon.userSettings.leftHandIKTarget && weaponType!=0 && !reload && !settingWeapon && !isAI)
        {
            if (!(weaponType==1 && !aim))
            {
                if (!isAnimationRunning("UnEquip_rifle_01_x1d",1) && !isAnimationRunning("Equip_Rifle_02_x2d", 1))
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    Transform target = currentWeapon.userSettings.leftHandIKTarget;
                    Vector3 targetPos = target.position;
                    Quaternion targetRot = target.rotation;
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPos);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, targetRot);
                }
                
            }
           
            //if (weaponType == 1 && !aim)
            //{
            //    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            //    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            //}
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }
}
