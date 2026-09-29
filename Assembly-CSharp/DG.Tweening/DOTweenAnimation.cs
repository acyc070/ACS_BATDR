using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DG.Tweening;

[AddComponentMenu("DOTween/DOTween Animation")]
public class DOTweenAnimation : ABSAnimationComponent
{
	public enum AnimationType
	{
		None = 0,
		Move = 1,
		LocalMove = 2,
		Rotate = 3,
		LocalRotate = 4,
		Scale = 5,
		Color = 6,
		Fade = 7,
		Text = 8,
		PunchPosition = 9,
		PunchRotation = 10,
		PunchScale = 11,
		ShakePosition = 12,
		ShakeRotation = 13,
		ShakeScale = 14,
		CameraAspect = 15,
		CameraBackgroundColor = 16,
		CameraFieldOfView = 17,
		CameraOrthoSize = 18,
		CameraPixelRect = 19,
		CameraRect = 20,
		UIWidthHeight = 21
	}

	public enum TargetType
	{
		Unset = 0,
		Camera = 1,
		CanvasGroup = 2,
		Image = 3,
		Light = 4,
		RectTransform = 5,
		Renderer = 6,
		SpriteRenderer = 7,
		Rigidbody = 8,
		Rigidbody2D = 9,
		Text = 10,
		Transform = 11,
		tk2dBaseSprite = 12,
		tk2dTextMesh = 13,
		TextMeshPro = 14,
		TextMeshProUGUI = 15
	}

	public bool targetIsSelf = true;

	public GameObject targetGO;

	public bool tweenTargetIsTargetGO = true;

	public float delay;

	public float duration = 1f;

	public Ease easeType = Ease.OutQuad;

	public AnimationCurve easeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public LoopType loopType;

	public int loops = 1;

	public string id = "";

	public bool isRelative;

	public bool isFrom;

	public bool isIndependentUpdate;

	public bool autoKill = true;

	public bool autoGenerate = true;

	public bool isActive = true;

	public bool isValid;

	public Component target;

	public AnimationType animationType;

	public TargetType targetType;

	public TargetType forcedTargetType;

	public bool autoPlay = true;

	public bool useTargetAsV3;

	public float endValueFloat;

	public Vector3 endValueV3;

	public Vector2 endValueV2;

	public Color endValueColor = new Color(1f, 1f, 1f, 1f);

	public string endValueString = "";

	public Rect endValueRect = new Rect(0f, 0f, 0f, 0f);

	public Transform endValueTransform;

	public bool optionalBool0;

	public bool optionalBool1;

	public float optionalFloat0;

	public int optionalInt0;

	public RotateMode optionalRotationMode;

	public ScrambleMode optionalScrambleMode;

	public string optionalString;

	private bool _tweenAutoGenerationCalled;

	private int _playCount = -1;

	public static event Action<DOTweenAnimation> OnReset;

	private static void Dispatch_OnReset(DOTweenAnimation anim)
	{
		if (DOTweenAnimation.OnReset != null)
		{
			DOTweenAnimation.OnReset(anim);
		}
	}

	private void Awake()
	{
		if (this.isActive && this.autoGenerate && (this.animationType != AnimationType.Move || !this.useTargetAsV3))
		{
			this.CreateTween(regenerateIfExists: false, this.autoPlay);
			this._tweenAutoGenerationCalled = true;
		}
	}

	private void Start()
	{
		if (!this._tweenAutoGenerationCalled && this.isActive && this.autoGenerate)
		{
			this.CreateTween(regenerateIfExists: false, this.autoPlay);
			this._tweenAutoGenerationCalled = true;
		}
	}

	private void Reset()
	{
		DOTweenAnimation.Dispatch_OnReset(this);
	}

	private void OnDestroy()
	{
		if (base.tween != null && base.tween.active)
		{
			base.tween.Kill();
		}
		base.tween = null;
	}

	public void RewindThenRecreateTween()
	{
		if (base.tween != null && base.tween.active)
		{
			base.tween.Rewind();
		}
		this.CreateTween(regenerateIfExists: true, andPlay: false);
	}

