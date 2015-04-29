
var OnSaveExpenseFormSuccess = function () {
    alert("Data Saved");
}

var ConfirmDelete = function () {
    var result = confirm("Are you sure you want to delete the item");
    return result;
}

$(function () {
    $(".bt-datetimepicker").datetimepicker();
    $(".bt-datepicker").datetimepicker({
        format: 'DD/MM/YYYY'
    });
}());