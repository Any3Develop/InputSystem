using System;
using System.Collections.Generic;
using System.Linq;
using InputSystem.Abstraction;
using InputSystem.DragDropSystem;
using InputSystem.DragDropSystem.Infrastructure;
using InputSystem.HoveringSystem;
using InputSystem.HoveringSystem.Infrastructure;
using InputSystem.PreviewSystem;
using InputSystem.PreviewSystem.Infrastructure;
using InputSystem.SelectionSystem;
using InputSystem.SelectionSystem.Infrastructure;
using InputSystem.GameCore.TargetSystem;
using UnityEngine.InputSystem;
using Zenject;

namespace InputSystem
{
	public class InputSystemInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			BindInput();
			DragSystemInstaller.Install(Container);
			HoveringSystemInstaller.Install(Container);
			SelectionSystemInstaller.Install(Container);
			PreviewSystemInstaller.Install(Container);
		}

		private void BindInput()
		{
			var gamePlayInput = new GamePlayInput();
			gamePlayInput.Enable(); // before binding input actions
			
			Container
				.Bind<GamePlayInput>()
				.FromInstance(gamePlayInput)
				.AsSingle()
				.NonLazy();

			Container
				.BindInterfacesTo<InputContextProcessor>()
				.AsSingle()
				.NonLazy();

			BindInputController<DragSystemActions>(gamePlayInput.DragSystem.Get());
			BindInputController<HoveringSystemActions>(gamePlayInput.HoveringSystem.Get());
			BindInputController<SelectionSystemActions>(gamePlayInput.SelectionSystem.Get());
			BindInputController<PreviewType>(gamePlayInput.PreviewSystem.Get());
			BindInputController<TargetSystemActions>(gamePlayInput.TargetSystem.Get());
		}

		private void BindInputController<TAction>(InputActionMap actionMap) where TAction : Enum
		{
			// Generics in assembly with IL2CPP - runtime error, deterministic generation of Generics required.
			// https://forum.unity.com/threads/is-there-any-limitations-to-deserializing-json-on-webgl.1250356/#post-7951618
			var args = MapInputActions(actionMap);
			var inputController = new InputController<TAction>(args);
			Container
				.BindInterfacesTo<InputController<TAction>>()
				.FromInstance(inputController)
				.AsSingle()
				.NonLazy();
		}

		private IEnumerable<IInputAction> MapInputActions(InputActionMap actionMap)
		{
			return actionMap.actions.Select(inputAction => new InputSystemInputAction(inputAction));
		}
	}
}