// Password eye toggle
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.pwd-toggle').forEach(function (btn) {
        btn.addEventListener('mousedown', function (e) {
            e.preventDefault();
            var input = this.closest('.pwd-wrapper').querySelector('.pwd-input');
            input.type = 'text';
            this.querySelector('i').className = 'fas fa-eye-slash';
        });
        btn.addEventListener('mouseup', function () {
            var input = this.closest('.pwd-wrapper').querySelector('.pwd-input');
            input.type = 'password';
            this.querySelector('i').className = 'fas fa-eye';
        });
        btn.addEventListener('mouseleave', function () {
            var input = this.closest('.pwd-wrapper').querySelector('.pwd-input');
            input.type = 'password';
            this.querySelector('i').className = 'fas fa-eye';
        });
    });
});

// Scroll reveal animation (triggers when element scrolls into view)
(function() {
    var revealed = new WeakSet();
    var observer = new IntersectionObserver(function(entries) {
        entries.forEach(function(entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('reveal-visible');
            } else {
                entry.target.classList.remove('reveal-visible');
            }
        });
    }, { threshold: 0.15 });

    document.addEventListener('DOMContentLoaded', function() {
        document.querySelectorAll('.reveal').forEach(function(el) {
            observer.observe(el);
        });
    });
})();
