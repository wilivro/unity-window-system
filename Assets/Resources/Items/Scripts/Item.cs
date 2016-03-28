using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
