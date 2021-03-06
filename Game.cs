using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = Godot.File;

public class Game : Spatial
{
	private static readonly Dictionary<string, object> defaultSettings = new Dictionary<string, object>
		{ {"graphics", 2}, {"relativeControls", true}, {"displayPlaying", true}, {"pfpAvatar", true}, {"saveScores", false} };

	public override void _Ready()
	{
		GetNode<Pins>("Pins").Connect("PinKnockedDown", this,nameof(OnPinKnockedDown));
	}

	public static void SaveSetting(string name, object value) =>
		JavaScript.Eval($"localStorage.setItem('{name}', '{value}');");
	
	public static object GetSetting(string name)
	{
		var res = JavaScript.Eval($"localStorage.getItem('{name}');");
		if (res is null || res.ToString() == "null" || res.ToString() == "undefined")
		{
			JavaScript.Eval($"localStorage.setItem('{name}', '{defaultSettings[name]}');");
			return defaultSettings[name];
		}
		return res;
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
			4
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
	
	private void OnScoresPressed()
	{
		var tween = GetNode<Tween>("Control/Tween");
		var scoresButton = GetNode<Button>("Control/ScoresButton");
		tween.InterpolateProperty(
			scoresButton,
			"rect_position",
			scoresButton.RectPosition,
			scoresButton.RectPosition.x == 0
					? new Vector2(scoresButton.GetNode<Panel>("Panel").RectSize.x, GetViewport().GetVisibleRect().Size.y / 2 - scoresButton.RectSize.y / 2)
					: new Vector2(0, GetViewport().GetVisibleRect().Size.y / 2 - scoresButton.RectSize.y / 2),
			1,
			Tween.TransitionType.Cubic,
			Tween.EaseType.Out
		);
		tween.Start();
	}
	
	private void OnPinKnockedDown(int pinIndex)
	{
		//foreach on this after each round and set all back to white
		((Button) GetNode("Control/DownPanel").GetChildren()[pinIndex]).Pressed = true;
	}
}

