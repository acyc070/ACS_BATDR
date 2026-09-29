using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using InControl;
using S13Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeGame : JMonoBehaviour
{
	[SerializeField]
	private bool m_IsGameInitializer;

	private List<Action> m_ActionSequence;

	private GameManager m_GameManager => GameManager.Instance;

	public override void Awake()
	{
		this.Initialize();
	}

	private void Initialize()
	{
		Application.targetFrameRate = ProjectSettings.Instance.TargetFrameRate;
		this.PreloadFramework();
	}

	private void PreloadFramework()
	{
		this.ClearActionSequence();
		this.m_ActionSequence = new List<Action>();
		this.m_ActionSequence.Add(this.InitAssetManager);
		this.m_ActionSequence.Add(this.InitDOTween);
		this.m_ActionSequence.Add(this.InitInControl);
		this.m_ActionSequence.Add(this.InitI2Localization);
		this.m_ActionSequence.Add(this.InitS13AudioManager);
		this.m_ActionSequence.Add(this.InitPoolingManager);
		this.m_ActionSequence.Add(this.InitSectionManager);
		this.m_ActionSequence.Add(this.InitRespawnManager);
		this.m_ActionSequence.Add(this.InitAchievementManager);
		this.m_ActionSequence.Add(this.InitUIManager);
		this.m_ActionSequence.Add(this.InitPlayerSettings);
		this.m_ActionSequence.Add(this.InitSaveData);
		for (int i = 0; i < this.m_ActionSequence.Count; i = unchecked(i + 1))
		{
			this.m_ActionSequence[i]?.Invoke();
		}
		base.StartCoroutine(this.PrepareFinalizeFramework());
	}

	private void InitAssetManager()
	{
		JDebug.Log("InitializeGame :: InitAssetManager", this, JDebug.JDebugType.Initialize);
		if (this.m_GameManager.AssetManager != null)
		{
			JDebug.LogWarning("Asset Manager already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			this.m_GameManager.AssetManager = AssetManager.Create();
		}
	}

	private void InitDOTween()
	{
		JDebug.Log("InitializeGame :: InitDOTween", this, JDebug.JDebugType.Initialize);
		DOTween.Init();
		DOTween.SetTweensCapacity(1024, 128);
	}

	private void InitInControl()
	{
		JDebug.Log("InitializeGame :: InitInControl", this, JDebug.JDebugType.Initialize);
		if (this.m_GameManager.InControlManager != null)
		{
			JDebug.LogWarning("InControllerManager already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			this.m_GameManager.InControlManager = new GameObject("[InControl]").AddComponent<InControlManager>();
		}
	}

	private void InitI2Localization()
	{
		JDebug.Log("InitializeGame :: InitI2Localization", this, JDebug.JDebugType.Initialize);
		I2LocalizationGroup i2LocalizationGroup;
		i2LocalizationGroup = Resources.Load<I2LocalizationGroup>("Localization/I2LocalizationGroup");
		for (int i = 0; i < i2LocalizationGroup.LanguageSourceAssets.Length; i = unchecked(i + 1))
		{
			LanguageSourceData mSource;
			mSource = i2LocalizationGroup.LanguageSourceAssets[i].mSource;
			if (!LocalizationManager.Sources.Contains(mSource))
			{
				LocalizationManager.AddSource(mSource);
			}
		}
	}

	private void InitS13AudioManager()
	{
		JDebug.Log("InitializeGame :: InitS13AudioManager", this, JDebug.JDebugType.Initialize);
		if (this.m_GameManager.AudioManager != null)
		{
			JDebug.LogWarning("S13Audio Manager already exists.", this, JDebug.JDebugType.Initialize);
			return;
		}
		S13Manager assetPrefab;
		assetPrefab = Resources.Load<S13Manager>("S13AudioManager");
		this.m_GameManager.AudioManager = GameManager.Instance.AssetManager.CreateAsset<S13Manager>(assetPrefab);
		this.m_GameManager.AudioManager.gameObject.name = "[S13AudioManager]";
	}

	private void InitPoolingManager()
	{
		JDebug.Log("InitializeGame :: InitPoolingManager", this, JDebug.JDebugType.Initialize);
		if (this.m_GameManager.PoolingManager != null)
		{
			JDebug.LogWarning("Pooling Manager already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			this.m_GameManager.PoolingManager = PoolingManager.Create();
		}
	}

	private void InitSectionManager()
	{
		JDebug.Log("InitializeGame :: InitSectionManager", this, JDebug.JDebugType.Initialize);
		if (this.m_GameManager.SectionManager != null)
		{
			JDebug.LogWarning("Section Manager already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			this.m_GameManager.SectionManager = SectionManager.Create();
		}
	}

	private void InitRespawnManager()
	{
		JDebug.Log("InitializeGame :: InitRespawnManager", this, JDebug.JDebugType.Initialize);
		if (this.m_GameManager.RespawnManager != null)
		{
			JDebug.LogWarning("Respawn Manager already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			this.m_GameManager.RespawnManager = RespawnManager.Create();
		}
	}

	private void InitAchievementManager()
	{
		JDebug.Log("InitializeGame :: InitAchievementManager", this, JDebug.JDebugType.Initialize);
		this.m_GameManager.AchievementManager = AchievementManager.Create();
		this.m_GameManager.AchievementManager.Init();
	}

	private void InitUIManager()
	{
		JDebug.Log("InitializeGame :: InitUIManager", this, JDebug.JDebugType.Initialize);
		if (this.m_GameManager.UIManager != null)
		{
			JDebug.LogWarning("UI Mananger already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			this.m_GameManager.UIManager = UIManager.Create();
		}
	}

	private void InitPlayerSettings()
	{
		JDebug.Log("InitializeGame :: InitPlayerSettings", this, JDebug.JDebugType.Initialize);
		if (this.m_GameManager.PlayerSettings != null)
		{
			JDebug.LogWarning("Player Settings already exists.", this, JDebug.JDebugType.Initialize);
		}
		else
		{
			this.m_GameManager.PlayerSettings = PlayerPreferences.Create();
		}
	}

	private void InitSaveData()
	{
		JDebug.Log("InitializeGame :: InitSaveData", this, JDebug.JDebugType.Initialize);
		if (this.m_GameManager.GameDataManager != null)
		{
			JDebug.LogWarning("Game Data Manager already exists.", this, JDebug.JDebugType.Initialize);
			return;
		}
		this.m_GameManager.GameData = new GameData();
		this.m_GameManager.GameDataManager = new GameDataManager();
		this.m_GameManager.GameDataManager.LoadGameData();
		if (!this.m_GameManager.GameData.Credits && this.m_GameManager.AchievementManager.GetAchievement(AchievementName.THE_MASTERS_PEN))
		{
			this.m_GameManager.GameData.SetCredits(has: true);
		}
	}

	private IEnumerator PrepareFinalizeFramework()
	{
		while (GameManager.Instance.bAsyncFileIOInProgress)
		{
			yield return null;
		}
		this.FinalizeFramework();
	}

	private void FinalizeFramework()
	{
		JDebug.Log("InitializeGame :: FinalizeFramework", this, JDebug.JDebugType.Initialize);
		if (this.m_GameManager.isGameLoaded)
		{
			GameManager.Instance.UIManager.Show<UITitleView>("UI/Views/UITitleView", "VIEW");
			return;
		}
		this.m_GameManager.isGameLoaded = true;
		if (this.m_IsGameInitializer)
		{
			SceneManager.LoadScene("Empty");
			GameManager.Instance.UIManager.Show<UIJDS>("UI/Videos/UIJDS", "VIEW").OnPlayOutComplete += this.HandleJDSIntroOnPlayOutComplete;
		}
		else
		{
			base.Dispose();
		}
	}

	private void HandleJDSIntroOnPlayOutComplete(object sender, EventArgs e)
	{
		JDebug.Log("InitializeGame :: HandleJDSIntroOnPlayOutComplete", this, JDebug.JDebugType.Initialize);
		(sender as UIJDS).OnPlayOutComplete -= this.HandleJDSIntroOnPlayOutComplete;
		this.ShowPhotosensitivityWarning();
	}

	private void HandleConsoleIntroOnPlayOutComplete(object sender, EventArgs e)
	{
		JDebug.Log("InitializeGame :: HandleConsoleIntroOnPlayOutComplete", this, JDebug.JDebugType.Initialize);
		(sender as UIConsole).OnPlayOutComplete -= this.HandleConsoleIntroOnPlayOutComplete;
		this.ShowPhotosensitivityWarning();
	}

	private void ShowPhotosensitivityWarning()
	{
		GameManager.Instance.UIManager.Show<UIPhotosensitiveWarning>("UI/Views/UIPhotosensitiveWarning", "VIEW").OnPlayOutComplete += this.HandlePhotosensitiveWarningOnPlayOutComplete;
	}

	private void HandlePhotosensitiveWarningOnPlayOutComplete(object sender, EventArgs e)
	{
		JDebug.Log("InitializeGame :: HandlePhotosensitiveWarningOnPlayOutComplete", this, JDebug.JDebugType.Initialize);
		(sender as UIPhotosensitiveWarning).OnPlayOutComplete -= this.HandlePhotosensitiveWarningOnPlayOutComplete;
		GameManager.Instance.UIManager.Show<UITitleView>("UI/Views/UITitleView", "VIEW");
	}

	private void ClearActionSequence()
	{
		if (this.m_ActionSequence != null)
		{
			this.m_ActionSequence.Clear();
			this.m_ActionSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.ClearActionSequence();
		base.OnDisposed();
	}
}
