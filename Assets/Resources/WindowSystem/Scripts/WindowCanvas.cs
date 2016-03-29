using UnityEngine;
using System.Collections;


public class WindowCanvas : MonoBehaviour {

	// Use this for initialization
	public static Scene.Language language;
	Window.Journal 	journal;
	public static Window.Inventory inventory;

	void Start () {
		language = new Scene.Language("pt-br");
		journal = new Window.Journal(transform);
		inventory = new Window.Inventory(transform);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonUp("Journal")){
			journal.Toggle();
		}
		if(Input.GetButtonUp("Inventory")){
			inventory.Toggle();
		}
	}
}
