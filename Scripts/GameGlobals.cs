using Godot;
using System;

public partial class GameGlobals : Node
{
	[Export]
	SlonResource[] AvailableSlons;
	
	Random random = new Random();
	
	public SlonResource PlayerSlon;
	public SlonResource EnemySlon;

	public GameRootController GameRef;
	
	public int PlayerMaxHP = 3;
	public int EnemyMaxHP = 3;
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
		PlayerMaxHP = 4;
		PlayerHP = 4;

		switch(eDiff)
		{
			case EnemyController.Difficulty.One: 	EnemyMaxHP 	= 3; break;
			case EnemyController.Difficulty.Two: 	EnemyMaxHP 	= 4; break;
			case EnemyController.Difficulty.Three: 	EnemyMaxHP 	= 4; break;
			case EnemyController.Difficulty.Four: 	EnemyMaxHP 	= 5; break;
			case EnemyController.Difficulty.Five: 	EnemyMaxHP 	= 5; break;
			case EnemyController.Difficulty.Six:	EnemyMaxHP 	= 6; break;
		}

		EnemyHP = EnemyMaxHP;
	}
	
	public void PrepareNewMatch()
	{
		Set = 0;
		++Match;
		ChooseNewSlons();
		
		SetupHealth((EnemyController.Difficulty)Match);
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
