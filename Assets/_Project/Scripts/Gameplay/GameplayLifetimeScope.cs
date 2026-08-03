using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Gameplay lifetime scope for VContainer DI
    /// Registers per-session services
    /// Referência: 99_agent_rules.md - DI via VContainer
    /// </summary>
    public class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Gameplay services
            builder.Register<SessionStateMachine>(Lifetime.Scoped);
            builder.Register<Combo.ComboSystem>(Lifetime.Scoped);
            builder.Register<Combo.ScoreSystem>(Lifetime.Scoped);
            builder.Register<Core.CoreController>(Lifetime.Scoped);

            // AI
            builder.Register<AI.DifficultyDirector>(Lifetime.Scoped);
            builder.Register<AI.WaveGenerator>(Lifetime.Scoped);
            builder.Register<AI.RiftSpawnDirector>(Lifetime.Scoped);

            Debug.Log("[VContainer] GameplayLifetimeScope configured");
        }
    }
}
