using UnityEngine;
using System.Collections;
using Rpg;
using Rpg.QuestSystem;
using Rpg.DialogueSystem;

public class WarpBehaviour : MonoBehaviour, IInteractable {

	// Use this for initialization
	public string name;

	Warp self;

	void Start () {
		self = new Warp(name);
	}

	public void OnInteractEnter(GameObject from) {
		if(self.Ready()) {
			Debug.Log("TP");
			return;
		}

		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos += new Vector3(0, 45, 0);

		self.span.Open(self.message, screenPos);
	}

	public void OnInteractExit(GameObject from){
		self.span.Close();
	}

	public bool AutoInteract() {
		return true;
	}

	public void Interact(GameObject to) {}
}
