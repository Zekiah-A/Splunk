using Godot;
using System;

public class Result : Panel
{
	private VideoPlayer vp;
	private Tween tween;
	
	public override void _Ready()
	{
		vp = GetNode<VideoPlayer>("VideoPlayer");
		tween = GetNode<Tween>("Tween");
		GetNode<Timer>("Timer").Connect("timeout", this, nameof(LerpTransition));
	}

	public async void PlayResult(int result)
	{
		Visible = true;
		GetNode<Timer>("Timer").Start();
		vp.Stream = ResourceLoader.Load<VideoStreamTheora>(result == 0 ? "strike_result.ogv" : result == 1 ? "spare_result.ogv" : null);
		vp.Play();
		await ToSignal(vp, "finished");
		Visible = false;
	}
	
	private async void LerpTransition() => ((ShaderMaterial) Material).SetShaderParam("cutoff", (float)((ShaderMaterial) Material).GetShaderParam("cutoff") - 0.1f);
}

public enum Results
{
	Strike,
	Spare,
	GutterBall
}
