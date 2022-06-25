using Godot;
using System;
using System.Collections.Generic;

public class Game : Spatial
{
	public List<Node> Players = new List<Node>();
	
	public override void _Ready()
	{
		
	}

	public void ResetMap()
	{
		foreach (var player in Players) (player as Player)?.ClearBalls();
	}

	private void OnCloseupAreaEntered(object body)
	{
		if (body is Ball)
		{
			GetNode<Camera>("CloseupCamera").MakeCurrent();
			GetNode<Timer>("CloseupTimer").Start();
		}
	}

	private void OnCloseupTimerTimeout() =>
		GetNode<Camera>("CloseupCamera").ClearCurrent();
}
