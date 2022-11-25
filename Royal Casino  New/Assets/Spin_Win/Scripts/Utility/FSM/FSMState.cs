abstract public class FSMState  <T>   
{
	abstract public void Enter (T owner);
	abstract public void Execute();
	abstract public void Exit();
}
