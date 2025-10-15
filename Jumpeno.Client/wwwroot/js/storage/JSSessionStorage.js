class JSSessionStorage {    
    static Get(key) {
        if (!key) throw new Error(ERROR.PARAMETERS_REQUIRED)
        return sessionStorage.getItem(key)        
    }
    
    static Set(key, value) {
        if (!key || (!value && value != "")) throw new Error(ERROR.PARAMETERS_REQUIRED)
        sessionStorage.setItem(key, value)
    }
    
    static Delete(key) {
        if (!key) throw new Error(ERROR.PARAMETERS_REQUIRED)
        sessionStorage.removeItem(key)
    }
}

window.JSSessionStorage = JSSessionStorage
