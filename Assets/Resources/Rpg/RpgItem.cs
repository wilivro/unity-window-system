using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Window;

namespace Rpg
{
	public class Item
	{
		public string baseName;
		public string name;
		public string description;
		public int iconIndex;
		public string imageName;
		public int qtd;
		public bool acumulative;
		public bool consumible;
		public bool unique;
		public string[] book;
		public bool visible;

		Sprite[] icons;
		public Sprite icon;
		Sprite[] images;
		public Sprite image;

		public Item(string _name) {
			baseName = _name;

			string path = "Items/ItemSource/Items/"+_name;
			TextAsset itemFile = Resources.Load(path) as TextAsset;

			if(itemFile == null) {
				visible = false;
				return;
			}
			visible = true;
			JsonUtility.FromJsonOverwrite(itemFile.text, this);
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

	namespace WindowSystem
	{
		public class ItemDescription : WindowBase
		{

			public ItemDescription(Transform _canvas) : base(_canvas) {
				prefab = Resources.Load("WindowSystem/Prefabs/Inventory/Description") as GameObject;
			}

			public void Open(Item it){
				base.Close();
				base.Open(true);
				instance.transform.Find("Close").GetComponent<Button>().onClick.AddListener(delegate {Close(true);});
				RectTransform rt = instance.GetComponent<RectTransform>();
				rt.SetParent(instance.transform, false);

				rt.Find("Name").GetComponent<Text>().text = it.name;
				rt.Find("Image").GetComponent<Image>().sprite = it.image;
				rt.Find("Scroll View").Find("Viewport").Find("Description").GetComponent<Text>().text = it.description;
				if(it.book == null){
					UnityEngine.Object.Destroy(rt.Find("Book").gameObject);	
					return;
				}
				rt.Find("Book").GetComponent<Button>().onClick.AddListener(delegate {OpenBook(it);});
			}

			void OpenBook(Item it){
				Transform target = GameObject.Find("Main Canvas").transform;
				Window.Book book = new Window.Book(target, it.book, true);
				book.Open();
			}
		}
	}
}
