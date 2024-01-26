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

	public override void AddInput(Vector2 vInput)
	{
		m_vInput = vInput * MovementStrength;
	}

	private void StageLineUp(Minigame.Stage eStage, double dTimeLeft)
	{
		ApplyForce(m_vInput);
		m_vInput = Vector2.Zero;

		base.Process(eStage, dTimeLeft);
	}

	

	
}