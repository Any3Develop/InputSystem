using System;

namespace InputSystem.PreviewSystem.Abstraction
{
	public interface IPreviewSystem
	{
		IPreviewable Current { get; }
		
		event Action OnPreview;
		
		event Action OnClose;
		
		void Registration(IPreviewable previewable);
		
		void UnRegistration(IPreviewable previewable);
		
		void Preview(IPreviewable previewable);
		
		void Close();

		void Clear();
	}
}