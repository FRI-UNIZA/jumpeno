class JSThemeProvider {
    static #CLASSNAME_NO_THEME = "no-theme";
    static #CLASSNAME_DARK_THEME = "dark-theme";
    static #CLASSNAME_LIGHT_THEME = "light-theme";

    static #CLASSNAME_THEME_UPDATED = "theme-updated";
    static #CLASSNAME_SETTING_THEME = "setting-theme";

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    static async Init(autodetect, key) {
        var theme = this.#ThemeCSSClass(JSCookies.Get(key))
        if (autodetect && !theme) this.#SetupPreferred();
        else if (document.body.classList.contains(this.#CLASSNAME_NO_THEME)) this.#SetupGiven(theme);
    }

    // Setup ------------------------------------------------------------------------------------------------------------------------------
    static #SetupTheme(callback) {
        JSAnimationHandler.DisableTransitions()
        document.body.classList.remove(this.#CLASSNAME_DARK_THEME)
        document.body.classList.remove(this.#CLASSNAME_LIGHT_THEME)
        callback()
        document.body.classList.remove(this.#CLASSNAME_NO_THEME)
        setTimeout(() => {
            JSAnimationHandler.RestoreTransitions()
            JSAnimationHandler.RenderFrames(3)
        }, 0)
    } 

    static #SetupPreferred() {
        this.#SetupTheme(() => {
            document.body.classList.add(this.DarkThemePreferred() ? this.#CLASSNAME_DARK_THEME : this.#CLASSNAME_LIGHT_THEME)
        });
    }

    static #SetupGiven(theme) {
        this.#SetupTheme(() => {
            if (theme) document.body.classList.add(theme)
            else document.body.classList.add(this.#CLASSNAME_DARK_THEME)
        });
    }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    static DarkThemePreferred() {
        return window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches ? false : true
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    static #ThemeCSSClass(classname) {
        if (!classname) return null
        return classname.replace('Theme', '-theme').toLowerCase()
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
