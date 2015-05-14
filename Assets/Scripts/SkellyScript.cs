/*
Author: Trevor Richardson
SkellyScript.cs
04-01-2015

	Simple script for instantiating skeletons.
	
 */

using UnityEngine;
using System.Collections;

public class SkellyScript : MonoBehaviour {
	
	void Start () {
		// Face towards camera and re-orient x rotation so it doesn't fly away
		transform.LookAt(Camera.main.transform);
		transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
	}
}
