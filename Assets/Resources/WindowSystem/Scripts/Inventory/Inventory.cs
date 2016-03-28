using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Inventory : MonoBehaviour {

	// Use this for initialization
	GameObject slot;
	GameObject item;
	GameObject description;
	GameObject instanceDescription; 
	void Start () {

		slot = Resources.Load("WindowSystem/Prefabs/Inventory/Slot") as GameObject;
		item = Resources.Load("WindowSystem/Prefabs/Inventory/Item") as GameObject;
		description = Resources.Load("WindowSystem/Prefabs/Inventory/Description") as GameObject;

		Item it = null;
		for(int i=0; i < Window.Inventory.capacity; i++){
			GameObject a = Instantiate(slot);
			RectTransform rta = a.GetComponent<RectTransform>();
			rta.SetParent(transform.Find("Slots") ,false);
			try {
				it = Rpg.Player.inventory.items[i];
				GameObject b = Instantiate(item);
				RectTransform rtb = b.GetComponent<RectTransform>();
				rtb.SetParent(rta, false);
				b.GetComponent<Image>().sprite = it.icon;
				rtb.Find("Qtd").Find("Text").GetComponent<Text>().text = it.qtd.ToString();

				Item itt = it;

				b.GetComponent<Button>().onClick.AddListener(delegate {OpenDescription(itt);});

			} catch	{}
		}

		transform.Find("Gold").Find("Text").GetComponent<Text>().text = Rpg.Player.inventory.GetGold().ToString();
	}

	void OpenDescription(Item it) {
		if(instanceDescription) Destroy(instanceDescription);
		instanceDescription = Instantiate(description);
		RectTransform rt = instanceDescription.GetComponent<RectTransform>();
		rt.SetParent(transform, false);

		rt.Find("Name").GetComponent<Text>().text = it.name;
		rt.Find("Image").GetComponent<Image>().sprite = it.image;
		rt.Find("Viewport").Find("Description").GetComponent<Text>().text = it.description;
		if(it.book == null){
			Destroy(rt.Find("Book").gameObject);	
			return;
		}
		rt.Find("Book").GetComponent<Button>().onClick.AddListener(delegate {OpenBook(it);});

	}

	void OpenBook(Item it){
		Transform target = GameObject.Find("Main Canvas").transform;
		Window.Book book = new Window.Book(target, it.book);
		book.Open();
	}
}
