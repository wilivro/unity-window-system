using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Player
{
	public string name;
	public string[] questLogParsed;
	public string[] inventoryParsed;
	public static List<Quest> questLog;
	public static List<Item> inventory;

	public Player(string _name){
		name = _name;

		questLog = new List<Quest>();
		inventory = new List<Item>();

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
		questLog.Add(new Quest(3));

	}

	public void ParseLists(){
		
		int i = 0;

		questLogParsed = new string[questLog.Count];
		foreach(Quest qt in questLog){
			questLogParsed[i++] = qt.ToString();
		}

		i = 0;

		inventoryParsed = new string[inventory.Count];
		foreach(Item it in inventory){
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
