class JSImage {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    static #BACKGROUND_COMPONENT_CLASS = 'background'
    static #COMPONENT_CLASS = 'image'
    static #BACKGROUND_ELEMENT_CLASS = 'background-element'
    static #ELEMENT_CLASS = 'image-element'
    static #LOADING_CLASS = 'loading'
    static #ERROR_CLASS = 'error'

    // Events -----------------------------------------------------------------------------------------------------------------------------
    static #InvokeOnLoad(id, component, jsOnly) {
        component.classList.remove(this.#LOADING_CLASS)
        if (jsOnly) return
        DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.IMAGE.ON_LOAD, id)
    }

    static #InvokeOnError(id, component, jsOnly) {
        if (!component.classList.contains(this.#ERROR_CLASS)) {
            component.classList.add(this.#ERROR_CLASS)
        }
        if (jsOnly) return
        DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.IMAGE.ON_ERROR, id)
    }

    // Theme update -----------------------------------------------------------------------------------------------------------------------
    static #FindCurrentTheme(text, suffix) {
        let index = text.lastIndexOf(suffix)
        if (index < 0) return null
        const endIndex = index + suffix.length
        do { index-- } while (text[index] != '-')
        return text.substring(index, endIndex)
    }

    static UpdateTheme(theme, suffix) {
        // 1) Images:
        let imageComponents = document.querySelectorAll(`.${this.#COMPONENT_CLASS}`)
        imageComponents.forEach(component => {
            const img = component.querySelector(`.${this.#ELEMENT_CLASS}`)
            if (!img) return true
            let src = img?.src
            if (!src) return true
            const currentTheme = this.#FindCurrentTheme(src, suffix)
            if (!currentTheme) return true
            src = src.replace(currentTheme, `-${theme}`)
            img.src = src
        })

        // 2) Backgrounds:
        imageComponents = document.querySelectorAll(`.${this.#BACKGROUND_COMPONENT_CLASS}`)
        imageComponents.forEach(component => {
            const background = component.querySelector(`.${this.#BACKGROUND_ELEMENT_CLASS}`)
            if (!background) return true
            let style = background?.style?.cssText
            if (!style) return true
            const currentTheme = this.#FindCurrentTheme(style, suffix)
            if (!currentTheme) return true
            style = style.replace(currentTheme, `-${theme}`)
            background.style = style
        })
    }

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    static Init(id, jsOnly) {
        const component = document.getElementById(id);
        if (!component) return
        const img = component.querySelector(`.${this.#ELEMENT_CLASS}`)
        if (!img) return

        if (img.complete) {
            if (img.naturalWidth > 0) this.#InvokeOnLoad(id, component, jsOnly)
            else this.#InvokeOnError(id, component, jsOnly)
        } else {
            img.addEventListener('load', () => this.#InvokeOnLoad(id, component, jsOnly))
            img.addEventListener('error', () => this.#InvokeOnError(id, component, jsOnly))
        }
    }

    static InitAll() {
        let imageComponents = document.querySelectorAll(`.${this.#COMPONENT_CLASS}`)
        imageComponents.forEach(component => {
            this.Init(component.id, true)
        })

        imageComponents = document.querySelectorAll(`.${this.#BACKGROUND_COMPONENT_CLASS}`)
        imageComponents.forEach(component => {
            this.Init(component.id, true)
        })
    }

    // State ------------------------------------------------------------------------------------------------------------------------------
    static #ImageState(img) {
        if (!img) return 0
        if (img.complete) {
            if (img.naturalWidth > 0) return 2
            else return 1
        } else {
            return 0
        }
    }

    static CheckState(url) {
        const img = document.createElement('img')
        if (!img) return this.#ImageState(null)
        img.src = url
        return this.#ImageState(img)
    }

    static CheckPreloadedState(preloaderID, url) {
        const preloader = document.getElementById(preloaderID)
        if (!preloader) return this.#ImageState(null)

        const img = preloader.querySelector(`[src="${url}"]`)
        if (!img) return this.#ImageState(null)

        return this.#ImageState(img)
    }
}

window.JSImage = JSImage
