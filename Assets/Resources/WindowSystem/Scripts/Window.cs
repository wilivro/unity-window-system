using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using Rpg;

namespace Window
{

	public class WindowBase //BaseClass
	{
		public GameObject prefab;
		public GameObject instance;
		public Transform myCanvas;

		bool opened;

		public WindowBase() {
			
		}

		public WindowBase(Transform _canvas) {
			myCanvas = _canvas;
			opened = false;
		}

		public virtual void Open() {
			if(prefab == null) return;
			if(opened) return;

			instance = UnityEngine.Object.Instantiate(prefab);
			RectTransform rt = instance.GetComponent<RectTransform>();
			rt.SetParent(myCanvas, false);
			opened = true;
		}

		public virtual void Close() {
			if(instance == null) return;
			UnityEngine.Object.Destroy(instance);
			opened = false;
		}

		public void Toggle() {
			if(!opened) {
				Open();
			} else {
				Close();
			}
		}
	}

	public class Journal : WindowBase
	{
		public Journal(Transform _canvas) : base(_canvas) {
			prefab = Resources.Load("WindowSystem/Prefabs/QuestJournal/QuestJournal") as GameObject;
			prefab.transform.Find("Name").Find("Text").GetComponent<UnityEngine.UI.Text>().text = WindowCanvas.language.journal;
		}
	};

	public class Inventory : WindowBase
	{
		public static int capacity = 30;

		GameObject slot;
		GameObject item;
		GameObject qtd;
		GameObject qtdInstance;
		bool qtdOpen;

		ItemDescription description;

		public Inventory(Transform _canvas) : base(_canvas) {
			prefab = Resources.Load("WindowSystem/Prefabs/Inventory/Inventory") as GameObject;
			prefab.transform.Find("Name").Find("Text").GetComponent<UnityEngine.UI.Text>().text = WindowCanvas.language.inventory;
			prefab.transform.Find("Gold").Find("Label").GetComponent<UnityEngine.UI.Text>().text = WindowCanvas.language.gold;
			slot = Resources.Load("WindowSystem/Prefabs/Inventory/Slot") as GameObject;
			item = Resources.Load("WindowSystem/Prefabs/Inventory/Item") as GameObject;
			qtd = Resources.Load("WindowSystem/Prefabs/Inventory/QtdInput") as GameObject;
			qtdOpen = false;
			
		}

		public void OpenQtd(Item it, InventoryTrash trash) {
			if(qtdOpen) return;

			if(!it.acumulative || it.qtd < 2){
				trash.RemoveItem(it.qtd);
				return;
			}

			qtdInstance = UnityEngine.Object.Instantiate(qtd);
			qtdInstance.transform.SetParent(instance.transform, false);

			UnityEngine.UI.InputField field = qtdInstance.transform.Find("InputField").gameObject
				.GetComponent<UnityEngine.UI.InputField>();

			field.text = it.qtd.ToString();
			field.Select();

			qtdInstance.transform.Find("Cancel").gameObject.GetComponent<UnityEngine.UI.Button>().onClick
				.AddListener(delegate {trash.Cancel();});

			qtdInstance.transform.Find("Submit").gameObject.GetComponent<UnityEngine.UI.Button>().onClick
				.AddListener(delegate {RemoveItem(it, trash);});

			qtdOpen = true;

		}

		void CloseQtd(){
			if(!qtdOpen) return;
			UnityEngine.Object.Destroy(qtdInstance);
			qtdOpen = false;
		}

		public void RemoveItem(Item it, InventoryTrash trash) {
			int value = Convert.ToInt32(qtdInstance.transform.Find("InputField").gameObject.GetComponent<UnityEngine.UI.InputField>().text);
			if(value > it.qtd) value = it.qtd;
			trash.RemoveItem(value);
		}

		public override void Open(){
			base.Open();
			instance.transform.Find("Close").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {Close();});
			instance.transform.Find("Trash").gameObject.AddComponent<InventoryTrash>();
			instance.transform.Find("Gold").Find("Text").GetComponent<Text>().text = Rpg.Player.inventory.GetGold().ToString();
			description = new ItemDescription(instance.transform);
			ShowItems();
		}

