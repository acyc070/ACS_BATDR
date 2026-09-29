using DG.Tweening;
using S13Audio.BATDR;
using UnityEngine;

public class CharacterStateAlert : CharacterState
{
	public CharacterStateAlert(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		base.Actor.ForceStop();
		base.Actor.SetMoveSpeed(0f);
		base.Actor.SetMoveXSpeed(0f);
		base.Actor.Agent.Agent.updatePosition = true;
		base.Actor.Agent.Agent.updateRotation = true;
		base.Actor.Agent.Agent.velocity = Vector3.zero;
		base.Actor.IsAlerted = true;
		base.Actor.SetNextState(State.Character.Follow);
		base.Actor.Content.Alert();
		GameManager.Instance.Player.AddEnemy(base.Actor);
		Enemy component;
		component = base.Actor.GetComponent<Enemy>();
		if (component != null && component.EnemyType == EnemyType.Keeper)
		{
			Vector3 eulerAngles;
			eulerAngles = Quaternion.LookRotation((base.Actor.Target.position - base.Actor.transform.position).normalized).eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			component.transform.DOKill();
			component.transform.DORotate(eulerAngles, 0.25f).SetEase(Ease.InOutSine);
		}
		base.Actor.AudioController?.React(BATDRNpcAudioController.NPCReaction.Alert);
	}

	protected override void InternalUpdate()
	{
		if (!(GameManager.Instance.Player == null) && GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			base.Actor.SetState(State.Character.Flee);
		}
	}
}
