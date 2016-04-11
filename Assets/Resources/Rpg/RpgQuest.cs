using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Window;
using Rpg.QuestSystem;

namespace Rpg
{
	namespace QuestSystem
	{
		public class Quest
		{
			public enum QuestStatus {progress, complete, archived};
			public string name;
			public string description;
			public float exp;
			public string icon;
			public string image;
			public List<string> rewardName;
			public List<Item> reward;
			public int index;
			public bool read;
			public QuestStatus status;

			public Quest(int questIndex) {
				string path = "Quests/Quests/quest"+questIndex;

				TextAsset questFile = Resources.Load(path) as TextAsset;

				JsonUtility.FromJsonOverwrite(questFile.text, this);
				index = questIndex;
				read = false;
				status = QuestStatus.progress;

				EventManager.Trigger("QuestAdd");
			}

			public override string ToString() {
				return name;
			}
		}
	}

	namespace WindowSystem
	{
		public class Journal : WindowBase
		{
			GameObject questJournalSelect;
			Transform tabs;
			GameObject space;
			RectTransform myRt;
			ToggleGroup tg;
			RectTransform pane;
			GameObject questPane;

			public static Color unReadColor = new Color(1f,1f,1f, 1);
			public static Color readColor = new Color(0.3f,0.3f,0.3f, 1);

			public Journal(Transform _canvas) : base(_canvas) {
				prefab = Resources.Load("WindowSystem/Prefabs/QuestJournal/QuestJournal") as GameObject;

				prefab
					.transform.Find("Name")
					.Find("Text")
					.GetComponent<Text>()
					.text = WindowCanvas.language.journal;

				questJournalSelect = Resources.Load("WindowSystem/Prefabs/QuestJournal/QuestJournalSelect") as GameObject;
				space = Resources.Load("WindowSystem/Prefabs/QuestJournal/Space") as GameObject;
				questPane = Resources.Load("WindowSystem/Prefabs/QuestJournal/QuestJournalPane") as GameObject;
			}

			public override void Open(bool preserveParent = false) {
				base.Open();
				myRt = instance.GetComponent<UnityEngine.RectTransform>();
				tg = instance.GetComponent<ToggleGroup>();

				instance
					.transform.Find("Close")
					.GetComponent<Button>()
					.onClick.AddListener(delegate {Close();});

				tabs = myRt.Find("Scroll View").Find("Viewport").Find("Tabs");
				pane = myRt.Find("Panel").GetComponent<RectTransform>();

				LoadJournal();
			}

			void LoadJournal() {
				int i = 0;
				GameObject a;

				UnityEngine.RectTransform rt;
				foreach(Quest qt in Player.questLog){
					a = UnityEngine.Object.Instantiate(questJournalSelect);
					Toggle at = a.GetComponent<Toggle>();

					at.group = tg;
					at.isOn = i++ == 0? true : false;

					rt = a.GetComponent<RectTransform>();
					rt.SetParent(tabs, false);

					Text label = a.transform.Find("Handler").Find("Text").gameObject.GetComponent<Text>();

					if(qt.read){
						label.color = Journal.readColor;
					} else {
						label.color = Journal.unReadColor;
					}

					ConfigSelectToggle(at, qt, label);

				}

				a = UnityEngine.Object.Instantiate(space);
				rt = a.GetComponent<RectTransform>();
				rt.SetParent(tabs, false);
			}

			void ConfigSelectToggle(Toggle at, Quest qt, Text label) {
				RectTransform rt = at.GetComponent<RectTransform>();
				rt.Find("Handler").Find("Text").GetComponent<Text>().text = qt.name;
				if(at.isOn) InstantiateQuestPane(false, qt, label);
				at.onValueChanged.AddListener(delegate {InstantiateQuestPane(!at.isOn, qt, label);});
			}

			public void InstantiateQuestPane(bool selected, Quest quest, Text label){
				if(selected) return;
				label.color = Journal.readColor;
				//Removing All
				foreach (RectTransform t in pane) {
					UnityEngine.Object.Destroy(t.gameObject);
				}

				quest.read = true;
				EventManager.Trigger("QuestRead");

				GameObject a = UnityEngine.Object.Instantiate(questPane);
				RectTransform rt = a.GetComponent<RectTransform>();
				rt.SetParent(pane, false);

				RectTransform content = rt.Find("Viewport").Find("Content") as RectTransform;

				content.Find("QuestName").GetComponent<Text>().text = quest.name;

				content.Find("QuestDescription").GetComponent<Text>().text = quest.description;
				//content.Find("Image").GetComponent<Image>().sprite = quest.description;
				RectTransform reward = content.Find("Reward") as RectTransform;
				reward.Find("Exp").GetComponent<Text>().text = quest.exp+" exp";

				Text status = content.Find("Status").gameObject.GetComponent<Text>();

				Debug.Log(quest.status);
				
				status.text = (quest.status == Quest.QuestStatus.complete) ? "Completed" : "";

			}

		};
	}
}