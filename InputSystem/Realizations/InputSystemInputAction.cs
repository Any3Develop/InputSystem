using System;
using InputSystem.Abstraction;
using UnityEngine.InputSystem;

namespace InputSystem
{
	public class InputSystemInputAction : IInputAction, IDisposable
	{
		private readonly InputAction bindedAction;

		public bool Enabled { get; private set; }
		public string Id => bindedAction.name;
		public event Action<IInputContext> OnStarted;
		public event Action<IInputContext> OnPerformed;
		public event Action<IInputContext> OnCanceled;
		public event Action OnEnableChanged;

		public InputSystemInputAction(InputAction bindAction)
		{
			bindedAction = bindAction;
			if (bindedAction.enabled) // initialize if enabled
				Enable();
		}

		public TValue ReadValue<TValue>() where TValue : struct
		{
			return bindedAction.ReadValue<TValue>();
		}

		public void Enable()
		{
			if (Enabled)
				return;

			Enabled = true;
			bindedAction.Enable();
			bindedAction.started += OnBindedActionStarted;
			bindedAction.performed += OnBindedActionPerformed;
			bindedAction.canceled += OnBindedActionCanceled;
			OnEnableChanged?.Invoke();
		}
		
		public void Disable()
		{
			if (!Enabled)
				return;
			
			Enabled = false;
			bindedAction.Disable();
			bindedAction.started -= OnBindedActionStarted;
			bindedAction.performed -= OnBindedActionPerformed;
			bindedAction.canceled -= OnBindedActionCanceled;
			OnEnableChanged?.Invoke();
		}
		
		private void OnBindedActionCanceled(InputAction.CallbackContext context)
		{
			OnCanceled?.Invoke(new InputSystemInputContext(context, Id));
		}

		private void OnBindedActionPerformed(InputAction.CallbackContext context)
		{
			OnPerformed?.Invoke(new InputSystemInputContext(context, Id));
		}

		private void OnBindedActionStarted(InputAction.CallbackContext context)
		{
			OnStarted?.Invoke(new InputSystemInputContext(context, Id));
		}

		public void Dispose()
		{
			Disable();
			OnEnableChanged = null;
		}
	}
}