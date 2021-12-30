using System;

namespace WorkTimeStat.Events
{
    public class TooltipChangeEventArgs : EventArgs
    {
        private readonly string toolTipMessage;

        public TooltipChangeEventArgs(string toolTipMessage)
        {
            this.toolTipMessage = toolTipMessage ?? throw new ArgumentNullException(nameof(toolTipMessage));
        }

        public string GetMessage()
        {
            return toolTipMessage;
        }
    }

    internal delegate void ChangeTaskbarTooltipEventHandler(TooltipChangeEventArgs args);
}
