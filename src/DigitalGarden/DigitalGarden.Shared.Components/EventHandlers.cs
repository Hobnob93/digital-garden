using Microsoft.AspNetCore.Components;

namespace DigitalGarden.Shared.Components;

[EventHandler("onanimationend", typeof(EventArgs),
    enableStopPropagation: true, enablePreventDefault: true)]
public class EventHandlers
{
}
