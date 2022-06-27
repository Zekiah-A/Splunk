using Godot;
using System;
using System.Drawing;

public class Ball : RigidBody
{
	//{50 < k < 50}.
	public float CvDir = 0;
	//Vector that the ball was thrown at.
	public Vector3 InitialVelocity = Vector3.Zero;
	private const float C = 1.8f;
	private const float D = 1.1f;
	
	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
		// j = CvDir {-50 < k < 50}; C = 1.8; D = 1.1;
		// (1/10000)j(c^(dx)){0 < x < 10}
		state.LinearVelocity = new Vector3(
			(float) (1e-4 * (CvDir * Math.Pow(C, D * Mathf.Abs(Translation.z))) + InitialVelocity.x),
			LinearVelocity.y,
			LinearVelocity.z
		);
	}
}
