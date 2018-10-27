$(function () {
    let button_submit = $('#login button');
    let error_user = $('#username-error');
    let error_password = $('#password-error');
    let input_user = $('#username');
    let input_password = $('#password');
    function check_user() {
        let length = input_user.val().length;
        if (length < 4 || length > 30) {
            button_submit.prop('disabled', true);
            error_user.show();
        } else {
            button_submit.prop('disabled', false);
            error_user.hide();
        }
    }

    function check_password() {
        let length = input_password.val().length;
        if (length < 8) {
            button_submit.prop('disabled', true);
            error_password.show();
        } else {
            button_submit.prop('disabled', false);
            error_password.hide();
        }
    }

    input_user.keyup(check_user);
    input_password.keyup(check_password);
    check_user();
    check_password();
});