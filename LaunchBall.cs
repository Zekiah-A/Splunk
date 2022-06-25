using Godot;
using System;
using Array = Godot.Collections.Array;

public class LaunchBall : Camera
{
	private RigidBody ball;
	private ImmediateGeometry aimerGeometry;
	private float curveValue = 50;
	private const int VelocityFwd = 3;
	private const int VelocityUp = 2;

	public override void _Ready()
	{
		aimerGeometry = GetTree().Root.GetChild(0).GetNode<ImmediateGeometry>("AimerGeometry");
		MakeCurrent();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (!mouseButton.Pressed)
			{
				//Projection from the bottom centre of the screen (where the ball is thrown from)
				var throwRoot = ProjectPosition(new Vector2(GetViewport().GetVisibleRect().Size.x / 2, GetViewport().GetVisibleRect().Size.y), 2);
				var ballScene = GD.Load<PackedScene>("res://Ball.tscn");
				ball = (RigidBody) ballScene.Instance();
				ball.Translation = throwRoot;
				var flick = new Vector2(GetViewport().GetVisibleRect().Size.x / 2 -  mouseButton.Position.x, GetViewport().GetVisibleRect().Size.y - mouseButton.Position.y).Normalized(); //"Flicks" from bottom centre screen to end
				// the x value is the inverse of how you flicked across
				//the y value gets how high the flick was in relation to the screen's height, and then multiplies by a constant, to determine throw height
				//The z value basically judges how far up you dragged your mouse to flick, to determine with how much power it is thrown, it is clamped so that it can not be negative
				((Ball) ball).InitialVelocity = ball.LinearVelocity = new Vector3(
					-flick.x,
					Mathf.Abs(GetViewport().GetVisibleRect().Size.y - mouseButton.Position.y) / GetViewport().GetVisibleRect().Size.y * VelocityUp,
					throwRoot.z - Mathf.Clamp(flick.y, 0, 1) * VelocityFwd
				);
				AddChild(ball);
			}
		}

		if (@event is InputEventMouseMotion mouseMotion)
		{
			aimerGeometry.Clear();
			aimerGeometry.Begin(Mesh.PrimitiveType.Triangles);

			aimerGeometry.SetNormal(new Vector3(0, 0, 1));
			aimerGeometry.SetUv(new Vector2(0, 0));
			
			for (int i = 0; i < 10; i++)
			{
				//y = 0.1f * (1 * Math.Pow(1.4f, i))
				aimerGeometry.AddVertex(new Vector3(
					(float) (0.1f * (1 * Math.Pow(1.4f, i))) - 1,
					GetTree().Root.GetChild(0).GetNode<Spatial>("AimerStart").Translation.y,
					-i
				));
			}
			aimerGeometry.End();
		}
	}

	private void OnCurveSliderChanged(float value) => curveValue = value;
}
