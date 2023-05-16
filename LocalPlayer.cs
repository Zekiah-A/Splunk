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
			RotateY(-mouseMotion.Relative.X * Sensitivity);
			playerHead.RotateX(mouseMotion.Relative.Y * Sensitivity);
			playerHead.Rotation = new Vector3(Mathf.Clamp(playerHead.Rotation.X, -1.2f, 1.2f), playerHead.Rotation.Y, playerHead.Rotation.Z);
		}
	}

	public void MoveTo(string objectPosition)
	{
		//If we are looking down the lane, we can just cull everything behind us. //Second layer not visible if at lane pos
		playerHead.GetNode<Camera3D>("Camera3D").SetCullMaskValue(1, objectPosition != "PlayerLanePosition");
		//PlayerLanePosition, PlayerDispenserPosition, PlayerSeatPosition/0,/1,/2
		if (Position == GetTree().Root.GetChild(0).GetNode<Node3D>(objectPosition).Position) return;
		var tween = CreateTween();
		tween.TweenProperty(
			this,
			"transform",
			GetTree().Root.GetChild(0).GetNode<Node3D>(objectPosition).Transform,
			3
		);
		tween.Play();
	}
}
