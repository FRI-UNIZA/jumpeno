namespace Jumpeno.Client.Interfaces;

/// <summary>Implement to properly transition disabled state with no animation.</summary>
public interface IDisabledComponent {
    public bool Disabled { get; }
    public void Notify();
}
