using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Game : Node3D
{
	private static readonly Dictionary<string, object> defaultSettings = new Dictionary<string, object>
		{ {"graphics", 2}, {"relativeControls", true}, {"displayPlaying", true}, {"pfpAvatar", true}, {"saveScores", false} };

	public override void _Ready()
	{
		GetNode<Pins>("Pins").Connect("PinKnockedDown", new Callable(this, nameof(OnPinKnockedDown)));
	}

	public static void SaveSetting(string name, object value) => GD.Print("Unimplemented");
	//	JavaScript.Eval($"localStorage.setItem('{name}', '{value}');");
	
	public static object GetSetting(string name)
	{
		/*
		var res = JavaScript.Eval($"localStorage.getItem('{name}');");
		if (res is null || res.ToString() == "null" || res.ToString() == "undefined")
		{
			JavaScript.Eval($"localStorage.setItem('{name}', '{defaultSettings[name]}');");
			return defaultSettings[name];
		}
		return res;
		*/
		GD.Print("Unimplemented");
		return null;
	}

	private void OnCloseupAreaEntered(object body)
	{
		if (body is Ball)
		{
			GetNode<Camera3D>("CloseupCamera").MakeCurrent();
			GetNode<Timer>("CloseupTimer").Start();
		}
	}

	private void OnCloseupTimerTimeout()
	{
		GetNode<Camera3D>("CloseupCamera").ClearCurrent();
		GetNode<Camera3D>("Player/Player/Head/Camera3D").MakeCurrent();
	}

	private void OnPlayPressed()
	{
		GetNode<AnimationPlayer>("AnimationPlayer").Play("intro");
		var tween = CreateTween().SetParallel();
		var titlePanel = GetNode<Control>("Control/TitlePanel");
		var creditsPanel = GetNode<Control>("Control/CreditsPanel");
		
		titlePanel.Modulate = Colors.White;
		titlePanel.Scale = Vector2.One;
		tween.TweenProperty(
			titlePanel,
			"modulate",
			Colors.Transparent,
			1
		);
		tween.TweenProperty(
			titlePanel,
			"scale",
			new Vector2(1.2f, 1.2f),
			1
		);
		tween.Chain()
			.TweenCallback(Callable.From(() =>
			{
				titlePanel.QueueFree();
				creditsPanel.Scale = Vector2.One;
				creditsPanel.Modulate = Colors.Transparent;
			}));
		tween.TweenProperty(
			creditsPanel,
			"scale",
			new Vector2(1.2f, 1.2f),
			4);
		tween
			.TweenProperty(
				creditsPanel,
				"modulate",
				Colors.White,
				5)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Cubic);
		tween
			.Chain()
			.TweenCallback(Callable.From(() =>
			{
				creditsPanel.QueueFree();
			}));
		tween.Play();
	}
	
	private void OnScoresPressed()
	{

		var tween = CreateTween();
		var scoresButton = GetNode<Button>("Control/ScoresButton");
		tween.TweenProperty(
				scoresButton,
				"position",
				scoresButton.Position.X == 0
					? new Vector2(scoresButton.GetNode<Panel>("Panel").Size.X, GetViewport().GetVisibleRect().Size.Y / 2 - scoresButton.Size.Y / 2)
					: new Vector2(0, GetViewport().GetVisibleRect().Size.Y / 2 - scoresButton.Size.Y / 2),
				1
			)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
		tween.Play();
	}
	
	private void OnPinKnockedDown(int pinIndex)
	{
		//foreach on this after each round and set all back to white
		((Button) GetNode("Control/DownPanel").GetChildren()[pinIndex]).ButtonPressed = true;
	}
}

