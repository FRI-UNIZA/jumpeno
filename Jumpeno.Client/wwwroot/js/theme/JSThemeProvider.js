class JSThemeProvider {
    static #THEME_SUFFIX = "-theme";

    static #CLASSNAME_NO_THEME = "no-theme";
    static #CLASSNAME_DARK_THEME = "dark-theme";
    static #CLASSNAME_LIGHT_THEME = "light-theme";

    static #CLASSNAME_THEME_UPDATED = "theme-updated";
    static #CLASSNAME_SETTING_THEME = "setting-theme";

    static #THEME_SWITCH_ELEMENT_CLASS = "theme-switch-element";

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    static async Init(autodetect, key) {
        var theme = this.#ThemeCSSClass(JSCookies.Get(key))
        if (autodetect && !theme) this.#SetupPreferred();
        else if (document.body.classList.contains(this.#CLASSNAME_NO_THEME)) this.#SetupGiven(theme);
    }

    // Setup ------------------------------------------------------------------------------------------------------------------------------
    static #SetupThemeSwitch(theme) {
        const switches = document.querySelectorAll(`.${this.#THEME_SWITCH_ELEMENT_CLASS} > button`)
        switches.forEach(component => {
            component.classList.remove('ant-switch-checked')
            if (theme !== this.#CLASSNAME_LIGHT_THEME) return
            component.classList.add('ant-switch-checked')
        })
    }
    
    static #SetupTheme(theme, callback) {
        JSAnimationHandler.DisableTransitions()
        document.body.classList.remove(this.#CLASSNAME_DARK_THEME)
        document.body.classList.remove(this.#CLASSNAME_LIGHT_THEME)
        this.#SetupThemeSwitch(theme)
        callback()
        JSImage.UpdateTheme(theme, this.#THEME_SUFFIX)
        document.body.classList.remove(this.#CLASSNAME_NO_THEME)
        setTimeout(() => {
            JSAnimationHandler.RestoreTransitions()
            JSAnimationHandler.RenderFrames(3)
        }, 0)
    }

    static #SetupPreferred() {
        const theme = this.DarkThemePreferred() ? this.#CLASSNAME_DARK_THEME : this.#CLASSNAME_LIGHT_THEME
        this.#SetupTheme(theme, () => {
            document.body.classList.add(theme)
        });
    }

    static #SetupGiven(theme) {
        theme = theme ? theme : this.#CLASSNAME_DARK_THEME
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
        document.body.classList.remove(this.#CLASSNAME_DARK_THEME)
        document.body.classList.remove(this.#CLASSNAME_LIGHT_THEME)
        document.body.classList.add(classname)
    }

    static StartSettingTheme() {
        document.body.classList.remove(this.#CLASSNAME_THEME_UPDATED)
        document.body.classList.add(this.#CLASSNAME_THEME_UPDATED)
        document.body.classList.remove(this.#CLASSNAME_SETTING_THEME)
        document.body.classList.add(this.#CLASSNAME_SETTING_THEME)
    }

    static FinishSettingTheme() {
        document.body.classList.remove(this.#CLASSNAME_SETTING_THEME)
    }
}

window.JSThemeProvider = JSThemeProvider
