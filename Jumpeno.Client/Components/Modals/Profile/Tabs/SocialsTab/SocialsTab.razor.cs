namespace Jumpeno.Client.Components;

public partial class SocialsTab : IProfileTab
{
    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set("socials-tab", Base);

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public async Task ConnectGoogle()
    {
        await Task.CompletedTask;
        _ = this;
        // TODO: Implement Google OAuth connection logic
        throw new NotImplementedException();
    }

    public async Task ConnectFacebook()
    {
        await Task.CompletedTask;
        _ = this;
        // TODO: Implement Facebook OAuth connection logic
        throw new NotImplementedException();
    }

    public Task ResetForm()
    {
        return Task.CompletedTask;
    }
}
