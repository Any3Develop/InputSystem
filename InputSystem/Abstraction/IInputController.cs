using System.Collections.Generic;

namespace InputSystem.Abstraction
{
	public interface IInputController
	{
		IInputAction Get(string id);
		IEnumerable<IInputAction> GetAll();
		void Enable();
		void Disable();
	}
	
	public interface IInputController<in T> : IInputController
	{
		IInputAction Get(T action);
	}
}