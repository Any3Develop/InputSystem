using System;
using System.Collections.Generic;
using System.Linq;
using InputSystem.Abstraction;
using InputSystem.HoveringSystem.Abstraction;
using InputSystem.Utils;
using InputSystem.Utils;
using DG.Tweening;
using Zenject;

namespace InputSystem.HoveringSystem
{
	public class HoveringSystem : IHoveringSystem, IDisposable, IInitializable
	{
		private readonly IInputController<HoveringSystemActions> inputController;
		private readonly IInputContextProcessor inputContextProcessor;

		private class HoveredInfo
		{
			public int FromSublingIndex;
			public bool Hovered;
			public Tween Hovering;
		}
		private readonly Dictionary<IHoverable, HoveredInfo> hoverables;
		public event Action<IHoverable> OnHoverEnter;
		public event Action<IHoverable> OnHoverExit;

		public HoveringSystem(IInputController<HoveringSystemActions> inputController, IInputContextProcessor inputContextProcessor)
		{
			this.inputController = inputController;
			this.inputContextProcessor = inputContextProcessor;
			hoverables = new Dictionary<IHoverable, HoveredInfo>();
		}
		
		public void Initialize()
		{
			inputController.GetAll().ForEach(input =>
			{
				input.OnStarted += OnHoveringInput;
				input.OnPerformed += OnHoveringInput;
				input.OnCanceled += OnHoveringInput;
			});
		}
		
		public void Dispose()
		{
			OnHoverEnter = null;
			OnHoverExit = null;
		}
		
		public void Registration(IHoverable hoverable)
		{
			if (hoverable == null)
				throw new NullReferenceException($"Target {nameof(IHoverable)} is missing");

			if (hoverables.ContainsKey(hoverable))
				throw new InvalidOperationException($"{hoverable} already registered");
			
			hoverables.Add(hoverable, new HoveredInfo{Hovered = false});
		}

		public void UnRegistration(IHoverable hoverable)
		{
			if (hoverable == null)
				throw new NullReferenceException($"Target {nameof(IHoverable)} is missing");

			if (!hoverables.ContainsKey(hoverable))
			{
				Debug.LogError("{hoverable} does not registered");
				return;
			}

			Cancel(hoverable);
			hoverables.Remove(hoverable);
		}
		
		public void Start(IHoverable hoverable)
		{
			if (hoverable == null)
				return;

			if (!hoverable.HoverableSetting.Enabled)
				return;
			
			if (!hoverables.TryGetValue(hoverable, out var hoveredInfo) || hoveredInfo is not { Hovered: false })
					return;
			
			hoverables.ToArray()
				.Where(x=> x.Value.Hovered)
				.ForEach(x => Cancel(x.Key));
			
			if (!hoverable.HoverableSetting.UseScaleUpAnimation)
			{
				hoveredInfo.Hovered = true;
				OnHoverEnter?.Invoke(hoverable);
				return;
			}
			
			hoveredInfo.FromSublingIndex = hoverable.SublingIndex;
			hoveredInfo.Hovering?.Kill();
			
			if (hoverable.HoverableSetting.ChangeSublingIndex)
				hoverable.SublingIndex = hoverable.HoverableSetting.MaxSublingIndex;
			
			hoveredInfo.Hovering = GetHoveringTween(hoverable);
			hoveredInfo.Hovered = true;
			OnHoverEnter?.Invoke(hoverable);
		}

		public void Cancel(IHoverable hoverable = null)
		{
			if (hoverable == null)
			{
				hoverables
					.ToArray()
					.Where(x=> x.Value.Hovered && x.Key != null)
					.ForEach(x => Cancel(x.Key));
				return;
			}
			
			if (!hoverables.TryGetValue(hoverable, out var hoveredInfo) || hoveredInfo is not { Hovered: true })
				return;
			
			if (!hoverable.HoverableSetting.UseScaleUpAnimation)
			{
				hoveredInfo.Hovered = false;
				OnHoverExit?.Invoke(hoverable);
				return;
			}
			
			hoveredInfo.Hovering?.Kill();
			
			if (hoverable.HoverableSetting.ChangeSublingIndex)
				hoverable.SublingIndex = hoveredInfo.FromSublingIndex;

			hoveredInfo.Hovering = GetHoveringTween(hoverable, false);
			hoveredInfo.Hovered = false;
			OnHoverExit?.Invoke(hoverable);
		}

		public void Clear()
		{
			Cancel();
			hoverables.Clear();
			OnHoverEnter = null;
			OnHoverExit = null;
		}

		private Tween GetHoveringTween(IHoverable hoverable, bool start = true)
		{
			if (hoverable == null)
				return default;
			
			return DOTween.To(() => hoverable.Size
					, value => hoverable.Size = value
					, start ? hoverable.HoverableSetting.DefaultSize * hoverable.HoverableSetting.MaxSize : hoverable.HoverableSetting.DefaultSize
					, hoverable.HoverableSetting.Duration)
				.SetAutoKill(true);
		}
		
		private bool GetHoverable(out IHoverable hoverable)
		{
			return hoverables.Keys.ToArray().TryGet(x => x.HoverableSetting.Enabled && x.CanHover() && InputHelper.IsPointerOver(x.TargetView), out hoverable);
		}
		
		private void OnHoveringInput(IInputContext context)
		{
			context = inputContextProcessor.Process(context);
			
			if ((context.Started || context.Performed) && GetHoverable(out var hoverable))
			{
				Start(hoverable);
				return;
			}
			
			if (context.Canceled || !GetHoverable(out _))
				Cancel();
		}
	}
}