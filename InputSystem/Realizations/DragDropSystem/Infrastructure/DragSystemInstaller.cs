using Zenject;

namespace InputSystem.DragDropSystem.Infrastructure
{
	public class DragSystemInstaller : Installer<DragSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container
				.BindInterfacesTo<DragDropSystem>()
				.AsSingle()
				.NonLazy();

			Container
				.BindInterfacesTo<DragDropInputHandler>()
				.AsSingle()
				.NonLazy();
		}
	}
}