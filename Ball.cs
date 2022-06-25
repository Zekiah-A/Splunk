using Godot;
using System;

public class Ball : RigidBody
{
	//How much the ball will move forward in a straight direction before it starts to curve.
	private const float Slide = 0.1f;
	//By how much the ball will start curving on the exponential.
	private const float CvFactor = 1.4f;
	//Either 1 (right), 0 (no curve) or -1 (left), decides whether the exponential function should be normal, flat, or inverted.
	public int CvDirection = 0;
	//Vector that the ball was thrown at.
	public Vector3 InitialVelocity = Vector3.Zero;
	
	public override void _IntegrateForces(PhysicsDirectBodyState state)
	{
		//y = ac(r)^x
		state.LinearVelocity = new Vector3(
			(float) (Slide * (CvDirection * Math.Pow(CvFactor, Math.Abs(Translation.z)))) + InitialVelocity.x,
			LinearVelocity.y,
			LinearVelocity.z
		);
	}
}
