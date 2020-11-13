using GameApi.Events;
using GameApi.Events.Data;
using GameApi.Entities;

namespace GameApi.Adapters {
    public interface IEventAdapter {
        Event NewGameEvent(Game game);
    }

    public class EventAdapter : IEventAdapter{
        public Event NewGameEvent(Game game) {
            NewGame data = new NewGame();
            data.Id = game.Id;
            data.UserId = game.UserId;
            data.AdventureId = game.Adventure.Id;
            data.AdventureName = game.Adventure.Name;

            Event evnt = new Event();
            evnt.Type = Event.NEW_GAME_EVENT_TYPE;
            evnt.Data = data;
            return evnt;
        }
    }
}