﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Reset() {
        SceneManager.LoadScene("SampleScene");
    }
    public void StartGame() {
        SceneManager.LoadScene("SampleScene");
    }
}
