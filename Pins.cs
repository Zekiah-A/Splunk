using Godot;
using System;
using System.Collections.Generic;

public partial class Pins : Node3D
{
	private Timer settleTimer;
	private AudioStreamPlayer3D audioPlayer;
	private List<int> pinsDown = new List<int>();
	private Random rand = new Random();
	[Signal] public delegate void PinKnockedDownEventHandler(int pinIndex);
	
	public override void _Ready()
	{
		audioPlayer = GetNode<AudioStreamPlayer3D>("ASP");
		settleTimer = GetNode<Timer>("SettleTimer");
	}

	private async void OnPinCollision(Node body, int pinIndex)
	{
		if (body is RigidBody3D rigidBody)
		{
			//Instance hit effect particles between the ball and pin hit. //We should use OnCollisionShapeEntered to get the normal properly.
			/*var particles = (Particles) GD.Load<PackedScene>("res://HitParticles.tscn").Instance(); //TODO: Load packed scenes like this beforehand to improve perf.
			particles.Position = rigidBody.Position; //GetNode<Node3D>($"{pinIndex}").Position + rigidBody.Position / 2;
			particles.Emitting = true;
			AddChild(particles);*/

			settleTimer.Start(3);
			if (!audioPlayer.Playing)
				PlayPinSound();
			else
			{
				await ToSignal(audioPlayer, "finished");
				PlayPinSound();
			}
		}
	}
	
	private void PlayPinSound()
	{
		//returns between 0-1, div by 2, add 0.5 so it is 0.75-1.25
		audioPlayer.PitchScale = (float) (rand.NextDouble() / 10) + 0.8f;
		audioPlayer.Play();
	}
	
	//After 3 seconds of no collisions, with the ball, pins, other pins, or the collider under the world, we assume that all pins have settled down, and now can count how many were knocked over.
	public async void OnSettleTimerTimeout()
	{
		for (int i = 0; i < GetChildCount() - 1; i++)
		{
			var pin = GetChildren()[i] is RigidBody3D ? (RigidBody3D) GetChildren()[i] : null;
			if (pin is null) return;
			if (pin.Position.Y < -0.1f || pin.RotationDegrees.X > 10 || pin.RotationDegrees.X < -10 || pin.RotationDegrees.Z > 10 || pin.RotationDegrees.Z < -10)
			{
				pin.QueueFree();
				EmitSignal(nameof(PinKnockedDownEventHandler), int.Parse(pin.Name));
			}
			((Result) GetTree().Root.GetChild(0).GetNode("Control/ResultPanel")).PlayResult(ResultType.Strike);
		}
	}
}
