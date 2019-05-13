using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public Text textLoadingProgress;

    public void Start()
    {
        DontDestroyOnLoad(this);
    }
    public void SetLoadingProgress(float progress)
    {
        textLoadingProgress.gameObject.SetActive(true);
        int loadingPercent = (int)(progress * 100f);
        textLoadingProgress.text = "Loading... " + loadingPercent.ToString() + "%";
    }
}
