namespace etrade_core.infrastructure.CustomMessageQueue.Enums
{
    public enum MessagePattern
    {
        SendAndForget,
        SendAndWait,
        PublishToQueue,
        PublishToAll,
        ScheduleSend,
        SchedulePublish
    }
}