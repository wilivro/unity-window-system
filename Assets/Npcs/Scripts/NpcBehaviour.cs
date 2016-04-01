using UnityEngine;
using System.Collections;
using Rpg;

public class NpcBehaviour : MonoBehaviour, IInteractable {

	// Use this for initialization
	public string name;
	Npc self;

	void Start () {
		self = new Npc(name);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnInteract(IInteractable ii) {
		
	}

	public void Interact() {}
}
