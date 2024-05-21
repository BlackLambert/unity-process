# unity-process
Basic process logic for the Unity Engine

## What is it good for?
This unity package is the basis for processes of all kind like scene changes or network communication.

## How to use it?
- Create a custom Process for your purpose by implementing the `Process` interface
- Add the Process to the `ProcessQueue`
- The `ProcessStarter` will pick the queued process up and start it.
- You can monitor the current Process by injecting the `Observable<Process>` into your class

## Keep in mind...
### ...that this package is based on this [dependency injection framwork](https://github.com/BlackLambert/unity-di-package)
The `ProcessQueue`, `Observable<Process>` and `ProcessStarter` need to be installed before they can be used.
Installer example:
```C#
public class ProcessInstaller : MonoInstaller
{
    public override void InstallBindings(Binder binder)
    {
        binder.Bind<ProcessQueue>()
            .ToNew<BasicProcessQueue>()
            .AsSingle();

        binder.BindToNewSelf<Observable<Process>>();
        
        binder.BindComponent<BasicProcessStarter>()
            .FromNewComponentOnNewGameObject("ProcessStarter", transform)
            .AsNonResolvable();
    }
}
```
