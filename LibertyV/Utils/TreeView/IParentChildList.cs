using System;

namespace System.Windows.Forms
{
	/// <summary>
	/// IParentChildList provides functions to navigate a mutliply-linked
	/// list organized in parent-child format. The current node may navigate
	/// upwards to its parent node, forward and backwards in the current
	/// level, and down to the next level of its children.
	/// </summary>
	public interface IParentChildList
	{
		object ParentNode();

		object PreviousSibling();
		object NextSibling();

		object FirstChild();
		object NextChild();
		object PreviousChild();
		object LastChild();
	}
}
