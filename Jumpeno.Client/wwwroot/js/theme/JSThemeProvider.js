class JSThemeProvider {
    static #CLASSNAME_NO_THEME = "no-theme";
    static #CLASSNAME_DARK_THEME = "dark-theme";
    static #CLASSNAME_LIGHT_THEME = "light-theme";

    static #CLASSNAME_THEME_UPDATED = "theme-updated";
    static #CLASSNAME_SETTING_THEME = "setting-theme";

    static SetupPreferred() {
        JSAnimationHandler.DisableTransitions()
        document.body.classList.remove(this.#CLASSNAME_DARK_THEME)
        document.body.classList.remove(this.#CLASSNAME_LIGHT_THEME)
        document.body.classList.add(this.DarkThemePreferred() ? this.#CLASSNAME_DARK_THEME : this.#CLASSNAME_LIGHT_THEME)
        document.body.classList.remove(this.#CLASSNAME_NO_THEME)
        window.setTimeout(() => JSAnimationHandler.RestoreTransitions(), 0)
    }

    static DarkThemePreferred() {
        return window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches ? false : true
    }

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
