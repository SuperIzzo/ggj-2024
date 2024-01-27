using Godot;

public static class GDExtensions
{	
	public static T GetChildByType<T>(this Node node, bool recursive = true)
		where T : Node
	{
		int childCount = node.GetChildCount();

		for (int i = 0; i < childCount; i++)
		{
			Node child = node.GetChild(i);
			if (child is T childT)
				return childT;

			if (recursive && child.GetChildCount() > 0)
			{
				T recursiveResult = child.GetChildByType<T>(true);
				if (recursiveResult != null)
					return recursiveResult;
			}
		}

		return null;
	}
	
	public static T GetChildByType<T>(this Node node, string name, bool recursive = true)
		where T : Node
	{
		int childCount = node.GetChildCount();

		for (int i = 0; i < childCount; i++)
		{
			Node child = node.GetChild(i);
			if (child is T childT && child.Name == name)
				return childT;

			if (recursive && child.GetChildCount() > 0)
			{
				T recursiveResult = child.GetChildByType<T>(name, true);
				if (recursiveResult != null)
					return recursiveResult;
			}
		}

		return null;
	}

	public static T GetParentByType<T>(this Node node)
		where T : Node
	{
		Node parent = node.GetParent();
		if (parent != null)
		{
			if (parent is T parentT)
			{
				return parentT;
			}
			else
			{
				return parent.GetParentByType<T>();
			}
		}

		return null;
	}
}
