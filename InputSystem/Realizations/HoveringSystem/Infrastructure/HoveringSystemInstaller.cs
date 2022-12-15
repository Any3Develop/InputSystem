using Zenject;

namespace InputSystem.HoveringSystem.Infrastructure
{
	public class HoveringSystemInstaller : Installer<HoveringSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container
				.BindInterfacesTo<HoveringSystem>()
				.AsSingle()
				.NonLazy();
		}
	}
}