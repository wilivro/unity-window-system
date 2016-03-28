using UnityEngine;
using System.Collections;


public class WindowCanvas : MonoBehaviour {

	// Use this for initialization
	Window.Journal journal;
	void Start () {
		journal = new Window.Journal(transform);
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonUp("Journal")){
			journal.Toggle();
		}
	}
}
