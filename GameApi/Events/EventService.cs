//this will abstract sending events to EventStore,
//so can change to different event store if necessary

namespace GameApi.Events
{
    public interface IEventService
    {
        void SendEvent(Event evnt);
    }
}