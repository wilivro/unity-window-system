using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quest
{
	public string name;
	public string description;
	public float exp;
	public string icon;
	public string image;	
	public string[] rewardName;
	public List<Item> reward;

	public Quest(int questIndex) {
		string path = "Quests/Quests/quest"+questIndex;

		TextAsset questFile = Resources.Load(path) as TextAsset;

 		JsonUtility.FromJsonOverwrite(questFile.text, this);
	}

}
