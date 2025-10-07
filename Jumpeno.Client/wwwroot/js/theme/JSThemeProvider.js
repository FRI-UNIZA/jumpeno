class JSThemeProvider {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    static CLASS_NO_THEME = "no-theme";
    static CLASS_DARK_THEME = "dark-theme";
    static CLASS_LIGHT_THEME = "light-theme";

    static CLASS_SETTING_THEME = "setting-theme";
    static CLASS_SETTING_THEME_ANIMATION = "setting-theme-animation";

    static CLASS_THEME_TRANSITION_CONTAINER = "theme-transition-container";

    static SUFFIX = "theme";
    static THEME_SUFFIX = `-${this.SUFFIX}`;

    // Initialization ---------------------------------------------------------------------------------------------------------------------
    static Init(autodetect, key) {
        var theme = this.#ThemeCSSClass(JSCookies.Get(key));
        if (autodetect && !theme) this.#SetupPreferred();
        else if (document.body.classList.contains(this.CLASS_NO_THEME)) this.#SetupGiven(theme);
        this.#UpdateStatusBar();
    }

    // Setup ------------------------------------------------------------------------------------------------------------------------------    
    static #SetupTheme(theme, callback) {
        JSAnimationHandler.DisableAnimation("body")
        document.body.classList.remove(this.CLASS_DARK_THEME)
        document.body.classList.remove(this.CLASS_LIGHT_THEME)
        callback()
        JSImage.UpdateTheme(theme, this.THEME_SUFFIX)
        document.body.classList.remove(this.CLASS_NO_THEME)
        setTimeout(() => {
            JSAnimationHandler.RestoreAnimation("body")
            JSAnimationHandler.RenderFrames(3)
        }, 0)
    }

    static #SetupPreferred() {
        const theme = this.DarkThemePreferred() ? this.CLASS_DARK_THEME : this.CLASS_LIGHT_THEME
        this.#SetupTheme(theme, () => {
            document.body.classList.add(theme)
        });
    }

    static #SetupGiven(theme) {
        theme = theme ? theme : this.CLASS_DARK_THEME
        this.#SetupTheme(theme, () => {
            document.body.classList.add(theme)
        });
    }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    static DarkThemePreferred() {
        return window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches ? false : true
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    static #ThemeCSSClass(classname) {
        if (!classname) return null
        return classname.replace('Theme', this.THEME_SUFFIX).toLowerCase()
    }

    static #UpdateStatusBar() {
        const color = getComputedStyle(document.body).backgroundColor;
        let meta = document.querySelector('meta[name="theme-color"]');
        if (!meta) {
            meta = document.createElement('meta');
            meta.name = 'theme-color';
            document.head.appendChild(meta);
        }
        meta.setAttribute('content', color);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    static SetCustomTheme(classname) {
        document.body.classList.remove(this.CLASS_DARK_THEME)
        document.body.classList.remove(this.CLASS_LIGHT_THEME)
        document.body.classList.add(classname)
    }

    static StartSettingTheme() {
        document.body.classList.remove(this.CLASS_SETTING_THEME)
        document.body.classList.add(this.CLASS_SETTING_THEME)
        document.body.classList.remove(this.CLASS_SETTING_THEME_ANIMATION)
        document.body.classList.add(this.CLASS_SETTING_THEME_ANIMATION)
    }

    static ApplyThemeAnimation() {
        document.body.classList.remove(this.CLASS_SETTING_THEME)
        this.#UpdateStatusBar();
    }

    static FinishSettingTheme() {
        document.body.classList.remove(this.CLASS_SETTING_THEME_ANIMATION)
    }
}

window.JSThemeProvider = JSThemeProvider
