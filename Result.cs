using Godot;
using System;

public partial class Result : Panel
{
	private VideoStreamPlayer vp;
	private Tween tween;
	
	public override void _Ready()
	{
		vp = GetNode<VideoStreamPlayer>("VideoStreamPlayer");
		tween = GetNode<Tween>("Tween");
		GetNode<Timer>("Timer").Connect("timeout", new Callable(this, nameof(LerpTransition)));
	}

	public async void PlayResult(ResultType result)
	{
		Visible = true;
		Modulate = Colors.White;
		((ShaderMaterial) Material).SetShaderParameter("cutoff", 1f);
		GetNode<Timer>("Timer").Start();
		vp.Stream = ResourceLoader.Load<VideoStreamTheora>(result == ResultType.Strike ? "strike_result.ogv" : result == ResultType.Spare ? "spare_result.ogv" : null);
		vp.Play();
		await ToSignal(vp, "finished");
		var tween = CreateTween();
		Modulate = Colors.White;
		tween.TweenProperty(
			this,
			"modulate",
			Colors.Transparent,
			0.1f
		);
		tween.TweenCallback(Callable.From(() =>
		{
			Visible = false;
		}));
		tween.Play();
	}
	
	private void LerpTransition() => ((ShaderMaterial) Material).SetShaderParameter("cutoff", (float)((ShaderMaterial) Material).GetShaderParameter("cutoff") - 0.05f);
}

public enum ResultType
{
	Strike,
	Spare,
	GutterBall
}
