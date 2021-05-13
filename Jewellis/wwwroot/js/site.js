/*==============================================
    site.js
    ---------------------
    Description: Main script for the site.
    Version: 1.0.0
    Last Update: 2021-05-13
==============================================*/
/*==============================================
Table of Contents:
------------------
    (1) - General Methods
    (2) - General Components
    (3) - Main Layout Functionality
==============================================*/

/*----------------------------------------------
 * (1) - General Methods
----------------------------------------------*/
/**
 * Disables/Enables window scrolling temporarily.
 */
var WindowScroll = (function () {
    var x, y;
    function hndlr() {
        window.scrollTo(x, y);
    }
    return {
        disable: function () {
            x = 0;
            y = $(document).scrollTop();

            if (window.addEventListener) {
                window.addEventListener("scroll", hndlr);
            } else {
                window.attachEvent("onscroll", hndlr);
            }
        },
        enable: function () {
            if (window.removeEventListener) {
                window.removeEventListener("scroll", hndlr);
            } else {
                window.detachEvent("onscroll", hndlr);
            }
        }
    }
})();

/**
 * Gets a cookie value, by the cookie name.
 * @param {any} cookieName The cookie name to get the value.
 */
function getCookie(cookieName) {
    var name = cookieName + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

/**
 * Sets a cookie, by the specified name and value.
 * @param {any} cookieName The name of the cookie to set.
 * @param {any} cookieValue The value of the cookie to set.
 * @param {any} daysToExpire The number of days the cookie will be expired.
 */
function setCookie(cookieName, cookieValue, daysToExpire) {
    var date = new Date();
    date.setTime(date.getTime() + (daysToExpire * 24 * 60 * 60 * 1000));
    var expires = "expires=" + date.toUTCString();

    document.cookie = cookieName + "=" + cookieValue + ";" + expires + ";path=/;secure;";
}

/*----------------------------------------------
 * (2) - General Components
----------------------------------------------*/
$(function () {

    // Registers bootstrap's popover elements.
    $(function () {
        $('[data-toggle="popover"]').popover();
    });

    // Attribute for bootstrap's dropdown - marks an inner link to not close the dropdown on click, because the default closes the dropdown.
    $('[data-dd-close="0"]').on('click.bs.dropdown', function (e) {
        e.stopPropagation();
        e.preventDefault();
    });

    // [Dropdown Mega] = A dropdown with several menus inside, with cool animation that "replaces" the current menu.
    // This sets the default menu before the dropdown is shown:
    $('[data-dd-mega]').parent('.dropdown').on('show.bs.dropdown', function () {
        var $dropdownMega = $(this).find('[data-dd-mega]');
        var $defaultMenu = $dropdownMega.find($dropdownMega.attr('data-dd-mega-start'));
        var $activeMenu = $dropdownMega.find('[data-dd-mega-active]');

        $activeMenu.attr('style', '');
        $activeMenu.removeAttr('data-dd-mega-active');
        $defaultMenu.attr('data-dd-mega-active', '');
    });
    // This changes the current menu to the clicked one:
    $('[data-dd-mega-to]').click(function (e) {
        e.preventDefault();
        var $dropdownMega = $(this).parents('[data-dd-mega]');
        var $fromDropdown = $dropdownMega.find('[data-dd-mega-active]');
        var $toDropdown = $dropdownMega.find($(this).attr('data-dd-mega-to'));
        var currentWidth = $fromDropdown.innerWidth();

        $fromDropdown.animate(200, function () {
            $(this).removeAttr('data-dd-mega-active');
            $(this).attr('style', '');
        });
        $toDropdown.css({ 'min-width': currentWidth });
        $toDropdown.fadeIn(200, function () {
            $(this).attr('data-dd-mega-active', '');
        });
    });

    // Input: Number functionality:
    // ---------------------------
    // Occurres when the input text is focused:
    $('[data-in-num]').focusin(function (e) {
        e.preventDefault();
        $(document).keydown(inputNumberKeyListener);
    });
    var inputNumberKeyListener = function (e) {
        var $input = $(e.target);
        var $container = $input.parent('[data-in-num]');

        // Arrow up:
        if (e.keyCode === 38) {
            $container.children('[data-in-num-inc]').click();
            e.preventDefault();
        }
        // Arrow down:
        else if (e.keyCode === 40) {
            $container.children('[data-in-num-dec]').click();
            e.preventDefault();
        }
        // Any letter:
        else if (event.keyCode >= 65 && event.keyCode <= 90) {
            e.preventDefault();
        }
    };
    $('[data-in-num]').focusout(function (e) {
        e.preventDefault();
        var $input = $(this).children('input');

        var minVal = Number.parseInt($(this).attr('data-in-num-min'));
        var maxVal = Number.parseInt($(this).attr('data-in-num-max'));
        var currentVal = Number.parseInt($input.val());

        if (!currentVal) {
            currentVal = 0;
            $input.val(currentVal);
        }

        if (maxVal) {
            if (currentVal > maxVal) {
                $input.val(maxVal);
            }
        }
        if (minVal) {
            if (currentVal < minVal) {
                $input.val(minVal);
            }
        }
        $(document).unbind('keydown', inputNumberKeyListener);
    });
    // Occurres when the increase button is clicked:
    $('[data-in-num-inc]').click(function (e) {
        e.preventDefault();
        var $container = $(this).parent('[data-in-num]');
        var $input = $container.children('input');

        var maxVal = Number.parseInt($container.attr('data-in-num-max'));
        var currentVal = Number.parseInt($input.val());

        if (maxVal) {
            if (currentVal + 1 <= maxVal) {
                $input.val(currentVal + 1);
            }
        } else {
            $input.val(currentVal + 1);
        }
    });
    // Occurres when the decrease button is clicked:
    $('[data-in-num-dec]').click(function (e) {
        e.preventDefault();
        var $container = $(this).parent('[data-in-num]');
        var $input = $container.children('input');

        var minVal = Number.parseInt($container.attr('data-in-num-min'));
        var currentVal = Number.parseInt($input.val());

        if (minVal) {
            if (currentVal - 1 >= minVal) {
                $input.val(currentVal - 1);
            }
        } else {
            $input.val(currentVal - 1);
        }
    });

});

/*----------------------------------------------
 * (3) - Main Layout Functionality
----------------------------------------------*/
$(function () {

    // Main Menu:
    // ----------
    // In desktop state - the main-menu behaves like a dropdown when hovering.
    // In mobile state - the main-menu converts to a toggler which toggles the main-menu, then inside - each dropdown is converted to a toggler as well.
    // Click on the main-menu toggler:
    $('#main-menu-toggler').click(function () {
        var isExpanded = $(this).attr('aria-expanded');
        var $header = $(this).parents('#main-header');
        var $menu = $header.find('#main-menu');
        var $dropdownTogglers = $menu.find('.dropdown-show > a');

        if (isExpanded === "true") {
            WindowScroll.enable();
            // Animates the menu:
            $menu.animate({
                opacity: 0,
                height: '0'
            }, 150, function () {
                $menu.removeClass('collapsed');
                $menu.attr('style', '');
            });
            // Returns to the dropdown desktop behavior:
            $dropdownTogglers.removeClass('dd-toggler');
            $dropdownTogglers.attr('href', $dropdownTogglers.attr('data-url-tmp'));
            $dropdownTogglers.removeAttr('data-url-tmp');
            // Updates the state of the main toggler:
            $(this).removeClass('collapsed');
            $(this).attr('aria-expanded', 'false');
        } else {
            WindowScroll.disable();
            // Animates the menu:
            $menu.css({
                'display': 'block',
                'height': '0',
                'opacity': '0'
            });
            $menu.addClass('collapsed');
            $menu.animate({
                opacity: 1,
                height: (window.innerHeight - $header.outerHeight())
            }, 200, function () {
                $menu.attr('style', '');
            });
            // Enters to the dropdown mobile behavior (togglers):
            $dropdownTogglers.addClass('dd-toggler')
            $dropdownTogglers.attr('data-url-tmp', $dropdownTogglers.attr('href'));
            $dropdownTogglers.attr('href', 'javascript: void(0);');
            // Updates the state of the main toggler:
            $(this).addClass('collapsed');
            $(this).attr('aria-expanded', 'true');
        }
    });
    // Click on a dropdown toggler (in mobile state):
    $(document).on('click', '#main-menu .dd-toggler', function () {
        var isExpanded = $(this).attr('aria-expanded');
        var $dropdownNav = $(this).parent('.dropdown-show').find('.dropdown-nav');

        if (isExpanded === "true") {
            $dropdownNav.hide(150, function () {
                $dropdownNav.attr('style', '');
            });
            $(this).attr('aria-expanded', 'false');
        } else {
            $dropdownNav.show(200);
            $(this).attr('aria-expanded', 'true');
        }
    });

    // Main Search:
    // ------------
    $('#main-search-open').on('click', function () {
        var $searchContainer = $('#main-search');

        WindowScroll.disable();
        $searchContainer.fadeIn(200);
        $searchContainer.find('#search-query').focus();
        // Listens to 'Esc' press, to close the main search:
        $(document).keydown(mainSearchEscListener);
        return false;
    });
    var mainSearchEscListener = function (e) {
        if (e.key === "Escape") {
            $('#main-search-close').click();
        }
    };
    $('#main-search-close').on('click', function () {
        var $searchContainer = $('#main-search');

        WindowScroll.enable();
        $searchContainer.fadeOut(300, function () {
            $searchContainer.find('#search-query').val('');
        });
        // Removes listener to 'Esc' press:
        $(document).unbind('keydown', mainSearchEscListener);
        return false;
    });

    // Theme change:
    // -------------
    $('[data-theme-set]').click(function () {
        var $themeMenu = $(this).parents('#main-theme-menu');

        // Updates the html:
        $('html[theme]').attr('theme', $(this).attr('data-theme-set'));
        $themeMenu.find('[data-dd-check]').appendTo($(this));
        $themeMenu.parent('[data-dd-mega]').find('#main-theme-btn').find('[data-variable]').text($(this).attr('data-theme-set-display'));
        // Updates the cookie:
        setCookie("theme", $(this).attr('data-theme-set-val'), 365 * 100);
    });

});

