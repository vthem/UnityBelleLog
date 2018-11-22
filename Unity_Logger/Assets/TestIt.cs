using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIt : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Func();
	}

    private void Func()
    {
        Debug.Log("Func");
        Func1();
    }

    private void Func1()
    {
        Debug.Log("Func1");
    }

    // Update is called once per frame
    void Update () {

        Debug.Log("Update");
	}
}
