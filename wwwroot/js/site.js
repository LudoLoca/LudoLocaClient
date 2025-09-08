// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code here if needed.
document.addEventListener('DOMContentLoaded', function () {
    var btn = document.getElementById('navbarHamburger');
    var canvas = document.getElementById('hamburgerArrow');
    var rc = canvas ? rough.canvas(canvas) : null;

    function drawArrow(isOpen) {
        if (!canvas || !rc) return;
        var ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        var theme = document.documentElement.getAttribute('data-bs-theme') || 'light';
        var stroke = theme === 'dark' ? '#fff' : '#222';

        // Arrow points down if closed, up if open
        if (!isOpen) {
            // Down arrow (chevron)
            rc.line(10, 14, 18, 22, { stroke: stroke, strokeWidth: 3, roughness: 2 });
            rc.line(26, 14, 18, 22, { stroke: stroke, strokeWidth: 3, roughness: 2 });
        } else {
            // Up arrow (chevron)
            rc.line(10, 22, 18, 14, { stroke: stroke, strokeWidth: 3, roughness: 2 });
            rc.line(26, 22, 18, 14, { stroke: stroke, strokeWidth: 3, roughness: 2 });
        }
    }

    // Initial draw
    drawArrow(false);

    // Redraw on theme change
    var observer = new MutationObserver(function () {
        drawArrow(btn.getAttribute('aria-expanded') === 'true');
    });
    observer.observe(document.documentElement, { attributes: true, attributeFilter: ['data-bs-theme'] });

    // Redraw on toggle
    if (btn) {
        btn.addEventListener('click', function () {
            var isOpen = btn.getAttribute('aria-expanded') === 'true';
            // The click handler toggles aria-expanded after this event, so use setTimeout
            setTimeout(function () {
                var nowOpen = btn.getAttribute('aria-expanded') === 'true';
                drawArrow(nowOpen);
            }, 10);
        });
    }

    window.addEventListener('resize', function () {
        drawArrow(btn.getAttribute('aria-expanded') === 'true');
    });

    window.addEventListener('pageshow', function () {
        drawArrow(btn.getAttribute('aria-expanded') === 'true');
    });
});