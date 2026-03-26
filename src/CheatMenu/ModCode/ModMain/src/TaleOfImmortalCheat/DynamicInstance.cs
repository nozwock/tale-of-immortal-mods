namespace TaleOfImmortalCheat;

public class DynamicInstance<T> where T : class
{
	private volatile T PreviousInstance;

	private volatile T CurrentInstace;

	public T Value
	{
		get
		{
			return CurrentInstace;
		}
		set
		{
			CurrentInstace = value;
		}
	}

	public bool HasChanged
	{
		get
		{
			bool result = CurrentInstace != PreviousInstance;
			PreviousInstance = CurrentInstace;
			return result;
		}
	}
}
