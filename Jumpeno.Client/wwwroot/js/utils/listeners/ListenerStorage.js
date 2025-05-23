class ListenerStorage {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    #Listeners
    #Event
    #Before
    #CreateArgs
    #After
    #Lock = new Locker()

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    constructor(event, createArgs = () => null, before = null, after = null) {
        this.#Listeners = {}
        this.#Event = event
        this.#Before = before
        this.#CreateArgs = createArgs
        this.#After = after
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    #GetID(objRef, method) {
        return `${objRef._id}-${method}`
    }

    #CreateListener(objRef, method) {
        return { objRef, method }
    }

    #InvokeListeners = async (e) => {
        await this.#Lock.TryExclusive(async () => {
            if (this.#Before) this.#Before()
            for (const listener of Object.values(this.#Listeners)) {
                await listener.objRef.invokeMethodAsync(listener.method, this.#CreateArgs(e))
            }
            if (this.#After) this.#After()
        })
    }
    
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    async AddEventListener(objRef, method, options = undefined) {
        await this.#Lock.Exclusive(async () => {
            const listener = this.#CreateListener(objRef, method)
            this.#Listeners[this.#GetID(objRef, method)] = listener
            if (Object.keys(this.#Listeners).length > 1) return
            window.addEventListener(this.#Event, this.#InvokeListeners, options)
        })
    }

    async RemoveEventListener(objRef, method, options = undefined) {
        await this.#Lock.Exclusive(async () => {
            delete this.#Listeners[this.#GetID(objRef, method)]
            if (Object.keys(this.#Listeners).length > 0) return
            window.removeEventListener(this.#Event, this.#InvokeListeners, options)
        })
    }
}
