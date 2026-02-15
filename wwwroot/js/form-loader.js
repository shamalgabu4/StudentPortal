/* ============================================
   LOADER / SPINNER UTILITY
   ============================================ */

class FormLoader {
    constructor() {
        this.spinnerHTML = `<span class="spinner-border spinner-border-sm spinner-animation" role="status" aria-hidden="true"></span>`;
    }

    /**
     * Enable loader on button
     * @param {HTMLElement} button - Button element
     * @param {string} loadingText - Text to show during loading (default: "Processing...")
     */
    enableLoader(button, loadingText = 'Processing...') {
        if (!button) return;
        
        // Store original state
        button._originalHTML = button.innerHTML;
        button._originalText = button.textContent;
        button._originalDisabled = button.disabled;
        
        // Update button
        button.disabled = true;
        button.innerHTML = `${this.spinnerHTML} ${loadingText}`;
    }

    /**
     * Disable loader on button (restore original state)
     * @param {HTMLElement} button - Button element
     */
    disableLoader(button) {
        if (!button) return;
        
        button.disabled = button._originalDisabled || false;
        button.innerHTML = button._originalHTML || button._originalText;
    }

    /**
     * Add form submission loader
     * @param {string} formSelector - Form CSS selector
     * @param {string} buttonSelector - Submit button CSS selector
     * @param {string} loadingText - Text to show during loading
     */
    addFormLoader(formSelector, buttonSelector, loadingText = 'Processing...') {
        const form = document.querySelector(formSelector);
        const button = form ? form.querySelector(buttonSelector) : null;
        
        if (!form || !button) return;
        
        form.addEventListener('submit', (e) => {
            // Only show loader if form is valid or we're allowing submission anyway
            this.enableLoader(button, loadingText);
        });
    }

    /**
     * Add loader to all forms with data-loader attribute
     * Usage: <button type="submit" data-loader="true" data-loading-text="Saving...">Save</button>
     */
    initializeAllLoaders() {
        document.addEventListener('submit', (e) => {
            if (e.target.tagName === 'FORM') {
                const submitBtn = e.target.querySelector('[type="submit"][data-loader="true"]');
                if (submitBtn) {
                    const loadingText = submitBtn.getAttribute('data-loading-text') || 'Processing...';
                    this.enableLoader(submitBtn, loadingText);
                }
            }
        });
    }

    /**
     * Disable all loaders (useful for AJAX responses)
     */
    disableAllLoaders() {
        document.querySelectorAll('[type="submit"][data-loader="true"]').forEach(btn => {
            this.disableLoader(btn);
        });
    }

    /**
     * Manual loader for specific actions
     * Usage: <button onclick="formLoader.quickLoad(this, 'Saving...')">Save</button>
     */
    quickLoad(element, loadingText = 'Processing...') {
        this.enableLoader(element, loadingText);
    }
}

// Initialize on page load
const formLoader = new FormLoader();
document.addEventListener('DOMContentLoaded', () => {
    formLoader.initializeAllLoaders();
});
