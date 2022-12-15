namespace InputSystem.Abstraction
{
	public interface IInputContextProcessor
	{
		IInputContext Process(IInputContext context);
	}
}