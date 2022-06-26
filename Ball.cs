using Godot;
using System;

public class Ball : RigidBody
{
	//How much the ball will move forward in a straight direction before it starts to curve.
	private const float Slide = 0.1f;
	//By how much the ball will start curving on the exponential.
	private const float CvFactor = 1.4f;
	//Fro, 1 (right), 0 (no curve) to -1 (left), decides whether the exponential function should be normal, flat, or inverted.
	public float CvDirection = 0;
	//Vector that the ball was thrown at.
	public Vector3 InitialVelocity = Vector3.Zero;
	
	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
		// j = 0 {-50 < k < 50}; c = 1.9; d = 0.9;
		// (1/10000)j(c^(dx)){0 < x < 10}
		float j = 50, c = 1.9f, d = 0.9f;
		state.LinearVelocity = new Vector3(
			(float) (1e-4 * (j * Math.Pow(c, d * Mathf.Abs(Translation.z))) + InitialVelocity.x),
			LinearVelocity.y,
			LinearVelocity.z
		);
	}
}
