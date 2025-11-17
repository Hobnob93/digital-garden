using DigitalGarden.Shared.Components.Builders;
using Microsoft.AspNetCore.Components.Web;

namespace DigitalGarden.Shared.Components.Layout;

public partial class Nav
{
    private bool MenuButtonActive { get; set; }
    private bool MenuButtonInteracted { get; set; }

    private void LinkClicked(MouseEventArgs eventArgs)
    {
        MenuButtonActive = false;
        StateHasChanged();
    }

    private void MenuToggleClicked(MouseEventArgs eventArgs)
    {
        MenuButtonInteracted = true;
        MenuButtonActive = !MenuButtonActive;
        StateHasChanged();
    }

    private string NavListClasses => new ClassBuilder()
        .Add("menu-show", condition: MenuButtonActive)
        .Build();

    private string MenuSpanClasses => new ClassBuilder()
        .Add("menu-button")
        .Add("interacted", condition: MenuButtonInteracted)
        .Add("active", condition: MenuButtonActive)
        .Build();
}
