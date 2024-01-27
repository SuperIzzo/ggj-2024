using Godot;
using System;

public partial class SlonStandoff : Node3D
{
	Node3D SlonASlot;
	Node3D SlonBSlot;
	Node3D SlonATarget;
	Node3D SlonBTarget;
	
	Vector3 SlonAStart;
	Vector3 SlonAEnd;
	Vector3 SlonBStart;
	Vector3 SlonBEnd;
	
	Quaternion SlonARotStart;
	Quaternion SlonBRotStart;
	Quaternion SlonARotEnd;
	Quaternion SlonBRotEnd;
	
	SlonController SlonModelA;
	SlonController SlonModelB;
	
	bool AreSlonsRunning = false;
	double RunDuration = 10;
	double RunTime = 0;

	public override void _Ready()
	{
		SlonASlot = this.GetChildByType<Node3D>("SlonA");
		SlonBSlot = this.GetChildByType<Node3D>("SlonB");
		SlonATarget = this.GetChildByType<Node3D>("SlonATarget");
		SlonBTarget = this.GetChildByType<Node3D>("SlonBTarget");
		
		SlonAStart = SlonASlot.Position;
		SlonBStart = SlonBSlot.Position;
		SlonAEnd = SlonATarget.Position;
		SlonBEnd = SlonBTarget.Position;
		
		SlonARotStart = SlonASlot.Quaternion;
		SlonBRotStart = SlonBSlot.Quaternion;
		SlonARotEnd = SlonATarget.Quaternion;
		SlonBRotEnd = SlonBTarget.Quaternion;
	}

	public override void _Process(double delta)
	{
		if (AreSlonsRunning)
		{
			RunTime += delta;
			float Progress = (float)(RunTime / RunDuration);
			Progress = Mathf.Clamp(Progress, 0.0f, 1.0f);
			
			SlonASlot.Position = SlonAStart.Lerp(SlonAEnd, Progress);
			SlonBSlot.Position = SlonBStart.Lerp(SlonBEnd, Progress);
			SlonASlot.Quaternion = SlonARotStart.Slerp(SlonARotEnd, Progress);
			SlonBSlot.Quaternion = SlonBRotStart.Slerp(SlonBRotEnd, Progress);
			
			GD.Print(SlonAEnd);
		}
	}
	
	public void SetUpStandoff(SlonResource SlonA, SlonResource SlonB)
	{
		SlonASlot.ClearAllChildren();
		SlonBSlot.ClearAllChildren();

		SlonModelA = SlonA.SlonModel.Instantiate() as SlonController;
		SlonModelB = SlonB.SlonModel.Instantiate() as SlonController;

		SlonASlot.AddChild(SlonModelA);
		SlonBSlot.AddChild(SlonModelB);
	}
	
	public void RunSlons(double duration)
	{
		SlonModelA.PlayRunAnim();
		SlonModelB.PlayRunAnim();
		
		AreSlonsRunning = true;
		RunDuration = duration;
		RunTime = 0.0;
	}
}
