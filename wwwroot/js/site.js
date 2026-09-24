// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    var revealElements = Array.prototype.slice.call(document.querySelectorAll('[data-reveal]'));

    if (revealElements.length === 0) {
        return;
    }

    var pending = new Set(revealElements);
    var hasScrolled = window.scrollY > 0;
    var observer = null;

    function isInViewport(element) {
        var rect = element.getBoundingClientRect();
        return rect.top < window.innerHeight * 0.85 && rect.bottom > window.innerHeight * 0.15;
    }

    function reveal(element) {
        element.classList.add('is-visible');
        pending.delete(element);
        if (observer) {
            observer.unobserve(element);
        }
    }

    function showAll() {
        revealElements.forEach(reveal);
    }

    function checkPending() {
        if (!hasScrolled) {
            return;
        }

        pending.forEach(function (element) {
            if (isInViewport(element)) {
                reveal(element);
            }
        });
    }

    window.addEventListener('scroll', function () {
        hasScrolled = true;
        checkPending();
    }, { passive: true });
    window.addEventListener('resize', checkPending);

    if (!('IntersectionObserver' in window)) {
        showAll();
        return;
    }

    observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                pending.add(entry.target);
                checkPending();
            }
        });
    }, {
        threshold: 0.15,
        rootMargin: '0px 0px -10% 0px'
    });

    revealElements.forEach(function (element) {
        observer.observe(element);
    });

    checkPending();
})();
