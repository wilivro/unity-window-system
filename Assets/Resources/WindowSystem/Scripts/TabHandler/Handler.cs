using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Handler : MonoBehaviour {

	// Use this for initialization
	RectTransform myrt;

	public Toggle toggle;
	public RectTransform pane;

	public void Start () {
		myrt = GetComponent<RectTransform>();
		pane = myrt.parent.parent.parent.Find("Panel").GetComponent<RectTransform>();

		toggle = GetComponent<Toggle>();
	}
	
}
