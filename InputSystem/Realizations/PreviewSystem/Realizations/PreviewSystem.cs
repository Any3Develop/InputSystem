using System;
using System.Collections.Generic;
using InputSystem.Abstraction;
using InputSystem.PreviewSystem.Abstraction;
using InputSystem.Utils;
using Zenject;

namespace InputSystem.PreviewSystem
{
	public class PreviewSystem : IPreviewSystem, IInitializable, IDisposable
	{
		private readonly IInputController<PreviewType> inputController;
		private readonly IPreviewView previewView;
		private readonly IInputContextProcessor inputContextProcessor;
		private readonly List<IPreviewable> registered;
		public IPreviewable Current { get; private set; }
		public event Action OnPreview;
		public event Action OnClose;

		public PreviewSystem(IInputController<PreviewType> inputController,
			IInputContextProcessor inputContextProcessor,
			IPreviewView previewView)
		{
			this.inputController = inputController;
			this.inputContextProcessor = inputContextProcessor;
			this.previewView = previewView;
			registered = new List<IPreviewable>();
		}
		
		public void Initialize()
		{
			inputController.GetAll().ForEach(input =>
			{
				input.OnStarted += context => OnPreviewInput(context, Enum.Parse<PreviewType>(input.Id));
				input.OnPerformed += context => OnPreviewInput(context, Enum.Parse<PreviewType>(input.Id));
				input.OnCanceled += context => OnPreviewInput(context, Enum.Parse<PreviewType>(input.Id));
			});
		}
		
		public void Dispose()
		{
			Current = null;
			OnPreview = null;
			OnClose = null;
		}

		public void Registration(IPreviewable previewable)
		{
			if (previewable == null)
				throw new NullReferenceException($"Target {nameof(IPreviewable)} is missing");

			if (registered.Contains(previewable))
				throw new InvalidOperationException($"{previewable} already registered");
			
			registered.Add(previewable);
		}

		public void UnRegistration(IPreviewable previewable)
		{
			if (previewable == null)
				throw new NullReferenceException($"Target {nameof(IPreviewable)} is missing");

			if (!registered.Contains(previewable))
			{
				Debug.LogError("{hoverable} does not registered");
				return;
			}
			
			if (Current == previewable)
				Close();
			
			registered.Remove(previewable);
		}
		
		public void Preview(IPreviewable previewable)
		{
			if (Current != null)
				Close();
			
			if (previewable == null || !previewable.PreviewSettings.Enabled)
				return;

			if (!registered.Contains(previewable))
				throw new InvalidOperationException($"{previewable} does not registered");

			Current = previewable;
			// Example
			switch (Current.PreviewSettings.PreviewType)
			{
				case PreviewType.Pile:
				case PreviewType.Hand:
				case PreviewType.Graveyard:
					previewView.InitAndShowCenter(Current.PreviewData, Current.PreviewSettings.PreviewTime);
					break;
				
				case PreviewType.Creature:
					previewView.InitAndShowSide(Current.PreviewData, Current.PreviewSettings.PreviewTime);
					break; 
				
				default: previewView.InitAndShowCorner(Current.PreviewData, Current.PreviewSettings.PreviewTime);
					break;
			}
			// Example
			OnPreview?.Invoke();
		}

		public void Close()
		{
			if (Current == null) 
				return;

			Current = null;
			previewView.Close();
			OnClose?.Invoke();
		}

		public void Clear()
		{
			Close();
			registered.Clear();
			Current = null;
			OnPreview = null;
			OnClose = null;
		}

		private bool GetPreviewable(PreviewType type, out IPreviewable previewable)
		{
			return registered.ToArray().TryGet(x => x.PreviewSettings.PreviewType == type
													&& x.PreviewSettings.Enabled 
													&& x.CanPreview()
													&& InputHelper.IsPointerOver(x.TargetView), out previewable);
		}
		
		private void OnPreviewInput(IInputContext context, PreviewType type)
		{
			context = inputContextProcessor.Process(context);

			if (Current != null)
			{
				if (Current.PreviewSettings.PreviewType != type)
					return;
				
				if (context.Canceled || !InputHelper.IsPointerOver(Current.TargetView))
					Close();
				
				return;
			}

			if ((context.Started || context.Performed) && GetPreviewable(type, out var previewable))
				Preview(previewable);
		}
	}
}