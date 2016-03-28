using UnityEngine;
using System.Collections;

namespace Window
{
	public class WindowSystem
	{
		public GameObject prefab;
		GameObject instance;
		Transform myCanvas;

		bool opened;

		public WindowSystem() {
			
		}

		public WindowSystem(Transform _canvas) {
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

	public class Journal : WindowSystem {
		public Journal(Transform _canvas) : base(_canvas) {
			prefab = Resources.Load("WindowSystem/Prefabs/QuestJournal/QuestJournal") as GameObject;
		}
	};

	public class Dialogue  : WindowSystem {};
	public class Text      : WindowSystem {};
	public class Inventory : WindowSystem {};
	public class Config    : WindowSystem {};
	public class PauseGame : WindowSystem {};

}
