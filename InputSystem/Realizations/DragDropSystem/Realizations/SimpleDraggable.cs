using InputSystem.DragDropSystem.Abstraction;
using InputSystem.Utils;
using UnityEngine;

namespace InputSystem.DragDropSystem
{
	public class SimpleDraggable : DisposableWithCts, IDraggable
	{
		public GameObject TargetView { get; private set; }
		public IDragSettings DragSettings { get; private set; }
		
		public SimpleDraggable Initialize(GameObject draggableView, IDragSettings dragSettings)
		{
			DragSettings = dragSettings;
			TargetView = draggableView;
			return this;
		}

		public virtual bool CanDrag()
		{
			return true;
		}

		public virtual bool CanDropIn(IDropHandler place)
		{
			return true;
		}
	}
}