using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

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

		public void Open() {
			if(prefab == null) return;
			if(opened) return;

			instance = UnityEngine.Object.Instantiate(prefab);
			RectTransform rt = instance.GetComponent<RectTransform>();
			rt.SetParent(myCanvas, false);
			opened = true;
		}

		public void Close() {
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

	public class Journal : WindowBase {
		public Journal(Transform _canvas) : base(_canvas) {
			prefab = Resources.Load("WindowSystem/Prefabs/QuestJournal/QuestJournal") as GameObject;
			prefab.transform.Find("Name").Find("Text").GetComponent<UnityEngine.UI.Text>().text = WindowCanvas.language.journal;
		}
	};

	public class Inventory : WindowBase {
		public static int capacity = 30;

		public Inventory(Transform _canvas) : base(_canvas) {
			prefab = Resources.Load("WindowSystem/Prefabs/Inventory/Inventory") as GameObject;
			prefab.transform.Find("Name").Find("Text").GetComponent<UnityEngine.UI.Text>().text = WindowCanvas.language.inventory;
			prefab.transform.Find("Gold").Find("Label").GetComponent<UnityEngine.UI.Text>().text = WindowCanvas.language.gold;
		}
	};

	public class Book      : WindowBase {
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
  					Debug.Log(texts[i]);
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
