using Godot;
using System;

public partial class GameRootController : Node3D
{
	[Export]
	SlonResource DefaultSlonA;
	
	[Export]
	SlonResource DefaultSlonB;
	
	Camera3D camera;
	double cameraSpeed = 2.5;
	
	Minigame minigame;
	SlonStandoff standoff;
	
	bool GameRunning = false;
	
	public override void _Ready()
	{
		standoff = this.GetChildByType<SlonStandoff>();
		minigame = this.GetChildByType<Minigame>();
		
		camera = GetParent().GetChildByType<Camera3D>();
		
		Setup(DefaultSlonA, DefaultSlonB);
		RunGame();
	}

	public override void _Process(double delta)
	{
		if (GameRunning)
		{
			camera.Position += Vector3.Left * (float)(delta * cameraSpeed);
			
			if (minigame.CurrentStage == Minigame.Stage.Exit
			 || minigame.CurrentStage == Minigame.Stage.DetermineResult)
			{
				GameRunning = false;
			}
		}
	}
	
	public void Setup(SlonResource SlonA, SlonResource SlonB)
	{
		GameRunning = false;
		standoff.SetUpStandoff(SlonA, SlonB);
		(minigame.GetParent() as Node2D).Visible = false;
	}
	
	public void RunGame()
	{
		GameRunning = true;
		standoff.RunSlons(5);
		
		minigame.TriggerStart();
		(minigame.GetParent() as Node2D).Visible = true;
	}
}
