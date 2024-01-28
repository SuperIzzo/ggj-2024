using Godot;
using System;

public partial class GameRootController : Node3D
{
	[Export]
	SlonResource DefaultSlonA;
	
	[Export]
	SlonResource DefaultSlonB;

	[Export]
	public AudioStreamPlayer2D Sound_Hit;

	[Export]
	public AudioStreamPlayer2D Sound_Block;

	[Export]
	public AudioStreamPlayer2D Sound_Walk;

	[Export]
	public AudioStreamPlayer2D Sound_Hurt;

	[Export]
	public AudioStreamPlayer2D Sound_TrunkMove;

	[Export]
	public AudioStreamPlayer2D Sound_KO;
	
	GameGlobals globals;
	
	Label labelA;
	Label labelB;
	
	Health healthA;
	Health healthB;
	
	Camera3D camera;
	double cameraSpeed = 2.5;
	double cameraTimer = 6;	
	double postGameTimer = 5;
	
	Minigame minigame;
	SlonStandoff standoff;
	
	public enum State
	{
		Idle,
		InProgress,
		PlayerWon,
		EnemyWon,
		DoubleKO,
		GameTied,
	}
	
	public State CurrentState = State.Idle;
	
	public override void _Ready()
	{
		standoff = this.GetChildByType<SlonStandoff>();
		minigame = this.GetChildByType<Minigame>();
		
		labelA = this.GetChildByType<Label>("LabelA");
		labelB = this.GetChildByType<Label>("LabelB");
		
		healthA = this.GetChildByType<Health>("ControlA");
		healthB = this.GetChildByType<Health>("ControlB");
		
		camera = GetParent().GetChildByType<Camera3D>();
		
		globals = GetNode<GameGlobals>("/root/GameGlobals");
		healthA.MaxHp = globals.PlayerMaxHP;
		healthB.MaxHp = globals.EnemyMaxHP;
		
		healthA.CurrentHp = globals.PlayerHP;
		healthB.CurrentHp = globals.EnemyHP;
		
		globals.GameRef = this;

		SetupRound(globals.PlayerSlon, globals.EnemySlon);
		RunGame();
	}

	public override void _Process(double delta)
	{
		healthA.CurrentHp = globals.PlayerHP;
		healthB.CurrentHp = globals.EnemyHP;
		
		switch(CurrentState)
		{
			case State.InProgress: 
				UpdateGame(delta); 
				break;
				
			case State.PlayerWon:
			case State.EnemyWon:
			case State.GameTied:
			case State.DoubleKO:
				UpdateGameOver(delta);
				break;
		}
	}
	
	public void UpdateGame(double delta)
	{
		cameraTimer -= delta;
		if (cameraTimer >= 0.0)
		{
			camera.Position += Vector3.Left * (float)(delta * cameraSpeed);
		}
		
		if (minigame.CurrentStage == Minigame.Stage.Done)
		{
			if (globals.PlayerHP == 0 && globals.EnemyHP == 0)
			{
				OnDoubleKO();
			}
			else if (globals.PlayerHP == 0)
			{
				OnPlayerDeceased();
			}
			else if (globals.EnemyHP <= 0)
			{
				OnEnemyDeceased();
			}
			else
			{
				OnGameTied();
			}
		}
	}
	
	public void UpdateGameOver(double delta)
	{
		postGameTimer -= delta;
		if (postGameTimer < 0)
		{
			if (CurrentState == State.GameTied)
			{				
				ResetMatch();
			}
			else
			{
				NextMatch();
			}
		}
	}
	
	public void SetupRound(SlonResource SlonA, SlonResource SlonB)
	{
		CurrentState = State.Idle;
		standoff.SetUpStandoff(SlonA, SlonB);
		(minigame.GetParent() as Node2D).Visible = false;
		
		labelA.Text = SlonA.Name;
		labelB.Text = SlonB.Name;
	}
	
	public void RunGame()
	{
		CurrentState = State.InProgress;
		standoff.RunSlons(5);
		
		minigame.TriggerStart();
		(minigame.GetParent() as Node2D).Visible = true;
	}
	
	public void OnPlayerDeceased()
	{
		CurrentState = State.PlayerWon;
	}
	
	public void OnEnemyDeceased()
	{
		CurrentState = State.EnemyWon;
	}
	
	public void OnDoubleKO()
	{
		CurrentState = State.DoubleKO;
	}
	
	public void OnGameTied()
	{
		CurrentState = State.GameTied;
	}
	
	public void ResetMatch()
	{
		globals.PrepareNewSet();
		GetTree().ChangeSceneToFile("res://Abby/ElephencingMainScene.tscn");
	}
	
	public void NextMatch()
	{
		globals.PrepareNewMatch();
		GetTree().ChangeSceneToFile("res://IzzoStuff/IntroScene/IntroScene.tscn");
	}
}
