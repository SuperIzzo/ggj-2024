using Godot;
using System;

public partial class ProtectController : LocationController
{
	private Vector2 m_vVelocity = new();
	private Vector2 m_vAcceleration = new();
	
	[Export]
	private float MovementStrength = 1.0f;

	[Export]
	private float Mass = 5.0f;

	[Export]
	private float Friction = 0.02f;

	public override void Process(Minigame.Stage eStage, double dTimeLeft)
	{
		
		switch(eStage)
		{
			case Minigame.Stage.LineUp:
			{
				StageLineUp();
				break;
			}
			case Minigame.Stage.Engage:
			{
				break;
			}
		}
	}

	private void StageLineUp()
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

		ApplyAcceleration();
		ApplyFriction();

		ProcessKeepInBounds();
	}

	private void ApplyForce(Vector2 v)
	{
		Vector2 vForce = v / Mass;
		m_vAcceleration += vForce;
	}

	private void ApplyAcceleration()
	{
		m_vVelocity += m_vAcceleration;
		Position += m_vVelocity;

		m_vAcceleration = Vector2.Zero;
	}

	private void ApplyFriction()
	{
		//float coefficient = 0.05f;
		Vector2 vFriction = m_vVelocity;

		vFriction *= -1.0f;
		vFriction = vFriction.Normalized();
		vFriction *= Friction;

		ApplyForce(vFriction);
	}

	private void ProcessKeepInBounds()
	{
		if(Position.X < 0)
		{
			Position = new Vector2(0, Position.Y);
			m_vVelocity.X = -m_vVelocity.X;
		}
		else if(Position.X > m_vBoundsSize.X)
		{
			Position = new Vector2(m_vBoundsSize.X, Position.Y);
			m_vVelocity.X = -m_vVelocity.X;
		}

		if(Position.Y < 0)
		{
			Position = new Vector2(Position.X, 0);
			m_vVelocity.Y = -m_vVelocity.Y;
		}
		else if(Position.Y > m_vBoundsSize.Y)
		{
			Position = new Vector2(Position.X, m_vBoundsSize.Y);
			m_vVelocity.Y = -m_vVelocity.Y;
		}
	}
}