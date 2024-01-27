using Godot;

public partial class slonTrunkControler : Node3D
{
	private Skeleton3D skeleton3D;
	private int trunk1BoneIdx;
	private int trunk2BoneIdx;
	private int trunk3BoneIdx;
	private int trunk4BoneIdx;
	private double rotationSpeed = 3f;
	
	public override void _Ready()
	{
		// Access the Skeleton3D node
		skeleton3D = GetNode<Skeleton3D>("Armature/Skeleton3D");

		// Find the bone indices by name
		trunk1BoneIdx = skeleton3D.FindBone("trunk1");
		trunk2BoneIdx = skeleton3D.FindBone("trunk2");
		trunk3BoneIdx = skeleton3D.FindBone("trunk3");
		trunk4BoneIdx = skeleton3D.FindBone("trunk4");

		if (trunk1BoneIdx == -1 || trunk2BoneIdx == -1 || trunk3BoneIdx == -1 || trunk4BoneIdx == -1)
		{
			GD.Print("Error: One or more trunk bones not found.");
		}
		else
		{
			GD.Print("Trunk found");
		}
	}
	 private void RotateTrunk(int trunkBoneIdx, Vector3 rotationAxis, double delta)
	{
		// Rotate trunk bone
		Quaternion trunkRotation = skeleton3D.GetBonePoseRotation(trunkBoneIdx);
		trunkRotation = trunkRotation * new Quaternion(rotationAxis, (float)(rotationSpeed * delta));
		skeleton3D.SetBonePoseRotation(trunkBoneIdx, trunkRotation);
	}
		
	public override void _Process(double delta)
	{
		 if (Input.IsActionPressed("ui_left"))
		 {
			// Rotate trunks along the Z-axis when the Left arrow key is pressed
			RotateTrunk(trunk1BoneIdx, -Vector3.Forward, delta);
			RotateTrunk(trunk2BoneIdx, -Vector3.Forward, delta);
		}
		if (Input.IsActionPressed("ui_right"))
		{
			// Rotate trunks along the Z-axis when the Right arrow key is pressed
			RotateTrunk(trunk1BoneIdx, Vector3.Forward, delta);
			RotateTrunk(trunk2BoneIdx, Vector3.Forward, delta);
		}
		
		if (Input.IsActionPressed("ui_up"))
		{
			// Move trunks up along the X-axis when the Up arrow key is pressed
			RotateTrunk(trunk1BoneIdx, -Vector3.Right, delta);
			RotateTrunk(trunk2BoneIdx, -Vector3.Right, delta);
			RotateTrunk(trunk3BoneIdx, -Vector3.Right, delta);
		}
		
		if (Input.IsActionPressed("ui_down"))
		{
			// Move trunks down along the X-axis when the Down arrow key is pressed
			RotateTrunk(trunk1BoneIdx, Vector3.Right, delta);
			RotateTrunk(trunk2BoneIdx, Vector3.Right, delta);
			RotateTrunk(trunk3BoneIdx, Vector3.Right, delta);
		}
	}
}
