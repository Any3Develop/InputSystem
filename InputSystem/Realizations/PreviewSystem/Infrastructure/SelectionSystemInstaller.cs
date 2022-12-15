using Zenject;

namespace InputSystem.PreviewSystem.Infrastructure
{
	public class PreviewSystemInstaller : Installer<PreviewSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container
				.BindInterfacesTo<PreviewSystem>()
				.AsSingle()
				.NonLazy();
		}
	}
}