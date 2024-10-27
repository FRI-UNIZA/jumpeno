class JSAnimationHandler {
    static #MARK = '; blazor-animation-handler: active; '
    static #TRANSITION_DISABLER = (timing) => `${this.#MARK}transition: ${timing} !important`
    static #LastTransitionDisabler = null;
    static #ANIMATION_DISABLER = `${this.#MARK}animation: none !important`
    static #LastAnimationDisabler = null;

    static IMUNE_TRANSITION_CLASSNAME = "animation-handler-imune-transition"
    static IMUNE_ANIMATION_CLASSNAME = "animation-handler-imune-animation";

    static #PSEUDO_STYLES_ID = 'pseudo-styles'

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

    static SetTransitions(timing) {
        if (this.#LastTransitionDisabler) return
        if (timing) this.#LastTransitionDisabler = this.#TRANSITION_DISABLER(`all ${timing}`)
        else this.#LastTransitionDisabler = this.#TRANSITION_DISABLER('none')

        document.querySelectorAll('*').forEach(el => {
            if (el.classList.contains(this.IMUNE_TRANSITION_CLASSNAME)) return
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
            if (el.classList.contains(this.IMUNE_ANIMATION_CLASSNAME)) return
            el.setAttribute('style', this.#ElementStyle(el) + this.#LastAnimationDisabler)
        })
    }

    static RestoreAnimations() {
        this.#Restore(true)
    }

    static async #OnAnimationEndListener(element, objRef, method, listener) {
        element.removeEventListener("animationend", listener)
        await objRef.invokeMethodAsync(method)
    }
    static CallOnAnimationEnd(selector, objRef, method) {
        const element = document.querySelector(selector)
        if (!element) return
        let listener
        listener = async () => await this.#OnAnimationEndListener(element, objRef, method, listener)
        element.addEventListener("animationend", listener)
    }
}

window.JSAnimationHandler = JSAnimationHandler
