using UnityEngine;

public static class AnimationExtensions
{
	public static bool Contains(this Animator animator, string name)
	{
		AnimatorControllerParameter[] parameters;
		parameters = animator.parameters;
		for (int i = 0; i < parameters.Length; i = unchecked(i + 1))
		{
			if (parameters[i].name == name)
			{
				return true;
			}
		}
		return false;
	}

	private static void Debug(Animator animator, string name)
	{
	}

	public static void SetMovementState(this Animator animator, float value, bool smooth = true, float dampTime = 0.1f)
	{
		AnimationExtensions.SetFloat(ref animator, "MovementState", value, smooth);
	}

	public static void SetMovementSpeed(this Animator animator, float value, bool smooth = true, float dampTime = 0.1f)
	{
		AnimationExtensions.SetFloat(ref animator, "MovementSpeed", value, smooth);
	}

	public static void SetShimmySpeed(this Animator animator, float value, bool smooth = true, float dampTime = 0.1f)
	{
		AnimationExtensions.SetFloat(ref animator, "ShimmySpeed", value, smooth);
	}

	public static void SetStrafeSpeed(this Animator animator, float value, bool smooth = true, float dampTime = 0.1f)
	{
		AnimationExtensions.SetFloat(ref animator, "StrafeSpeed", value, smooth);
	}

	public static void SetCrouchState(this Animator animator, float value, bool smooth = true, float dampTime = 0.1f)
	{
		AnimationExtensions.SetFloat(ref animator, "CrouchState", value, smooth);
	}

	public static void SetCrouchSpeed(this Animator animator, float value, bool smooth = true, float dampTime = 0.1f)
	{
		AnimationExtensions.SetFloat(ref animator, "CrouchSpeed", value, smooth);
	}

	public static void SetCombatState(this Animator animator, float value, bool smooth = true, float dampTime = 0.1f)
	{
		AnimationExtensions.SetFloat(ref animator, "CombatState", value, smooth);
	}

	public static void SetAttackType(this Animator animator, float value)
	{
		AnimationExtensions.SetFloat(ref animator, "AttackType", value, smooth: false);
	}

	private static void SetFloat(ref Animator animator, string name, float value, bool smooth = true, float dampTime = 0.1f)
	{
		if (animator.Contains(name))
		{
			if (smooth)
			{
				float num;
				num = animator.GetFloat(name);
				if (num > value + 0.01f || num < value - 0.01f)
				{
					animator.SetFloat(name, value, dampTime, Time.deltaTime);
				}
				else
				{
					animator.SetFloat(name, value);
				}
			}
			else
			{
				animator.SetFloat(name, value);
			}
		}
		else
		{
			AnimationExtensions.Debug(animator, name);
		}
	}

	public static void Jump(this Animator animator)
	{
		AnimationExtensions.SetTrigger(animator, "Jump");
	}

	public static void Attack(this Animator animator)
	{
		AnimationExtensions.SetTrigger(animator, "Attack");
	}

	public static void Attack(this Animator animator, float value)
	{
		AnimationExtensions.SetFloat(ref animator, "AttackType", value, smooth: false);
		animator.Attack();
	}

	public static void AttackLunge(this Animator animator)
	{
		AnimationExtensions.SetTrigger(animator, "AttackLeap");
	}

	public static void AttackLunge(this Animator animator, float value)
	{
		AnimationExtensions.SetFloat(ref animator, "AttackType", value, smooth: false);
		animator.AttackLunge();
	}

	public static void AttackRanged(this Animator animator)
	{
		AnimationExtensions.SetTrigger(animator, "AttackRanged");
	}

	public static void AttackRanged(this Animator animator, float value)
	{
		AnimationExtensions.SetFloat(ref animator, "AttackType", value, smooth: false);
		animator.AttackRanged();
	}

	public static void Hit(this Animator animator)
	{
		AnimationExtensions.SetTrigger(animator, "Hit");
	}

	public static void Hit(this Animator animator, float value)
	{
		AnimationExtensions.SetFloat(ref animator, "HitType", value, smooth: false);
		animator.Hit();
	}

	public static void Death(this Animator animator)
	{
		animator.SetTrigger("Death");
	}

	public static void Death(this Animator animator, float value)
	{
		animator.Death();
	}

	private static void SetTrigger(Animator animator, string name)
	{
		if (animator.Contains(name))
		{
			animator.SetTrigger(name);
		}
		else
		{
			AnimationExtensions.Debug(animator, name);
		}
	}
}
