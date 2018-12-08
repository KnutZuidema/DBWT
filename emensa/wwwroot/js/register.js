$(function () {
    let role = $("#role");
    let employee = $("#employee-data");
    let student = $("#student-data");
    let guest = $("#guest-data");
    function select() {
        switch (role.val()) {
            case "guest":
                guest.show();
                employee.hide();
                student.hide();
                break;
            case "employee":
                guest.hide();
                employee.show();
                student.hide();
                break;
            case "student":
                guest.hide();
                employee.hide();
                student.show();
                break;
        }
    }
    role.change(select);
    select();
});