	public void RewindThenRecreateTweenAndPlay()
	{
		if (base.tween != null && base.tween.active)
		{
			base.tween.Rewind();
		}
		this.CreateTween(regenerateIfExists: true);
	}

	public void RecreateTween()
	{
		this.CreateTween(regenerateIfExists: true, andPlay: false);
	}

	public void RecreateTweenAndPlay()
	{
		this.CreateTween(regenerateIfExists: true);
	}

	public void CreateTween(bool regenerateIfExists = false, bool andPlay = true)
	{
		if (!this.isValid)
		{
			if (regenerateIfExists)
			{
				Debug.LogWarning($"{base.gameObject.name} :: This DOTweenAnimation isn't valid and its tween won't be created", base.gameObject);
			}
			return;
		}
		if (base.tween != null)
		{
			if (base.tween.active)
			{
				if (!regenerateIfExists)
				{
					return;
				}
				base.tween.Kill();
			}
			base.tween = null;
		}
		GameObject tweenGO;
		tweenGO = this.GetTweenGO();
		if (this.target == null || tweenGO == null)
		{
			if (this.targetIsSelf && this.target == null)
			{
				Debug.LogWarning($"{base.gameObject.name} :: This DOTweenAnimation's target is NULL, because the animation was created with a DOTween Pro version older than 0.9.255. To fix this, exit Play mode then simply select this object, and it will update automatically", base.gameObject);
			}
			else
			{
				Debug.LogWarning($"{base.gameObject.name} :: This DOTweenAnimation's target/GameObject is unset: the tween will not be created.", base.gameObject);
			}
			return;
		}
		if (this.forcedTargetType != TargetType.Unset)
		{
			this.targetType = this.forcedTargetType;
		}
		if (this.targetType == TargetType.Unset)
		{
			this.targetType = DOTweenAnimation.TypeToDOTargetType(this.target.GetType());
		}
		switch (this.animationType)
		{
		case AnimationType.Move:
			if (this.useTargetAsV3)
			{
				this.isRelative = false;
				if (this.endValueTransform == null)
				{
					Debug.LogWarning($"{base.gameObject.name} :: This tween's TO target is NULL, a Vector3 of (0,0,0) will be used instead", base.gameObject);
					this.endValueV3 = Vector3.zero;
				}
				else if (this.targetType == TargetType.RectTransform)
				{
					RectTransform rectTransform;
					rectTransform = this.endValueTransform as RectTransform;
					if (rectTransform == null)
					{
						Debug.LogWarning($"{base.gameObject.name} :: This tween's TO target should be a RectTransform, a Vector3 of (0,0,0) will be used instead", base.gameObject);
						this.endValueV3 = Vector3.zero;
					}
					else
					{
						RectTransform rectTransform2;
						rectTransform2 = this.target as RectTransform;
						if (rectTransform2 == null)
						{
							Debug.LogWarning($"{base.gameObject.name} :: This tween's target and TO target are not of the same type. Please reassign the values", base.gameObject);
						}
						else
						{
							this.endValueV3 = DOTweenModuleUI.Utils.SwitchToRectTransform(rectTransform, rectTransform2);
						}
					}
				}
				else
				{
					this.endValueV3 = this.endValueTransform.position;
				}
			}
			switch (this.targetType)
			{
			case TargetType.Transform:
				base.tween = ((Transform)this.target).DOMove(this.endValueV3, this.duration, this.optionalBool0);
				break;
			case TargetType.RectTransform:
				base.tween = ((RectTransform)this.target).DOAnchorPos3D(this.endValueV3, this.duration, this.optionalBool0);
				break;
			case TargetType.Rigidbody:
				base.tween = ((Rigidbody)this.target).DOMove(this.endValueV3, this.duration, this.optionalBool0);
				break;
			case TargetType.Rigidbody2D:
				base.tween = ((Rigidbody2D)this.target).DOMove(this.endValueV3, this.duration, this.optionalBool0);
				break;
			}
			break;
		case AnimationType.LocalMove:
			base.tween = tweenGO.transform.DOLocalMove(this.endValueV3, this.duration, this.optionalBool0);
			break;
		case AnimationType.Rotate:
			switch (this.targetType)
			{
			case TargetType.Transform:
				base.tween = ((Transform)this.target).DORotate(this.endValueV3, this.duration, this.optionalRotationMode);
				break;
			case TargetType.Rigidbody:
				base.tween = ((Rigidbody)this.target).DORotate(this.endValueV3, this.duration, this.optionalRotationMode);
				break;
			case TargetType.Rigidbody2D:
				base.tween = ((Rigidbody2D)this.target).DORotate(this.endValueFloat, this.duration);
				break;
			}
			break;
		case AnimationType.LocalRotate:
			base.tween = tweenGO.transform.DOLocalRotate(this.endValueV3, this.duration, this.optionalRotationMode);
			break;
		case AnimationType.Scale:
			_ = this.targetType;
			base.tween = tweenGO.transform.DOScale(this.optionalBool0 ? new Vector3(this.endValueFloat, this.endValueFloat, this.endValueFloat) : this.endValueV3, this.duration);
			break;
		case AnimationType.UIWidthHeight:
			base.tween = ((RectTransform)this.target).DOSizeDelta(this.optionalBool0 ? new Vector2(this.endValueFloat, this.endValueFloat) : this.endValueV2, this.duration);
			break;
		case AnimationType.Color:
			this.isRelative = false;
			switch (this.targetType)
			{
			case TargetType.Renderer:
				base.tween = ((Renderer)this.target).material.DOColor(this.endValueColor, this.duration);
				break;
			case TargetType.Light:
				base.tween = ((Light)this.target).DOColor(this.endValueColor, this.duration);
				break;
			case TargetType.SpriteRenderer:
				base.tween = ((SpriteRenderer)this.target).DOColor(this.endValueColor, this.duration);
				break;
			case TargetType.Image:
				base.tween = ((Graphic)this.target).DOColor(this.endValueColor, this.duration);
				break;
			case TargetType.Text:
				base.tween = ((Text)this.target).DOColor(this.endValueColor, this.duration);
				break;
			case TargetType.TextMeshProUGUI:
				base.tween = ((TextMeshProUGUI)this.target).DOColor(this.endValueColor, this.duration);
				break;
			case TargetType.TextMeshPro:
				base.tween = ((TextMeshPro)this.target).DOColor(this.endValueColor, this.duration);
				break;
			}
			break;
		case AnimationType.Fade:
			this.isRelative = false;
			switch (this.targetType)
			{
			case TargetType.Renderer:
				base.tween = ((Renderer)this.target).material.DOFade(this.endValueFloat, this.duration);
				break;
			case TargetType.Light:
				base.tween = ((Light)this.target).DOIntensity(this.endValueFloat, this.duration);
				break;
			case TargetType.SpriteRenderer:
				base.tween = ((SpriteRenderer)this.target).DOFade(this.endValueFloat, this.duration);
				break;
			case TargetType.Image:
				base.tween = ((Graphic)this.target).DOFade(this.endValueFloat, this.duration);
				break;
			case TargetType.Text:
				base.tween = ((Text)this.target).DOFade(this.endValueFloat, this.duration);
				break;
			case TargetType.CanvasGroup:
				base.tween = ((CanvasGroup)this.target).DOFade(this.endValueFloat, this.duration);
				break;
			case TargetType.TextMeshProUGUI:
				base.tween = ((TextMeshProUGUI)this.target).DOFade(this.endValueFloat, this.duration);
				break;
			case TargetType.TextMeshPro:
				base.tween = ((TextMeshPro)this.target).DOFade(this.endValueFloat, this.duration);
				break;
			}
			break;
		case AnimationType.Text:
			if (this.targetType == TargetType.Text)
			{
				base.tween = ((Text)this.target).DOText(this.endValueString, this.duration, this.optionalBool0, this.optionalScrambleMode, this.optionalString);
			}
			switch (this.targetType)
			{
			case TargetType.TextMeshProUGUI:
				base.tween = ((TextMeshProUGUI)this.target).DOText(this.endValueString, this.duration, this.optionalBool0, this.optionalScrambleMode, this.optionalString);
				break;
			case TargetType.TextMeshPro:
				base.tween = ((TextMeshPro)this.target).DOText(this.endValueString, this.duration, this.optionalBool0, this.optionalScrambleMode, this.optionalString);
				break;
			}
			break;
		case AnimationType.PunchPosition:
			switch (this.targetType)
			{
			case TargetType.Transform:
				base.tween = ((Transform)this.target).DOPunchPosition(this.endValueV3, this.duration, this.optionalInt0, this.optionalFloat0, this.optionalBool0);
				break;
			case TargetType.RectTransform:
				base.tween = ((RectTransform)this.target).DOPunchAnchorPos(this.endValueV3, this.duration, this.optionalInt0, this.optionalFloat0, this.optionalBool0);
				break;
			}
			break;
		case AnimationType.PunchScale:
			base.tween = tweenGO.transform.DOPunchScale(this.endValueV3, this.duration, this.optionalInt0, this.optionalFloat0);
			break;
		case AnimationType.PunchRotation:
			base.tween = tweenGO.transform.DOPunchRotation(this.endValueV3, this.duration, this.optionalInt0, this.optionalFloat0);
			break;
		case AnimationType.ShakePosition:
			switch (this.targetType)
			{
			case TargetType.Transform:
				base.tween = ((Transform)this.target).DOShakePosition(this.duration, this.endValueV3, this.optionalInt0, this.optionalFloat0, this.optionalBool0, this.optionalBool1);
				break;
			case TargetType.RectTransform:
				base.tween = ((RectTransform)this.target).DOShakeAnchorPos(this.duration, this.endValueV3, this.optionalInt0, this.optionalFloat0, this.optionalBool0, this.optionalBool1);
				break;
			}
			break;
		case AnimationType.ShakeScale:
			base.tween = tweenGO.transform.DOShakeScale(this.duration, this.endValueV3, this.optionalInt0, this.optionalFloat0, this.optionalBool1);
			break;
		case AnimationType.ShakeRotation:
			base.tween = tweenGO.transform.DOShakeRotation(this.duration, this.endValueV3, this.optionalInt0, this.optionalFloat0, this.optionalBool1);
			break;
		case AnimationType.CameraAspect:
			base.tween = ((Camera)this.target).DOAspect(this.endValueFloat, this.duration);
			break;
		case AnimationType.CameraBackgroundColor:
			base.tween = ((Camera)this.target).DOColor(this.endValueColor, this.duration);
			break;
		case AnimationType.CameraFieldOfView:
			base.tween = ((Camera)this.target).DOFieldOfView(this.endValueFloat, this.duration);
			break;
		case AnimationType.CameraOrthoSize:
			base.tween = ((Camera)this.target).DOOrthoSize(this.endValueFloat, this.duration);
			break;
		case AnimationType.CameraPixelRect:
			base.tween = ((Camera)this.target).DOPixelRect(this.endValueRect, this.duration);
			break;
		case AnimationType.CameraRect:
			base.tween = ((Camera)this.target).DORect(this.endValueRect, this.duration);
			break;
		}
		if (base.tween == null)
		{
			return;
		}
		if (this.isFrom)
		{
			((Tweener)base.tween).From(this.isRelative);
		}
		else
		{
			base.tween.SetRelative(this.isRelative);
		}
		TweenSettingsExtensions.SetTarget(target: this.GetTweenTarget(), t: base.tween).SetDelay(this.delay).SetLoops(this.loops, this.loopType)
			.SetAutoKill(this.autoKill)
			.OnKill(() =>
			{
				base.tween = null;
			});
		if (base.isSpeedBased)
		{
			base.tween.SetSpeedBased();
		}
		if (this.easeType == Ease.INTERNAL_Custom)
		{
			base.tween.SetEase(this.easeCurve);
		}
		else
		{
			base.tween.SetEase(this.easeType);
		}
		if (!string.IsNullOrEmpty(this.id))
		{
			base.tween.SetId(this.id);
		}
		base.tween.SetUpdate(this.isIndependentUpdate);
		if (base.hasOnStart)
		{
			if (base.onStart != null)
			{
				base.tween.OnStart(base.onStart.Invoke);
			}
		}
		else
		{
			base.onStart = null;
		}
		if (base.hasOnPlay)
		{
			if (base.onPlay != null)
			{
				base.tween.OnPlay(base.onPlay.Invoke);
			}
		}
		else
		{
			base.onPlay = null;
		}
		if (base.hasOnUpdate)
		{
			if (base.onUpdate != null)
			{
				base.tween.OnUpdate(base.onUpdate.Invoke);
			}
		}
		else
		{
			base.onUpdate = null;
		}
		if (base.hasOnStepComplete)
		{
			if (base.onStepComplete != null)
			{
				base.tween.OnStepComplete(base.onStepComplete.Invoke);
			}
		}
		else
		{
			base.onStepComplete = null;
		}
		if (base.hasOnComplete)
		{
			if (base.onComplete != null)
			{
				base.tween.OnComplete(base.onComplete.Invoke);
			}
		}
		else
		{
			base.onComplete = null;
		}
		if (base.hasOnRewind)
		{
			if (base.onRewind != null)
			{
				base.tween.OnRewind(base.onRewind.Invoke);
			}
		}
		else
		{
			base.onRewind = null;
		}
		if (andPlay)
		{
			base.tween.Play();
		}
		else
		{
			base.tween.Pause();
		}
		if (base.hasOnTweenCreated && base.onTweenCreated != null)
		{
			base.onTweenCreated.Invoke();
		}
	}

