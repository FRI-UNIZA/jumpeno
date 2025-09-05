class JSAnimationHandler {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    // Mark:
    static #MARK = '; blazor-animation-handler: active; '
    // Pseudo styles:
    static #PSEUDO_STYLES_ID = 'pseudo-styles'
    // Class:
    static CLASS_IMUNE_TRANSITION = "animation-handler-imune-transition"
    static CLASS_IMUNE_ANIMATION = "animation-handler-imune-animation";

    // Disablers --------------------------------------------------------------------------------------------------------------------------
    static #TRANSITION_DISABLER = (timing) => `${this.#MARK}transition: ${timing} !important`
    static #LastTransitionDisabler = null;
    static #ANIMATION_DISABLER = `${this.#MARK}animation: none !important`
    static #LastAnimationDisabler = null;

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    static #SetPseudoElementStyle(selector, pseudoElement, style) {
        // Create a style element if it doesn't exist
        let styleSheet = document.getElementById(this.#PSEUDO_STYLES_ID)
        if (!styleSheet) {
            styleSheet = document.createElement('style')
            styleSheet.id = this.#PSEUDO_STYLES_ID
            document.head.appendChild(styleSheet)
        }

        const rule = `${selector}${pseudoElement} { ${style} }`
    
        // Add or replace the rule in the stylesheet
        const sheet = styleSheet.sheet;
        const rules = sheet.cssRules || sheet.rules;
        for (let i = 0; i < rules.length; i++) {
            if (rules[i].selectorText === `${selector}${pseudoElement}`) {
                sheet.deleteRule(i);
                break;
            }
        }
        sheet.insertRule(rule, sheet.cssRules.length);
    }

    static #RemovePseudoStyles() {
        let styleSheet = document.getElementById(this.#PSEUDO_STYLES_ID)
        if (styleSheet) document.head.removeChild(styleSheet)
    }

    static #ElementStyle(element) {
        let style = element.getAttribute('style')
        if (style) style = style.trim()
        else style = ''
        return style
    }

    static #Restore(animations) {
        this.#RemovePseudoStyles()
        document.querySelectorAll('*').forEach(el => {
            let style = el.getAttribute('style')
            if (style) {
                style = style.replace(animations ? this.#LastAnimationDisabler : this.#LastTransitionDisabler, '')
                if (style.trim() == '') el.removeAttribute('style') 
                else el.setAttribute('style', style)
            }
        })

        if (animations) this.#LastAnimationDisabler = null
        else this.#LastTransitionDisabler = null
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    static SetTransitions(timing) {
        if (this.#LastTransitionDisabler) return
        if (timing) this.#LastTransitionDisabler = this.#TRANSITION_DISABLER(`all ${timing}`)
        else this.#LastTransitionDisabler = this.#TRANSITION_DISABLER('none')

        document.querySelectorAll('*').forEach(el => {
            if (el.classList.contains(this.CLASS_IMUNE_TRANSITION)) return
            el.setAttribute('style', this.#ElementStyle(el) + this.#LastTransitionDisabler)
            this.#SetPseudoElementStyle(el.tagName, '::before', this.#LastTransitionDisabler)
            this.#SetPseudoElementStyle(el.tagName, '::after', this.#LastTransitionDisabler)
        })
    }

    static DisableTransitions() {
        this.SetTransitions()
    }

    static RestoreTransitions() {
        this.#Restore(false)
    }

    static DisableAnimations() {
        if (this.#LastAnimationDisabler) return
        this.#LastAnimationDisabler = this.#ANIMATION_DISABLER

        document.querySelectorAll('*').forEach(el => {
            if (el.classList.contains(this.CLASS_IMUNE_ANIMATION)) return
            el.setAttribute('style', this.#ElementStyle(el) + this.#LastAnimationDisabler)
        })
    }

    static RestoreAnimations() {
        this.#Restore(true)
    }

    // Animation end ----------------------------------------------------------------------------------------------------------------------
    static async #OnAnimationEndListener(element, objRef, method, listener) {
        element.removeEventListener("animationend", listener)
        try { await objRef.invokeMethodAsync(method) } catch {}
    }
    static CallOnAnimationEnd(selector, objRef, method) {
        const element = document.querySelector(selector)
        if (!element) return
        let listener = async () => await this.#OnAnimationEndListener(element, objRef, method, listener)
        element.addEventListener("animationend", listener)
    }

    // Transition end ---------------------------------------------------------------------------------------------------------------------
    static async #OnTransitionEndListener(element, objRef, method, listener) {
        element.removeEventListener("transitionend", listener)
        try { await objRef.invokeMethodAsync(method) } catch {}
    }
    static CallOnTransitionEnd(selector, objRef, method) {
        const element = document.querySelector(selector)
        if (!element) return
        let listener = async () => await this.#OnTransitionEndListener(element, objRef, method, listener)
        element.addEventListener("transitionend", listener)
    }

    // Frames -----------------------------------------------------------------------------------------------------------------------------
    static RenderFrames(count) {
        if (count <= 0) return
        setTimeout(() => requestAnimationFrame(() => this.RenderFrames(--count)), 0)
    }
}

window.JSAnimationHandler = JSAnimationHandler
