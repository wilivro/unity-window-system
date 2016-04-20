using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

		private UnityAction<object[]> OnGiveQuest;
		private UnityAction<object[]> OncompleteQuest;
		private UnityAction<object[]> OnGiveItem;
		private UnityAction<object[]> OnGetItem;

		public Player(string _name) :  base(_name){

			name = _name;
			questLog = new List<Quest>();
			inventory = new Rpg.Inventory();

			//questLog.Add(new Quest(1));

			inventory.Add(new Item("i01"),3);
			inventory.Add(new Item("i03"));

			OnGiveQuest = new UnityAction<object[]> (OnGiveQuestCallback);
			EventManager.AddListener("PlayerReciveQuest", OnGiveQuest);

			OnGiveItem = new UnityAction<object[]> (OnGiveItemCallback);
			EventManager.AddListener("PlayerReciveItem", OnGiveItem);

			OncompleteQuest = new UnityAction<object[]> (OncompleteQuestCallback);
			EventManager.AddListener("PlayerCompleteQuest", OncompleteQuest);

			OnGetItem = new UnityAction<object[]> (OnGetItemCallback);
			EventManager.AddListener("PlayerRemoveItem", OnGetItem);


		}

		void OnGiveQuestCallback(object[] param) {
			string[] give = (string[])param[0];

			for(int i = 0; i < give.Length; i++) {
				questLog.Add(new Quest(give[i]));
			}
		}

		void OnGiveItemCallback(object[] param) {
			string[] give = (string[])param[0];

			for(int i = 0; i < give.Length; i++) {
				inventory.Add(new Item(give[i]));
			}
		}

		void OnGetItemCallback(object[] param) {
			string[] getItem = (string[])param[0];

			for(int i = 0; i < getItem.Length; i++) {
				inventory.Remove(new Item(getItem[i]));
			}
		}

		void OncompleteQuestCallback(object[] param) {
			int[] quests = (int[])param[0];

			if(quests == null) return;

			for(int i=0; i < quests.Length; i++) {
				Quest quest = questLog.Where(q => q.index == quests[i]).Single();
				if(quest != null) quest.status = Quest.QuestStatus.complete;
			} 

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

			dialogueControl = new DialogueControl(dialogue, dummyDialogue, name);
		}
	}

	public class Warp
	{
		public string[] requirements;

		public DialogueControl dialogueControl;

		public Warp() {

		}

		public Warp(string id) {
			//string path = ()(GameController.database.Find(id)).SelectPath();

			//TextAsset questFile = Resources.Load(path) as TextAsset;
			//JsonUtility.FromJsonOverwrite(questFile.text, this);

			//dialogueControl = new DialogueControl(dialogue, dummyDialogue, name);

		}

		public bool Ready() {
			return Log.HasKey(requirements);
		}
	}

	public class Log
	{
		public static Dictionary <string, string> activities = new Dictionary<string, string>();

		public static void Register(string _key, string _value) {
			try {
				EventManager.Trigger("QuestHelperItemCheck", new object[1] {_key});
				activities.Add(_key, _value);
				Debug.Log("Logging "+_key+" , "+_value);
			} catch {
				Debug.Log("Already registred "+_key);
			}
		}

		public static bool HasKey(string _key) {
			string _value = null;
			if(activities.TryGetValue(_key, out _value)) {
				return true;
			}

			return false;
		}

		public static bool HasKey(string[] arr) {
			foreach(string s in arr) {
				if(!HasKey(s)) return false;
			}
			return true;
		}

		public static void Register(DialogueSystem.LogData[] registerLog){
			foreach(DialogueSystem.LogData l in registerLog) {
				Register(l.key, l.message);
			}
		}

	}

	[Serializable]
	public class DatabaseDictionary : IDatabaseItem
	{
		public string key;
		public string label;

		public string GetKey() {
			return key;
		}

		public string GetLabel() {
			return label;
		}
	}

	[Serializable]
	public class DatabaseItem : IDatabaseItem
	{
		public string key;
		public string path;
		public string filename;

		public string GetKey() {
			return key;
		}

		public string GetFullPath(){
			return path+filename;
		}

		public string GetPath(){
			return path;
		}
	}

	[Serializable]
	public class DatabaseQuest : DatabaseItem
	{
	}

	[Serializable]
	public class DatabaseFace : DatabaseItem
	{
	}

	public interface IDatabaseItem
	{		
		string GetKey();
	}

	public class DatabaseJson
	{
		public DatabaseDictionary[] data;
		public DatabaseQuest[] quests;
		//public DatabaseFace[] faces;
		public DatabaseItem[] items;

		public DatabaseJson(string _path) {
			TextAsset file = Resources.Load(_path) as TextAsset;
			JsonUtility.FromJsonOverwrite(file.text, this);
		}
	}

	public class Database
	{
		public DatabaseJson json;

		public List<IDatabaseItem> i;

		public Database(string _path) {

			json = new DatabaseJson(_path);

			i = new List<IDatabaseItem>();

			FieldInfo[] fields = json.GetType().GetFields();

			foreach (FieldInfo fieldInfo in fields)
			{
			    Debug.Log("Field: " + fieldInfo.Name);
			    Debug.Log(typeof(DatabaseJson).GetField(fieldInfo.Name).GetValue(json));
			    IDatabaseItem[] property = (IDatabaseItem[])(typeof(DatabaseJson).GetField(fieldInfo.Name).GetValue(json));
			    List<IDatabaseItem> list = property.ToList();

			    foreach(IDatabaseItem it in list) {
			    	i.Add(it);
			    }
			}
			
		}

		public IDatabaseItem Find(string _key) {
			try {
				return i.Where(d => d.GetKey() == _key).Single();
			} catch {
				return null;
			}
		}
		
	}
	
}


