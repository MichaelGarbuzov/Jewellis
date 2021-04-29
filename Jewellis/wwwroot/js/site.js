/*==============================================
    site.js
    ---------------------
    Description: Main script for the site.
    Version: 1.0.0
    Last Update: 2021-04-30
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


/*----------------------------------------------
 * (2) - General Components
----------------------------------------------*/
$(function () {

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

});

