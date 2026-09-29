using UnityEngine;

public static class RigidbodyDataExtensions
{
	public static void SetData<T>(this T rigidbody, RigidbodyData rigidbodyData) where T : Rigidbody
	{
		rigidbody.velocity = rigidbodyData.Velocity;
		rigidbody.angularVelocity = rigidbodyData.AngularVelocity;
		rigidbody.isKinematic = rigidbodyData.IsKinematic;
		rigidbody.useGravity = rigidbodyData.UseGravity;
	}
}
