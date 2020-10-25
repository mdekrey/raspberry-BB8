using System;
using System.Linq;
using System.Reactive.Linq;

namespace BB8.Gamepad
{
    public interface IGamepad
    {
        string Name { get; }
        IObservable<(GamepadState state, GamepadEventArgs eventArgs)> GamepadStateChanged { get; }
    }

    public static class GamepadExtensions
    {
        public static IObservable<EventedGamepad> BufferEvents(this IGamepad gamepad, TimeSpan bufferDuration) =>
            gamepad.GamepadStateChanged
                .Buffer(bufferDuration, System.Reactive.Concurrency.TaskPoolScheduler.Default)
                .Where(changes => changes.Any())
                .Select(stateChanges => new EventedGamepad(gamepad.Name, state: stateChanges.Last().state, eventArgs: stateChanges.Select(change => change.eventArgs).ToArray()))
                .StartWith(new EventedGamepad(name: gamepad.Name, state: GamepadState.Empty, eventArgs: Array.Empty<GamepadEventArgs>()));
    }
}