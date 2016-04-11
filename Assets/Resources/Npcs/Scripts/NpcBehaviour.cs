using UnityEngine;
using System.Collections;
using Rpg;
using Rpg.QuestSystem;
using Rpg.DialogueSystem;

public class NpcBehaviour : MonoBehaviour, IInteractable {

	// Use this for initialization
	public string name;
	Npc self;
	GameObject interactable;

	void Start () {
		self = new Npc(name);
	}

	public void OnInteract(GameObject from) {
		self.dialogueControl.Start();
	}

	public void Interact(GameObject to) {}
}
