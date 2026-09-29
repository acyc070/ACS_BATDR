public static class PlayerInputConstants
{
	public static string INTERACT;

	public static string CROUCH;

	public static string RUN;

	public static string CANCEL;

	static PlayerInputConstants()
	{
		PlayerInputConstants.INTERACT = "E";
		PlayerInputConstants.CROUCH = "C";
		PlayerInputConstants.RUN = "LEFT SHIFT";
		PlayerInputConstants.CANCEL = "F";
	}
}
