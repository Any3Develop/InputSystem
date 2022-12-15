namespace InputSystem.PreviewSystem.Abstraction
{
	public interface IPreviewSetting
	{
		PreviewType PreviewType { get; }
		
		float PreviewTime { get; }
		
		bool Enabled { get; set; }
	}
}