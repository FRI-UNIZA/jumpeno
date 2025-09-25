namespace Jumpeno.Client.Interfaces;

// Base -----------------------------------------------------------------------------------------------------------------------------------
public interface IPreRendered : IRenderable {
    public bool IsPrerendered { get; }
    public Task<bool> PreRender(object? @params = null) => throw new NotImplementedException();
}

// Generic --------------------------------------------------------------------------------------------------------------------------------
public interface IPreRendered<T> : IRenderable<T>, IPreRendered {
    public async new sealed Task<bool> PreRender(object? @params = null) {
        if (@params is not T parameters) return false;
        return await PreRender(parameters);
    }
    public Task<bool> PreRender(T @params);
}
