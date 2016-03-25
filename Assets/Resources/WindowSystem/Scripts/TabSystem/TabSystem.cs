using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TabSystem : MonoBehaviour {


	public List<GameObject> tabs;
	private List<GameObject> instanceTabs;
	private RectTransform myRt;
	private ToggleGroup tg;

	void Start () {
		myRt = GetComponent<RectTransform>();

		tg = GetComponent<ToggleGroup>();

		RectTransform rt;
		for(int i=0; i< tabs.Count; i++){
			print(tabs[i]);
			GameObject a = Instantiate(tabs[i]);
			Toggle at = a.GetComponent<Toggle>();
			at.group = tg;
			at.isOn = i == 0? true : false;
			rt = a.GetComponent<RectTransform>();
			rt.SetParent(myRt, false);
			rt = rt.Find("Handler") as RectTransform;
			rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (i*32) +15,0);
			rt.sizeDelta = new Vector2(160, 30);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
