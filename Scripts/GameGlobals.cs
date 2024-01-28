using Godot;
using System;

public partial class GameGlobals : Node
{
	[Export]
	SlonResource[] AvailableSlons;
	
	Random random = new Random();
	
	public SlonResource PlayerSlon;
	public SlonResource EnemySlon;
	
	public override void _Ready()
	{
		GD.Print("Start");
		
		random.Next();
		random.Next();
		random.Next();
		ChooseNewSlons();
	}
	
	public void ChooseNewSlons()
	{
		int randomIndex = random.Next(0, AvailableSlons.Length);
		PlayerSlon = AvailableSlons[randomIndex];
		
		randomIndex = random.Next(0, AvailableSlons.Length);
		EnemySlon = AvailableSlons[randomIndex];
	}
}
