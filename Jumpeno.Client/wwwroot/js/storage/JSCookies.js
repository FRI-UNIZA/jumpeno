class JSCookies {
    static Get(key) {
        if (!key) throw new Error(ERROR.PARAMETERS_REQUIRED)
        const keyEQ = `${key}=`
        const arr = document.cookie.split(';')
        for (let i = 0; i < arr.length; i++) {
            let cookie = arr[i].trim()
            if (cookie.indexOf(keyEQ) == 0) {
                return cookie.substring(keyEQ.length).trim()
            }
        }
        return null;
    }
    
    static Set(key, value, expires = null, domain = null, path, secure = true, sameSite = null) {
        if (!key || (!value && value != "") || !path) {
            throw new Error(ERROR.PARAMETERS_REQUIRED)
        }
        expires = expires ? `; expires=${expires}` : ''
        domain = domain ? `; domain=${domain}` : ''
        secure = secure ? '; secure' : ''
        sameSite = sameSite ? `; SameSite=${sameSite}` : ''
        document.cookie = `${key}=${value}${expires}${domain}; path=${path}${secure}${sameSite}`
    }
    
    static Delete(key, domain = null, path) {
        if (!key || !path) throw new Error(ERROR.PARAMETERS_REQUIRED)
        domain = domain ? `; domain=${domain}` : ''
        document.cookie = `${key}=; Max-Age=-99999999${domain}; path=${path}`
    }
}

window.JSCookies = JSCookies
