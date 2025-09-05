var { 
    OverlayScrollbars, 
    ScrollbarsHidingPlugin, 
    SizeObserverPlugin, 
    ClickScrollPlugin  
} = OverlayScrollbarsGlobal;

class JSScrollArea {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    static #CLASS_CUSTOM_THEME = 'scroll-area-custom-theme'
    static #CLASS_SCROLLBAR = 'os-scrollbar'

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    static #ScrollAreas = {}
    static #CurrentTheme = null
    static #CustomThemes = {}
    static #Listeners = {}

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    static #GetViewPort(id) {
        return this.#ScrollAreas[id]?.elements()?.viewport
    }

    static #GetPosition(viewport) {
        return {
            ScrollWidth: viewport.scrollWidth,
            ClientWidth: viewport.clientWidth,
            ScrollLeft: viewport.scrollLeft,
            ScrollHeight: viewport.scrollHeight,
            ClientHeight: viewport.clientHeight,
            ScrollTop: viewport.scrollTop
        }
    }
    
    static Activate(id, theme, autoHide, overflowX, overflowY) {
        const element = document.querySelector(`#${id}`)
        if (!element) return
        this.#ScrollAreas[id] = OverlayScrollbars({
            target: element
        }, {
            scrollbars: {
                theme: theme ? `${theme} ${this.#CLASS_CUSTOM_THEME}` : `${this.#CurrentTheme}`,
                autoHide
            },
            overflow: {
                x: overflowX ? undefined : 'hidden',
                y: overflowY ? undefined : 'hidden'
            }
        })
        if (theme) this.#CustomThemes[id] = theme
    }

    static Destroy(id) {
        const instance = this.#ScrollAreas[id]
        if (instance) instance.destroy()
        delete this.#ScrollAreas[id]
        delete this.#CustomThemes[id]
    }

    static SetTheme(theme) {
        if (this.#CurrentTheme) {
            const areas = document.getElementsByClassName(this.#CurrentTheme);
            [...areas].forEach(area => {
                if (!area.classList.contains(this.#CLASS_CUSTOM_THEME)) {
                    area.classList.remove(this.#CurrentTheme)
                    area.classList.add(theme)
                }
            })
        }
        this.#CurrentTheme = theme
    }

    static Update(id, theme) {
        const areas = document.querySelectorAll(`#${id} > .${this.#CustomThemes[id]}`);
        [...areas].forEach(area => {
            area.classList.remove(this.#CustomThemes[id])
            area.classList.add(theme)
        })
        this.#CustomThemes[id] = theme
    }

    static #ForEachScrollbar(id, callback) {
        const area = document.getElementById(id)
        if (!area) return
        const scrollbars = area.querySelectorAll(`#${id} > .${this.#CLASS_SCROLLBAR}`)
        if (!scrollbars) return
        [...scrollbars].forEach(scrollbar => callback(scrollbar))
    }

    static HideScrollbars(id) {
        this.#ForEachScrollbar(id, scrollbar => {
            scrollbar.classList.remove(CSS_CLASS.NO_DISPLAY)
            scrollbar.classList.add(CSS_CLASS.NO_DISPLAY)
        })
    }

    static ShowScrollbars(id) {
        this.#ForEachScrollbar(id, scrollbar => {
            scrollbar.classList.remove(CSS_CLASS.NO_DISPLAY)
        })
    }

    static Scroll(id, left, top) {
        this.#GetViewPort(id)?.scrollTo({ left, top })
    }

    static ItemPosition(id, selector) {
        const scrollableDiv = this.#GetViewPort(id)
        const item = scrollableDiv.querySelector(selector)

        const itemRect = item.getBoundingClientRect();
        const containerRect = scrollableDiv.getBoundingClientRect();

        const relativeTop = itemRect.top - containerRect.top + scrollableDiv.scrollTop;
        const relativeLeft = itemRect.left - containerRect.left + scrollableDiv.scrollLeft;

        return {
            Width: itemRect.width,
            Left: relativeLeft,
            Height: itemRect.height,
            Top: relativeTop
        }
    }

    static Position(id) {
        return this.#GetPosition(this.#GetViewPort(id))
    }

    static AddScrollListener(id) {
        this.RemoveScrollListener(id)
        this.#Listeners[id] = (e) => {
            DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.SCROLL_AREA.ON_SCROLL, id, this.#GetPosition(e.target))
        }
        this.#GetViewPort(id).addEventListener('scroll', this.#Listeners[id])
    }

    static RemoveScrollListener(id) {
        if (!this.#Listeners[id]) return
        this.#GetViewPort(id).removeEventListener('scroll', this.#Listeners[id])
        delete this.#Listeners[id]
    }
}

window.JSScrollArea = JSScrollArea
