using Godot;
using System;
using Array = Godot.Collections.Array;

public partial class LaunchBall : Camera3D
{
	private RigidBody3D ball;
	private ImmediateMesh aimerGeometry;
	private float cvValue = 0;
	private const int VelocityFwd = 8;
	private const int VelocityUp = 2;
	private const int InvalidFlickDist = 20;
	private Vector2 mouseFlickStart = Vector2.Zero;
	private TextureRect[] aimerPointers;
	private readonly Vector2 ptrMidpoint = new Vector2(40, 40);
	private readonly Color aimerNormal = new Color(1, 1, 1, 0.5f);
	private readonly Color aimerInvalid = new Color(1, 0.2f, 0.2f, 0.5f);
	private bool invalidFlick;

	public override void _Ready()
	{
		aimerGeometry = GetTree().Root.GetChild(0).GetNode<ImmediateMesh>("AimerGeometry");
		aimerPointers = new[]
		{
			GetTree().Root.GetChild(0).GetNode<TextureRect>("Control/AimerPointer"),
			GetTree().Root.GetChild(0).GetNode<TextureRect>("Control/AimerPointer2")
		};
		aimerPointers[0].Visible = false; aimerPointers[1].Visible = false;
		RedrawAimer();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.Pressed)
			{
				mouseFlickStart = mouseButton.Position;
				aimerPointers[0].Visible = true;
				aimerPointers[0].Position = mouseButton.Position - ptrMidpoint;
				aimerPointers[1].Visible = true;
			}
			else
			{
				GetTree().Root.GetChild(0).GetNode<TextureRect>("Control/AimerPointer").Visible = false;
				GetTree().Root.GetChild(0).GetNode<TextureRect>("Control/AimerPointer2").Visible = false;
				if (invalidFlick) return;
				//Projection from the bottom centre of the screen (where the ball is thrown from)
				var throwRoot = ProjectPosition(new Vector2(GetViewport().GetVisibleRect().Size.x / 2, GetViewport().GetVisibleRect().Size.y), 2);
				var ballScene = GD.Load<PackedScene>("res://Ball.tscn"); //TODO: Load packed scenes like this beforehand to improve perf.
				ball = (RigidBody3D) ballScene.Instance();
				ball.Position = throwRoot;
				var flick = new Vector2(mouseFlickStart.x - mouseButton.Position.x, mouseFlickStart.y - mouseButton.Position.y).Normalized(); //Flicks from start to end
				//var flick = new Vector2(GetViewport().GetVisibleRect().Size.x / 2 -  mouseButton.Position.x, GetViewport().GetVisibleRect().Size.y - mouseButton.Position.y).Normalized(); //"Flicks" from bottom centre screen to end
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

		if (@event is InputEventMouseMotion mouseMotion)
		{
			//If the two aimers are over each other, we allow the player to cancel the throw.
			aimerPointers[1].Modulate = aimerPointers[0].Position.DistanceTo(aimerPointers[1].Position) < InvalidFlickDist ? aimerInvalid : aimerNormal;
			invalidFlick = aimerPointers[0].Position.DistanceTo(aimerPointers[1].Position) < InvalidFlickDist;
			aimerPointers[1].Position = mouseMotion.Position - ptrMidpoint;
		}
	}

	public override void _Process(double delta)
	{
		if (Input.GetGyroscope().z > 20) //Does not work on mobile web, may need to modify godot for this functionality
			Translate(new Vector3(1, 0, 0));
		else if (Input.GetGyroscope().z < -20)
			Translate(new Vector3(-1, 0, 0));
	}
	
	//Optimised by BlobKat
	private void RedrawAimer()
	{
		aimerGeometry.Clear();
		aimerGeometry.Begin(Mesh.PrimitiveType.Triangles);
		aimerGeometry.SetNormal(new Vector3(0, 0, 1));
		var geometryY = GetTree().Root.GetChild(0).GetNode<Node3D>("AimerStart").Position.y;
		float func = 0; //Line up the end of the top of these polys, with the beginning of the next
		for (var i = 0; i < 10; i++)
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
