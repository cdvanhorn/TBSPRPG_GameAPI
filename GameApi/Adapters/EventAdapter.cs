using TbspRpgLib.Events;
using TbspRpgLib.Events.Content;
using GameApi.Entities;

namespace GameApi.Adapters {
    public interface IEventAdapter {
        Event NewGameEvent(Game game);
    }

    public class EventAdapter : IEventAdapter{
        public Event NewGameEvent(Game game) {
            NewGame data = new NewGame();
            data.Id = game.Id.ToString();
            data.UserId = game.UserId.ToString();
            data.AdventureId = game.Adventure.Id.ToString();
            data.AdventureName = game.Adventure.Name;

            NewGameEvent evt = new NewGameEvent(data);
            return evt;
        }
    }
}