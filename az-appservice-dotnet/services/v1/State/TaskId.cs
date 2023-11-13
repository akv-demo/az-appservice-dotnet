namespace az_appservice_dotnet.services.v1.State;

/*
 * <summary>
 * I prefer to wrap ID types in order to have ability to change the underlying type in the future.
 *
 * It is not within IProcessingStateService because of historical reasons ans suspicion
 * that it will be used in other places someday.
 * </summary>
 */
public struct TaskId
{
    private int _id;
    public static implicit operator TaskId(int id) => new TaskId { _id = id };
    public static implicit operator int(TaskId taskId) => taskId._id;
}