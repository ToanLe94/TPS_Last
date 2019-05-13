using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fader : MonoBehaviour {
    public LoadingUI loadingUI;

    string sceneToLoad;

    /// <summary>
    /// Call this method whenever you want to load a scene with a fader :)
    /// </summary>
    /// <param name="sceneName">Scene to Load</param>
    public void FadeIntoLevel(string sceneName) {
        loadingUI.gameObject.SetActive(true);
        //SceneManager.LoadSceneAsync(sceneName).allowSceneActivation = false ;
        sceneToLoad = sceneName;
        GetComponent<Animator>().Play("Fader In");
        //Invoke("load", 1f);
        StartCoroutine(LoadSceneAsync());
    }

    //void load()
    //{
    //    //finally load scene
    //    SceneManager.LoadScene(sceneToLoad);

    //    if (EasyAudioUtility.instance.soundSceneManager)
    //    {
    //        EasyAudioUtility.instance.soundSceneManager.onSceneChange(sceneToLoad);

    //    }
    //}

    private IEnumerator LoadSceneAsync()
    {
        Debug.Log("Load scene " + sceneToLoad);
        yield return new WaitForSeconds(1f);
        var loadingOpr = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!loadingOpr.isDone)
        {
            loadingUI.SetLoadingProgress(loadingOpr.progress);
            yield return null;
        }
        loadingUI.SetLoadingProgress(1f);
        yield return new WaitForSeconds(1f);

        if (EasyAudioUtility.instance.soundSceneManager)
        {
            EasyAudioUtility.instance.soundSceneManager.onSceneChange(sceneToLoad);
        }

        Destroy(loadingUI.gameObject);
    }
}
