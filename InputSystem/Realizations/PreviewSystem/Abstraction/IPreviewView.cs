namespace InputSystem.PreviewSystem.Abstraction
{
	public interface IPreviewView // Some preview view
	{
		void InitAndShowSide(IObjectData data, float lifeTime = -1);

		void InitAndShowCenter(IObjectData data, float lifeTime = -1);
		
		void InitAndShowCorner(IObjectData data, float lifeTime = -1);
		
		void Close(bool noAnimation = true);
	}
}