/*==============================================
    custom-validations.js
    ---------------------
    Description: Custom validations for the site.
    Version: 1.0.0
    Last Update: 2021-06-16
==============================================*/

// By default validator ignores hidden fields.
// So enables validation on hidden fields:
$.validator.setDefaults({ ignore: null });

// [DateTimeGreaterThan] = Validates that the datetime is greater than the datetime of the specified property name.
$.validator.addMethod('dtgt', function (value, element, params) {
    var thisDate = new Date(value); // Gets the date of the property.
    var lowerDateVal = $('#' + params.lower).val(); // Gets the lower date string.
    var allowEqual = (params.equal.toLowerCase() === 'true'); // Gets the indicator if to allow equal dates.

    // Checks if the lower date has a value:
    if (lowerDateVal) {
        // Creates a date object from the lower date string:
        var lowerDate = new Date(lowerDateVal);

        if ((thisDate.valueOf() < lowerDate.valueOf()) || (thisDate.valueOf() == lowerDate.valueOf() && allowEqual === false)) {
            return false;
        }
    }
    return true;
});
$.validator.unobtrusive.adapters.add('dtgt', ['lower', 'equal'], function (options) {
    // Adds validation rule for HTML elements that contain data-val-dgt- attribute:
    options.rules['dtgt'] = {
        lower: options.params.lower,
        equal: options.params.equal
    };
    options.messages['dtgt'] = options.message;
});

// [RequiredIfChecked] = Validates that the property has a value in it only if the specified property is checked (or not checked).
$.validator.addMethod('reqifchk', function (value, element, params) {
    let $prop = $('#' + params.prop); // Gets the check property.

    // Checks if the check property equals the check state requested (in order to apply the required logic on this property):
    if (($prop.is(":checked") && params.chk === 'true') || (!($prop.is(":checked")) && params.chk === 'false')) {
        // Enables the required validation - checks this property has a value:
        if (!value) {
            return false;
        }
    }
    return true;
});
$.validator.unobtrusive.adapters.add('reqifchk', ['prop', 'chk'], function (options) {
    // Adds validation rule for HTML elements that contain data-val-reqifchk- attribute:
    options.rules['reqifchk'] = {
        prop: options.params.prop,
        chk: options.params.chk
    };
    options.messages['reqifchk'] = options.message;
});

// [RequiredIf] = Validates that the property has a value in it only if the specified property has the specified value.
$.validator.addMethod('reqif', function (value, element, params) {
    let $prop = $('#' + params.prop); // Gets the other property.
    let requiredOnValue = params.val;

    // Checks if the other property value equals the requiredOnValue requested (in order to apply the required logic on this property):
    if ($prop.val() === requiredOnValue) {
        // Enables the required validation - checks this property has a value:
        if (!value) {
            return false;
        }
    }
    return true;
});
$.validator.unobtrusive.adapters.add('reqif', ['prop', 'val'], function (options) {
    // Adds validation rule for HTML elements that contain data-val-reqif- attribute:
    options.rules['reqif'] = {
        prop: options.params.prop,
        val: options.params.val
    };
    options.messages['reqif'] = options.message;
});

// [FullName] = Validates that the property value has a full name in it.
$.validator.addMethod('fullname', function (value, element, params) {
    // Checks the string contains a space between the words (after trimmed):
    if (value.trim().indexOf(" ") > -1) {
        return true;
    } else {
        return false;
    }
});
$.validator.unobtrusive.adapters.add('fullname', [], function (options) {
    // Adds validation rule for HTML elements that contain data-val-fullname- attribute:
    options.rules['fullname'] = {};
    options.messages['fullname'] = options.message;
});
