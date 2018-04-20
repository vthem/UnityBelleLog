using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZobLoggerInitializer : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        Zob.LogSystem.Instance.Initialize(null);
	}
}
