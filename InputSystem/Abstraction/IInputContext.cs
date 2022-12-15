using System;

namespace InputSystem.Abstraction
{
	public interface IInputContext
	{
		string Id { get; }
		
		bool Started { get; }
		
		bool Performed { get; }
		
		bool Canceled { get; }
		
		Type ControlValueType { get; }
		
		TValue ReadValue<TValue>() where TValue : struct;
		
		bool TryReadValue<TValue>(out TValue result) where TValue : struct;
	}
}