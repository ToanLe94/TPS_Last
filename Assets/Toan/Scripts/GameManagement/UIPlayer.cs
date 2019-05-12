using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    private UserInput player { get { return FindObjectOfType<UserInput>(); } set { player = value; } }


    private CharacterStats charStats;
    private WeaponHandler wp;


    public Text healthtext;
    public Slider healthBar;
    public Text kindOfWeapon;
    public Text armo;
    // Start is called before the first frame update
    void Start()
    {
        if (player)
        {
            wp = player.GetComponent<WeaponHandler>();
            charStats = player.GetComponent<CharacterStats>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (wp)
        {

            if (wp.currentWeapon == null)
            {
                kindOfWeapon.text = "Unarmed.";
            }
            else
            {
                if (kindOfWeapon)
                {
                    kindOfWeapon.text = "Police 9mm (" +wp.currentWeapon.weaponType + ")";
                }
                if (armo)
                {
                    armo.text = wp.currentWeapon.ammo.clipAmmo + "//" + wp.currentWeapon.ammo.carryingAmmo;
                }
               
            }

        }
        if (healthBar && healthtext && charStats)
        {
            //Debug.Log("healthbar value :" + playerUI.healthBar.value + "health cua player: " + charStats.health);
            healthBar.value = charStats.health;
            healthtext.text = Mathf.Round(healthBar.value).ToString();
        }
    }

}
