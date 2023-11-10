namespace az_appservice_dotnet.services;

public struct TaskId
{
    private int _id;
    public static implicit operator TaskId(int id) => new TaskId { _id = id };
    public static implicit operator int(TaskId taskId) => taskId._id;
}