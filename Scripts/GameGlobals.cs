using Godot;
using System;

public partial class GameGlobals : Node
{
	[Export]
	SlonResource[] AvailableSlons;
	
	Random random = new Random();
	
	public SlonResource PlayerSlon;
	public SlonResource EnemySlon;
	
	public int PlayerHP = 3;
	public int EnemyHP = 3;
	
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

	public void SetupHealth(EnemyController.Difficulty eDiff)
	{
		PlayerHP = 4;

		switch(eDiff)
		{
			case EnemyController.Difficulty.One: 	EnemyHP 	= 3; break;
			case EnemyController.Difficulty.Two: 	EnemyHP 	= 4; break;
			case EnemyController.Difficulty.Three: 	EnemyHP 	= 4; break;
			case EnemyController.Difficulty.Four: 	EnemyHP 	= 5; break;
			case EnemyController.Difficulty.Five: 	EnemyHP 	= 5; break;
			case EnemyController.Difficulty.Six:	EnemyHP 	= 6; break;
		}
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
