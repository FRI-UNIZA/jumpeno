class JSSwitch {
    static InitInstance(id, suffix, label) {
        const component = document.getElementById(id)
        if (!component) return
        const element = component.querySelector("button")
        if (!element) return
        element.id = `${id}-${suffix}`
        element.ariaLabel = label
    }
}

window.JSSwitch = JSSwitch
