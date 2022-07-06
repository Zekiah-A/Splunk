using Godot;
using System;

public class Result : Panel
{
	private VideoPlayer vp;
	
	public override void _Ready()
	{
		vp = GetNode<VideoPlayer>("VideoPlayer");
	}

	public async void PlayResult(int result)
	{
		Visible = true;
		vp.Stream = ResourceLoader.Load<VideoStreamTheora>(result == 0 ? "strike_result.ogv" : result == 1 ? "spare_result.ogv" : null);
		vp.Play();
		await ToSignal(vp, "finished");
		Visible = false;
	}
}

public enum Results
{
	Strike,
	Spare,
	GutterBall
}
