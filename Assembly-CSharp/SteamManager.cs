using System;
using System.Text;
using Steamworks;
using UnityEngine;

[DisallowMultipleComponent]
public class SteamManager : JMonoBehaviour
{
	private static bool s_EverInialized;

	private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

	public bool IsInitialized { get; private set; }

	private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
	}

	public override void Awake()
	{
		if (SteamManager.s_EverInialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			this.SteamDebug("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			this.SteamDebug("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
			{
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException ex)
		{
			this.SteamDebug("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + ex, this);
			Application.Quit();
			return;
		}
		this.IsInitialized = SteamAPI.Init();
		if (!this.IsInitialized)
		{
			this.SteamDebug("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}
		SteamUserStats.RequestCurrentStats();
		SteamManager.s_EverInialized = true;
	}

	public override void OnEnable()
	{
		if (this.IsInitialized)
		{
			if (this.m_SteamAPIWarningMessageHook == null)
			{
				this.m_SteamAPIWarningMessageHook = SteamManager.SteamAPIDebugTextHook;
				SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
			}
			if (this.m_GameOverlayActivated == null)
			{
				this.m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(this.OnGameOverlayActivated);
			}
		}
	}

	private void Update()
	{
		if (this.IsInitialized)
		{
			SteamAPI.RunCallbacks();
		}
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		if (pCallback.m_bActive != 0)
		{
			if (!GameManager.Instance.IsPaused)
			{
				Time.timeScale = 0f;
			}
			if (AudioListener.volume >= 1f)
			{
				AudioListener.volume = 0f;
			}
		}
		else
		{
			if (!GameManager.Instance.IsPaused)
			{
				Time.timeScale = 1f;
			}
			if (AudioListener.volume <= 0f)
			{
				AudioListener.volume = 1f;
			}
		}
	}

	private void SteamDebug(object message, global::UnityEngine.Object context)
	{
	}

	protected override void OnDisposed()
	{
		if (this.IsInitialized)
		{
			SteamAPI.Shutdown();
		}
		this.m_SteamAPIWarningMessageHook = null;
		this.m_GameOverlayActivated = null;
		SteamManager.s_EverInialized = false;
		base.OnDisposed();
	}
}
