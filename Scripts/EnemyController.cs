using Godot;
using System;

public class EnemyController
{
	public enum Difficulty
	{
		One = 1,
		Two,
		Three,
		Four,
		Five,
		Six,
		Seven
	}

	public struct Action
	{
		public double Interval;
		public double Timer;
		public float MoveStrength;
		public float RandomisedAngle;

		public Vector2 Output;
	}

	private Action m_attack;
	private Action m_protect;

	private Vector2 m_vMaxBounds;
	private float m_fActionRadius;

	public Vector2 GetAttackInput() => m_attack.Output;
	public Vector2 GetProtectInput() => m_protect.Output;



	public void Init(Difficulty eDiff, Vector2 vMaxBounds, float fActionRadius)
	{
		m_vMaxBounds = vMaxBounds;
		m_fActionRadius = fActionRadius;

		m_attack = new()
		{
			Interval = 0.5,
			Timer = 0.0,
			MoveStrength = 1.5f,
			RandomisedAngle = 40.0f
		};

		m_protect = new()
		{
			Interval = 1.0,
			Timer = 0.0,
			MoveStrength = 2.0f,
			RandomisedAngle = 40.0f
		};
	}

	public void Process(Vector2 vPlayerAttack, Vector2 vPlayerProtect, Vector2 vEnemyAttack, Vector2 vEnemyProtect, double delta)
	{
		ProcessFleeInput(ref m_attack, vEnemyAttack, vPlayerProtect, delta);
		ProcessChaseInput(ref m_protect, vEnemyProtect, vPlayerAttack, delta);
	}

	private void ProcessFleeInput(ref Action rAction, Vector2 vSelf, Vector2 vTarget, double delta)
	{
		rAction.Timer -= delta;
		if(rAction.Timer > 0.0)
		{
			rAction.Output = Vector2.Zero;
			return;
		}

		rAction.Timer = rAction.Interval;

		
	}

	private void ProcessChaseInput(ref Action rAction, Vector2 vSelf, Vector2 vTarget, double delta)
	{
		rAction.Timer -= delta;
		if(rAction.Timer > 0.0)
		{
			rAction.Output = Vector2.Zero;
			return;
		}

		rAction.Timer = rAction.Interval;

		Vector2 vDirToPlayerAttack = vTarget - vSelf;
		Vector2 vDir = RandomiseDirectionAngle(vDirToPlayerAttack, rAction.RandomisedAngle);

		vDir = vDir.Normalized();

		rAction.Output = vDir * rAction.MoveStrength;
	}
	
	private Vector2 RandomiseDirectionAngle(Vector2 vDir, float fAngleMax)
	{
		var random = new RandomNumberGenerator();
		random.Randomize();
		
		float fRandomAngle = -fAngleMax + ((fAngleMax * 2.0f) * random.Randf());
		float theta = Mathf.DegToRad(fRandomAngle);

		float x = vDir.X;
        float y = vDir.Y;

		float cs = Mathf.Cos(theta);
        float sn = Mathf.Sin(theta);
        
        float px = x * cs - y * sn; 
        float py = x * sn + y * cs;

		return new Vector2(px, py);
	}
}
