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

	public async void PlayResult(ResultType result)
	{
		Visible = true;
		Modulate = Colors.White;
		((ShaderMaterial) Material).SetShaderParam("cutoff", 1f);
		GetNode<Timer>("Timer").Start();
		vp.Stream = ResourceLoader.Load<VideoStreamTheora>(result == ResultType.Strike ? "strike_result.ogv" : result == ResultType.Spare ? "spare_result.ogv" : null);
		vp.Play();
		await ToSignal(vp, "finished");
		tween.InterpolateProperty(this, "modulate", Colors.White, Colors.Transparent, 0.1f);
		tween.Start();
		await ToSignal(tween, "tween_all_completed");
		Visible = false;
	}
	
	private void LerpTransition() => ((ShaderMaterial) Material).SetShaderParam("cutoff", (float)((ShaderMaterial) Material).GetShaderParam("cutoff") - 0.05f);
}

public enum ResultType
{
	Strike,
	Spare,
	GutterBall
}
