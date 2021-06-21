/*==============================================
    site.js
    ---------------------
    Description: Main script for the site.
    Version: 1.0.0
    Last Update: 2021-06-21
==============================================*/
/*==============================================
Table of Contents:
------------------
    (1) - General Declarations
    (2) - General Methods
    (3) - General Components
    (4) - Main Layout Functionality
==============================================*/

/*----------------------------------------------
 * (1) - General Declarations
----------------------------------------------*/
var AppKeys = {
    Cookies: {
        ClientTheme: "theme",
        ClientCurrency: "currency",
        ClientCart: "cart"
    }
};


/*----------------------------------------------
 * (2) - General Methods
----------------------------------------------*/
var ArrayHelper = {
    /**
     * Checks if an array contains an object by the specified key value.
     */
    containsObjectByKey: function (array, key, value) {
        for (let i = 0; i < array.length; i++) {
            if (array[i][key] === value) {
                return true;
            }
        }
        return false;
    },
    /**
     * Gets an object by the specified key value in the array.
     */
    getObjectByKey: function (array, key, value) {
        for (let i = 0; i < array.length; i++) {
            if (array[i][key] === value) {
                return array[i];
            }
        }
        return null;
    },
    /**
     * Removes an object from the array by the specified key value.
     */
    removeObjectByKey: function (array, key, value) {
        for (let i = 0; i < array.length; i++) {
            if (array[i][key] === value) {
                array.splice(i, 1);
            }
        }
    }
};

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

/**
 * Binds the object to the specified template, and returns the binded template.
 */
function bindObjectToTemplate(object, template) {
    let templateVar = template;

    for (var key of Object.keys(object)) {
        if (object[key] !== null) {
            templateVar = templateVar.replaceAll('{' + key + '}', object[key]);
        } else {
            templateVar = templateVar.replaceAll('{' + key + '}', '');
        }
    }
    return templateVar;
}


