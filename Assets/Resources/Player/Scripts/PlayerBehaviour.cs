using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using Rpg;

public class PlayerBehaviour : MonoBehaviour, IControlable, IInteractable {

	// Use this for initialization
	public Rpg.Player self;

	Rigidbody2D rbody;
	GameObject interactable;

	void Start () {
		self = new Rpg.Player("Teste");
		self.Save();
		rbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		Control();
		Interact(interactable);
	}

	public void OnInteract(GameObject from) {

	}

	public void Interact(GameObject to) {
		if(to == null) return;

		if(CrossPlatformInputManager.GetButtonDown("Submit")){
			to.SendMessage("OnInteract", gameObject);
		}
	}

	public void Control() {

		Vector2 movement_vector = new Vector2();
        Vector2 axis = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        
  		movement_vector = axis;

		rbody.MovePosition(rbody.position + movement_vector*Time.deltaTime);
	}

	bool IsInteractable(GameObject test){
		return test.GetComponent<IInteractable>() != null;
	}

	void OnTriggerEnter2D(Collider2D other){
		if(IsInteractable(other.gameObject)) {
			interactable = other.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D other){
		interactable = null;
	}

	void OnCollisionEnter2D(Collision2D other) {
		OnTriggerEnter2D(other.collider);
	}

	void OnCollisionExit2D(Collision2D other) {
		OnTriggerExit2D(other.collider);
	}


}
