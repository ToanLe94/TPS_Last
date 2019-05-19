using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerLoadScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grim"))
        {
            //SceneManager.LoadScene("Map_02");
            if (SceneManager.GetActiveScene().name == "Map_01")
            {
                //load level via fader
                Fader fader = FindObjectOfType<Fader>();
                //fader.FadeIntoLevel("LoadingScreen"); // load your scene
                fader.FadeIntoLevel("Map_02"); // load your scene
                Debug.Log("Load Map_02");
            }
            else if (SceneManager.GetActiveScene().name == "Map_02")
            {
                Fader fader = FindObjectOfType<Fader>();
                //fader.FadeIntoLevel("LoadingScreen"); // load your scene
                fader.FadeIntoLevel("Map_03"); // load your scene
            }
            //loads a specific scene
            //#if !EMM_ES2
            //            PlayerPrefs.SetString("sceneToLoad", levelToLoad);
            //#else
            //            ES2.Save(levelToLoad, "sceneToLoad");
            //#endif

         
        }
    }
 
}
