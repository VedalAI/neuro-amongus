using System;

namespace Neuro.Recording.Common;

public static class TaskTypeExtensions
{
    public static TaskType ForMessage(this TaskTypes taskType)
    {
        return Enum.TryParse(taskType.ToString(), out TaskType result) ? result : TaskType.NoneTaskType;
    }
}
