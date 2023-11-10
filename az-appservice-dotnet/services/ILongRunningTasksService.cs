using az_appservice_dotnet.models;
using az_appservice_dotnet.services.v1;

namespace az_appservice_dotnet.services;

public interface ILongRunningTasksService
{
    public TaskId StartLongRunningTasksAsync(uint seconds);
    /// <summary>
    ///   Returns the progress of the task in percentage.
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns>-1 if task not found</returns>
    public int GetLongRunningTaskProgress(TaskId taskId);
    public void CancelLongRunningTask(TaskId taskId);
    public List<TaskId> GetLongRunningTasks();
}