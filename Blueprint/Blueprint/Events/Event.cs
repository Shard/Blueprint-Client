using System;
using System.Collections.Generic;

namespace Blueprint.Events
{
    class Event
    {
        public bool Active;
        public enum Triggers
        {
            PlayerNear,
            PlayerInteract
        }
        public Triggers Trigger;

        public enum Actions
        {
            OpenBank,
            Animation,
            Solid
        }
        public Actions Action;
        public string ActionData;

        public Event(Triggers trigger, Actions action, string action_data = "")
        {
            Trigger = trigger;
            Action = action;
            ActionData = action_data;
            Active = false;
        }

        public void TriggerEvent()
        {
            Active = true;
        }
    }
}
