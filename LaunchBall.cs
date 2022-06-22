using Godot;
using System;

public class LaunchBall : Camera
{
	public Vector2 mouseFlickStart = Vector2.Zero;
	
	//TODO - Add torque so that the ball always curves towards the centre a little if you throw off centre
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.Pressed)
				mouseFlickStart = mouseButton.Position;
			else
			{
				//Projection from the bottom centre of the screen (where the ball is thrown from)
				var throwRoot = ProjectPosition(new Vector2(GetViewport().GetVisibleRect().Size.x / 2, GetViewport().GetVisibleRect().Size.y), 2);
				var ballScene = GD.Load<PackedScene>("res://Ball.tscn");
				var ball = (RigidBody) ballScene.Instance();
				ball.Translation = throwRoot;
				var flick = new Vector2(mouseFlickStart.x - mouseButton.Position.x, mouseFlickStart.y - mouseButton.Position.y).Normalized();
				// the x value is the inverse of how you flicked across
				//the y value gets how high the flick was in relation to the screen's height, and then multiplies by a constant, to determine throw height
				//The z value basically judges how far up you dragged your mouse to flick, to determine with how much power it is thrown, it is clamped so that it can not be negative
				ball.LinearVelocity = new Vector3(
					-flick.x,
					Mathf.Abs(mouseFlickStart.y - mouseButton.Position.y) / GetViewport().GetVisibleRect().Size.y * 2,
					throwRoot.z - Mathf.Clamp(flick.y, 0, 1) * 3
				);
				AddChild(ball);
			}
		}
	}
}
