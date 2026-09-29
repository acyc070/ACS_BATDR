using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(800)]
public class SectionEnemyController : SectionController
{
	[SerializeField]
	private Requirements m_Requirements;

	[SerializeField]
	private EnemySelector m_EnemySelector;

	[SerializeField]
	private int m_SpawnLimit = 4;

	[SerializeField]
	private float m_TimerLimitMin = 90f;

	[SerializeField]
	private float m_TimerLimitMax = 150f;

	private SectionCharacterNodeController m_SectionCharacterNodeController;

	private List<Enemy> m_Enemies = new List<Enemy>();

	private List<LootableEnemy> m_LootableEnemies = new List<LootableEnemy>();

	private bool m_CanSpawn;

	private float m_Timer;

	private float m_TimerLimit;

	protected override IEnumerator InternalInitialize()
	{
		this.m_SectionCharacterNodeController = base.transform.GetComponentInChildren<SectionCharacterNodeController>();
		yield return null;
		if (this.m_Requirements != null && !this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
		yield return new WaitForEndOfFrame();
		SectionDataObject sectionDataObject;
		sectionDataObject = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(base.Section.SectionID);
		if (sectionDataObject != null)
		{
			if (sectionDataObject.LootableEnemyData != null && sectionDataObject.LootableEnemyData.Values != null)
			{
				foreach (LootableEnemyDataObject value in sectionDataObject.LootableEnemyData.Values)
				{
					if (value == null)
					{
						continue;
					}
					CharacterContent characterContent;
					characterContent = GameManager.Instance.AssetManager.CreateAsset<CharacterContent>(PrefabCheck.GetCharacter(value.EnemyType, value.LostOneType));
					if (!(characterContent != null))
					{
						continue;
					}
					characterContent.SoftInitialize();
					characterContent.UpdateMaterials();
					if (characterContent.RagdollController != null)
					{
						characterContent.Animator.enabled = false;
						characterContent.RagdollController.ForceActivate();
						characterContent.RagdollController.Sleep();
					}
					else
					{
						Collider[] componentsInChildren;
						componentsInChildren = characterContent.GetComponentsInChildren<Collider>(includeInactive: true);
						for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
						{
							componentsInChildren[i].enabled = true;
						}
					}
					LootableEnemy lootableEnemy;
					lootableEnemy = new GameObject("LootableEnemy").AddComponent<LootableEnemy>();
					lootableEnemy.Initialize(characterContent.gameObject, sectionDataObject.ID, value.EnemyType, value.LostOneType);
					lootableEnemy.SetData(value.ID);
				}
			}
			if (sectionDataObject.EnemyData != null && sectionDataObject.EnemyData.Values != null)
			{
				foreach (EnemyDataObject value2 in sectionDataObject.EnemyData.Values)
				{
					if (value2 != null)
					{
						Enemy enemy;
						enemy = GameManager.Instance.AssetManager.CreateAsset<Enemy>(PrefabCheck.GetEnemy(value2.EnemyType));
						enemy.transform.position = value2.GameObjectDatas.Transform.Position;
						enemy.transform.eulerAngles = value2.GameObjectDatas.Transform.EulerAngles;
						if (value2.EnemyType == EnemyType.LostOne || value2.EnemyType == EnemyType.LostOneColor)
						{
							CharacterContent characterContent2;
							characterContent2 = GameManager.Instance.AssetManager.CreateAsset<CharacterContent>(PrefabCheck.GetCharacter(value2.EnemyType, value2.LostOneType));
							characterContent2.transform.SetParent(enemy.transform);
							characterContent2.transform.localPosition = Vector3.zero;
							characterContent2.transform.localEulerAngles = Vector3.zero;
							characterContent2.transform.localScale = Vector3.one;
						}
						CharacterNodeGroup characterNodeGroup;
						characterNodeGroup = this.GetNodeGroup(value2.CurrentNodeID);
						if (characterNodeGroup == null)
						{
							characterNodeGroup = this.GetRandomCharacterNode(enemy.transform.position);
						}
						Enemy enemy2;
						enemy2 = this.Spawn(enemy, characterNodeGroup.Controller, getNewNode: false, value2.CombatState == 1);
						enemy2.SetSection(sectionDataObject.ID);
						enemy2.SetData(value2.ID);
						enemy2.transform.position = value2.GameObjectDatas.Transform.Position;
						enemy2.transform.eulerAngles = value2.GameObjectDatas.Transform.EulerAngles;
					}
				}
			}
		}
		yield return null;
	}

	private void Update()
	{
		if (GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		this.CheckFleeState();
		if (!this.m_CanSpawn)
		{
			return;
		}
		if (this.m_Timer >= this.m_TimerLimit && this.m_Enemies.Count < this.m_SpawnLimit)
		{
			this.ResetTimer();
			CharacterNodeGroup farthest;
			farthest = this.GetFarthest();
			if (farthest != null)
			{
				this.Spawn(this.m_EnemySelector.Get(), farthest.Controller, getNewNode: true, isAmbush: false).AddData();
			}
		}
		else
		{
			this.m_Timer += Time.deltaTime;
		}
	}

	private void CheckFleeState()
	{
		for (int i = 0; i < this.m_Enemies.Count; i = unchecked(i + 1))
		{
			Enemy enemy;
			enemy = this.m_Enemies[i];
			if (enemy != null && enemy.CurrentState == State.Character.Flee && Vector3.Distance(enemy.transform.position, enemy.CurrentNode.transform.position) <= enemy.Agent.Agent.stoppingDistance)
			{
				enemy.SetState(State.Character.Patrol);
			}
		}
	}

	public Enemy Spawn(EnemySelector enemySelector, CharacterNode characterNode, bool getNewNode, bool isAmbush)
	{
		return this.Spawn(enemySelector.Get(), characterNode, getNewNode, isAmbush);
	}

	public Enemy Spawn(Enemy _enemy, CharacterNode characterNode, bool getNewNode, bool isAmbush)
	{
		if (_enemy != null && characterNode != null)
		{
			_enemy.transform.position = characterNode.transform.position;
			_enemy.transform.eulerAngles = characterNode.transform.eulerAngles;
			if (isAmbush)
			{
				_enemy.Initialize();
				_enemy.SetNode(characterNode);
				_enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
				_enemy.OnNodeReached += this.HandleEnemyOnNodeReached;
				_enemy.SetTarget(GameManager.Instance.Player.transform);
				_enemy.SetState(State.Character.Follow);
				this.AddEnemy(_enemy);
			}
			else
			{
				_enemy.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
				_enemy.OnInitializeOnComplete += this.HandleEnemyOnInitializeOnComplete;
				_enemy.Initialize();
				_enemy.SetSection(base.Section.SectionID);
				if (getNewNode)
				{
					CharacterNodeGroup randomCharacterNode;
					randomCharacterNode = this.GetRandomCharacterNode(_enemy.transform.position);
					int num;
					num = 0;
					bool flag;
					flag = false;
					if (this.m_SectionCharacterNodeController.Group.Length > 1)
					{
						while (randomCharacterNode.Controller == characterNode && !flag)
						{
							randomCharacterNode = this.GetRandomCharacterNode(_enemy.transform.position);
							num = unchecked(num + 1);
							if (num >= 100)
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						_enemy.SetNode(characterNode);
					}
					else
					{
						_enemy.SetNode(randomCharacterNode.Controller);
					}
				}
				else
				{
					_enemy.SetNode(characterNode);
				}
				this.AddEnemy(_enemy);
			}
		}
		return _enemy;
	}

	public void AddEnemy(Enemy enemy)
	{
		if (!this.m_Enemies.Contains(enemy))
		{
			enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
			enemy.OnCharacterDeath += this.HandleEnemyOnCharacterDeath;
			this.m_Enemies.Add(enemy);
			enemy.transform.SetParent(base.transform);
		}
	}

	public void AddToSection(Enemy enemy)
	{
		this.AddEnemy(enemy);
		enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
		enemy.OnNodeReached += this.HandleEnemyOnNodeReached;
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		Enemy enemy;
		enemy = sender as Enemy;
		enemy.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
		if (this.m_Enemies.Contains(enemy))
		{
			this.m_Enemies.Remove(enemy);
		}
	}

	private void HandleEnemyOnInitializeOnComplete(object sender, EventArgs e)
	{
		Enemy obj;
		obj = sender as Enemy;
		obj.OnInitializeOnComplete -= this.HandleEnemyOnInitializeOnComplete;
		obj.OnNodeReached -= this.HandleEnemyOnNodeReached;
		obj.OnNodeReached += this.HandleEnemyOnNodeReached;
		obj.SetState(State.Character.Patrol);
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		Enemy enemy;
		enemy = sender as Enemy;
		enemy.OnNodeReached -= this.HandleEnemyOnNodeReached;
		if (enemy.CurrentState != State.Character.Death)
		{
			base.StartCoroutine(this.GetNewNode(enemy));
		}
		else
		{
			enemy.CancelSlide();
		}
	}

	private IEnumerator GetNewNode(Enemy enemy)
	{
		CharacterNodeGroup newNode;
		newNode = this.GetRandomCharacterNode(enemy.transform.position);
		if (this.m_SectionCharacterNodeController.Group.Length > 1)
		{
			while (newNode.Controller == enemy.CurrentNode)
			{
				newNode = this.GetRandomCharacterNode(enemy.transform.position);
				yield return new WaitForEndOfFrame();
			}
		}
		yield return new WaitForEndOfFrame();
		enemy.OnNodeReached += this.HandleEnemyOnNodeReached;
		enemy.SetNode(newNode.Controller);
	}

	public void AddLootableEnemy(LootableEnemy lootableEnemy)
	{
		if (this.m_LootableEnemies.Contains(lootableEnemy))
		{
			return;
		}
		int num;
		num = 3;
		foreach (LootableEnemy lootableEnemy3 in this.m_LootableEnemies)
		{
			if (lootableEnemy3.EnemyType == EnemyType.KingWidow || lootableEnemy3.LostOneType == LostOneType.Male_Amok)
			{
				num = 4;
			}
		}
		if (this.m_LootableEnemies.Count >= num)
		{
			LootableEnemy lootableEnemy2;
			lootableEnemy2 = this.m_LootableEnemies[0];
			lootableEnemy2.OnInteract -= this.HandleLootableEnemyOnInteract;
			if (lootableEnemy2.EnemyType == EnemyType.KingWidow || lootableEnemy2.LostOneType == LostOneType.Male_Amok)
			{
				lootableEnemy2 = this.m_LootableEnemies[1];
			}
			this.m_LootableEnemies.Remove(lootableEnemy2);
			lootableEnemy2.SelfDestruct();
		}
		lootableEnemy.OnInteract -= this.HandleLootableEnemyOnInteract;
		lootableEnemy.OnInteract += this.HandleLootableEnemyOnInteract;
		this.m_LootableEnemies.Add(lootableEnemy);
	}

	private void HandleLootableEnemyOnInteract(object sender, EventArgs e)
	{
		LootableEnemy lootableEnemy;
		lootableEnemy = sender as LootableEnemy;
		lootableEnemy.OnInteract -= this.HandleLootableEnemyOnInteract;
		if (this.m_LootableEnemies.Contains(lootableEnemy))
		{
			this.m_LootableEnemies.Remove(lootableEnemy);
		}
	}

	private CharacterNodeGroup GetFarthest()
	{
		CharacterNodeGroup result;
		result = null;
		float num;
		num = 0f;
		Vector3 position;
		position = GameManager.Instance.Player.transform.position;
		if (base.Section != null && this.m_SectionCharacterNodeController != null)
		{
			CharacterNodeGroup[] array;
			array = this.m_SectionCharacterNodeController.Group;
			foreach (CharacterNodeGroup characterNodeGroup in array)
			{
				if (characterNodeGroup.Controller.IsActive)
				{
					float num2;
					num2 = Vector3.Distance(characterNodeGroup.Controller.transform.position, position);
					if (num2 > num)
					{
						result = characterNodeGroup;
						num = num2;
					}
				}
			}
		}
		return result;
	}

	private CharacterNodeGroup GetRandomCharacterNode(Vector3 startPosition)
	{
		CharacterNodeGroup characterNodeGroup;
		characterNodeGroup = null;
		if (base.Section != null && this.m_SectionCharacterNodeController != null)
		{
			characterNodeGroup = this.m_SectionCharacterNodeController.Group[global::UnityEngine.Random.Range(0, this.m_SectionCharacterNodeController.Group.Length)];
			NavMeshPath navMeshPath;
			navMeshPath = new NavMeshPath();
			if (NavMesh.CalculatePath(startPosition, characterNodeGroup.Controller.transform.position, -1, navMeshPath) && (navMeshPath.status == NavMeshPathStatus.PathInvalid || navMeshPath.status == NavMeshPathStatus.PathPartial || !characterNodeGroup.Controller.IsActive))
			{
				characterNodeGroup = this.GetRandomCharacterNode(startPosition);
			}
		}
		return characterNodeGroup;
	}

	private CharacterNodeGroup GetNodeGroup(int id)
	{
		CharacterNodeGroup result;
		result = null;
		if (base.Section != null && this.m_SectionCharacterNodeController != null)
		{
			for (int i = 0; i < this.m_SectionCharacterNodeController.Group.Length; i = unchecked(i + 1))
			{
				CharacterNodeGroup characterNodeGroup;
				characterNodeGroup = this.m_SectionCharacterNodeController.Group[i];
				if (characterNodeGroup.ID == id)
				{
					result = characterNodeGroup;
					break;
				}
			}
		}
		return result;
	}

	public void ResetTimer()
	{
		this.m_Timer = 0f;
		this.m_TimerLimit = global::UnityEngine.Random.Range(this.m_TimerLimitMin, this.m_TimerLimitMax);
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("SectionInkDemonController :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		this.CheckStatus();
	}

	private bool CheckStatus()
	{
		bool flag;
		flag = true;
		if (this.m_Requirements != null)
		{
			flag = this.m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			this.m_CanSpawn = true;
		}
		return flag;
	}

	private void ClearEnemies()
	{
		if (this.m_Enemies != null)
		{
			this.m_Enemies.Clear();
			this.m_Enemies = null;
		}
	}

	private void ClearLootableEnemies()
	{
		if (this.m_LootableEnemies != null)
		{
			this.m_LootableEnemies.Clear();
			this.m_LootableEnemies = null;
		}
	}

	protected override void RemoveListeners()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
	}

	public override void SaveData()
	{
		unchecked
		{
			for (int i = 0; i < this.m_LootableEnemies.Count; i++)
			{
				this.m_LootableEnemies[i].UpdateData();
			}
			for (int j = 0; j < this.m_Enemies.Count; j++)
			{
				this.m_Enemies[j].UpdateData();
			}
		}
	}

	protected override void OnDisposed()
	{
		base.StopAllCoroutines();
		this.SaveData();
		this.ClearEnemies();
		this.ClearLootableEnemies();
		this.m_SectionCharacterNodeController = null;
		base.OnDisposed();
	}
}
