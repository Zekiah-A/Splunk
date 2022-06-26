using Godot;
using System;

public class Ball : RigidBody
{
	//{50 < k < 50}.
	public float CvDir = 0;
	//Vector that the ball was thrown at.
	public Vector3 InitialVelocity = Vector3.Zero;
	
	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
		// j = CvDir {-50 < k < 50}; c = 1.9; d = 0.9;
		// (1/10000)j(c^(dx)){0 < x < 10}
		float c = 1.9f, d = 0.9f;
		state.LinearVelocity = new Vector3(
			(float) (1e-4 * (CvDir * Math.Pow(c, d * Mathf.Abs(Translation.z))) + InitialVelocity.x),
			LinearVelocity.y,
			LinearVelocity.z
		);
	}
}
