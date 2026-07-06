(function () {
    'use strict';

    /**
     * Desktop: abrir/cerrar submenus con el cursor; las opciones son clicables sin clic previo en el toggle.
     * El hueco entre enlace y menu se reduce en CSS (margin-top 0).
     */
    function initDesktopDropdownHover(root) {
        if (typeof bootstrap === 'undefined' || !bootstrap.Dropdown) return;

        var mq = window.matchMedia('(min-width: 992px)');
        var disposers = [];

        function dispose() {
            disposers.forEach(function (d) {
                if (d.timerRef && d.timerRef.id) clearTimeout(d.timerRef.id);
                if (d.dropdown && typeof d.dropdown.hide === 'function') {
                    try {
                        d.dropdown.hide();
                    } catch (e) {}
                }
                d.item.removeEventListener('mouseenter', d.onEnter);
                d.item.removeEventListener('mouseleave', d.onLeave);
                if (d.onToggleClick) {
                    d.toggle.removeEventListener('click', d.onToggleClick, true);
                }
            });
            disposers = [];
        }

        function bind() {
            dispose();
            if (!mq.matches) return;

            root.querySelectorAll('.navbar-nav .nav-item.dropdown').forEach(function (item) {
                var toggle = item.querySelector('[data-bs-toggle="dropdown"]');
                if (!toggle) return;

                var dropdown = bootstrap.Dropdown.getOrCreateInstance(toggle, { autoClose: true });
                var timerRef = { id: null };

                function onEnter() {
                    if (timerRef.id) clearTimeout(timerRef.id);
                    timerRef.id = null;
                    dropdown.show();
                }

                function onLeave() {
                    timerRef.id = setTimeout(function () {
                        timerRef.id = null;
                        dropdown.hide();
                    }, 180);
                }

                function onToggleClick(e) {
                    if (!mq.matches) return;
                    e.preventDefault();
                }

                item.addEventListener('mouseenter', onEnter);
                item.addEventListener('mouseleave', onLeave);
                toggle.addEventListener('click', onToggleClick, true);

                disposers.push({
                    item: item,
                    toggle: toggle,
                    dropdown: dropdown,
                    timerRef: timerRef,
                    onEnter: onEnter,
                    onLeave: onLeave,
                    onToggleClick: onToggleClick
                });
            });
        }

        bind();
        if (typeof mq.addEventListener === 'function') {
            mq.addEventListener('change', bind);
        } else if (typeof mq.addListener === 'function') {
            mq.addListener(bind);
        }
    }

    function init() {
        var root = document.querySelector('[data-header-econ]');
        if (!root) return;

        function setHeaderVar() {
            var h = root.offsetHeight || 0;
            document.documentElement.style.setProperty('--header-h', h + 'px');
        }

        setHeaderVar();
        window.addEventListener('resize', setHeaderVar);

        if (typeof ResizeObserver !== 'undefined') {
            var ro = new ResizeObserver(setHeaderVar);
            ro.observe(root);
        }

        var onScroll = function () {
            if (window.scrollY > 4) {
                root.classList.add('is-scrolled');
            } else {
                root.classList.remove('is-scrolled');
            }
        };
        onScroll();
        window.addEventListener('scroll', onScroll, { passive: true });

        var collapse = root.querySelector('#navbarCollapseEcon');
        if (collapse && typeof bootstrap !== 'undefined' && bootstrap.Collapse) {
            collapse.addEventListener('show.bs.collapse', function () {
                document.body.classList.add('menu-open');
            });
            collapse.addEventListener('hide.bs.collapse', function () {
                document.body.classList.remove('menu-open');
            });
        }

        initDesktopDropdownHover(root);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
