using System.Collections.Generic;
using SBaier.DI;

namespace SBaier.Process.Samples
{
    public class ProcessWithDurationInstaller : MonoInstaller
    {
        public override void InstallBindings(Binder binder)
        {
            binder.BindInstance(new List<ProcessArguments>());
            binder.BindInstance(new Observable<ProcessArguments>());
            binder.BindComponent<CoroutineHelper>()
                .FromNewComponentOnNewGameObject("CoroutineHelper", transform, false);
        }
    }
}