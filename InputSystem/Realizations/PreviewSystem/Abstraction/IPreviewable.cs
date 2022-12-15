using UnityEngine;

namespace InputSystem.PreviewSystem.Abstraction
{
	public interface IPreviewable
	{
		/// <summary>
		/// Target of some reference view
		/// </summary>
		GameObject TargetView { get; }
		
		IObjectData PreviewData { get; }
		
		IPreviewSetting PreviewSettings { get; }
		
		bool CanPreview();
	}
}