(function ($) {
    "use strict";

    // Sticky Navbar
    $(window).scroll(function () {
        if ($(this).scrollTop() > 40) {
            $('.navbar').addClass('sticky-top');
        } else {
            $('.navbar').removeClass('sticky-top');
        }
    });
    
    // Dropdown on mouse hover
    $(document).ready(function () {
        function toggleNavbarMethod() {
            if ($(window).width() > 992) {
                $('.navbar .dropdown').on('mouseover', function () {
                    $('.dropdown-toggle', this).trigger('click');
                }).on('mouseout', function () {
                    $('.dropdown-toggle', this).trigger('click').blur();
                });
            } else {
                $('.navbar .dropdown').off('mouseover').off('mouseout');
            }
        }
        toggleNavbarMethod();
        $(window).resize(toggleNavbarMethod);
    });
        
    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({scrollTop: 0}, 1500, 'easeInOutExpo');
        return false;
    });
    
    // Testimonials carousel
    $(".testimonial-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1000,
        items: 1,
        dots: false,
        loop: true,
        nav : true,
        navText : [
            '<i class="bi bi-arrow-left"></i>',
            '<i class="bi bi-arrow-right"></i>'
        ],
    });

    // Reveal-on-scroll (Waypoints). Skipped when the user prefers reduced motion.
    $(document).ready(function () {
        var prefersReducedMotion = window.matchMedia &&
            window.matchMedia('(prefers-reduced-motion: reduce)').matches;

        if (prefersReducedMotion || typeof $.fn.waypoint !== 'function') {
            $('.reveal').addClass('is-visible');
            return;
        }

        $('.reveal').each(function () {
            $(this).waypoint(function () {
                $(this.element).addClass('is-visible');
            }, { offset: '85%' });
        });
    });

    // Carousel pause/play toggle (WCAG 2.2.2 - pausable auto-rotation)
    $(document).ready(function () {
        $('.carousel-pause-toggle').on('click', function () {
            var $btn = $(this);
            var targetSelector = $btn.attr('data-bs-target');
            var carouselEl = targetSelector ? document.querySelector(targetSelector) : null;
            if (!carouselEl || typeof bootstrap === 'undefined') { return; }

            var instance = bootstrap.Carousel.getOrCreateInstance(carouselEl);
            var isPaused = $btn.attr('aria-pressed') === 'true';

            if (isPaused) {
                instance.cycle();
                $btn.attr('aria-pressed', 'false')
                    .attr('aria-label', 'Pause automatic slide rotation')
                    .find('i').attr('class', 'bi bi-pause-fill');
            } else {
                instance.pause();
                $btn.attr('aria-pressed', 'true')
                    .attr('aria-label', 'Resume automatic slide rotation')
                    .find('i').attr('class', 'bi bi-play-fill');
            }
        });
    });

})(jQuery);

// OpenAi API
function promptChatGpt(artist) {
    if (artist.length == 0) { return; }

    $.ajax({
        url: '/api/chatgpt/?art=' + encodeURI(artist),
        method: 'GET'
    })
        .done(function (data) {
            var loading = $("#loading");
            loading.remove();
            if (data.length > 0) {
                $("#chatGpt").html(data);
            }
            else {
                $("#chatGpt").html("ChatGPT haven't found any facts.");
            }
        });
}

function promptChatGptRelease(artist, release) {
    if (artist.length == 0 || release.length == 0) { return; }

    $.ajax({
        url: '/api/chatgpt/?art=' + encodeURI(artist) + '&rel=' + encodeURI(release),
        method: 'GET'
    })
        .done(function (data) {
            var loading = $("#loading-release");
            loading.remove();
            if (data.length > 0) {
                $("#chatGpt-release").html(data);
            }
            else {
                $("#chatGpt-release").html("ChatGPT haven't found any facts.");
            }
        });
}
