/*==============================================
    custom-validations.js
    ---------------------
    Description: Custom validations for the site.
    Version: 1.0.0
    Last Update: 2021-06-01
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
