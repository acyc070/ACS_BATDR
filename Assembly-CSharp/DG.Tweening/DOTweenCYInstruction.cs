using UnityEngine;

namespace DG.Tweening;

public static class DOTweenCYInstruction
{
	public class WaitForCompletion : CustomYieldInstruction
	{
		private readonly Tween t;

		public override bool keepWaiting
		{
			get
			{
				if (this.t.active)
				{
					return !this.t.IsComplete();
				}
				return false;
			}
		}

		public WaitForCompletion(Tween tween)
		{
			this.t = tween;
		}
	}

	public class WaitForRewind : CustomYieldInstruction
	{
		private readonly Tween t;

		public override bool keepWaiting
		{
			get
			{
				if (this.t.active)
				{
					if (this.t.playedOnce)
					{
						return this.t.position * (float)unchecked(this.t.CompletedLoops() + 1) > 0f;
					}
					return true;
				}
				return false;
			}
		}

		public WaitForRewind(Tween tween)
		{
			this.t = tween;
		}
	}

	public class WaitForKill : CustomYieldInstruction
	{
		private readonly Tween t;

		public override bool keepWaiting => this.t.active;

		public WaitForKill(Tween tween)
		{
			this.t = tween;
		}
	}

	public class WaitForElapsedLoops : CustomYieldInstruction
	{
		private readonly Tween t;

		private readonly int elapsedLoops;

		public override bool keepWaiting
		{
			get
			{
				if (this.t.active)
				{
					return this.t.CompletedLoops() < this.elapsedLoops;
				}
				return false;
			}
		}

		public WaitForElapsedLoops(Tween tween, int elapsedLoops)
		{
			this.t = tween;
			this.elapsedLoops = elapsedLoops;
		}
	}

	public class WaitForPosition : CustomYieldInstruction
	{
		private readonly Tween t;

		private readonly float position;

		public override bool keepWaiting
		{
			get
			{
				if (this.t.active)
				{
					return this.t.position * (float)unchecked(this.t.CompletedLoops() + 1) < this.position;
				}
				return false;
			}
		}

		public WaitForPosition(Tween tween, float position)
		{
			this.t = tween;
			this.position = position;
		}
	}

	public class WaitForStart : CustomYieldInstruction
	{
		private readonly Tween t;

		public override bool keepWaiting
		{
			get
			{
				if (this.t.active)
				{
					return !this.t.playedOnce;
				}
				return false;
			}
		}

		public WaitForStart(Tween tween)
		{
			this.t = tween;
		}
	}
}
