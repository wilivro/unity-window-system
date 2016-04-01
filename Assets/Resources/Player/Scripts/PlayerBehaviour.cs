using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using Rpg;

public class PlayerBehaviour : MonoBehaviour, IControlable, IInteractable {

	// Use this for initialization
	Rpg.Player player;
	Rigidbody2D rbody;

	GameObject interactable;

	void Awake () {
		player = new Rpg.Player("Teste");
		player.Save();
		rbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		Control();
		Interact();
	}

	public void OnInteract(IInteractable ii) {

	}

	public void Interact() {
		if(CrossPlatformInputManager.GetButtonDown("Submit")){
			print("oi");
			interactable.gameObject.SendMessage("OnInteract", this);
		}
	}

	public void Control() {

		Vector2 movement_vector = new Vector2();
        Vector2 axis = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        
  		movement_vector = axis;

		rbody.MovePosition(rbody.position + movement_vector*Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other){
		interactable = other.gameObject;
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
