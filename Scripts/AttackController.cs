using Godot;
using System;

public partial class AttackController : LocationController
{
	private Vector2 m_vMouseRelative;

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

	public void UpdateMouseInput(Vector2 vInput)
	{
		float fStrength = Mathf.Remap(vInput.Length(), 0.0f, 50.0f, 0.0f, 1.0f);
		Vector2 vNorm = vInput.Normalized();

		m_vMouseRelative = vNorm * MovementStrength * fStrength;
	}

	private void StageLineUp(Minigame.Stage eStage, double dTimeLeft)
	{
		ApplyForce(m_vMouseRelative * MovementStrength);
		m_vMouseRelative = Vector2.Zero;
		
		base.Process(eStage, dTimeLeft);
	}
	
}
