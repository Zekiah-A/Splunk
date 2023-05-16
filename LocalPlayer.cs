using Godot;
using System;

public partial class LocalPlayer : Node3D
{
	private Tween tween;
	private Node3D playerHead;
	private const float Sensitivity = 0.005f;
	public bool LookLocked = true;

	public override void _Ready()
	{
		tween = GetNode<Tween>("Tween");
		playerHead = GetNode<Node3D>("Player/Head");
		
		MoveTo("PlayerLanePosition");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (LookLocked) return;
		if (@event is InputEventMouseMotion mouseMotion)
		{
			RotateY(-mouseMotion.Relative.x * Sensitivity);
			playerHead.RotateX(mouseMotion.Relative.y * Sensitivity);
			playerHead.Rotation = new Vector3(Mathf.Clamp(playerHead.Rotation.x, -1.2f, 1.2f), playerHead.Rotation.y, playerHead.Rotation.z);
		}
	}

	public void MoveTo(string objectPosition)
	{
		//If we are looking down the lane, we can just cull everything behind us. //Second layer not visible if at lane pos
		playerHead.GetNode<Camera3D>("Camera3D").SetCullMaskValue(1, objectPosition != "PlayerLanePosition");
		//PlayerLanePosition, PlayerDispenserPosition, PlayerSeatPosition/0,/1,/2
		if (Position == GetTree().Root.GetChild(0).GetNode<Node3D>(objectPosition).Position) return;
		tween.InterpolateProperty(
			this,
			"transform",
			Transform3D,
			GetTree().Root.GetChild(0).GetNode<Node3D>(objectPosition).Transform3D,
			3
		);
		tween.Start();
	}
}
