// SlonResource.cs
using Godot;

public partial class SlonResource : Resource
{
	[Export]
	public string Name;
	
	[Export]
	public string IntroTitle;
	
	[Export]
	public string IntroSubtitle;
	
	[Export]
	public PackedScene SlonModel;
}
