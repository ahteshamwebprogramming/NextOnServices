function isValidateForm(formId) {
    $('.requiredInput').on('click change paste keyup', function () {
        var element = $(this);

        if (element.is('select')) {
            //var select2Container = element.next('.select2-container');
            //if (select2Container.length > 0) {
            //    select2Container.find('.select2-selection').removeClass('parsley-error');
            //}
            element.removeClass('error');
            // Remove the <p> tag if it exists
            element.closest('.form-group').find('.error-mandatory').remove();
        } else {
            // Handle regular input
            element.removeClass('error');
            // Remove the <p> tag if it exists
            element.closest('.form-group').find('.error-mandatory').remove();
        }
    });

    var isValid = true;

    // Select all inputs and selects with both dbCol and required attributes
    let formElements = $("#" + formId).find(".requiredInput");
    formElements.each(function () {
        var element = $(this);
        var value = element.val();

        // Check if the value is invalid
        if (value === "" || value === "0" || value === "Choose option" || value.toUpperCase() === "SELECT" || value.toUpperCase() === "--SELECT--") {
            isValid = false;

            // Remove any existing error classes and error message <p> tags
            element.removeClass('error');
            element.closest('.form-group').find('.error-mandatory').remove();

            // Add the error class to the input
            element.addClass('error');

            // Add the <p> tag with the error message
            element.closest('.form-group').append('<p class="error-mandatory">This field is mandatory</p>');
        } else {
            if (element.is('select')) {
                //// Handle Select2
                //var select2Container = element.next('.select2-container');
                //if (select2Container.length > 0) {
                //    // Remove parsley-error class from the appropriate span within Select2
                //    select2Container.find('.select2-selection').removeClass('parsley-error');
                //}
                // Handle regular input
                element.removeClass('error');
                // Remove the <p> tag if it exists
                element.closest('.form-group').find('.error-mandatory').remove();
            } else {
                // Handle regular input
                element.removeClass('error');
                // Remove the <p> tag if it exists
                element.closest('.form-group').find('.error-mandatory').remove();
            }
        }
    });

    return isValid;
}
