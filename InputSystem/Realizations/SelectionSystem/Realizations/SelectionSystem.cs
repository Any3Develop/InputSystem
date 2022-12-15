using System;
using System.Collections.Generic;
using System.Linq;
using InputSystem.Abstraction;
using InputSystem.HoveringSystem.Abstraction;
using InputSystem.SelectionSystem.Abstraction;
using InputSystem.Utils;
using Zenject;

namespace InputSystem.SelectionSystem
{
	public class SelectionSystem : ISelectionSystem, IInitializable, IDisposable
	{
		private readonly IInputController<SelectionSystemActions> inputController;
		private readonly List<ISelectable> selectables;
		private ISelectable current;
		public event Action<ISelectable> OnSelected;
		public event Action OnCanceled;

		public SelectionSystem(IInputController<SelectionSystemActions> inputController)
		{
			this.inputController = inputController;
			selectables = new List<ISelectable>();
		}
		
		public void Initialize()
		{
			inputController.GetAll().ForEach(input =>
			{
				input.OnPerformed += context => OnInputPreformed(context, Enum.Parse<SelectionSystemActions>(input.Id));
				input.OnCanceled += context => OnInputPreformed(context, Enum.Parse<SelectionSystemActions>(input.Id));
			});
		}
		
		public void Dispose()
		{
			selectables.Clear();
			OnSelected = null;
			OnCanceled = null;
		}

		public void Registration(ISelectable selectable)
		{
			if (selectable == null)
				throw new NullReferenceException($"Target {nameof(IHoverable)} is missing");

			if (selectables.Contains(selectable))
				throw new InvalidOperationException($"{selectable} already registered");
			
			selectables.Add(selectable);
		}

		public void UnRegistration(ISelectable selectable)
		{
			if (selectable == null)
				throw new NullReferenceException($"Target {nameof(IHoverable)} is missing");

			if (!selectables.Contains(selectable))
			{
				Debug.LogError("{hoverable} does not registered");
				return;
			}
			
			if (current == selectable)
				Cancel();
			
			selectables.Remove(selectable);
		}
		
		public void Select(ISelectable selectable = null)
		{
			if (selectable != null && (!selectable.CanSelect() || selectable == current))
				return;
			
			if (current != null && current != selectable)
				Cancel();
			
			current = selectable;
			OnSelected?.Invoke(selectable);
		}

		public void Cancel()
		{
			current = null;
			OnCanceled?.Invoke();
		}

		public void Clear()
		{
			selectables.Clear();
			current = null;
			OnSelected = null;
			OnCanceled = null;
		}

		private ISelectable GetSelectable()
		{
			return selectables.ToArray().FirstOrDefault(x => x.CanSelect() && InputHelper.IsPointerOver(x.TargetView));
		}

		private void OnInputPreformed(IInputContext context, SelectionSystemActions systemActions)
		{
			if (systemActions == SelectionSystemActions.Cancel)
			{
				if (!context.Performed)
					return;
				
				Cancel();
				return;
			}

			if (context.Canceled)
			{
				current = null;
				return;
			}
			
			if (context.Performed)
				Select(GetSelectable());
		}
	}
}