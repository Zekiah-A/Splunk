using Godot;
using System;
using System.Collections.Generic;

public class Pins : Spatial
{
	private Timer settleTimer;
	private AudioStreamPlayer3D audioPlayer;
	private List<int> pinsDown = new List<int>();
	private Random rand = new Random();
	
	public override void _Ready()
	{
		audioPlayer = GetNode<AudioStreamPlayer3D>("PinsAudioStreamPlayer");
		settleTimer = GetNode<Timer>("SettleTimer");
	}

	private void OnPinCollision(object body, int pinIndex)
	{
		if (body is RigidBody)
		{
			settleTimer.Start(3);
			audioPlayer.PitchScale = (float) (rand.NextDouble() / 2) + 0.75f; //returns between 0-1, div by 2, add 0.5 so it is 0.75-1.25
			audioPlayer.Play();
		}
	}
	
	//After 3 seconds of no collisions, we assume that all pins have settled down, and now can count how many were knocked over
	private void OnSettleTimerTimeout()
	{
		foreach (var pin in GetChildren())
		{
			var pinAsRigidbody = pin as RigidBody;

			/*
			if ((int) Mathf.Floor(((RigidBody) pin).RotationDegrees.z / 10) != 0 || (int) Mathf.Floor(((RigidBody) pin).RotationDegrees.z / 10) != 0)
			{
				GD.Print("Knocked over pin");
			}
			*/
		}
	}
}

