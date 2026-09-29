using DG.Tweening;
using UnityEngine;

public class UINavigation : UIController
{
	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private RectTransform m_Direction;

	private Transform m_Target;

	protected override void OnInitialized(object _data)
	{
		base.m_Visuals.gameObject.SetActive(value: false);
		this.m_CanvasGroup.alpha = 0f;
	}

	private void Update()
	{
		if (!base.IsDisposed && !(GameManager.Instance.Player == null))
		{
			this.UpdateNavigation();
		}
	}

	private void UpdateNavigation()
	{
		if (this.m_Target == null)
		{
			if (base.m_Visuals.gameObject.activeSelf)
			{
				base.m_Visuals.gameObject.SetActive(value: false);
			}
			return;
		}
		Transform transform;
		transform = GameManager.Instance.GameCamera.Camera.transform;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(screenPoint: RectTransformUtility.WorldToScreenPoint(GameManager.Instance.GameCamera.Camera, this.m_Target.position), rect: base.rectTransform, cam: GameManager.Instance.UIManager.Camera, localPoint: out var localPoint))
		{
			base.m_Visuals.anchoredPosition = localPoint;
			Vector2 anchoredPosition;
			anchoredPosition = base.m_Visuals.anchoredPosition;
			float num;
			num = -1820f;
			float num2;
			num2 = 1820f;
			float num3;
			num3 = -980f;
			float num4;
			num4 = 980f;
			float x;
			x = anchoredPosition.x;
			float num5;
			num5 = Mathf.Clamp(x, num, num2);
			float y;
			y = anchoredPosition.y;
			anchoredPosition = new Vector2(num5, Mathf.Clamp(y, num3, num4));
			float target;
			target = 0.1f;
			bool flag;
			flag = x < num2 && x > num && y < num4 && y > num3;
			if (Vector3.Dot((this.m_Target.position - transform.position).normalized, transform.forward) <= 0f)
			{
				anchoredPosition = new Vector3(0f - num5, num3);
				flag = false;
			}
			else if (!Physics.Linecast(transform.position, this.m_Target.position, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore))
			{
				target = ((Vector3.Distance(transform.position, this.m_Target.position) < 5f) ? 0.1f : 0.9f);
			}
			float currentVelocity;
			currentVelocity = 0f;
			this.m_CanvasGroup.alpha = Mathf.SmoothDamp(this.m_CanvasGroup.alpha, target, ref currentVelocity, Time.deltaTime * 4f);
			base.m_Visuals.anchoredPosition = anchoredPosition;
			Vector3 vector;
			vector = base.m_Visuals.anchoredPosition - base.rectTransform.anchoredPosition;
			float angle;
			angle = Mathf.Atan2(vector.y, vector.x) * 57.29578f + 90f;
			Quaternion b;
			b = (flag ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.AngleAxis(angle, Vector3.forward));
			this.m_Direction.rotation = Quaternion.Slerp(this.m_Direction.rotation, b, Time.deltaTime * 4f);
			if (!base.m_Visuals.gameObject.activeSelf)
			{
				this.m_CanvasGroup.alpha = 0f;
				base.m_Visuals.gameObject.SetActive(value: true);
			}
		}
	}

	public void SetTarget(Transform target)
	{
		if (target != null)
		{
			this.m_CanvasGroup.DOKill();
			this.m_Target = target;
		}
		else
		{
			this.Clear();
		}
	}

	public void Clear()
	{
		this.m_CanvasGroup.DOKill();
		this.m_CanvasGroup.DOFade(0f, 0.25f).SetEase(Ease.Linear).OnComplete(this.PlayOut);
	}

	protected override void OnDisposed()
	{
		this.m_CanvasGroup.DOKill();
		this.m_Target = null;
		base.OnDisposed();
	}
}
