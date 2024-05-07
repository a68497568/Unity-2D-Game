﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Prequel : MonoBehaviour
{

    void Start()
    {
        Invoke("SkipStory", 30f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            SkipStory();
        }
    }

    public void SkipStory()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //載入下一個場景
    }

}
