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

	private void OnCloseupTimerTimeout() =>
		GetNode<Camera>("CloseupCamera").ClearCurrent();
}
