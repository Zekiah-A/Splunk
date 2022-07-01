using Godot;
using System;
using System.Collections.Generic;

public class Pins : Spatial
{
	private Timer settleTimer;
	private AudioStreamPlayer3D audioPlayer;
	private List<int> pinsDown = new List<int>();
	private Random rand = new Random();
	[Signal] public delegate void PinKnockedDown(int pinIndex);
	
	public override void _Ready()
	{
		audioPlayer = GetNode<AudioStreamPlayer3D>("ASP");
		settleTimer = GetNode<Timer>("SettleTimer");
	}

	private async void OnPinCollision(object body, int pinIndex)
	{
		if (body is RigidBody)
		{
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
	public void OnSettleTimerTimeout()
	{
		for (int i = 0; i < GetChildCount() - 1; i++)
		{
			var pin = GetChildren()[i] is RigidBody ? (RigidBody) GetChildren()[i] : null;
			if (pin is null) return;
			if (pin.Translation.y < -1 || pin.RotationDegrees.x > 10 || pin.RotationDegrees.x < -10 || pin.RotationDegrees.z > 10 || pin.RotationDegrees.z < -10)
			{
				pin.QueueFree();
				EmitSignal(nameof(PinKnockedDown), int.Parse(pin.Name));
			}
		}
	}
}
