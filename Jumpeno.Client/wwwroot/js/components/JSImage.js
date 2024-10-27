class JSImage {
    static #BACKGROUND_COMPONENT_CLASS = 'background-component'
    static #COMPONENT_CLASS = 'image-component'
    static #ELEMENT_CLASS = 'image-element'
    static #LOADING_CLASS = 'loading'
    static #ERROR_CLASS = 'error'

    static #InvokeOnLoad(id, component, jsOnly) {
        component.classList.remove(this.#LOADING_CLASS)
        if (jsOnly) return;
        DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.IMAGE.ON_LOAD, id);
    }

    static #InvokeOnError(id, component, jsOnly) {
        if (!component.classList.contains(this.#ERROR_CLASS)) {
            component.classList.add(this.#ERROR_CLASS)
        }
        if (jsOnly) return;
        DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.IMAGE.ON_ERROR, id);
    }

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

    static #ImageState(img) {
        if (!img) return 0
        if (img.complete) {
            if (img.naturalWidth > 0) return 2
            else return 1
        } else {
            return 0
        }
    }

    static CheckState(id) {
        const component = document.getElementById(id);
        if (!component)  return this.#ImageState(null)
        
        const img = component.querySelector(`.${this.#ELEMENT_CLASS}`)
        if (!img) return this.#ImageState(null)

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
