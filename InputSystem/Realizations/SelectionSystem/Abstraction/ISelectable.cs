using UnityEngine;

namespace InputSystem.SelectionSystem.Abstraction
{
	public interface ISelectable
	{
		/// <summary>
		/// Target of some reference view
		/// </summary>
		GameObject TargetView { get; }

		bool CanSelect();
	}
}