using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTest : MonoBehaviour {
    VT.Logger _logger = new VT.Logger("ahaha");

	// Use this for initialization
	void Start () {
        _logger.Trace();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
