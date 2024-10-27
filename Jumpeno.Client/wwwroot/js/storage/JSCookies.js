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
    
    static Set(key, value, expires = null, domain, path, secure = true, sameSite = null) {
        if (!key || (!value && value != "") || !domain || !path) {
            throw new Error(ERROR.PARAMETERS_REQUIRED)
        }
        expires = expires ? `; expires=${expires}` : ''
        secure = secure ? '; secure' : ''
        sameSite = sameSite ? `; SameSite=${sameSite}` : ''
        document.cookie = `${key}=${value}${expires}; domain=${domain}; path=${path}${secure}${sameSite}`
    }
    
    static Delete(key, domain, path) {
        if (!key || !domain || !path) throw new Error(ERROR.PARAMETERS_REQUIRED)
        document.cookie = `${key}=; Max-Age=-99999999; domain=${domain}; path=${path}`
    }
}

window.JSCookies = JSCookies
