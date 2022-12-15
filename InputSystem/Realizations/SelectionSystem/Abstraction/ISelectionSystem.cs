using System;

namespace InputSystem.SelectionSystem.Abstraction
{
	public interface ISelectionSystem
	{
		public event Action<ISelectable> OnSelected;
		
		public event Action OnCanceled;

		void Registration(ISelectable selectable);
		
		void UnRegistration(ISelectable selectable);
		
		void Select(ISelectable selectable);
		
		void Cancel();
		
		void Clear();
	}
}