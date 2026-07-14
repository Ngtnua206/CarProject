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

// Nav scroll arrows for desktop
(function() {
    var wrap = document.getElementById('navScrollWrap');
    var leftBtn = document.getElementById('scrollLeft');
    var rightBtn = document.getElementById('scrollRight');
    if (!wrap || !leftBtn || !rightBtn) return;

    function updateScrollButtons() {
        var atStart = wrap.scrollLeft <= 2;
        var atEnd = wrap.scrollLeft >= wrap.scrollWidth - wrap.clientWidth - 2;
        leftBtn.classList.toggle('visible', !atStart);
        rightBtn.classList.toggle('visible', !atEnd);
    }

    leftBtn.addEventListener('click', function() {
        wrap.scrollBy({ left: -250, behavior: 'smooth' });
        setTimeout(updateScrollButtons, 150);
    });
    rightBtn.addEventListener('click', function() {
        wrap.scrollBy({ left: 250, behavior: 'smooth' });
        setTimeout(updateScrollButtons, 150);
    });
    wrap.addEventListener('scroll', updateScrollButtons);
    window.addEventListener('resize', updateScrollButtons);
    updateScrollButtons();
})();

// Mobile hamburger toggle
(function() {
    var toggle = document.getElementById('mobileMenuToggle');
    var panel = document.getElementById('mobileNavPanel');
    if (!toggle || !panel) return;

    toggle.addEventListener('click', function() {
        panel.classList.toggle('open');
        var bars = toggle.querySelectorAll('.toggler-bar');
        if (panel.classList.contains('open')) {
            bars[0].style.transform = 'rotate(45deg) translate(4px, 4px)';
            bars[1].style.opacity = '0';
            bars[2].style.transform = 'rotate(-45deg) translate(4px, -4px)';
        } else {
            bars[0].style.transform = '';
            bars[1].style.opacity = '';
            bars[2].style.transform = '';
        }
    });

    // Close panel when clicking a link inside
    panel.querySelectorAll('.nav-link').forEach(function(link) {
        link.addEventListener('click', function() {
            panel.classList.remove('open');
            toggle.querySelectorAll('.toggler-bar').forEach(function(bar) {
                bar.style.transform = '';
                bar.style.opacity = '';
            });
        });
    });
})();

// Theme toggle (dark/light) with curtain animation
(function() {
    var html = document.documentElement;
    var toggle = document.getElementById('themeToggle');
    var icon = document.getElementById('themeIcon');
    var curtain = document.getElementById('themeCurtain');
    if (!toggle || !icon || !curtain) return;

    function setTheme(theme) {
        html.setAttribute('data-theme', theme);
        localStorage.setItem('site-theme', theme);
        icon.className = theme === 'light' ? 'fas fa-sun' : 'fas fa-moon';
    }

    // Load saved theme
    var saved = localStorage.getItem('site-theme');
    if (saved) setTheme(saved);

    toggle.addEventListener('click', function() {
        var current = html.getAttribute('data-theme');
        var next = current === 'light' ? 'dark' : 'light';

        // Curtain close
        curtain.classList.remove('active', 'closing');
        void curtain.offsetHeight;
        curtain.classList.add('closing');

        setTimeout(function() {
            setTheme(next);
            // Curtain open
            curtain.classList.remove('closing');
            curtain.classList.add('active');
        }, 350);

        setTimeout(function() {
            curtain.classList.remove('active');
        }, 950);
    });
})();

// Mobile nav dropdown toggle (Giới thiệu → Về chúng tôi / Liên hệ)
function toggleMobileDropdown(id) {
    var el = document.getElementById(id);
    if (el) el.classList.toggle('open');
}

// Close mobile dropdown when clicking a link inside
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('.mobile-nav-panel .nav-dropdown-item').forEach(function(item) {
        item.addEventListener('click', function() {
            var panel = document.getElementById('mobileNavPanel');
            if (panel) panel.classList.remove('open');
            document.querySelectorAll('#mobileMenuToggle .toggler-bar').forEach(function(bar) {
                bar.style.transform = '';
                bar.style.opacity = '';
            });
        });
    });
});

// Scroll reveal animation (plays on every scroll-down, no reverse when scrolling up)
(function() {
    var observer = new IntersectionObserver(function(entries) {
        entries.forEach(function(entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('reveal-visible');
            } else {
                entry.target.style.transition = 'none';
                entry.target.classList.remove('reveal-visible');
                void entry.target.offsetHeight;
                entry.target.style.transition = '';
            }
        });
    }, { threshold: 0.15 });

    function observeReveals() {
        document.querySelectorAll('.reveal').forEach(function(el) {
            observer.observe(el);
        });
    }

    document.addEventListener('DOMContentLoaded', observeReveals);
    if (window.MutationObserver) {
        var mo = new MutationObserver(observeReveals);
        mo.observe(document.body, { childList: true, subtree: true });
    }
})();
