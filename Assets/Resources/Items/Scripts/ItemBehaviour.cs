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

	public void OnInteractEnter(GameObject from) {
		if(self.preRequirements != null && !Log.HasKey(self.preRequirements)) return;
		
		if(self.book != null && self.autoOpen) {
			Transform target = GameObject.Find("GameController").transform;
			Window.Book book = new Window.Book(target, self.book);
			book.Open();
		}

		if(self.receive) Player.inventory.Add(self);

		if(self.registerLog != null) Log.Register(self.registerLog);

		if(!self.permanent) Destroy(gameObject);
	}

	public bool AutoInteract() {
		return false;
	}

	public void OnInteractExit(GameObject from){}

	public void Interact(GameObject to) {}
}
