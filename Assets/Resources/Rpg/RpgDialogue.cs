using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Window;
using Rpg.WindowSystem;
using Rpg.QuestSystem;

namespace Rpg
{

	namespace DialogueSystem
	{

		[Serializable]
		public class Dialogue
		{
			public int quest;
			public Speech[] before;
			public Speech[] request;
			public Speech[] inProgress;
			public Speech[] after;
			public int[] requirementsQuest;
			public string[] requirementsItems;
		}

		[Serializable]
		public class Speech
		{
			public string name;
			public string text;
			public Choice[] choice;
			public int gotoSpeech = -1;
			public int[] giveQuest;
			public string[] giveItem;
			public string[] getItem;
			public bool exit;
			public int[] completeQuest;
		}

		[Serializable]
		public class Choice
		{
			public string text;
			public int posImage;
			public string imageBase;
			public bool correct;
			public int gotoSpeech = -1;
			public int[] giveQuest;
			public string[] giveItem;
			public string[] getItem;
			public int[] completeQuest;
		}

		public class DialogueControl
		{
			Dialogue[] dialogue;
			Speech[] dummy;
			WindowSystem.Dialogue dialogueWindow;
			Transform canvas;
			string npcName;

			Speech[] actualDialogue;

			private UnityAction<object[]> OnGiveQuest;
			private UnityAction<object[]> OncompleteQuest;
			private UnityAction<object[]> OnGiveItem;
			private UnityAction<object[]> OnGetItem;

			public DialogueControl(ref Dialogue[] _dialogue, ref Speech[] _dummy, string _npcName){
				canvas = GameObject.Find("Main Canvas").transform;
				dialogue = _dialogue;
				npcName = _npcName;
				dummy = _dummy;

				dialogueWindow = new WindowSystem.Dialogue(canvas, npcName);

				OnGiveQuest = new UnityAction<object[]> (OnGiveQuestCallback);
				EventManager.AddListener("SpeechGiveQuest", OnGiveQuest);

				OnGiveItem = new UnityAction<object[]> (OnGiveItemCallback);
				EventManager.AddListener("SpeechGiveItem", OnGiveItem);

				OncompleteQuest = new UnityAction<object[]> (OncompleteQuestCallback);
				EventManager.AddListener("completeQuest", OncompleteQuest);

				OnGetItem = new UnityAction<object[]> (OnGetItemCallback);
				EventManager.AddListener("GetItem", OnGetItem);
			}

			void OnGiveQuestCallback(object[] param) {
				int[] give = (int[])param[0];

				for(int i = 0; i < give.Length; i++) {
					Player.questLog.Add(new Quest(give[i]));
				}
			}

			void OnGiveItemCallback(object[] param) {
				string[] give = (string[])param[0];

				for(int i = 0; i < give.Length; i++) {
					Player.inventory.Add(new Item(give[i]));
				}
			}

			void OnGetItemCallback(object[] param) {
				string[] getItem = (string[])param[0];

				for(int i = 0; i < getItem.Length; i++) {
					Player.inventory.Remove(new Item(getItem[i]));
				}
			}

			void OncompleteQuestCallback(object[] param) {
				int[] quests = (int[])param[0];

				if(quests == null) return;

				for(int i=0; i < quests.Length; i++) {
					Quest quest = Player.questLog.Where(q => q.index == quests[i]).Single();
					if(quest != null) quest.status = Quest.QuestStatus.complete;
				} 

			}

			bool IsSubArray<T>(T[] universe, T[] planet) {

				bool exists = false;
				for(int i = 0; i < planet.Length; i++) {
					for(int j = 0; j < universe.Length; j++) {
						if(planet[i].Equals(universe[j])){
							exists = true;
							continue;
						}
					}
					if(!exists) return false;
				}



				return true;
			}

			bool RequestQuest(){
				//lista de quests do player
				if(dialogue == null) return false;

				int[] questLog = Player.questLog
								.Where(q => q.status == Quest.QuestStatus.archived)
								.Select(q => q.index)
								.ToArray();

				Dialogue[] D = dialogue.Where(d =>
					(Array.IndexOf(questLog, d.quest) < 0) &&
					(IsSubArray<int>(questLog, d.requirementsQuest))
				).ToArray();

				if(D.Length >= 1) {
					actualDialogue = D[0].request;
					dialogueWindow.Open(actualDialogue);
					return true;
				}



				return false;
			}

			bool HasQuest(){
				if(dialogue == null) return false;

				int[] questLog = Player.questLog
								.Where(q => q.status != Quest.QuestStatus.archived)
								.Select(q => q.index)
								.ToArray();

				Dialogue[] D = dialogue
									.Where(d => Array.IndexOf(questLog, d.quest) >= 0)
									.ToArray();

				if(D.Length > 0) {
					Dialogue d = D.First();
					Quest what = Player.questLog.Where(q => q.index == d.quest).Single();

					if(what.status == Quest.QuestStatus.archived){
						return false;
					}

					if(what.status == Quest.QuestStatus.complete){
						actualDialogue = d.after;
						what.status = Quest.QuestStatus.archived;
					}
					else
						actualDialogue = d.inProgress;

					dialogueWindow.Open(actualDialogue);
					return true;
				}

				return false;
			}

			bool NotReadyQuest(){
				if(dialogue == null) return false;

				int[] questLog = Player.questLog
								.Where(q => q.status == Quest.QuestStatus.archived)
								.Select(q => q.index)
								.ToArray();

				Dialogue[] D = dialogue.Where(d =>
					(Array.IndexOf(questLog, d.quest) < 0) &&
					(!IsSubArray<int>(questLog, d.requirementsQuest) )
				).ToArray();

				if(D.Length >= 1) {
					actualDialogue = D[0].before;
					dialogueWindow.Open(actualDialogue);
					return true;
				}

				return false;
			}

