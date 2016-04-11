﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Window;
using Rpg.QuestSystem;
using Rpg.DialogueSystem;

namespace Rpg
{

	namespace QuestSystem{}

	public interface IInteractable
	{
		void OnInteract(GameObject from);
		void Interact(GameObject to);
	}

	public interface IControlable
	{
		void Control();

	}

	public class Character
	{
		public string name;

		public Character(string _name){
			name = _name;
		}
	} 

	public class Player : Character
	{
		public static string name;
		public string[] questLogParsed;
		public string[] inventoryParsed;
		public static List<Quest> questLog;
		public static Inventory inventory;

		public Player(string _name) :  base(_name){

			name = _name;
			questLog = new List<Quest>();
			inventory = new Rpg.Inventory();

			//questLog.Add(new Quest(1));

			inventory.Add(new Item("potion"),3);

			inventory.Add(new Item("mana"),5);

			inventory.Add(new Item("cartilha"));


		}

		public void ParseLists(){
			
			int i = 0;

			questLogParsed = new string[questLog.Count];
			foreach(Quest qt in questLog){
				questLogParsed[i++] = qt.ToString();
			}

			i = 0;

			inventoryParsed = new string[inventory.Count];
			foreach(Item it in inventory.items){
				inventoryParsed[i++] = it.ToString();
			}
		}

		public bool Save(){

			ParseLists();

			string dir = "SaveData";

			if(!Directory.Exists(dir)) {
	           Directory.CreateDirectory(dir);
	        }

			string path = null;
			path = "SaveData/save_"+name+".json";
			#if UNITY_ANDROID
				path = Application.persistentDataPath+"/save_" + name +".json";
			#endif
			#if UNITY_EDITOR
				path = "Assets/Resources/Player/SaveData/save_"+name+".json";
			#endif

			string str = JsonUtility.ToJson(this);
			using (FileStream fs = new FileStream(path, FileMode.Create)){
				using (StreamWriter writer = new StreamWriter(fs)){
			    	writer.Write(str);
			 	}
			}

			#if UNITY_EDITOR
				UnityEditor.AssetDatabase.Refresh ();
			#endif

			return File.Exists(path);
		}
	}


	public class Npc : Character
	{
		public string name;
		public Speech[] dummyDialogue;
		public int[] questList;
		public Dialogue[]  dialogue;
		public DialogueControl dialogueControl;

		public Npc(string _name) : base(_name) {
			string path = "Npcs/source/"+_name;

			TextAsset questFile = Resources.Load(path) as TextAsset;
			JsonUtility.FromJsonOverwrite(questFile.text, this);

			dialogueControl = new DialogueControl(ref dialogue, ref dummyDialogue, name);
		}
	}
	
}


