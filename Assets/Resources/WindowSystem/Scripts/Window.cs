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
		public static WindowBase openedNow = null;

		bool opened;

		public WindowBase() {
			
		}

		public WindowBase(Transform _canvas) {
			myCanvas = _canvas;
			opened = false;
		}

		public void StandAlone() {
			Debug.Log(openedNow);
			try
			{
				openedNow.Close();
				
			} catch {

			}
			openedNow = this;
			
			
		}

		public virtual void Open(bool preserveParent = false) {
			if(prefab == null) return;
			if(opened) return;
			if(!preserveParent) StandAlone();

			instance = UnityEngine.Object.Instantiate(prefab);
			RectTransform rt = instance.GetComponent<RectTransform>();
			rt.SetParent(myCanvas, false);
			opened = true;
		}

		public virtual void Close(bool preserveParent = false) {
			if(instance == null) return;
			UnityEngine.Object.Destroy(instance);
			opened = false;
			if(!preserveParent) openedNow = null;
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
		GameObject questJournalSelect;
		Transform tabs;
		GameObject space;
		RectTransform myRt;
		ToggleGroup tg;
		RectTransform pane;
		GameObject questPane;

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

				ConfigSelectToggle(at, qt);
				
			}

			a = UnityEngine.Object.Instantiate(space);
			rt = a.GetComponent<RectTransform>();
			rt.SetParent(tabs, false);
		}

		void ConfigSelectToggle(Toggle at, Quest qt) {
			RectTransform rt = at.GetComponent<RectTransform>();
			rt.Find("Handler").Find("Text").GetComponent<Text>().text = qt.name;
			if(at.isOn) InstantiateQuestPane(false, qt);
			at.onValueChanged.AddListener(delegate {InstantiateQuestPane(!at.isOn, qt);});
		}

		public void InstantiateQuestPane(bool selected, Quest quest){
			if(selected) return;
			//Removing All
			foreach (RectTransform t in pane) {
			    	UnityEngine.Object.Destroy(t.gameObject);
		 	}

			GameObject a = UnityEngine.Object.Instantiate(questPane);
			RectTransform rt = a.GetComponent<RectTransform>();
			rt.SetParent(pane, false);
			
			RectTransform content = rt.Find("Viewport").Find("Content") as RectTransform;
			content.Find("QuestName").GetComponent<Text>().text = quest.name;
			content.Find("QuestDescription").GetComponent<Text>().text = quest.description;
			//content.Find("Image").GetComponent<Image>().sprite = quest.description;
			RectTransform reward = content.Find("Reward") as RectTransform;
			reward.Find("Exp").GetComponent<Text>().text = quest.exp+" exp";

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
			prefab.transform.Find("Name").Find("Text").GetComponent<Text>().text = WindowCanvas.language.inventory;
			prefab.transform.Find("Gold").Find("Label").GetComponent<Text>().text = WindowCanvas.language.gold;
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

			InputField field = qtdInstance.transform.Find("InputField").gameObject
				.GetComponent<InputField>();

			field.text = it.qtd.ToString();
			field.Select();

			qtdInstance.transform.Find("Cancel").gameObject.GetComponent<Button>().onClick
				.AddListener(delegate {trash.Cancel();});

			qtdInstance.transform.Find("Submit").gameObject.GetComponent<Button>().onClick
				.AddListener(delegate {RemoveItem(it, trash);});

			qtdOpen = true;

		}

		void CloseQtd(){
			if(!qtdOpen) return;
			UnityEngine.Object.Destroy(qtdInstance);
			qtdOpen = false;
		}

		public void RemoveItem(Item it, InventoryTrash trash) {
			int value = Convert.ToInt32(qtdInstance.transform.Find("InputField").gameObject.GetComponent<InputField>().text);
			if(value > it.qtd) value = it.qtd;
			trash.RemoveItem(value);
		}

		public override void Open(bool preserveParent = false){
			base.Open();
			instance.transform.Find("Close").GetComponent<Button>().onClick.AddListener(delegate {Close();});
			instance.transform.Find("Trash").gameObject.AddComponent<InventoryTrash>();
			instance.transform.Find("Gold").Find("Text").GetComponent<Text>().text = Player.inventory.GetGold().ToString();
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
					it = Player.inventory.items[i];
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

	public class Book : WindowBase
	{
		string[] pages;
		GameObject text;
		GameObject image;
		bool preserveParent;
		public Book(Transform _canvas, string[] _pages, bool _preserveParent = false) : base(_canvas) {
			prefab = Resources.Load("WindowSystem/Prefabs/Book/Book") as GameObject;
			text = Resources.Load("WindowSystem/Prefabs/Book/Text") as GameObject;
			image = Resources.Load("WindowSystem/Prefabs/Book/Image") as GameObject;
			pages = _pages;
			preserveParent = _preserveParent;

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

				instance.transform.Find("Close").GetComponent<Button>().onClick.AddListener(delegate {Close(preserveParent);});
			}

			Transform container = instance.transform.Find("Scroll View").Find("Viewport").Find("Container");

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
  					a.GetComponent<Text>().text = texts[i];
  				}
  				if(i <  images.Count){
  					GameObject b = UnityEngine.Object.Instantiate(image);
  					b.transform.SetParent(container, false);

  					string imageName = images[i].ToString().Replace("{{", "");
  					imageName = imageName.Replace("}}", "");

  					Sprite imageFile = Resources.LoadAll<Sprite>("Items/ItemSource/Images/"+ imageName)[0];

  					b.GetComponent<Image>().sprite = imageFile;	
  				}
  			}

  			instance.transform.Find("Pages").Find("Prev")
  				.GetComponent<Button>()
  				.onClick.AddListener(delegate {OpenPage(page-1);});

			instance.transform.Find("Pages").Find("Next")
  				.GetComponent<Button>()
  				.onClick.AddListener(delegate {OpenPage(page+1);});

		}
	};

	public class Dialogue  : WindowBase {};
	public class Config    : WindowBase {};
	public class PauseGame : WindowBase {};

}
