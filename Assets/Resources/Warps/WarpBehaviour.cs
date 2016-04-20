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

	public void OnInteract(GameObject from) {
		self.dialogueControl.Start();
	}

	public void Interact(GameObject to) {}
}
