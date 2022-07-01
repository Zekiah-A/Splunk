using Godot;
using System;
using Array = Godot.Collections.Array;

public class LaunchBall : Camera
{
	private RigidBody ball;
	private ImmediateGeometry aimerGeometry;
	private float cvValue = 0;
	private const int VelocityFwd = 6;
	private const int VelocityUp = 2;
	private Vector2 mouseFlickStart = Vector2.Zero;

	public override void _Ready()
	{
		aimerGeometry = GetTree().Root.GetChild(0).GetNode<ImmediateGeometry>("AimerGeometry");
		RedrawAimer();
	}

	public override void _UnhandledInput(InputEvent @event)
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
				ball = (RigidBody) ballScene.Instance();
				ball.Translation = throwRoot;
				//var flick = new Vector2(mouseFlickStart.x - mouseButton.Position.x, mouseFlickStart.y - mouseButton.Position.y).Normalized(); //Flicks from start to end
				var flick = new Vector2(GetViewport().GetVisibleRect().Size.x / 2 -  mouseButton.Position.x, GetViewport().GetVisibleRect().Size.y - mouseButton.Position.y).Normalized(); //"Flicks" from bottom centre screen to end
				// the x value is the inverse of how you flicked across
				//the y value gets how high the flick was in relation to the screen's height, and then multiplies by a constant, to determine throw height
				//The z value basically judges how far up you dragged your mouse to flick, to determine with how much power it is thrown, it is clamped so that it can not be negative
				((Ball) ball).InitialVelocity = ball.LinearVelocity = new Vector3(
					-flick.x,
					Mathf.Abs(GetViewport().GetVisibleRect().Size.y - mouseButton.Position.y) / GetViewport().GetVisibleRect().Size.y * VelocityUp,
					throwRoot.z - Mathf.Clamp(flick.y, 0, 1) * VelocityFwd
				);
				((Ball) ball).CvDir = cvValue;
				AddChild(ball);
			}
		}
	}
	
	//Optimised by BlobKat
	private void RedrawAimer()
	{
		aimerGeometry.Clear();
		aimerGeometry.Begin(Mesh.PrimitiveType.Triangles);
		aimerGeometry.SetNormal(new Vector3(0, 0, 1));
		var geometryY = GetTree().Root.GetChild(0).GetNode<Spatial>("AimerStart").Translation.y;
		float func = 0; //Line up the end of the top of these polys, with the beginning of the next
		for (int i = 0; i < 10; i++)
		{
			var nextFunc = (float) (1e-4 * (cvValue * Math.Pow(1.8f, 1.1f * i+1)));
			aimerGeometry.SetUv(Vector2.Zero); //Bottom Left
			aimerGeometry.AddVertex(new Vector3((i == 0 ? 0 : func) - 0.3f, geometryY, -i));
			aimerGeometry.SetUv(Vector2.Down); //Top left
			aimerGeometry.AddVertex(new Vector3(nextFunc - 0.3f, geometryY, -i - 1));
			aimerGeometry.SetUv(Vector2.Right); //Bottom right
			aimerGeometry.AddVertex(new Vector3((i == 0 ? 0 : func) + 0.3f, geometryY, -i));
			aimerGeometry.SetUv(Vector2.Right); //Bottom right
			aimerGeometry.AddVertex(new Vector3((i == 0 ? 0 : func) + 0.3f, geometryY, -i));
			aimerGeometry.SetUv(Vector2.Down); //Top Left
			aimerGeometry.AddVertex(new Vector3(nextFunc - 0.3f, geometryY, -i - 1));
			aimerGeometry.SetUv(Vector2.One); //Top Right
			aimerGeometry.AddVertex(new Vector3(nextFunc + 0.3f, geometryY, -i - 1));
			func = nextFunc;
		}
		aimerGeometry.End();
	}
	
	private void OnCurveSliderChanged(float value)
	{
		cvValue = value;
		RedrawAimer();
	}
}