	public List<Tween> GetTweens()
	{
		List<Tween> list;
		list = new List<Tween>();
		DOTweenAnimation[] components;
		components = base.GetComponents<DOTweenAnimation>();
		foreach (DOTweenAnimation dOTweenAnimation in components)
		{
			if (dOTweenAnimation.tween != null && dOTweenAnimation.tween.active)
			{
				list.Add(dOTweenAnimation.tween);
			}
		}
		return list;
	}

	public void SetAnimationTarget(Component tweenTarget, bool useTweenTargetGameObjectForGroupOperations = true)
	{
		if (DOTweenAnimation.TypeToDOTargetType(this.target.GetType()) != this.targetType)
		{
			Debug.LogError("DOTweenAnimation ► SetAnimationTarget: the new target is of a different type from the one set in the Inspector");
			return;
		}
		this.target = tweenTarget;
		this.targetGO = this.target.gameObject;
		this.tweenTargetIsTargetGO = useTweenTargetGameObjectForGroupOperations;
	}

	public override void DOPlay()
	{
		DOTween.Play(this.GetTweenTarget());
	}

	public override void DOPlayBackwards()
	{
		DOTween.PlayBackwards(this.GetTweenTarget());
	}

	public override void DOPlayForward()
	{
		DOTween.PlayForward(this.GetTweenTarget());
	}

