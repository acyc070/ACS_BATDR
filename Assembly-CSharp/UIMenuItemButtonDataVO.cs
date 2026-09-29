using System;

public class UIMenuItemButtonDataVO : JDisposable
{
	public string Label { get; private set; }

	public Action Callback { get; private set; }

	public static UIMenuItemButtonDataVO Create(string label, Action callback)
	{
		return new UIMenuItemButtonDataVO
		{
			Label = label,
			Callback = callback
		};
	}

	public void UpdateLabel(string label)
	{
		this.Label = label;
	}

	public void UpdateCallback(Action callback)
	{
		this.Callback = callback;
	}

	protected override void OnDisposed()
	{
		this.Callback = null;
		base.OnDisposed();
	}
}
