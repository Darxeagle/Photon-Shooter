using PhotonShooter.Scripts;
using PhotonShooter.Scripts.Connection;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ConnectionManager>().AsSingle();
        Container.Bind<MatchManager>().AsSingle();
        Container.Bind<ProjectController>().AsSingle().NonLazy();
    }
}