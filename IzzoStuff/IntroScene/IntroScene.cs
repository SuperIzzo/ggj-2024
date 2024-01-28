using Godot;
using System;

public partial class IntroScene : Node3D
{
	private double timer = 7.0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameGlobals globals = GetNode<GameGlobals>("/root/GameGlobals");
		
		var IntroA = this.GetChildByType<Intro>("IntroA");
		var IntroB = this.GetChildByType<Intro>("IntroB");
		
		IntroA.SetSlon(globals.PlayerSlon);
		IntroB.SetSlon(globals.EnemySlon);
	}
	
	public override void _Process(double delta)
	{
		timer -= delta;
		if (timer <= 0)
		{
			GetTree().ChangeSceneToFile("res://Abby/ElephencingMainScene.tscn");
		}
	}
}
