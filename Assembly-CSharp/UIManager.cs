using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class UIManager : JMonoBehaviour
{
	private UIGameMenu m_UIGameMenu;

	public UIVisualControls UIVisualControls { get; private set; }

	public Camera Camera { get; private set; }

	public Dictionary<string, UILayer> UILayers { get; private set; }

	public AudioListener AudioListener { get; private set; }

	public static UIManager Create(object _data = null)
	{
		UIManager uIManager;
		uIManager = new GameObject("[UIManager]").AddComponent<UIManager>();
		UIManager.SetCursor(active: false);
		uIManager.AudioListener = uIManager.gameObject.AddComponent<AudioListener>();
		InputManager.OnActiveDeviceDetached += UIManager.HandleOnActiveDeviceDetached;
		return uIManager;
	}

	public static UIElement CreateElement(RectTransform parent, string _prefabKey, object _data = null)
	{
		UIElement uIElement;
		uIElement = GameManager.Instance.AssetManager.CreateAsset<UIElement>(_prefabKey, _data);
		uIElement.rectTransform.SetParent(parent);
		uIElement.transform.localPosition = Vector3.zero;
		uIElement.rectTransform.anchoredPosition = Vector2.zero;
		uIElement.transform.localScale = Vector3.one;
		return uIElement;
	}

	public void SetAudioListener(bool active)
	{
		if (this.AudioListener != null)
		{
			this.AudioListener.enabled = active;
		}
	}

	public static void SetCursor(bool active)
	{
		Texture2D asset;
		asset = GameManager.Instance.AssetManager.GetAsset<Texture2D>("cursor_icon_02");
		if (GameManager.Instance.IsRealWorld)
		{
			asset = GameManager.Instance.AssetManager.GetAsset<Texture2D>("cursor_icon_01");
		}
		Cursor.SetCursor(asset, Vector2.zero, CursorMode.Auto);
		Cursor.lockState = ((!active) ? CursorLockMode.Locked : CursorLockMode.None);
		Cursor.visible = active;
	}

	public void Clear(params string[] uiLayer)
	{
		foreach (string key in uiLayer)
		{
			if (this.UILayers.ContainsKey(key))
			{
				UIController[] componentsInChildren;
				componentsInChildren = this.UILayers[key].GetComponentsInChildren<UIController>();
				for (int j = 0; j < componentsInChildren.Length; j = unchecked(j + 1))
				{
					componentsInChildren[j]?.Dispose();
				}
			}
		}
	}

	public void Clear()
	{
		foreach (UILayer value in this.UILayers.Values)
		{
			UIController[] componentsInChildren;
			componentsInChildren = value.GetComponentsInChildren<UIController>();
			for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
			{
				componentsInChildren[i]?.Dispose();
			}
		}
	}

	public void ClearAllExclude(params string[] uiLayers)
	{
		unchecked
		{
			foreach (string key in this.UILayers.Keys)
			{
				bool flag;
				flag = false;
				for (int i = 0; i < uiLayers.Length; i++)
				{
					if (uiLayers[i] == key)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					UIController[] componentsInChildren;
					componentsInChildren = this.UILayers[key].GetComponentsInChildren<UIController>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i]?.Dispose();
					}
				}
			}
		}
	}

	public override void Awake()
	{
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.UIVisualControls = GameManager.Instance.AssetManager.CreateAsset<UIVisualControls>("UI/Core/UIVisualControls");
		this.UIVisualControls.transform.SetParent(base.transform);
		this.Camera = this.UIVisualControls.UICamera;
		this.UILayers = new Dictionary<string, UILayer>
		{
			{
				"VIEW",
				UILayer.Create("VIEW", 0f, base.transform)
			},
			{
				"BORDERS",
				UILayer.Create("BORDERS", -10f, base.transform)
			},
			{
				"NAVIGATION",
				UILayer.Create("NAVIGATION", -20f, base.transform)
			},
			{
				"HUD",
				UILayer.Create("HUD", -30f, base.transform)
			},
			{
				"CROSSHAIR",
				UILayer.Create("CROSSHAIR", -40f, base.transform)
			},
			{
				"TELEPORT",
				UILayer.Create("TELEPORT", -50f, base.transform)
			},
			{
				"OBJECTIVE",
				UILayer.Create("OBJECTIVE", -60f, base.transform)
			},
			{
				"AUDIO LOG",
				UILayer.Create("AUDIO LOG", -70f, base.transform)
			},
			{
				"MEMO",
				UILayer.Create("MEMO", -80f, base.transform)
			},
			{
				"MODAL",
				UILayer.Create("MODAL", -90f, base.transform)
			},
			{
				"PROMPT",
				UILayer.Create("PROMPT", -100f, base.transform)
			},
			{
				"INSPECT",
				UILayer.Create("INSPECT", -110f, base.transform)
			},
			{
				"BLOCKER",
				UILayer.Create("BLOCKER", -200f, base.transform)
			},
			{
				"NOTIFICATION TEXT",
				UILayer.Create("NOTIFICATION TEXT", -210f, base.transform)
			},
			{
				"NOTIFICATION BOX",
				UILayer.Create("NOTIFICATION BOX", -220f, base.transform)
			},
			{
				"CHAPTER TITLE",
				UILayer.Create("CHAPTER TITLE", -250f, base.transform)
			},
			{
				"CUTSCENE BARS",
				UILayer.Create("CUTSCENE BARS", -230f, base.transform)
			},
			{
				"SUBTITLES",
				UILayer.Create("SUBTITLES", -240f, base.transform)
			},
			{
				"UPPER BLOCKER",
				UILayer.Create("UPPER BLOCKER", -300f, base.transform)
			},
			{
				"GAME MENU",
				UILayer.Create("GAME MENU", -310f, base.transform)
			},
			{
				"GAME MENU PROMPT",
				UILayer.Create("GAME MENU PROMPT", -320f, base.transform)
			},
			{
				"GAME MENU SCREEN BLOCKER",
				UILayer.Create("GAME MENU SCREEN BLOCKER", -400f, base.transform)
			},
			{
				"ASYNC LOADER",
				UILayer.Create("ASYNC LOADER", -500f, base.transform)
			},
			{
				"ERROR",
				UILayer.Create("ERROR", -600f, base.transform)
			}
		};
	}

	private void InputManager_OnActiveDeviceDetached(InputDevice obj)
	{
	}

	public void Update()
	{
		if (!GameManager.Instance.IsPauseReady || !PlayerInput.Pause())
		{
			return;
		}
		if (!GameManager.Instance.IsPaused)
		{
			if (this.m_UIGameMenu == null)
			{
				GameManager.Instance.Pause();
				this.m_UIGameMenu = UILayouts.GameMenu();
			}
		}
		else if (this.m_UIGameMenu != null && this.m_UIGameMenu.IsActive)
		{
			this.m_UIGameMenu.OnPlayOutComplete -= this.HandleUIGameMenuOnPlayOutComplete;
			this.m_UIGameMenu.OnPlayOutComplete += this.HandleUIGameMenuOnPlayOutComplete;
			this.m_UIGameMenu.PlayOut();
		}
	}

	public void PlayOutGameMenu()
	{
		if (this.m_UIGameMenu != null)
		{
			if (this.m_UIGameMenu.IsActive)
			{
				this.m_UIGameMenu.OnPlayOutComplete -= this.HandleUIGameMenuOnPlayOutComplete;
				this.m_UIGameMenu.OnPlayOutComplete += this.HandleUIGameMenuOnPlayOutComplete;
				this.m_UIGameMenu.PlayOut();
			}
		}
		else
		{
			GameManager.Instance.ForcedUnpause();
			this.ClearGameMenu();
		}
	}

	public void ClearGameMenu()
	{
		if (this.m_UIGameMenu != null)
		{
			this.m_UIGameMenu.OnPlayOutComplete -= this.HandleUIGameMenuOnPlayOutComplete;
			this.m_UIGameMenu.Dispose();
		}
	}

	private void HandleUIGameMenuOnPlayOutComplete(object sender, EventArgs e)
	{
		this.m_UIGameMenu.OnPlayOutComplete -= this.HandleUIGameMenuOnPlayOutComplete;
		this.m_UIGameMenu = null;
		GameManager.Instance.Pause();
	}

	public T Show<T>(string assetKey, string layer, object data = null)
	{
		GameObject gameObject;
		gameObject = global::UnityEngine.Object.Instantiate(GameManager.Instance.AssetManager.GetAsset<GameObject>(assetKey));
		UILayer uILayer;
		uILayer = this.UILayers[layer];
		gameObject.transform.position = uILayer.transform.position;
		gameObject.transform.eulerAngles = uILayer.transform.eulerAngles;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.SetParent(uILayer.transform);
		T component;
		component = gameObject.GetComponent<T>();
		UIController uIController;
		uIController = component as UIController;
		if (uIController != null)
		{
			uIController.Initialize(data);
			JDebug.Log(uIController.name + " :: OnInitialized", uIController, JDebug.JDebugType.UI);
			uIController.PlayIn();
		}
		uILayer.SetController(uIController);
		return component;
	}

	private static void HandleOnActiveDeviceDetached(InputDevice obj)
	{
		if (GameManager.Instance.IsPauseReady && !GameManager.Instance.IsPaused)
		{
			GameManager.Instance.UIManager.UpdateGameMenu();
		}
	}

	public void UpdateGameMenu()
	{
		if (this.m_UIGameMenu == null)
		{
			GameManager.Instance.Pause();
			this.m_UIGameMenu = UILayouts.GameMenu();
		}
	}

	protected override void OnDisposed()
	{
		this.UIVisualControls = null;
		this.Camera = null;
		if (this.UILayers != null)
		{
			this.UILayers.Clear();
			this.UILayers = null;
		}
		this.AudioListener = null;
		this.m_UIGameMenu = null;
		InputManager.OnActiveDeviceDetached -= UIManager.HandleOnActiveDeviceDetached;
		base.OnDisposed();
	}
}
