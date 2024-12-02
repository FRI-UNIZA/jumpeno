class Mutex {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    #Mutex = Promise.resolve()
    #Resolve = () => {}

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    Lock = () => new Promise((resolve) => {
        this.#Mutex = this.#Mutex.then(() => new Promise(unlock => {
            this.#Resolve = unlock
            resolve()
        }))
    })

    Unlock = () => this.#Resolve()
}
