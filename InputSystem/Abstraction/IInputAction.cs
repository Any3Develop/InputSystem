﻿using System;

namespace InputSystem.Abstraction
{
	public interface IInputAction
	{
		bool Enabled { get; }
		string Id { get; }
		event Action<IInputContext> OnStarted;
		event Action<IInputContext> OnPerformed;
		event Action<IInputContext> OnCanceled;
		event Action OnEnableChanged;
		TValue ReadValue<TValue>() where TValue : struct;
		void Enable();
		void Disable();
	}
}