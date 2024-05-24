using SBaier.DI;

namespace SBaier.Process
{
    public class BasicProcessInstaller : MonoInstaller
    {
        public override void InstallBindings(Binder binder)
        {
            binder.Bind<ProcessQueue>()
                .ToNew<BasicProcessQueue>()
                .AsSingle();

            binder.Bind<Observable<Process>>()
                .And<ReadonlyObservable<Process>>()
                .ToNew<Observable<Process>>()
                .AsSingle();
        
            binder.BindComponent<BasicProcessStarter>()
                .FromNewComponentOnNewGameObject("ProcessStarter", transform)
                .AsNonResolvable();
        }
    }
}