		public void ShowItems(){

			CloseQtd();
			description.Close();

			Transform slots = instance.transform.Find("Slots");
			foreach(Transform tr in slots){
				UnityEngine.Object.Destroy(tr.gameObject);
			}
			
			Item it = null;
			for(int i = 0; i < capacity; i++){
				GameObject a = UnityEngine.Object.Instantiate(slot);
				RectTransform rta = a.GetComponent<RectTransform>();
				rta.SetParent(slots ,false);

				try {
					it = Rpg.Player.inventory.items[i];
					GameObject b = UnityEngine.Object.Instantiate(item);
					b.name = it.name;
					InventoryDraggableItem idi = b.AddComponent<InventoryDraggableItem>();
					idi.reference = it;
					RectTransform rtb = b.GetComponent<RectTransform>();
					rtb.SetParent(rta, false);
					b.GetComponent<Image>().sprite = it.icon;
					rtb.Find("Qtd").Find("Text").GetComponent<Text>().text = it.qtd.ToString();

					Item itt = it;

					b.GetComponent<Button>().onClick.AddListener(delegate {description.Open(itt);});

				} catch	{}
			}
		}
	};

	public class ItemDescription : WindowBase
	{
		
		public ItemDescription(Transform _canvas) : base(_canvas) {
			prefab = Resources.Load("WindowSystem/Prefabs/Inventory/Description") as GameObject;
		}

		public void Open(Item it){
			base.Close();
			base.Open();
			instance.transform.Find("Close").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {Close();});
			RectTransform rt = instance.GetComponent<RectTransform>();
			rt.SetParent(instance.transform, false);

			rt.Find("Name").GetComponent<Text>().text = it.name;
			rt.Find("Image").GetComponent<Image>().sprite = it.image;
			rt.Find("Viewport").Find("Description").GetComponent<Text>().text = it.description;
			if(it.book == null){
				UnityEngine.Object.Destroy(rt.Find("Book").gameObject);	
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

	public class Book : WindowBase
	{
		string[] pages;
		GameObject text;
		GameObject image;
		public Book(Transform _canvas, string[] _pages) : base(_canvas) {
			prefab = Resources.Load("WindowSystem/Prefabs/Book/Book") as GameObject;
			text = Resources.Load("WindowSystem/Prefabs/Book/Text") as GameObject;
			image = Resources.Load("WindowSystem/Prefabs/Book/Image") as GameObject;
			pages = _pages;

		}

		public void Open(){
			OpenPage(0);
		}

		public void OpenPage(int page) {

			if(page < 0) return;
			if(page >= pages.Length) return;

			if(instance == null){
				instance = UnityEngine.Object.Instantiate(prefab);
				instance.transform.SetParent(myCanvas, false);

				instance.transform.Find("Close").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {Close();});
			}

			Transform container = instance.transform.Find("Viewport").Find("Container");

			string pattern = @"\{\{[a-zA-Z0-9]+\}\}";
			string pattern2 = @"[a-zA-Z0-9]+";

			foreach(Transform t in container){
				UnityEngine.Object.Destroy(t.gameObject);
			}

  			MatchCollection  images = Regex.Matches(pages[page], pattern);
  			string[] texts = Regex.Split(pages[page], pattern);

  			for(int i=0; i < texts.Length; i++) {
  				if(texts[i] != ""){
  					GameObject a = UnityEngine.Object.Instantiate(text);
  					a.transform.SetParent(container, false);
  					a.GetComponent<UnityEngine.UI.Text>().text = texts[i];
  				}
  				if(i <  images.Count){
  					GameObject b = UnityEngine.Object.Instantiate(image);
  					b.transform.SetParent(container, false);

  					string imageName = images[i].ToString().Replace("{{", "");
  					imageName = imageName.Replace("}}", "");

  					Sprite imageFile = Resources.LoadAll<Sprite>("Items/ItemSource/Images/"+ imageName)[0];

  					b.GetComponent<UnityEngine.UI.Image>().sprite = imageFile;	
  				}
  			}

  			instance.transform.Find("Pages").Find("Prev")
  				.GetComponent<UnityEngine.UI.Button>()
  				.onClick.AddListener(delegate {OpenPage(page-1);});

			instance.transform.Find("Pages").Find("Next")
  				.GetComponent<UnityEngine.UI.Button>()
  				.onClick.AddListener(delegate {OpenPage(page+1);});

		}
	};

	public class Dialogue  : WindowBase {};
	public class Config    : WindowBase {};
	public class PauseGame : WindowBase {};

}