	public override void DOPause()
	{
		DOTween.Pause(this.GetTweenTarget());
	}

	public override void DOTogglePause()
	{
		DOTween.TogglePause(this.GetTweenTarget());
	}

	public override void DORewind()
	{
		this._playCount = -1;
		DOTweenAnimation[] components;
		components = base.gameObject.GetComponents<DOTweenAnimation>();
		unchecked
		{
			for (int num = components.Length - 1; num > -1; num--)
			{
				Tween tween;
				tween = components[num].tween;
				if (tween != null && tween.IsInitialized())
				{
					components[num].tween.Rewind();
				}
			}
		}
	}

	public override void DORestart()
	{
		this.DORestart(fromHere: false);
	}

	public override void DORestart(bool fromHere)
	{
		this._playCount = -1;
		if (base.tween == null)
		{
			if (Debugger.logPriority > 1)
			{
				Debugger.LogNullTween(base.tween);
			}
			return;
		}
		if (fromHere && this.isRelative)
		{
			this.ReEvaluateRelativeTween();
		}
		DOTween.Restart(this.GetTweenTarget());
	}

	public override void DOComplete()
	{
		DOTween.Complete(this.GetTweenTarget());
	}

	public override void DOKill()
	{
		DOTween.Kill(this.GetTweenTarget());
		base.tween = null;
	}

