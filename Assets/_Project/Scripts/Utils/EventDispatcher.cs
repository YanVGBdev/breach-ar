using UnityEngine;
using System;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.Utils
{
    /// <summary>
    /// Simple event dispatcher for global events
    /// </summary>
    public class EventDispatcher : MonoBehaviour
    {
        private Dictionary<string, Delegate> events;

        [Inject]
        private void Initialize()
        {
            events = new Dictionary<string, Delegate>();
        }

        /// <summary>
        /// Subscribe to an event
        /// </summary>
        public void Subscribe(string eventName, Action handler)
        {
            if (!events.ContainsKey(eventName))
            {
                events[eventName] = handler;
            }
            else
            {
                events[eventName] = Delegate.Combine(events[eventName], handler);
            }
        }

        /// <summary>
        /// Subscribe to an event with data
        /// </summary>
        public void Subscribe<T>(string eventName, Action<T> handler)
        {
            string key = $"{eventName}_{typeof(T).Name}";
            if (!events.ContainsKey(key))
            {
                events[key] = handler;
            }
            else
            {
                events[key] = Delegate.Combine(events[key], handler);
            }
        }

        /// <summary>
        /// Unsubscribe from an event
        /// </summary>
        public void Unsubscribe(string eventName, Action handler)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName] = Delegate.Remove(events[eventName], handler);
                if (events[eventName] == null)
                {
                    events.Remove(eventName);
                }
            }
        }

        /// <summary>
        /// Unsubscribe from an event with data
        /// </summary>
        public void Unsubscribe<T>(string eventName, Action<T> handler)
        {
            string key = $"{eventName}_{typeof(T).Name}";
            if (events.ContainsKey(key))
            {
                events[key] = Delegate.Remove(events[key], handler);
                if (events[key] == null)
                {
                    events.Remove(key);
                }
            }
        }

        /// <summary>
        /// Dispatch an event
        /// </summary>
        public void Dispatch(string eventName)
        {
            if (events.ContainsKey(eventName))
            {
                (events[eventName] as Action)?.Invoke();
            }
        }

        /// <summary>
        /// Dispatch an event with data
        /// </summary>
        public void Dispatch<T>(string eventName, T data)
        {
            string key = $"{eventName}_{typeof(T).Name}";
            if (events.ContainsKey(key))
            {
                (events[key] as Action<T>)?.Invoke(data);
            }
        }

        /// <summary>
        /// Clear all events
        /// </summary>
        public void Clear()
        {
            events.Clear();
        }
    }

    /// <summary>
    /// Event names constants
    /// </summary>
    public static class GameEvents
    {
        // Gameplay
        public const string WAVE_STARTED = "wave_started";
        public const string WAVE_COMPLETED = "wave_completed";
        public const string FRAGMENT_KILLED = "fragment_killed";
        public const string RIFT_CLOSED = "rift_closed";
        public const string CORE_DAMAGED = "core_damaged";
        public const string COMBO_CHANGED = "combo_changed";
        public const string SCORE_CHANGED = "score_changed";
        public const string POWERUP_COLLECTED = "powerup_collected";
        public const string BOSS_DEFEATED = "boss_defeated";
        public const string GAME_OVER = "game_over";

        // UI
        public const string MENU_OPENED = "menu_opened";
        public const string MENU_CLOSED = "menu_closed";
        public const string PAUSE_TOGGLED = "pause_toggled";

        // System
        public const string SETTINGS_CHANGED = "settings_changed";
        public const string SAVE_COMPLETED = "save_completed";
        public const string LOAD_COMPLETED = "load_completed";
    }
}
