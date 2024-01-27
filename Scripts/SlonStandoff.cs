using Godot;
using System;

public partial class SlonStandoff : Node3D
{
	[Export]
	SlonResource SlonA;
	
	[Export]
	SlonResource SlonB;
	
	Node3D SlonASlot;
	Node3D SlonBSlot;
	
	Node SlonModelA;
	Node SlonModelB;
	
	public void SetUpStandoff(SlonResource SlonA, SlonResource SlonB)
	{
		SlonASlot.ClearAllChildren();
		SlonBSlot.ClearAllChildren();
		
		SlonModelA = SlonA.SlonModel.Instantiate();
		SlonModelB = SlonB.SlonModel.Instantiate();
		
		SlonASlot.AddChild(SlonModelA);
		SlonBSlot.AddChild(SlonModelB);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SlonASlot = this.GetChildByType<Node3D>("SlonA");
		SlonBSlot = this.GetChildByType<Node3D>("SlonB");
		SetUpStandoff(SlonA, SlonB);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