	public void DOPlayById(string id)
	{
		DOTween.Play(this.GetTweenTarget(), id);
	}

	public void DOPlayAllById(string id)
	{
		DOTween.Play(id);
	}

	public void DOPauseAllById(string id)
	{
		DOTween.Pause(id);
	}

	public void DOPlayBackwardsById(string id)
	{
		DOTween.PlayBackwards(this.GetTweenTarget(), id);
	}

	public void DOPlayBackwardsAllById(string id)
	{
		DOTween.PlayBackwards(id);
	}

	public void DOPlayForwardById(string id)
	{
		DOTween.PlayForward(this.GetTweenTarget(), id);
	}

	public void DOPlayForwardAllById(string id)
	{
		DOTween.PlayForward(id);
	}

	public void DOPlayNext()
	{
		DOTweenAnimation[] components;
		components = base.GetComponents<DOTweenAnimation>();
		unchecked
		{
			while (this._playCount < components.Length - 1)
			{
				this._playCount++;
				DOTweenAnimation dOTweenAnimation;
				dOTweenAnimation = components[this._playCount];
				if (dOTweenAnimation != null && dOTweenAnimation.tween != null && dOTweenAnimation.tween.active && !dOTweenAnimation.tween.IsPlaying() && !dOTweenAnimation.tween.IsComplete())
				{
					dOTweenAnimation.tween.Play();
					break;
				}
			}
		}
	}

