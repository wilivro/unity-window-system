﻿using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

	// Use this for initialization
	Rpg.Player player;
	void Awake () {
		player = new Rpg.Player("Teste");
		player.Save();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