/*----------------------------------------------
 * (3) - General Components
----------------------------------------------*/
$(function () {

    // Registers Bootstrap's tooltip elements:
    $('[data-toggle="tooltip"]').tooltip();

    // Registers Bootstrap's popover elements:
    $('[data-toggle="popover"]').popover();

    // Attribute for bootstrap's dropdown - marks an inner link to not close the dropdown on click, because the default closes the dropdown.
    $('[data-dd-close="0"]').on('click.bs.dropdown', function (e) {
        e.stopPropagation();
    });
    $('[data-dd-close="1"]').on('click.bs.dropdown', function (e) {
        $(this).parents('.dropdown').find('[data-toggle]').dropdown('toggle');
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
    $(document).on('focusin', '[data-in-num]', function (e) {
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
    $(document).on('focusout', '[data-in-num]', function (e) {
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
    $(document).on('click', '[data-in-num-inc]', function (e) {
        e.preventDefault();
        var $container = $(this).parent('[data-in-num]');
        var $input = $container.children('input');

        var maxVal = Number.parseInt($container.attr('data-in-num-max'));
        var currentVal = Number.parseInt($input.val());

        if (maxVal) {
            if (currentVal + 1 <= maxVal) {
                $input.val(currentVal + 1);
                $input.trigger('change');
            }
        } else {
            $input.val(currentVal + 1);
            $input.trigger('change');
        }
    });
    // Occurres when the decrease button is clicked:
    $(document).on('click', '[data-in-num-dec]', function (e) {
        e.preventDefault();
        var $container = $(this).parent('[data-in-num]');
        var $input = $container.children('input');

        var minVal = Number.parseInt($container.attr('data-in-num-min'));
        var currentVal = Number.parseInt($input.val());

        if (minVal) {
            if (currentVal - 1 >= minVal) {
                $input.val(currentVal - 1);
                $input.trigger('change');
            }
        } else {
            $input.val(currentVal - 1);
            $input.trigger('change');
        }
    });

    // Stepper:
    // --------
    $('[data-step-target]').click(function () {
        var $newTab = $(this);
        var $stepper = $newTab.parents('[data-stepper]');
        var $stepTabs = $stepper.find('[data-step-target]');
        var $currentTab = $stepTabs.filter('.current');
        // Checks if it's already the current step:
        if ($newTab.is($currentTab)) {
            return;
        }
        var $newPanel = $stepper.find($newTab.attr('data-step-target'));
        var $currentPanel = $stepper.find($currentTab.attr('data-step-target'));

        if ($currentPanel.attr('data-step-edited')) {
            validatePanel($currentPanel);
        }
        if (isValidPanel($currentPanel)) {
            $currentTab.addClass('done');
        } else {
            $currentTab.removeClass('done');
        }

        // Changes the tabs:
        $currentTab.removeClass('current');
        $currentTab.attr('aria-selected', 'false');
        $newTab.addClass('current');
        $newTab.attr('aria-selected', 'true');
        // Changes the panels:
        $currentPanel.fadeOut(100, function () {
            $currentPanel.removeClass('active');
            $newPanel.fadeIn(200, function () {
                $newPanel.addClass('active');
            });

            // Checks if the new tab is the last one, in order to display the "finish" action:
            if ($newTab.is($stepTabs.last())) {
                $stepper.find('[data-step-next]').css('display', 'none');
                $stepper.find('[data-step-finish]').css('display', 'block');
            } else {
                $stepper.find('[data-step-finish]').css('display', 'none');
                $stepper.find('[data-step-next]').css('display', 'block');
            }

            // Checks if the new tab is the first one, in order to hide the "previous" action:
            if ($newTab.is($stepTabs.first())) {
                $stepper.find('[data-step-prev]').css('visibility', 'hidden');
            } else {
                $stepper.find('[data-step-prev]').css('visibility', 'visible');
            }
        });
    });
    $('[data-step-prev]').click(function () {
        var $stepper = $(this).parents('[data-stepper]');
        var $currentTab = $stepper.find('[data-step-target].current');

        // Gets the previous tab:
        var $prevTab = $currentTab.parent().prev().children();
        if ($prevTab) {
            $prevTab.click();
        }
    });
    $('[data-step-next]').click(function () {
        let $stepper = $(this).parents('[data-stepper]');
        let $currentTab = $stepper.find('[data-step-target].current');
        let $currentPanel = $stepper.find($currentTab.attr('data-step-target'));

        // Validates the current panel:
        if (validatePanel($currentPanel)) {
            $currentTab.addClass('done');
            $currentPanel.attr('data-step-validated', 'true');

            // Goes to the next tab:
            let $nextTab = $currentTab.parent().next().children();
            if ($nextTab) {
                $nextTab.click();
            }
        }
    });
    var isValidPanel = function ($panel) {
        if ($panel.attr('data-step-edited') || $panel.attr('data-step-validated')) {
            return ($panel.find('.field-validation-error').length < 1);
        } else {
            return false;
        }
    };
    var validatePanel = function ($panel) {
        let errorCounter = 0;
        $panel.find('input,textarea').each(function () {
            if (!$(this).valid()) {
                errorCounter++;
            }
        });
        return (errorCounter == 0);
    };
    $('[data-stepper-form]').submit(function () {
        // On form submit, checks to find errors in the form, and then navigates to the related panel.
        let $thisForm = $(this);
        let $errorFields = $thisForm.find('.field-validation-error');
        if ($errorFields.length > 0) {
            let $panelContainer = $errorFields.first().parents('.step-panel');
            let $stepTarget = $thisForm.find('[data-step-target="#' + $panelContainer.attr('id') + '"]');
            $stepTarget.click();
        }
    });
    $(document).ready(function () {
        // If stepper form exists in the page:
        let $stepper = $('[data-stepper-form]');
        if ($stepper.length) {
            // Adds a listener to enter keypress (to go next):
            $(document).keydown(stepperFormKeyPressListener);
            // Adds a listener to change event of fields, in order to indicate when a panel has edited:
            $stepper.find('input,textarea').each(function () {
                $(this).change(function () {
                    $(this).parents('.step-panel').attr('data-step-edited', 'true');
                });
            });
        }
    });
    var stepperFormKeyPressListener = function (e) {
        // "Enter" keypress:
        if (e.keyCode === 13) {
            e.preventDefault();
            let $nextBtn = $('[data-step-next]');
            if ($nextBtn.is(':visible')) {
                $nextBtn.click();
            } else {
                $('[data-stepper-form]').submit();
            }
        }
    };

    // Tab Switcher:
    // -------------
    $('[data-tab-target]').click(function () {
        var $newTab = $(this);
        var $container = $newTab.parents('[data-tab-switcher]');
        var $currentTab = $container.find('[data-tab-target].current');
        // Checks if it's already the current step:
        if ($newTab.is($currentTab)) {
            return;
        }
        // Changes the tabs:
        $currentTab.removeClass('current');
        $currentTab.attr('aria-selected', 'false');
        $newTab.addClass('current');
        $newTab.attr('aria-selected', 'true');
        // Changes the panels:
        var $newPanel = $container.find($newTab.attr('data-tab-target'));
        var $currentPanel = $container.find($currentTab.attr('data-tab-target'));
        $currentPanel.fadeOut(100, function () {
            $currentPanel.removeClass('active');
            $newPanel.fadeIn(200, function () {
                $newPanel.addClass('active');
            });
        });
    });

    // Checkbox Element Toggler:
    // -------------------------
    $('[data-check-toggle-target]').click(function () {
        var $target = $($(this).attr('data-check-toggle-target'));

        if ($target.attr('data-check-toggle-state') === '0') {
            $target.fadeIn(200);
            $target.attr('data-check-toggle-state', '1');
        } else {
            $target.fadeOut(100);
            $target.attr('data-check-toggle-state', '0');
        }
    });

    // Slider:
    // -------
    $('[data-slider]').each(function () {
        $(this).append('<div class="selected-range-fill"></div>');
        updateSelectedRangeFill($(this));
    });
    function updateSelectedRangeFill($slider) {
        var $fill = $slider.find('.selected-range-fill');
        var $minElm = $slider.find('[data-slider-min]');
        var $maxElm = $slider.find('[data-slider-max]');

        var sliderStep = parseFloat($minElm.attr('step'));
        var minVal = parseFloat($minElm.attr('min'));
        var maxVal = parseFloat($minElm.attr('max'));
        var currentMin = parseFloat($minElm.val());
        var currentMax = parseFloat($maxElm.val());

        var totalWidth = $slider.outerWidth();
        var stepWeightInPX = (totalWidth / ((maxVal - minVal) / sliderStep));

        var stepsFromMin = (currentMin - minVal);
        var stepsFromMax = (maxVal - currentMax);
        var totalMinStepsWeightInPX = (stepsFromMin * stepWeightInPX);
        var totalMaxStepsWeightInPX = (stepsFromMax * stepWeightInPX);

        $fill.css("left", totalMinStepsWeightInPX);
        $fill.css("width", (totalWidth - totalMinStepsWeightInPX - totalMaxStepsWeightInPX));
    };
    $('[data-slider-min]').on('input', function () {
        // Checks the min slider did not pass the max slider:
        var $slider = $(this).parent('[data-slider]');
        var $max = $slider.find('[data-slider-max]');

        if (parseFloat($(this).val()) > parseFloat($max.val())) {
            $(this).val($max.val());
        } else {
            // Updates the associated label:
            var $target = $($(this).attr('data-slider-label-target'));
            if ($target) {
                $target.text($(this).val());
                $target.trigger('change');
            }
            // Updates the selected range fill:
            updateSelectedRangeFill($slider);
        }
    });
    $('[data-slider-max]').on('input', function () {
        // Checks the max slider did not pass the min slider:
        var $slider = $(this).parent('[data-slider]');
        var $min = $slider.find('[data-slider-min]');

        if (parseFloat($(this).val()) < parseFloat($min.val())) {
            $(this).val($min.val());
        } else {
            // Updates the associated label:
            var $target = $($(this).attr('data-slider-label-target'));
            if ($target) {
                $target.text($(this).val());
                $target.trigger('change');
            }
            // Updates the selected range fill:
            updateSelectedRangeFill($slider);
        }
    });
    $(window).resize(function () {
        $('[data-slider]').each(function () {
            updateSelectedRangeFill($(this));
        });
    });

    // Submit Button Loader:
    // ---------------------
    $('[data-submit-loader]').each(function () {
        var $thisSubmitBtn = $(this);
        // Finds the related form:
        var $form;
        if ($thisSubmitBtn.attr('data-submit-loader')) {
            $form = $($thisSubmitBtn.attr('data-submit-loader'));
        } else {
            $form = $thisSubmitBtn.parents('form');
        }

        // Registers to the submit event of the form:
        $form.submit(function () {
            if (!$.isFunction($.fn.valid) || $(this).valid() === true) {
                $thisSubmitBtn.attr('disabled', '');
                if (!$thisSubmitBtn.find('.spinner-border').length) {
                    $thisSubmitBtn.prepend('<span class="spinner-border icon-top-adjust mr-3" role="status" aria-hidden="true"></span><span class= "sr-only">Loading...</span>');
                }
            } else {
                $thisSubmitBtn.removeAttr('disabled');
                $thisSubmitBtn.find('.spinner-border').remove();
            }
        });

        // Binds to the invalid event, mostly for remote validation:
        $form.bind("invalid-form.validate", function () {
            $thisSubmitBtn.removeAttr('disabled');
            $thisSubmitBtn.find('.spinner-border').remove();
        });
    });

    // Remote Submit Form Button:
    // --------------------------
    $('[data-remote-submit]').click(function (e) {
        e.preventDefault();
        var $form = $($(this).attr('data-remote-submit'));
        $form.submit();
    });

    // AJAX Unobtrusive Submition:
    // ---------------------------
    $('[data-ajax="true"]').submit(function (e) {
        e.preventDefault();
        var $form = $(this);

        // Checks the form is valid:
        if (!$form.valid()) {
            return;
        }

        // Builds the view model:
        var viewModel = {};
        $form.find('[name]').each(function () {
            viewModel[$(this).attr('name')] = $(this).val();
        });

        var method = $form.attr('data-ajax-method');

        // Sends the AJAX request:
        $.ajax({
            type: method,
            url: $form.attr('action'),
            data: (method === 'post' ? JSON.stringify(viewModel) : viewModel),
            headers: { 'RequestVerificationToken': viewModel["__RequestVerificationToken"] },
            contentType: 'application/json',
            dataType: 'json',
            complete: function () {
                // Removes the submit loader if exists:
                var $submitLoader = $form.find('[data-submit-loader]');
                if ($submitLoader) {
                    $submitLoader.removeAttr('disabled');
                    $submitLoader.find('.spinner-border').remove();
                }

                // Triggers the complete callback:
                if ($form.attr('data-ajax-complete')) {
                    eval($form.attr('data-ajax-complete') + '();');
                }
            },
            success: function (response) {
                // Updates the html:
                if ($form.attr('data-ajax-update')) {
                    var $updateElement = $($form.attr('data-ajax-update'));
                    $updateElement.empty();
                    $updateElement.show();
                    // Checks if there's a template:
                    if ($form.attr('data-ajax-update-template')) {
                        var template = $($form.attr('data-ajax-update-template')).html();
                        $.each(response, function (i, val) {
                            var templateVar = template;

                            $.each(val, function (key, value) {
                                templateVar = templateVar.replaceAll('{' + key + '}', value);
                            });

                            $updateElement.append(templateVar);
                        });
                    } else {
                        $updateElement.text(response.message);
                    }
                }

                // Clears the form on success:
                if ($form.attr('data-ajax-success-clear')) {
                    $form.find('input:not([type="hidden"])').val('');
                }

                // Triggers the success callback:
                if ($form.attr('data-ajax-success')) {
                    eval($form.attr('data-ajax-success') + '(response);');
                }
            },
            error: function (response) {
                if ($form.attr('data-ajax-update')) {
                    var $msgElement = $($form.attr('data-ajax-update'));
                    $msgElement.text(response.message);
                }

                // Triggers the failure callback:
                if ($form.attr('data-ajax-failure')) {
                    eval($form.attr('data-ajax-failure') + '(response);');
                }
            }
        });
    });

    // Changes the value of an input element by click:
    // -----------------------------------------------
    $('[data-change-val]').click(function (e) {
        e.preventDefault();
        var $this = $(this);
        var $toChange = $($this.attr('data-change-val'));
        $toChange.val($this.val());
        $toChange.trigger('change');
    });
    $('[data-update-val]').click(function () {
        var $this = $(this);
        var $toUpdate = $($this.attr('data-update-val'));
        $toUpdate.val($this.val());
        $toUpdate.trigger('change');
    });

    // Submits the parent form, on value change of this element:
    // ---------------------------------------------------------
    $('[data-on-change-submit]').change(function (e) {
        e.preventDefault();
        $(this).parents('form').submit();
    });

});


/*----------------------------------------------
 * (4) - Main Layout Functionality
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
        $searchContainer.find('#main-search-query').focus();
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
            $searchContainer.find('#main-search-query').val('');
        });
        // Removes listener to 'Esc' press:
        $(document).unbind('keydown', mainSearchEscListener);
        return false;
    });

    // Theme change:
    // -------------
    $('[data-theme-set]').click(function (e) {
        e.preventDefault();
        var $this = $(this);
        let $themeMenu = $this.parents('#main-theme-menu');

        // Updates the html:
        $('html[theme]').attr('theme', $this.attr('data-theme-set'));
        $themeMenu.find('[data-dd-check]').appendTo($this);
        $themeMenu.parent('[data-dd-mega]').find('#main-theme-btn').find('[data-updatable]').text($this.attr('data-theme-set-display'));
        // Updates the cookie:
        setCookie(AppKeys.Cookies.ClientTheme, $this.attr('data-theme-set'), 365 * 100);
        // Sends an AJAX request, to update the server side if needed (when user is authenticated):
        $.ajax({
            type: 'post',
            url: $themeMenu.attr('data-theme-set-link'),
            data: JSON.stringify($this.attr('data-theme-set')),
            contentType: 'application/json',
            dataType: 'json',
            success: function () {
                $this.trigger('change');
            }
        });
    });

    // Currency change:
    // ----------------
    $('[data-currency-set]').click(function (e) {
        e.preventDefault();
        var $currencyMenu = $(this).parents('#main-currency-menu');

        if ($(this).find('[data-dd-check]').length > 0 || $(this).find('.spinner-border').length > 0) {
            return false;
        }

        // Updates the html - shows a loader:
        $(this).append('<span class="spinner-border txt-accent icon-top-adjust mr-2" role="status" aria-hidden="true"></span><span class= "sr-only">Loading...</span>');
        // Updates the cookie:
        setCookie(AppKeys.Cookies.ClientCurrency, $(this).attr('data-currency-set'), 365 * 100);
        // Sends an AJAX request, to update the server side if needed (when user is authenticated):
        $.ajax({
            type: 'post',
            url: $currencyMenu.attr('data-currency-set-link'),
            data: JSON.stringify($(this).attr('data-currency-set')),
            contentType: 'application/json',
            dataType: 'json',
            complete: function () {
                // Reloads the page:
                location.reload();
            }
        });
        return false;
    });

    // Add To Cart (Mini Cart):
    // ------------------------
    $(document).on('click', '[data-add-to-cart]', function () {
        if (!clientCart)
            alert("Failed to add to cart, please try again later.");

        let product = JSON.parse($(this).attr('data-add-to-cart'));

        // Gets the quantity to add (if requested):
        let $quantityInput = $(this).parent().find('[data-add-to-cart-quantity]');
        let quantityRequest;
        if ($quantityInput) {
            quantityRequest = parseInt($quantityInput.val());
        }

        // Checks if the product already exists in the cart:
        let currentProductInCart = ArrayHelper.getObjectByKey(clientCart, 'id', product.id);
        let cacheToRevert = Object.assign({}, currentProductInCart);
        if (currentProductInCart) {
            if (quantityRequest) {
                currentProductInCart.quantity = quantityRequest;
            } else {
                currentProductInCart.quantity++;
            }
            product.quantity = currentProductInCart.quantity;
        } else {
            if (quantityRequest) {
                product.quantity = quantityRequest;
            } else {
                product.quantity = 1;
            }
            clientCart.unshift(product);
        }

        // Updates the html of the cart:
        updateMiniCartHtml(clientCart);

        // Updates the server using AJAX about adding this item:
        let $miniCartContainer = $('#mini-cart');
        $.ajax({
            type: 'post',
            url: $miniCartContainer.attr('data-mini-cart-set-link'),
            data: { productId: product.id, quantity: product.quantity },
            error: function () {
                // Reverts the change:
                if (currentProductInCart) {
                    currentProductInCart.quantity = cacheToRevert.quantity;
                } else {
                    ArrayHelper.removeObjectByKey(clientCart, 'id', product.id);
                }
                // Updates the html of the cart:
                updateMiniCartHtml(clientCart);
            }
        });
    });
    /**
     * Updates the html of the mini shopping cart in the header (layout), by the specified JSON array of products.
     */
    var updateMiniCartHtml = function (products) {
        let $container = $('#mini-cart');
        let $miniCartCounter = $container.parent().find('#mini-cart-counter');
        let $miniCartWrapper = $container.children('.mini-cart-wrapper');
        let $miniCartItems = $miniCartWrapper.children('.mini-cart-items');
        let $miniCartTotal = $miniCartWrapper.find('#mini-cart-total');
        let miniCartItemTemplate = $container.children('#mini-cart-item-template').html();

        $miniCartItems.empty();
        $miniCartTotal.empty();

        // Updates the cart:
        if (products && products.length > 0) {
            let totalPrice = 0;

            // Appends the product items:
            for (let i = 0; i < products.length; i++) {
                let bindedTemplate = bindObjectToTemplate(products[i], miniCartItemTemplate);

                // Checks if there's a sale on the product:
                if (products[i].regularPrice) {
                    bindedTemplate = $(bindedTemplate).find('.actual-price').addClass('txt-accent').parents('.single-mini-product');
                }
                $miniCartItems.append(bindedTemplate);

                // Adds to the total price:
                totalPrice += (parseFloat(products[i].actualPrice.substring(1)) * products[i].quantity);
            }
            // Sets the total price:
            $miniCartTotal.text(products[0].actualPrice.charAt(0) + totalPrice.toFixed(2));

            // Updates the counter:
            $miniCartCounter.text(products.length);
            $miniCartCounter.show();
            // Updates the cart container:
            $container.find('.mini-cart-empty').hide();
            $miniCartWrapper.show();
        } else {
            // Updates the counter:
            $miniCartCounter.text('0');
            $miniCartCounter.hide();
            // Updates the cart container:
            $miniCartWrapper.hide();
            $container.find('.mini-cart-empty').show();
        }
    };
    $('#mini-cart').on('click', '[data-remove-from-cart]', function () {
        if (!clientCart)
            alert("Failed to remove from cart, please try again later.");

        // Removes the product from the cart:
        let productId = parseInt($(this).attr('data-remove-from-cart'));
        ArrayHelper.removeObjectByKey(clientCart, 'id', productId);

        // Updates the html of the cart:
        updateMiniCartHtml(clientCart);

        // Updates the server using AJAX about adding this item:
        let $miniCartContainer = $('#mini-cart');
        $.ajax({
            type: 'post',
            url: $miniCartContainer.attr('data-mini-cart-remove-link'),
            data: { productId: productId },
            error: function () {
                alert("Failed to remove from cart, please try again later.");
            }
        });
    });

    // Add To Wishlist (Mini Wishlist):
    // --------------------------------
    $(document).on('click', '[data-add-to-wishlist]', function () {
        if (typeof userWishlist === 'undefined' || !userWishlist)
            return false;

        let product = JSON.parse($(this).attr('data-add-to-wishlist'));

        // Checks if the product already exists in the wishlist:
        let currentProductInWishlist = ArrayHelper.getObjectByKey(userWishlist, 'id', product.id);
        if (!currentProductInWishlist) {
            userWishlist.unshift(product);
        }

        // Updates the html of the wishlist:
        updateMiniWishlistHtml(userWishlist);

        // Updates the server using AJAX about adding this item:
        let $miniWishlistContainer = $('#mini-wishlist');
        $.ajax({
            type: 'post',
            url: $miniWishlistContainer.attr('data-wishlist-add-link'),
            data: { productId: product.id },
            error: function () {
                // Reverts the change:
                if (!currentProductInWishlist) {
                    ArrayHelper.removeObjectByKey(userWishlist, 'id', product.id);
                }
                // Updates the html of the wishlist:
                updateMiniWishlistHtml(userWishlist);
            }
        });
    });
    /**
     * Updates the html of the mini wishlist, by the specified JSON array of products.
     */
    var updateMiniWishlistHtml = function (products) {
        let $container = $('#mini-wishlist');
        let $miniWishlistItems = $container.children('.mini-wishlist-items');
        let miniWishlistItemTemplate = $container.children('#mini-wishlist-item-template').html();

        $miniWishlistItems.empty();

        // Updates the wishlist:
        if (products && products.length > 0) {
            // Appends the product items:
            for (let i = 0; i < products.length; i++) {
                let bindedTemplate = bindObjectToTemplate(products[i], miniWishlistItemTemplate);
                $miniWishlistItems.append(bindedTemplate);
                $miniWishlistItems.children().last().find('[data-add-to-cart]').attr('data-add-to-cart', JSON.stringify(products[i]));
            }
            // Updates the wishlist container:
            $container.find('.mini-wishlist-empty').hide();
            $miniWishlistItems.show();
        } else {
            // Updates the wishlist container:
            $miniWishlistItems.hide();
            $container.find('.mini-wishlist-empty').show();
        }
    };
    $('#mini-wishlist').on('click', '[data-remove-from-wishlist]', function () {
        if (!userWishlist)
            alert("Failed to remove from wishlist, please try again later.");

        // Removes the product from the wishlist:
        let productId = parseInt($(this).attr('data-remove-from-wishlist'));
        ArrayHelper.removeObjectByKey(userWishlist, 'id', productId);

        // Updates the html of the wishlist:
        $(this).parents('.single-mini-product').remove();

        if (userWishlist.length < 1) {
            // Updates the wishlist container:
            let $container = $('#mini-wishlist');
            $container.find('.mini-wishlist-items').hide();
            $container.find('.mini-wishlist-empty').show();
        }

        // Updates the server using AJAX about adding this item:
        let $miniWishlistContainer = $('#mini-wishlist');
        $.ajax({
            type: 'post',
            url: $miniWishlistContainer.attr('data-wishlist-remove-link'),
            data: { productId: productId },
            error: function () {
                alert("Failed to remove from wishlist, please try again later.");
            }
        });
    });

});

