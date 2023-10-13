$(document).ready(function () {
    $("#datepicker1, #datepicker2").datepicker({
        dateFormat: "mm/dd/yy",
        maxDate: '0',
        changeMonth: true,
        changeYear: true,
        yearRange: "-100:+0"
    });

});

function allowOnlyAlphabets(event) {
    var charCode = event.which || event.keyCode;

    // Check if the pressed key is an alphabetic character
    if ((charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122)) {
        // Prevent the key press if it's not an alphabet
        event.preventDefault();
        return false;
    }

    return true;
}

function allowOnlyNumbers(event) {
    var charCode = event.which || event.keyCode;

    // Check if the pressed key is a numeric digit (0-9)
    if (charCode < 48 || charCode > 57) {
        // Prevent the key press if it's not a numeric digit
        event.preventDefault();
        return false;
    }

    return true;
}
