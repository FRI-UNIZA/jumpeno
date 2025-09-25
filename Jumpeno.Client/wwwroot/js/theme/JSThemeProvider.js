class JSThemeProvider {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    static #THEME_SUFFIX = "-theme";

    static #CLASS_NO_THEME = "no-theme";
    static #CLASS_DARK_THEME = "dark-theme";
    static #CLASS_LIGHT_THEME = "light-theme";

    static #CLASS_THEME_UPDATED = "theme-updated";
    static #CLASS_SETTING_THEME = "setting-theme";

    static #THEME_SWITCH_ELEMENT_CLASS = "theme-switch-element";

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    static Init(autodetect, key) {
        var theme = this.#ThemeCSSClass(JSCookies.Get(key))
        if (autodetect && !theme) this.#SetupPreferred();
        else if (document.body.classList.contains(this.#CLASS_NO_THEME)) this.#SetupGiven(theme);
    }

    // Setup ------------------------------------------------------------------------------------------------------------------------------
    static #SetupThemeSwitch(theme) {
        const switches = document.querySelectorAll(`.${this.#THEME_SWITCH_ELEMENT_CLASS} > button`)
        switches.forEach(component => {
            component.classList.remove('ant-switch-checked')
            if (theme !== this.#CLASS_LIGHT_THEME) return
            component.classList.add('ant-switch-checked')
        })
    }
    
    static #SetupTheme(theme, callback) {
        JSAnimationHandler.DisableTransitions()
        document.body.classList.remove(this.#CLASS_DARK_THEME)
        document.body.classList.remove(this.#CLASS_LIGHT_THEME)
        this.#SetupThemeSwitch(theme)
        callback()
        JSImage.UpdateTheme(theme, this.#THEME_SUFFIX)
        document.body.classList.remove(this.#CLASS_NO_THEME)
        setTimeout(() => {
            JSAnimationHandler.RestoreTransitions()
            JSAnimationHandler.RenderFrames(3)
        }, 0)
    }

    static #SetupPreferred() {
        const theme = this.DarkThemePreferred() ? this.#CLASS_DARK_THEME : this.#CLASS_LIGHT_THEME
        this.#SetupTheme(theme, () => {
            document.body.classList.add(theme)
        });
    }

    static #SetupGiven(theme) {
        theme = theme ? theme : this.#CLASS_DARK_THEME
        this.#SetupTheme(theme, () => {
            document.body.classList.add(theme)
        });
    }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    static DarkThemePreferred() {
        return window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches ? false : true
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    static #ThemeCSSClass(classname) {
        if (!classname) return null
        return classname.replace('Theme', this.#THEME_SUFFIX).toLowerCase()
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    static SetCustomTheme(classname) {
        document.body.classList.remove(this.#CLASS_DARK_THEME)
        document.body.classList.remove(this.#CLASS_LIGHT_THEME)
        document.body.classList.add(classname)
    }

    static StartSettingTheme() {
        document.body.classList.remove(this.#CLASS_THEME_UPDATED)
        document.body.classList.add(this.#CLASS_THEME_UPDATED)
        document.body.classList.remove(this.#CLASS_SETTING_THEME)
        document.body.classList.add(this.#CLASS_SETTING_THEME)
    }

    static FinishSettingTheme() {
        document.body.classList.remove(this.#CLASS_SETTING_THEME)
    }
}

window.JSThemeProvider = JSThemeProvider
