namespace az_appservice_dotnet.models.v1;

public class LongRunningWorkloads: ILongRunningWorkload
{
    private readonly uint _duration;
    private uint _current;
    
    public LongRunningWorkloads(uint seconds)
    {
        _duration = seconds;
        _current = 0;
    }

    public void Next(uint seconds)
    {
        uint next = _current + seconds;
        _current = next > _duration ? _duration : next;
    }

    public uint Progress => _current * 100 / _duration;
}