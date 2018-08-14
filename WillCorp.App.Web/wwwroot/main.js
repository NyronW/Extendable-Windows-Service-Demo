var modalDialog = (function () {

    $("#modal-btn-ok").on("click", function () {
        $("#modal").modal('hide');
    });

    return {
        show: function (message) {
            $("#modal-text").text(message);
            $('#modal').modal({ keyboard: false });
        }
    }
}());

