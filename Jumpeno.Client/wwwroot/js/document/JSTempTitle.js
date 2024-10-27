class JSTempTitle {
    static #ID = 'page_temp_title';

    static Add() {
        const element = document.createElement('title');
        element.id = this.#ID;
        element.textContent = document.title;
        document.body.appendChild(element, document.body);
    }

    static Remove() {
        const element = document.getElementById(this.#ID);
        if (element) {
            document.body.removeChild(element);
        }
    }
}

window.JSTempTitle = JSTempTitle;