	public void DORewindAndPlayNext()
	{
		this._playCount = -1;
		DOTween.Rewind(this.GetTweenTarget());
		this.DOPlayNext();
	}

	public void DORewindAllById(string id)
	{
		this._playCount = -1;
		DOTween.Rewind(id);
	}

	public void DORestartById(string id)
	{
		this._playCount = -1;
		DOTween.Restart(this.GetTweenTarget(), id);
	}

	public void DORestartAllById(string id)
	{
		this._playCount = -1;
		DOTween.Restart(id);
	}

	public void DOKillById(string id)
	{
		DOTween.Kill(this.GetTweenTarget(), id);
	}

	public void DOKillAllById(string id)
	{
		DOTween.Kill(id);
	}

	public static TargetType TypeToDOTargetType(Type t)
	{
		string text;
		text = t.ToString();
		int num;
		num = text.LastIndexOf(".");
		if (num != -1)
		{
			text = text.Substring(unchecked(num + 1));
		}
		if (text.IndexOf("Renderer") != -1 && text != "SpriteRenderer")
		{
			text = "Renderer";
		}
		if (text == "RawImage" || text == "Graphic")
		{
			text = "Image";
		}
		return (TargetType)Enum.Parse(typeof(TargetType), text);
	}

	public Tween CreateEditorPreview()
	{
		if (Application.isPlaying)
		{
			return null;
		}
		this.CreateTween(regenerateIfExists: true, this.autoPlay);
		return base.tween;
	}

	private GameObject GetTweenGO()
	{
		if (!this.targetIsSelf)
		{
			return this.targetGO;
		}
		return base.gameObject;
	}

	private GameObject GetTweenTarget()
	{
		if (!this.targetIsSelf && this.tweenTargetIsTargetGO)
		{
			return this.targetGO;
		}
		return base.gameObject;
	}

	private void ReEvaluateRelativeTween()
	{
		GameObject tweenGO;
		tweenGO = this.GetTweenGO();
		if (tweenGO == null)
		{
			Debug.LogWarning($"{base.gameObject.name} :: This DOTweenAnimation's target/GameObject is unset: the tween will not be created.", base.gameObject);
		}
		else if (this.animationType == AnimationType.Move)
		{
			((Tweener)base.tween).ChangeEndValue(tweenGO.transform.position + this.endValueV3, snapStartValue: true);
		}
		else if (this.animationType == AnimationType.LocalMove)
		{
			((Tweener)base.tween).ChangeEndValue(tweenGO.transform.localPosition + this.endValueV3, snapStartValue: true);
		}
	}
}
