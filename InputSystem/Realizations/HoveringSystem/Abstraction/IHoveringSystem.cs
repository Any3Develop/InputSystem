using System;

namespace InputSystem.HoveringSystem.Abstraction
{
	public interface IHoveringSystem
	{
		event Action<IHoverable> OnHoverEnter;

		event Action<IHoverable> OnHoverExit;
		
		void Registration(IHoverable hoverable);
		
		void UnRegistration(IHoverable hoverable);

		void Start(IHoverable hoverable);
		
		void Cancel(IHoverable hoverable = null);

		void Clear();
	}
}