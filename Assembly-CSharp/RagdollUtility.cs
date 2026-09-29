using UnityEngine;

public static class RagdollUtility
{
	public static RagdollBody Generate(Transform boneRoot)
	{
		RagdollBody ragdollBody;
		ragdollBody = new RagdollBody();
		Transform[] componentsInChildren;
		componentsInChildren = boneRoot.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform == boneRoot)
			{
				continue;
			}
			if (transform.name.Contains("Pelvis"))
			{
				ragdollBody.Pelvis = transform;
			}
			else if (transform.name.Contains("BN_L") || transform.name.Contains("CTRL_L"))
			{
				if (transform.name.Contains("Thigh") || transform.name.Contains("Leg"))
				{
					ragdollBody.LeftHips = transform;
				}
				else if (transform.name.Contains("Knee") || transform.name.Contains("Shin"))
				{
					ragdollBody.LeftKnee = transform;
				}
				else if (transform.name.Contains("Foot") || transform.name.Contains("Feet"))
				{
					ragdollBody.LeftFoot = transform;
				}
				else if (transform.name.Contains("Clavicle"))
				{
					ragdollBody.LeftArm = transform;
				}
				else if (transform.name.Contains("Elbow") || transform.name.Contains("ForeArm"))
				{
					ragdollBody.LeftElbow = transform;
				}
				else
				{
					if (!transform.name.Contains("Hand") && !transform.name.Contains("Wrist"))
					{
						continue;
					}
					ragdollBody.LeftHand = transform;
				}
			}
			else if (transform.name.Contains("BN_R") || transform.name.Contains("CTRL_R"))
			{
				if (transform.name.Contains("Thigh") || transform.name.Contains("Leg"))
				{
					ragdollBody.RightHips = transform;
				}
				else if (transform.name.Contains("Knee") || transform.name.Contains("Shin"))
				{
					ragdollBody.RightKnee = transform;
				}
				else if (transform.name.Contains("Foot") || transform.name.Contains("Feet"))
				{
					ragdollBody.RightFoot = transform;
				}
				else if (transform.name.Contains("Clavicle"))
				{
					ragdollBody.RightArm = transform;
				}
				else if (transform.name.Contains("Elbow") || transform.name.Contains("ForeArm"))
				{
					ragdollBody.RightElbow = transform;
				}
				else
				{
					if (!transform.name.Contains("Hand") && !transform.name.Contains("Wrist"))
					{
						continue;
					}
					ragdollBody.RightHand = transform;
				}
			}
			else if (transform.name.Contains("BN_Spine"))
			{
				if (!transform.name.Contains("3") && !transform.name.Contains("_C"))
				{
					continue;
				}
				ragdollBody.Spine = transform;
			}
			else
			{
				if (!transform.name.Contains("BN_Head"))
				{
					continue;
				}
				ragdollBody.Head = transform;
			}
			transform.gameObject.AddComponent<BoxCollider>().size = Vector3.one * 0.2f;
			transform.gameObject.AddComponent<Rigidbody>();
		}
		CharacterJoint characterJoint;
		characterJoint = ragdollBody.LeftHips.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint2;
		characterJoint2 = ragdollBody.LeftKnee.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint3;
		characterJoint3 = ragdollBody.LeftFoot.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint4;
		characterJoint4 = ragdollBody.RightHips.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint5;
		characterJoint5 = ragdollBody.RightKnee.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint6;
		characterJoint6 = ragdollBody.RightFoot.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint7;
		characterJoint7 = ragdollBody.LeftArm.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint8;
		characterJoint8 = ragdollBody.LeftElbow.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint9;
		characterJoint9 = ragdollBody.LeftHand.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint10;
		characterJoint10 = ragdollBody.RightArm.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint11;
		characterJoint11 = ragdollBody.RightElbow.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint12;
		characterJoint12 = ragdollBody.RightHand.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint13;
		characterJoint13 = ragdollBody.Spine.gameObject.AddComponent<CharacterJoint>();
		CharacterJoint characterJoint14;
		characterJoint14 = ragdollBody.Head.gameObject.AddComponent<CharacterJoint>();
		characterJoint.connectedBody = ragdollBody.Pelvis.GetComponent<Rigidbody>();
		characterJoint2.connectedBody = characterJoint.GetComponent<Rigidbody>();
		characterJoint3.connectedBody = characterJoint2.GetComponent<Rigidbody>();
		characterJoint4.connectedBody = ragdollBody.Pelvis.GetComponent<Rigidbody>();
		characterJoint5.connectedBody = characterJoint4.GetComponent<Rigidbody>();
		characterJoint6.connectedBody = characterJoint5.GetComponent<Rigidbody>();
		characterJoint7.connectedBody = characterJoint13.GetComponent<Rigidbody>();
		characterJoint8.connectedBody = characterJoint7.GetComponent<Rigidbody>();
		characterJoint9.connectedBody = characterJoint8.GetComponent<Rigidbody>();
		characterJoint10.connectedBody = characterJoint13.GetComponent<Rigidbody>();
		characterJoint11.connectedBody = characterJoint10.GetComponent<Rigidbody>();
		characterJoint12.connectedBody = characterJoint11.GetComponent<Rigidbody>();
		characterJoint13.connectedBody = ragdollBody.Pelvis.GetComponent<Rigidbody>();
		characterJoint14.connectedBody = characterJoint13.GetComponent<Rigidbody>();
		return ragdollBody;
	}
}
