using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;


public class WindowCanvas : MonoBehaviour {

	// Use this for initialization
	public static Scene.Language   language;
	public static Window.Journal   journal;
	public static Window.Inventory inventory;

	GameObject wrapper;

	void Start () {
		language  = new Scene.Language("pt-br");
		journal   = new Window.Journal(transform);
		inventory = new Window.Inventory(transform);

		wrapper = transform.Find("Wrapper").gameObject;
	}

	void WrapperHandler(){
		if(Window.WindowBase.openedNow != null){
			wrapper.active = true;
			return;
		}

		wrapper.active = false;
	}
	
	// Update is called once per frame
	void Update () {
		WrapperHandler();
		if(CrossPlatformInputManager.GetButtonUp("Journal")){
			journal.Toggle();
		}
		if(CrossPlatformInputManager.GetButtonUp("Inventory")){
			inventory.Toggle();
		}
	}
}
