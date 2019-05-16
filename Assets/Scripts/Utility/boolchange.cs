using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boolchange : MonoBehaviour
{

    public bool test;

    void Start()
    {
    }

    public void SetFalse() {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void SetTrue() {
        this.transform.GetChild(0).gameObject.SetActive(true);
    }
}
