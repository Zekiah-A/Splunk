using Godot;
using System;
using System.Collections.Generic;

public class Game : Spatial
{
	public override void _Ready()
	{
		
	}

	private void OnCloseupAreaEntered(object body)
	{
		if (body is Ball)
		{
			GetNode<Camera>("CloseupCamera").MakeCurrent();
			GetNode<Timer>("CloseupTimer").Start();
		}
	}

	private void OnCloseupTimerTimeout()
	{
		GetNode<Camera>("CloseupCamera").ClearCurrent();
		GetNode<Camera>("Player/Player/Head/Camera").MakeCurrent();
	}

	private async void OnPlayPressed()
	{
		var tween = GetNode<Tween>("Control/Tween");
		GetNode<AnimationPlayer>("AnimationPlayer").Play("intro");
		//This can be optimised
		tween.InterpolateProperty(
			GetNode("Control/TitlePanel"),
			"rect_scale",
			Vector2.One,
			new Vector2(1.2f, 1.2f),
			1
		);
		tween.InterpolateProperty(
			GetNode("Control/TitlePanel"),
			"modulate",
			Colors.White,
			new Color(1, 1, 1, 0),
			1,
			Tween.TransitionType.Cubic,
			Tween.EaseType.Out
		);
		tween.Start();
		await ToSignal(tween, "tween_all_completed");
		GetNode("Control/TitlePanel").QueueFree();
		tween.InterpolateProperty(
			GetNode("Control/CreditsPanel"),
			"rect_scale",
			Vector2.One,
			new Vector2(1.2f, 1.2f),
			5
		);
		tween.InterpolateProperty(
			GetNode("Control/CreditsPanel"),
			"modulate",
			new Color(1, 1, 1, 0),
			Colors.White,
			5,
			Tween.TransitionType.Cubic,
			Tween.EaseType.Out
		);
		tween.Start();
		await ToSignal(tween, "tween_completed");
		GetNode("Control/CreditsPanel").QueueFree();
	}
}
