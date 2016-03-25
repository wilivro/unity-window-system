using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Handler : MonoBehaviour {

	// Use this for initialization
	Toggle toggle;
	GameObject panel;
	void Start () {
		toggle = gameObject.GetComponent<Toggle>();
		panel = transform.Find("Panel").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		panel.active = toggle.isOn;
	}
}
