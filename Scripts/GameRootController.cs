using Godot;
using System;

public partial class GameRootController : Node3D
{
	[Export]
	SlonResource SlonA;
	
	[Export]
	SlonResource SlonB;
	
	SlonStandoff standoff;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		standoff = this.GetChildByType<SlonStandoff>();
		
		standoff.SetUpStandoff(SlonA, SlonB);
		standoff.RunSlons(10);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
