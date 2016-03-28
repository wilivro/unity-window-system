using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuestJournal : MonoBehaviour
{


	GameObject questJournalSelect;
	GameObject space;
	private RectTransform myRt;
	private ToggleGroup tg;

	void Start () {
		myRt = GetComponent<RectTransform>();

		tg = GetComponent<ToggleGroup>();

		RectTransform rt;

		questJournalSelect = Resources.Load("WindowSystem/Prefabs/QuestJournal/QuestJournalSelect") as GameObject;
		space = Resources.Load("WindowSystem/Prefabs/QuestJournal/Space") as GameObject;

		int i = 0;
		GameObject a;
		foreach(Quest qt in Rpg.Player.questLog){
			a = Instantiate(questJournalSelect);
			Toggle at = a.GetComponent<Toggle>();

			at.group = tg;
			at.isOn = i++ == 0? true : false;

			rt = a.GetComponent<RectTransform>();
			rt.SetParent(myRt.Find("Viewport").Find("Tabs"), false);

			rt.gameObject.AddComponent<QuestJournalHandler>().quest = qt;
			
		}

		a = Instantiate(space);
		rt = a.GetComponent<RectTransform>();
		rt.SetParent(myRt.Find("Viewport").Find("Tabs"), false);
	}

}
