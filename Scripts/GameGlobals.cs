using Godot;
using System;

public partial class GameGlobals : Node
{
	[Export]
	SlonResource[] AvailableSlons;
	
	Random random = new Random();
	
	public SlonResource PlayerSlon;
	public SlonResource EnemySlon;
	
	public double PlayerHP = 100;
	public double EnemyHP = 100;
	
	public int Match = 0;
	public int Set = 0;
	
	public override void _Ready()
	{
		GD.Print("Start");
		
		random.Next();
		random.Next();
		random.Next();
		PrepareNewMatch();
	}
	
	public void PrepareNewMatch()
	{
		Match++;
		ChooseNewSlons();
		
		PlayerHP = 150;
		EnemyHP = 90 + Match*10;
	}
	
	public void PrepareNewSet()
	{
		Set++;
	}
	
	public void ChooseNewSlons()
	{
		int randomIndex = random.Next(0, AvailableSlons.Length);
		PlayerSlon = AvailableSlons[randomIndex];
		
		randomIndex = random.Next(0, AvailableSlons.Length);
		EnemySlon = AvailableSlons[randomIndex];
	}
}
