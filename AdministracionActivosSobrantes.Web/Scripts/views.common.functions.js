function writeAlert(control, msg, type) {
    if (type === 'error') {
        type = 'danger';
    }
    var errMsg = '<div style="text-align: center" class="alert alert-dismissible alert-' + type + '"><a class="close" data-dismiss="alert">×</a><h4 class="alert-heading">' + msg + '</h4></div>';
    $('#' + control).html(errMsg);
}

function clearErrors() {
    $('.alert').hide();
}

function IsNumeric(data) {
    return parseFloat(data) == data;
}
