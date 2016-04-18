using UnityEngine;
using System.Collections;
using Window;
using Rpg;
using Rpg.QuestSystem;
using Rpg.DialogueSystem;

public class ItemBehaviour : MonoBehaviour, IInteractable {

	// Use this for initialization
	public string name;
	Item self;
	GameObject interactable;

	void Start () {
		self = new Item(name);
	}

	public void OnInteract(GameObject from) {
		if(self.preRequirements != null && !Log.HasKey(self.preRequirements)) return;
		
		if(self.book != null && self.autoOpen) {
			Debug.Log("Open");
			Transform target = GameObject.Find("Main Canvas").transform;
			Window.Book book = new Window.Book(target, self.book);
			book.Open();
		}

		if(self.receive) Player.inventory.Add(self);

		if(self.registerLog != null) Log.Register(self.registerLog);

		if(!self.permanent) Destroy(gameObject);
	}

	public void Interact(GameObject to) {}
}
