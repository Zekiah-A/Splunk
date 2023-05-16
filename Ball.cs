using Godot;
using System;
using System.Drawing;

public partial class Ball : RigidBody3D
{
	//{50 < k < 50}.
	public float CvDir = 0;
	public bool PathInterrupted = false;
	public Vector3 InitialVelocity = Vector3.Zero; //Vector that the ball was thrown at.
	
	private const float C = 1.8f;
	private const float D = 1.1f;
	
	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		//For example, if the ball collided with a wall, we cancel out the spin or else weirdness may occur
		if (PathInterrupted) return;
		// j = CvDir {-50 < k < 50}; C = 1.8; D = 1.1;
		// (1/10000)j(c^(dx)){0 < x < 10}
		state.LinearVelocity = new Vector3(
			(float) (1e-4 * (CvDir * Math.Pow(C, D * Mathf.Abs(Position.z))) + InitialVelocity.x),
			LinearVelocity.y,
			LinearVelocity.z
		);
	}
	
	private void OnBodyShapeCollision(RID body_rid, object body, int body_shape_index, int local_shape_index)
	{
		if (body is StaticBody3D staticBody && staticBody.IsInGroup("gutter_barriers"))
			PathInterrupted = true;
	}
}


