using UnityEngine;

public class MovablePath : JMonoBehaviour
{
	[SerializeField]
	private MovableLocation[] m_Path;

	[SerializeField]
	private MovableLocation m_StartLocation;

	public MovableLocation[] Path => this.m_Path;

	public MovableLocation StartLocation => this.m_StartLocation;

	public int GetStartLocation()
	{
		int result;
		result = 0;
		for (int i = 0; i < this.m_Path.Length; i = unchecked(i + 1))
		{
			if (this.m_Path[i] == this.m_StartLocation)
			{
				result = i;
				break;
			}
		}
		return result;
	}
}
