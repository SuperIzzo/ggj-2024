using Godot;
using System;
using System.Collections.Generic;

public partial class LabelControl : Control
{
	private Label Screentext;
	private List<String> randomEvents;
	private double timer = 0;
	[Export]
	double time = 3;
	private bool isTextClear = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Screentext = GetNode<Label>("RandomEventGenerator");
		randomEvents = new List<string>();
//region Events list.
		randomEvents.Add("You no longer have your horse");
		randomEvents.Add("A mischievous monkey steals your fencing gear.");
		randomEvents.Add("A Specific portal opens.");
		randomEvents.Add("Perception check failed.");
		randomEvents.Add("Larry will remember this.");
		randomEvents.Add("The bloody moon has risen.");
		randomEvents.Add("Congratulations, it's a boy!");
		randomEvents.Add("Double experience.");
		randomEvents.Add("The referee has declared a penalty shot.");
		randomEvents.Add("Touche");
		randomEvents.Add("Genie disappears; two wishes remaining.");
		randomEvents.Add("You're out of ammo");
		randomEvents.Add("Kasahune is Nearby");
//endregion
		UpdateRandomEvent();
		
	}
	
	public void UpdateRandomEvent()
	{
		if(randomEvents.Count > 0)
		{
			int randomIndex = new Random().Next(0, randomEvents.Count);
			Screentext.Text = randomEvents[randomIndex];
			isTextClear = false;
		}
	}
	
	public void ClearEventText()
	{
		if(isTextClear == false)
		{
			Screentext.Text = "";
			isTextClear = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		timer += delta;
		if (timer >= time)
		{
			timer = 0;
			ClearEventText();
		}
	}
}
