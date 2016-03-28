using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Rpg
{
	public class Inventory
	{
		public List<Item> items;
		int gold;
		public int Count {
			get {
				return items.Count;
			}

			set {

			}
		}

		public Inventory(){
			items = new List<Item>();
			gold = 100;
		}

		public Item Add(Item i){
			if(!i.acumulative){
				items.Add(i);
				return i;
			}

			Item it = items.Find(itt => itt.name == i.name);
			if(it != null) {
				it.qtd += i.qtd;
				return it;
			}

			items.Add(i);

			return i;
		}

		public Item Add(Item i, int qtd){
			i.qtd = qtd;
			return Add(i);
		}

		public Item Remove(Item i) {
			Item it = items.Find(itt => itt.name == i.name);
			if(it != null) items.Remove(i);

			return it;
		}

		public int GetGold(){
			return gold;
		}

		public int AddGold(int _add){
			gold += _add;

			return gold;
		}
	}

	public class Player
	{
		public string name;
		public string[] questLogParsed;
		public string[] inventoryParsed;
		public static List<Quest> questLog;
		public static Inventory inventory;

		public Player(string _name){
			name = _name;

			questLog = new List<Quest>();
			inventory = new Rpg.Inventory();

			questLog.Add(new Quest(1));
			questLog.Add(new Quest(2));
			questLog.Add(new Quest(3));
			questLog.Add(new Quest(3));
			questLog.Add(new Quest(3));
			questLog.Add(new Quest(3));
			questLog.Add(new Quest(3));
			questLog.Add(new Quest(3));
			questLog.Add(new Quest(3));
			questLog.Add(new Quest(3));
			questLog.Add(new Quest(3));
			questLog.Add(new Quest(3));
			questLog.Add(new Quest(1));

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
}
