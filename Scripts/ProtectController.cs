using Godot;
using System;

public partial class ProtectController : LocationController
{
	public override void Process(Minigame.Stage eStage, double dTimeLeft)
	{
		
		switch(eStage)
		{
			case Minigame.Stage.LineUp:
			{
				StageLineUp(eStage, dTimeLeft);
				break;
			}
			case Minigame.Stage.Engage:
			{
				break;
			}
		}
	}

	private void StageLineUp(Minigame.Stage eStage, double dTimeLeft)
	{
		Vector2 vInput = new();

		// Add to the janky ps1 demo vibe, only allow one input at a time
		//	and make the player keep pressing the direction
		if(Input.IsActionJustPressed("Key_Left"))
		{
			vInput.X = -MovementStrength;
		}
		else if(Input.IsActionJustPressed("Key_Right"))
		{
			vInput.X = MovementStrength;
		}
		else if(Input.IsActionJustPressed("Key_Up"))
		{
			vInput.Y = -MovementStrength;
		}
		else if(Input.IsActionJustPressed("Key_Down"))
		{
			vInput.Y = MovementStrength;
		}

		ApplyForce(vInput);
		
		base.Process(eStage, dTimeLeft);
	}

	

	
}