class JSLocalStorage {    
    static Get(key) {
        if (!key) throw new Error(ERROR.PARAMETERS_REQUIRED)
        return localStorage.getItem(key)        
    }
    
    static Set(key, value) {
        if (!key || (!value && value != "")) throw new Error(ERROR.PARAMETERS_REQUIRED)
        localStorage.setItem(key, value)
    }
    
    static Delete(key) {
        if (!key) throw new Error(ERROR.PARAMETERS_REQUIRED)
        localStorage.removeItem(key)
    }
}

window.JSLocalStorage = JSLocalStorage
