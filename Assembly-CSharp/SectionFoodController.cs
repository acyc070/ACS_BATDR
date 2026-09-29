using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionFoodController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private FoodGroup[] m_Group;

	public SectionID SectionID => this.m_SectionID;

	public FoodGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			FoodGroup foodGroup;
			foodGroup = this.m_Group[i];
			if (foodGroup != null)
			{
				foodGroup.Controller.OnActivate -= this.HandleControllerOnActivate;
				if ((FoodDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, FoodDataObject>(this.m_SectionID, foodGroup.ID) != null)
				{
					foodGroup.IsComplete = true;
					foodGroup.Controller.Content.ForceActivateComplete();
				}
				else
				{
					(foodGroup.Controller.Content as FoodContent).InitializeContent();
					foodGroup.Controller.OnActivate += this.HandleControllerOnActivate;
				}
			}
			yield return null;
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		Food food;
		food = sender as Food;
		food.OnActivate -= this.HandleControllerOnActivate;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			FoodGroup foodGroup;
			foodGroup = this.m_Group[i];
			if (foodGroup.Controller == food)
			{
				if ((FoodDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, FoodDataObject>(this.m_SectionID, foodGroup.ID) == null)
				{
					FoodDataObject value;
					value = DataObject<int, FoodDataObject>.Create(foodGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, value);
				}
				foodGroup.IsComplete = true;
				break;
			}
		}
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			this.m_Group[i].Controller.OnActivate -= this.HandleControllerOnActivate;
		}
		base.OnDisposed();
	}
}
