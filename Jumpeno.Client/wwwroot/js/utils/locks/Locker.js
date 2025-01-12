class Locker {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    #Mutex = new Mutex()

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    Lock = async () => await this.#Mutex.Lock()
    Unlock = () => this.#Mutex.Unlock()

    // Callbacks --------------------------------------------------------------------------------------------------------------------------
    async Exclusive(callback) {
        var token = new LockToken(this.Unlock)
        await this.Lock()
        try { return await callback(token) }
        finally { token.Unlock() }
    }

    async TryExclusive(callback, fallback = undefined) {
        try { return await this.Exclusive(callback) }
        catch { return fallback }
    }
}
