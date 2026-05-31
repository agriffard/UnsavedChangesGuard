// Registers/unregisters the browser beforeunload guard.

let _handler = null;

export function registerBeforeUnload() {
    if (_handler) return;

    _handler = (e) => {
        e.preventDefault();
        // Modern browsers require returnValue to be set (legacy support)
        e.returnValue = '';
    };

    window.addEventListener('beforeunload', _handler);
}

export function unregisterBeforeUnload() {
    if (!_handler) return;

    window.removeEventListener('beforeunload', _handler);
    _handler = null;
}
