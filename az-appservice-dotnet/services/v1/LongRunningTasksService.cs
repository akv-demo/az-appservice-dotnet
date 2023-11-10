using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using az_appservice_dotnet.models;

[assembly: InternalsVisibleTo("az-appservice-dotnet.xUnit")]
namespace az_appservice_dotnet.services.v1;

public class LongRunningTasksService: ILongRunningTasksService
{
    private readonly ILongRunningWorkloadFactory _longRunningWorkloadFactory;

    private readonly ConcurrentDictionary<int, Task<ILongRunningWorkload>> _taskDictionary;
    private readonly ConcurrentDictionary<int, CancellationTokenSource> _cancellationTokenSourceDictionary;
    private readonly object _lockUpdateDictionaries;
    
    public LongRunningTasksService(ILongRunningWorkloadFactory longRunningWorkloadFactory)
    {
        _lockUpdateDictionaries = new object();
        // TODO: investigate the cost of allocating TaskId and probably replace int with TaskId
        _taskDictionary = new ConcurrentDictionary<int, Task<ILongRunningWorkload>>();
        _cancellationTokenSourceDictionary = new ConcurrentDictionary<int, CancellationTokenSource>();
        _longRunningWorkloadFactory = longRunningWorkloadFactory;
    }
    
    public TaskId StartLongRunningTasksAsync(uint seconds)
    {
        var workload = _longRunningWorkloadFactory.Create(seconds);
        var cancellationTokenSource = new CancellationTokenSource();
        Task<ILongRunningWorkload> task = Task.Run(() =>
        {
            var spent = 0;
            while (spent < seconds)
            {
                workload.Next(1);
                Thread.Sleep(1000);
                spent++;
            }
            return workload;
        }, cancellationTokenSource.Token);
        lock (_lockUpdateDictionaries) {
          _taskDictionary.TryAdd(task.Id, task);
          _cancellationTokenSourceDictionary.TryAdd(task.Id, cancellationTokenSource);
        }
        return task.Id;
    }
    
    public int GetLongRunningTaskProgress(TaskId taskId)
    {
        return _taskDictionary.TryGetValue(taskId, out Task<ILongRunningWorkload>? task) ? (int)task.Result.Progress : -1;
    }
    
    public void CancelLongRunningTask(TaskId taskId)
    {
        if (_cancellationTokenSourceDictionary.TryGetValue(taskId, out CancellationTokenSource? cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
        }
    }

    public List<TaskId> GetLongRunningTasks()
    {
        // TODO: The using Select to convert int to TaskId instead of using a TaskId as the key in the dictionary
        //       is caused by a lack of the knowledge of the cost of using TaskId as the key in the dictionary.
        //       Investigate the cost of using TaskId as the key in the dictionary and replace int with TaskId
        return _taskDictionary.Keys.Select(id => (TaskId)id).ToList();
    }
    
    protected internal ConcurrentDictionary<int, Task<ILongRunningWorkload>> GetTaskDictionaryTest()
    {
        return _taskDictionary;
    }
}