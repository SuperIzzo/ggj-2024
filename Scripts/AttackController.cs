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

	public void AddInput(Vector2 vInput)
	{
		// Accept input on a range of 0 to 50
		float fStrength = Mathf.Clamp(Mathf.Remap(vInput.Length(), 0.0f, 50.0f, 0.0f, 2.0f), 0.0f, 2.0f);
		Vector2 vNorm = vInput.Normalized();

		m_vMouseRelative = vNorm * MovementStrength * fStrength;
	}

	public void AddInputArrows(Vector2 vInput)
	{
		m_vMouseRelative = vInput * MovementStrength;
	}

	private void StageLineUp(Minigame.Stage eStage, double dTimeLeft)
	{
		ApplyForce(m_vMouseRelative);
		m_vMouseRelative = Vector2.Zero;

		base.Process(eStage, dTimeLeft);
	}
	
}