			void DummySpeech() {
				System.Random rnd = new System.Random();
				int what = rnd.Next(0, dummy.Length);
				actualDialogue = new Speech[1] {dummy[what]};
				dialogueWindow.Open(actualDialogue);
			}

			public void Start() {
				if(HasQuest()) return;
				if(RequestQuest()) return;
				if(NotReadyQuest()) return;

				DummySpeech();

			}
		}

	}

	namespace WindowSystem
	{
		public class Dialogue : WindowBase
		{
			private UnityAction<object[]> OnNextPage;
			private UnityAction<object[]> OnWriteEnd;

			GameObject text;

			public DialogueSystem.Speech[] dialogue;
			public int page;
			string npcName;

			GameObject choices;
			GameObject choice;
			GameObject chs;

			public Dialogue(Transform _canvas, string _npcName) : base(_canvas) {
				prefab = Resources.Load("WindowSystem/Prefabs/Dialogue/DialogueWindow") as GameObject;
				choices = Resources.Load("WindowSystem/Prefabs/Dialogue/choices") as GameObject;
				choice = Resources.Load("WindowSystem/Prefabs/Dialogue/choice") as GameObject;

				OnNextPage = new UnityAction<object[]> (OnNextPageCallback);
				EventManager.AddListener("DialoguePageComplete", OnNextPage);


				OnWriteEnd = new UnityAction<object[]> (OnWriteEndCallback);
				EventManager.AddListener("DialogueWriteEnd", OnWriteEnd);

				npcName = _npcName;
			}

			void TriggerGiveQuest() {
				if(dialogue[page].giveQuest != null){
					EventManager.Trigger("SpeechGiveQuest", new object[1]{dialogue[page].giveQuest});
				}
			}
			void TriggerGiveItem() {
				if(dialogue[page].giveItem != null){
					EventManager.Trigger("SpeechGiveItem", new object[1]{dialogue[page].giveItem});
				}
			}
			void TriggerGetItem() {
				if(dialogue[page].getItem != null){
					EventManager.Trigger("GetItem", new object[1]{dialogue[page].getItem});
				}
			}

			void OnNextPageCallback(object[] param) {
				TriggerGiveQuest();
				TriggerGiveItem();
				TriggerGetItem();

				EventManager.Trigger("completeQuest", new object[1]{dialogue[page].completeQuest});

				if(dialogue[page].exit != null && dialogue[page].exit){
					Close();
					return;
				}

				if(dialogue[page].gotoSpeech >= 0) {
					page = dialogue[page].gotoSpeech;
				} else {
					page++;
				}

				ShowSpeech();
			}

			void OnChoiceCallback(DialogueSystem.Choice _ch){
				TriggerGiveQuest();
				TriggerGiveItem();
				TriggerGetItem();

				if(_ch.giveQuest != null){
					EventManager.Trigger("SpeechGiveQuest", new object[1]{_ch.giveQuest});
				}

				if(_ch.giveItem != null){
					EventManager.Trigger("SpeechGiveItem", new object[1]{_ch.giveItem});
				}

				if(_ch.getItem != null){
					EventManager.Trigger("GetItem", new object[1]{_ch.getItem});
				}

				EventManager.Trigger("completeQuest", new object[1]{_ch.completeQuest});

				page = _ch.gotoSpeech;
				UnityEngine.Object.Destroy(chs);
				ShowSpeech();
			}

			public void Open(DialogueSystem.Speech[] _d) {
				base.Open();
				dialogue = _d;
				page = 0;
				ShowSpeech();
			}

			string FormatText(string _name) {
				string formatted = _name.Replace("{{name}}", npcName);
				formatted = formatted.Replace("{{player}}", Player.name);
				formatted = formatted.Replace("\t", "");
				formatted = formatted.Trim();

				return formatted;
			}

			void ShowSpeech() {

				if(page >= dialogue.Length) {
					Close();
					return;
				};

				instance.transform.Find("Name").Find("Text").GetComponent<Text>().text = FormatText(dialogue[page].name);

				text = instance.transform.Find("Window").gameObject;
				text.SendMessage("Write", FormatText(dialogue[page].text));
				
			}

			void ShowChoices() {
				chs = UnityEngine.Object.Instantiate(choices);
				chs.transform.SetParent(instance.transform, false);
				chs.name = "Choices";

				for(int i=0; i < dialogue[page].choice.Length; i++) {
					GameObject ch = UnityEngine.Object.Instantiate(choice);
					ch.transform.SetParent(chs.transform, false);
					ch.transform.Find("Text").GetComponent<Text>().text = dialogue[page].choice[i].text;
					Sprite[] imgBase = Resources.LoadAll<Sprite>(dialogue[page].choice[i].imageBase);

					ch.transform.Find("Image").GetComponent<Image>().sprite = imgBase[dialogue[page].choice[i].posImage];

					DialogueSystem.Choice cho = dialogue[page].choice[i];

					Button bt = ch.GetComponent<Button>();
					bt.onClick.AddListener(delegate {
						OnChoiceCallback(cho);	
					});
				}
			}

			void OnWriteEndCallback(object[] param){
				if(dialogue[page].choice == null) return;

				if(dialogue[page].choice.Length > 0)
					ShowChoices();
			}

		}
	}
	
}