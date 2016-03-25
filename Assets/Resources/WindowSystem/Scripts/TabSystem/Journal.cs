using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Journal : MonoBehaviour {

	// Use this for initialization
	Quest q;
	RectTransform myrt;
	public RectTransform pane;
	public GameObject questPane;
	void Start () {
		q = new Quest(1);
		myrt = GetComponent<RectTransform>();
		pane = myrt.parent.parent.Find("Panel").GetComponent<RectTransform>();
		questPane = Resources.Load("WindowSystem/Prefabs/Quest/QuestPane") as GameObject;

		InstantiateQuestPane();

	}

	public void InstantiateQuestPane(){

		//Removing All
		foreach (RectTransform t in pane) {
		    	Destroy(t.gameObject);
	 	}

		GameObject a = Instantiate(questPane);
		RectTransform rt = a.GetComponent<RectTransform>();
		rt.SetParent(pane);
		
		rt.localScale = new Vector3(1f,1f,1f);
		RectTransform content = rt.Find("Viewport").Find("Content") as RectTransform;
		content.Find("QuestName").GetComponent<Text>().text = q.name;
		content.Find("QuestDescription").GetComponent<Text>().text = q.description;
		//content.Find("Image").GetComponent<Image>().sprite = q.description;
		RectTransform reward = content.Find("Reward") as RectTransform;
		reward.Find("Exp").GetComponent<Text>().text = q.exp+" exp";

	}
	
	// Update is called once per frame
	void Update () {

	}
}
