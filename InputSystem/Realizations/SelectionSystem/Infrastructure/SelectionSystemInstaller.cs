using Zenject;

namespace InputSystem.SelectionSystem.Infrastructure
{
	public class SelectionSystemInstaller : Installer<SelectionSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container
				.BindInterfacesTo<SelectionSystem>()
				.AsSingle()
				.NonLazy();
		}
	}
}