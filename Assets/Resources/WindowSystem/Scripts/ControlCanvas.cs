using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlCanvas : MonoBehaviour {

	// Use this for initialization
	Canvas canvas;
	void Start () {
		canvas = GetComponent<Canvas>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Window.WindowBase.openedNow != null){
			canvas.enabled = false;
			return;
		}

		canvas.enabled = true;
	}
}
