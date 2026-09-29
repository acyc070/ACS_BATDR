using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SectionManager : JDisposable
{
	private Dictionary<SectionID, Section> m_Sections = new Dictionary<SectionID, Section>();

	private HashSet<SectionID> m_SectionsLoading = new HashSet<SectionID>();

	public event EventHandler OnLoaded;

	public static SectionManager Create()
	{
		return new SectionManager();
	}

	public Section GetSection(SectionID sectionID)
	{
		Section result;
		result = null;
		if (this.Contains(sectionID))
		{
			result = this.m_Sections[sectionID];
		}
		return result;
	}

	public Section[] GetAllSections()
	{
		List<Section> list;
		list = new List<Section>();
		foreach (KeyValuePair<SectionID, Section> section in this.m_Sections)
		{
			if (!list.Contains(section.Value))
			{
				list.Add(section.Value);
			}
		}
		return list.ToArray();
	}

	public bool Contains(SectionID sectionID)
	{
		return this.m_Sections.ContainsKey(sectionID);
	}

	public void Add(Section section)
	{
		if (!this.Contains(section.SectionID))
		{
			this.m_Sections.Add(section.SectionID, section);
		}
	}

	public void Remove(SectionID sectionID)
	{
		if (this.Contains(sectionID))
		{
			this.m_Sections.Remove(sectionID);
			SceneManager.UnloadSceneAsync(sectionID.ToString());
		}
	}

	public void InitializeSection(SectionID sectionID)
	{
		if (this.Contains(sectionID))
		{
			Section section;
			section = this.GetSection(sectionID);
			if (section != null)
			{
				if (!section.IsInitialized)
				{
					section.Initialize();
				}
				else
				{
					section.SetActive(active: true);
				}
				GameManager.Instance.InkDemonManager?.CheckAvailability();
			}
		}
		else if (!this.m_SectionsLoading.Contains(sectionID))
		{
			this.LoadSectionAsync(sectionID);
		}
		else
		{
			this.OnLoaded -= this.HandleLoadSectionAsyncOnLoaded;
			this.OnLoaded += this.HandleLoadSectionAsyncOnLoaded;
		}
	}

	private void HandleLoadSectionAsyncOnLoaded(object sender, EventArgs e)
	{
		this.OnLoaded -= this.HandleLoadSectionAsyncOnLoaded;
		Section section;
		section = sender as Section;
		if (section != null)
		{
			if (!section.IsInitialized)
			{
				section.Initialize();
			}
			else
			{
				section.SetActive(active: true);
			}
			GameManager.Instance.InkDemonManager?.CheckAvailability();
		}
	}

	public async Task LoadSectionAsync(SectionID sectionID, bool initialize = true)
	{
		if (!this.Contains(sectionID) && !this.m_SectionsLoading.Contains(sectionID))
		{
			this.m_SectionsLoading.Add(sectionID);
			SectionDataObject sectionDataObject;
			sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(sectionID);
			if (sectionDataObject == null)
			{
				GameManager.Instance.GameData.CurrentSave.Save(sectionID, DataObject<SectionID, SectionDataObject>.Create(sectionID));
			}
			else
			{
				Debug.Log("SectionDataObject Found: " + sectionDataObject);
			}
			AsyncOperation sectionLoadOp;
			sectionLoadOp = SceneManager.LoadSceneAsync(sectionID.ToString(), LoadSceneMode.Additive);
			sectionLoadOp.allowSceneActivation = true;
			while (!sectionLoadOp.isDone)
			{
				await Task.Yield();
			}
			Section section;
			section = null;
			GameObject[] rootGameObjects;
			rootGameObjects = SceneManager.GetSceneByName(sectionID.ToString()).GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				if (!(gameObject.name == sectionID.ToString()))
				{
					continue;
				}
				section = gameObject.GetComponent<Section>();
				section.Load(sectionDataObject);
				if (initialize)
				{
					section.Initialize();
					while (!section.IsReady)
					{
						await Task.Yield();
					}
				}
				GameManager.Instance.SectionManager.Add(section);
				break;
			}
			if (section != null)
			{
				this.OnLoaded.Send(section);
			}
			else
			{
				Debug.LogError("There was an error loading section: " + sectionID);
			}
			this.m_SectionsLoading.Remove(sectionID);
		}
		else
		{
			if (!this.Contains(sectionID) || !initialize)
			{
				return;
			}
			Section section;
			section = this.m_Sections[sectionID];
			if (!section.IsInitialized)
			{
				section.Initialize();
				while (!section.IsReady)
				{
					await Task.Yield();
				}
			}
			else
			{
				section.SetActive(active: true);
			}
		}
	}

	public async void LoadSectionsAsync(SectionID[] sectionIDs, SectionID[] initializeSectionIDs, Action callback)
	{
		await Task.WhenAll(sectionIDs.Select((SectionID i) => this.LoadSectionAsync(i, initialize: false)));
		await Task.WhenAll(initializeSectionIDs.Select((SectionID i) => this.LoadSectionAsync(i)));
		callback?.Invoke();
	}

	public void Clear()
	{
		if (this.m_Sections == null)
		{
			return;
		}
		foreach (KeyValuePair<SectionID, Section> section in this.m_Sections)
		{
			if (section.Value != null)
			{
				SceneManager.UnloadSceneAsync(section.Key.ToString());
			}
		}
		this.m_Sections.Clear();
	}

	public void Kill()
	{
		this.Clear();
		this.m_Sections = null;
	}

	protected override void OnDisposed()
	{
		this.Kill();
		this.OnLoaded = null;
		base.OnDisposed();
	}
}
