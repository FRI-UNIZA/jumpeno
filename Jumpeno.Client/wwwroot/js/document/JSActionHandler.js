class JSActionHandler {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    static #FOCUS_SELECTOR = 'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    static #FocusIDs = [];
    static #Autocomplete = [];
    static #NotAriaHidden = [];

    // Autocomplete -----------------------------------------------------------------------------------------------------------------------
    static DisableAutocomplete() {
        const activeElement = document.activeElement;
        if (activeElement && activeElement.id !== undefined && activeElement.tagName.toLowerCase() === 'input') {
            const autocomplete = activeElement.getAttribute('autocomplete');
            this.#Autocomplete.push({ id: activeElement.id, autocomplete })
            activeElement.setAttribute('autocomplete', 'off');
        }
    }

    static PopAutocomplete() {
        return this.#Autocomplete.pop()
    }

    static EnableAutocomplete() {
        const o = this.PopAutocomplete()
        if (!o || o.id === undefined || o.autocomplete === undefined) {
            return;
        }
        const activeElement = document.getElementById(o.id);
        if (activeElement) {
            if (o.autocomplete === null) {
                activeElement.removeAttribute('autocomplete');
            } else {
                activeElement.setAttribute('autocomplete', o.autocomplete);
            }
        }
    }

    // Focus [Predicates] -----------------------------------------------------------------------------------------------------------------
    static ActiveID = () => document.activeElement.id;

    static HasFocus(selector) {
        var element = document.querySelector(selector);
        if (!element) return false;
        return element == document.activeElement;
    }

    static FocusedChildID(selector) {
        var element = document.querySelector(selector);
        if (!element) return null;
        if (!element.contains(document.activeElement)) return null;
        return document.activeElement.id;
    }

    // Focus [Stack] ----------------------------------------------------------------------------------------------------------------------
    static #BlurActiveFocus() {
        const activeElement = document.activeElement
        if (activeElement && activeElement.id !== undefined) {
            this.#FocusIDs.push({ id: activeElement.id })
            activeElement.blur()
        }
    }
    static BlurFocus(focusAfterID) {
        this.#BlurActiveFocus()
        if (!focusAfterID) this.SetFocus(focusAfterID)
    }

    static PopFocus() { return this.#FocusIDs.pop() }

    static async RestoreFocus() {
        const o = this.PopFocus()
        if (o !== undefined && o.id !== undefined) {
            var p = new Promise(resolve => {
                window.requestAnimationFrame(() => {
                    const activeElement = document.getElementById(o.id);
                    if (activeElement) activeElement.focus()
                    else document?.getElementById(JSWebDocument.ID)?.focus()
                    resolve()
                })
            })
            await p
        } else {
            document.getElementById(JSWebDocument.ID)?.focus()
        }
    }

    static SaveActiveElement() {
        const activeElement = document.activeElement;
        if (activeElement && activeElement.id !== undefined) {
            this.#FocusIDs.push({ id: activeElement.id });
        }
    }

    static GetRestoreID() {
        if (this.#FocusIDs.length == 0) return null
        return this.#FocusIDs[this.#FocusIDs.length - 1].id;
    }

    // Focus [set] ------------------------------------------------------------------------------------------------------------------------
    static #SetFocusOnElement(element) {
        if (element) {
            element.focus()
            return
        }
        const webDocument = document.getElementById(JSWebDocument.ID);
        if (webDocument) {
            webDocument.focus()
        }
    }
    static SetFocus(id) {
        this.#SetFocusOnElement(document.getElementById(id))
    }

    static FocusFirst(id) {
        const wrapper = document.getElementById(id)
        if (!wrapper) return

        const focusableElements = wrapper.querySelectorAll(this.#FOCUS_SELECTOR)
        if (focusableElements.lenght <= 0) wrapper.focus()
        else focusableElements[0].focus()
    }

    static FocusLast(id) {
        const wrapper = document.getElementById(id)
        if (!wrapper) return

        const focusableElements = wrapper.querySelectorAll(this.#FOCUS_SELECTOR)
        if (focusableElements.lenght <= 0) wrapper.focus()
        else focusableElements[focusableElements.length - 1].focus()
    }

    // Keyboard ---------------------------------------------------------------------------------------------------------------------------
    static #DisableKeyboard(event) {
        event.stopImmediatePropagation();
        event.preventDefault();
    }

    static DisableKeyboardActions() {
        document.addEventListener('keydown', this.#DisableKeyboard, true)
    }

    static EnableKeyboardActions() {
        document.removeEventListener('keydown', this.#DisableKeyboard, true)
    }

    // Input ------------------------------------------------------------------------------------------------------------------------------
    static InputCursorPosition(id) {
        const input = document.getElementById(id)
        if (!input) return null
        return input.selectionStart
    }

    static SetInputCursorPosition(id, position) {
        const input = document.getElementById(id)
        if (!input) return
        input.setSelectionRange(position, position)
    }

    // Tabs -------------------------------------------------------------------------------------------------------------------------------
    static DisableTabs(exceptID, exceptData) {
        let selector = '*:not([aria-hidden="true"]):not(html):not(body)'
        if (exceptID) selector = `${selector}:not(#${exceptID})`
        if (exceptData) selector = `${selector}:not([${exceptData}])`
        const notAriaHidden = document.querySelectorAll(selector)
        notAriaHidden.forEach(element => {
            element.setAttribute('aria-hidden', 'true')
        })
        this.#NotAriaHidden.push(notAriaHidden)
    }

    static PopTabs() {
        return this.#NotAriaHidden.pop()
    }

    static EnableTabs() {
        const o = this.PopTabs()
        if (!o) return
        o.forEach(element => {
            element.removeAttribute('aria-hidden')
        })
    }

    static EnableTabsForDescendants(id) {
        const length = this.#NotAriaHidden.length
        if (length <= 0) return
        const o = this.#NotAriaHidden[length - 1]

        let targetElement = document.getElementById(id);
        if (!targetElement) return

        let descendants = Array.from(o).filter(element => {
            return targetElement.contains(element);
        })

        descendants.forEach(element => {
            element.removeAttribute('aria-hidden')
        })
    }

    // Inert ------------------------------------------------------------------------------------------------------------------------------
    static SetInert(selector) {
        const elements = document.querySelectorAll(selector)
        if (!elements) return
        [...elements].forEach(element => {
            element.setAttribute('inert', true)
        })

    }

    static RemoveInert(selector) {
        const elements = document.querySelectorAll(selector)
        if (!elements) return
        [...elements].forEach(element => {
            element.removeAttribute('inert')
        })
    }

    // Click ------------------------------------------------------------------------------------------------------------------------------
    static Click(selector) {
        const element = document.querySelector(selector)
        if (!element) return
        element.click()
    }
}

window.JSActionHandler = JSActionHandler;
