public interface IThemeFactory
{
    IEntity CreateEntity();
    IStateMachine CreateStateMachine();
    ITask CreateTask();
}
