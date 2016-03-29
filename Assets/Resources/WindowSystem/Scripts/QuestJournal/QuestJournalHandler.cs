using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rpg;

public class QuestJournalHandler : Handler, ISelectable {

	public Quest quest;
	public GameObject questPane;
	RectTransform rt;

	void Start () {
		base.Start();
		rt = GetComponent<RectTransform>();
		rt.Find("Handler").Find("Text").GetComponent<Text>().text = quest.name;
		questPane = Resources.Load("WindowSystem/Prefabs/QuestJournal/QuestJournalPane") as GameObject;

		if(toggle.isOn) InstantiateQuestPane();

		toggle.onValueChanged.AddListener(delegate {OnSelect(!toggle.isOn);});

	}

	public void InstantiateQuestPane(){

		//Removing All
		foreach (RectTransform t in pane) {
		    	Destroy(t.gameObject);
	 	}

		GameObject a = Instantiate(questPane);
		RectTransform rt = a.GetComponent<RectTransform>();
		rt.SetParent(pane, false);
		
		RectTransform content = rt.Find("Viewport").Find("Content") as RectTransform;
		content.Find("QuestName").GetComponent<Text>().text = quest.name;
		content.Find("QuestDescription").GetComponent<Text>().text = quest.description;
		//content.Find("Image").GetComponent<Image>().sprite = quest.description;
		RectTransform reward = content.Find("Reward") as RectTransform;
		reward.Find("Exp").GetComponent<Text>().text = quest.exp+" exp";

	}

	public void OnSelect(bool selected) {
		if(selected) return;
		InstantiateQuestPane();
	}
}
