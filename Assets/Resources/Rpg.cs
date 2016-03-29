using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Rpg
{
	public class Item
	{
		public string name;
		public string description;
		public int iconIndex;
		public string imageName;
		public int qtd;
		public bool acumulative;
		public bool consumible;
		public bool unique;
		public string[] book;

		Sprite[] icons;
		public Sprite icon;
		Sprite[] images;
		public Sprite image;


		public Item(string _name) {
			string path = "Items/ItemSource/Items/"+_name;
			TextAsset questFile = Resources.Load(path) as TextAsset;
	 		JsonUtility.FromJsonOverwrite(questFile.text, this);
	 		icons = Resources.LoadAll<Sprite>("Items/ItemSource/Images/icons");
	 		images = Resources.LoadAll<Sprite>("Items/ItemSource/Images/"+imageName);

	 		icon = icons[iconIndex];
	 		image = images[0];
	 		
	 		qtd = 1;
		}

		public override string ToString() {
			return name;
		}
	}

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
			if(i.acumulative){
				i.qtd = qtd;
				return Add(i);
			} else {
				for(int n = 0; n < qtd; n++){
					Add(i);
				}
			}

			return i;
		}

		public Item Remove(Item i, int qtd = 1) {
			Item it = items.Find(itt => itt.name == i.name);

			if(it != null && it.acumulative && it.qtd > 1){
				it.qtd -= qtd;
				if(it.qtd <=0) items.Remove(it);
				return it;
			}

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

	public class Quest
	{
		public string name;
		public string description;
		public float exp;
		public string icon;
		public string image;
		public List<string> rewardName;
		public List<Item> reward;
		public int index;

		public Quest(int questIndex) {
			string path = "Quests/Quests/quest"+questIndex;

			TextAsset questFile = Resources.Load(path) as TextAsset;

	 		JsonUtility.FromJsonOverwrite(questFile.text, this);
	 		index = questIndex;
		}

		public override string ToString() {
			return name;
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
