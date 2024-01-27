using Godot;
using System;

// Disable warning on un-needed parentheses, it's less about mathmatical order of
//	operations and more about the intention of the maths going on to make it easier to read
#pragma warning disable IDE0047 

public class EnemyController
{
	public enum Difficulty
	{
		One = 1,
		Two,
		Three,
		Four,
		Five,
		Six
	}

	public struct Action
	{
		// Config
		public double Interval;
		public float MoveStrength;
		public float RandomisedAngle;
		public Vector2 MaxSpeed;
		public double IgnoreBurstMax; // Good values 1.0 to 0.5
		public double IgnoreBurstMin; // Good values 0.5 to 0.2
		public float WanderStrength;
		public float MaxFleeStrength;
		public float DistToFlee;

		// Results
		public Vector2 Output;

		// Runtime
		public double Timer;
		public double TimeToIgnoreTarget;
		public double TimeToChill;
		public Vector2 DirOnStartIgnore;
		public Vector2 RandoChillDirection;
	}

	private Action m_attack;
	private Action m_protect;

	private Vector2 m_vMaxBounds; // Min bounds is 0,0
	private float m_fActionRadius;

	public Vector2 GetAttackInput() => m_attack.Output;
	public Vector2 GetProtectInput() => m_protect.Output;

	private Vector2 m_vFleeDirection;

	public void Init(Difficulty eDiff, Vector2 vMaxBounds, float fActionRadius)
	{
		m_vMaxBounds = vMaxBounds;
		m_fActionRadius = fActionRadius;

		m_attack = new()
		{
			Interval = 0.0,
			Timer = 0.0,
			MoveStrength = 20.0f,
			RandomisedAngle = 40.0f,
			MaxSpeed = new Vector2(5.0f, 5.0f),
			IgnoreBurstMax = 0.9,
			IgnoreBurstMin = 0.4,
			WanderStrength = 0.45f,
			RandoChillDirection = GetRandomDir(),
			MaxFleeStrength = 0.75f,
			DistToFlee = m_vMaxBounds.X * 0.4f
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
		rAction.TimeToIgnoreTarget -= delta;
		rAction.TimeToChill -= delta;

		if(rAction.Timer > 0.0)
		{
			rAction.Output = Vector2.Zero;
			return;
		}

		rAction.Timer = rAction.Interval;

		// How long to avoid for when moving away from boundaries
		double dTimeToAvoid = 1.0;


		bool bIgnoringTarget = rAction.TimeToIgnoreTarget > 0.0;
		float fDistToTarget = vTarget.DistanceTo(vSelf);
		float fDistanceStrength = Mathf.Clamp(Mathf.Remap(fDistToTarget, m_fActionRadius * 2.0f, rAction.DistToFlee, rAction.MaxFleeStrength, 0.0f), 0.0f, rAction.MaxFleeStrength);
		Vector2 vDirToFlee = -(vTarget - vSelf);
		Vector2 vBoundaryFlee = Vector2.Zero;
		
		vDirToFlee = RandomiseDirectionAngle(vDirToFlee, rAction.RandomisedAngle);
		vDirToFlee = vDirToFlee.Normalized();

		if(bIgnoringTarget)
		{
			vDirToFlee = rAction.DirOnStartIgnore;
			fDistanceStrength = (float)Mathf.Remap(rAction.TimeToIgnoreTarget, dTimeToAvoid, dTimeToAvoid * 0.5, rAction.IgnoreBurstMax, rAction.IgnoreBurstMin);
		}
		else if(fDistanceStrength < 0.25f)
		{
			vDirToFlee = rAction.RandoChillDirection;
			fDistanceStrength = rAction.WanderStrength;

			if(rAction.TimeToIgnoreTarget < -2.0)
			{
				rAction.TimeToIgnoreTarget = 0.0;
				rAction.RandoChillDirection = GetRandomDir();
			}
		}

		float fDistToBoundaryH = vSelf.X > m_vMaxBounds.X / 2 ? m_vMaxBounds.X - vSelf.X : vSelf.X;
		float fDistToBoundaryV = vSelf.Y > m_vMaxBounds.Y / 2 ? m_vMaxBounds.Y - vSelf.Y : vSelf.Y;

		float fBoundaryStrH = Mathf.Clamp(Mathf.Remap(fDistToBoundaryH, m_fActionRadius / 2, m_fActionRadius * 2.0f, 1.0f, 0.0f), 0.0f, 1.0f);
		float fBoundaryStrV = Mathf.Clamp(Mathf.Remap(fDistToBoundaryV, m_fActionRadius / 2, m_fActionRadius * 2.0f, 1.0f, 0.0f), 0.0f, 1.0f);

		// Move away from horizontal bounds
		if(fBoundaryStrH > 0.0f)
		{
			fBoundaryStrH = Mathf.Remap(fBoundaryStrH, 0.0f, 1.0f, 0.4f, 1.0f);

			vBoundaryFlee += new Vector2(vSelf.X > m_vMaxBounds.X / 2 ? -1.0f : 1.0f, GetRandomAxisDir()) * fBoundaryStrH;
			vDirToFlee = new Vector2(vDirToFlee.X * (1.0f - fBoundaryStrH), vDirToFlee.Y);
			
			if(!bIgnoringTarget)
			{
				rAction.TimeToIgnoreTarget = dTimeToAvoid;

				rAction.RandoChillDirection = GetRandomDir();
				rAction.DirOnStartIgnore = new Vector2(vSelf.X > m_vMaxBounds.X / 2 ? -1.0f : 1.0f, GetRandomAxisDir());
			}
		}

		// Move away from vertical bounds
		if(fBoundaryStrV > 0.0f)
		{
			fBoundaryStrV = Mathf.Remap(fBoundaryStrV, 0.0f, 1.0f, 0.4f, 1.0f);

			vBoundaryFlee += new Vector2(GetRandomAxisDir(), vSelf.Y > m_vMaxBounds.Y / 2 ? -1.0f : 1.0f) * fBoundaryStrV;
			vDirToFlee = new Vector2(vDirToFlee.X, vDirToFlee.Y * (1.0f - fBoundaryStrV));
			
			if(!bIgnoringTarget)
			{
				rAction.TimeToIgnoreTarget = dTimeToAvoid;
				
				rAction.RandoChillDirection = GetRandomDir();
				rAction.DirOnStartIgnore = new Vector2(
					fBoundaryStrH > 0.0f ? rAction.DirOnStartIgnore.X : GetRandomAxisDir(),
					vSelf.Y > m_vMaxBounds.Y / 2 ? -1.0f : 1.0f);
			}
		}

		// Falloff our last remembered direction
		m_vFleeDirection *= (bIgnoringTarget ? 0.25f : 0.1f);
		
		vDirToFlee = vDirToFlee.Normalized();
		vBoundaryFlee = vBoundaryFlee.Normalized();

		m_vFleeDirection += vDirToFlee * fDistanceStrength;
		m_vFleeDirection += vBoundaryFlee;

		m_vFleeDirection = m_vFleeDirection.Clamp(-rAction.MaxSpeed, rAction.MaxSpeed);

		rAction.Output = m_vFleeDirection * rAction.MoveStrength;
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

	private Vector2 GetRandomDir()
	{
		var random = new RandomNumberGenerator();
		random.Randomize();
		return new Vector2(-1.0f + (2.0f * random.Randf()), -1.0f + (2.0f * random.Randf()));
	}

	private float GetRandomAxisDir()
	{
		var random = new RandomNumberGenerator();
		random.Randomize();

		return -1.0f + (2.0f * random.Randf());
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
