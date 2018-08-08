var modalConfirm = (function () {
    var _callback;
    var _confirnBtnClass = "btn-primary";

    $("#modal-btn-yes").on("click", function () {
        if (_callback) {
            _callback(true);
        }

        $("#mi-modal").modal('hide');
    });

    return {
        show: function (message,buttonClass, callback) {
            $("#modal-text").text(message);

            $("#modal-btn-yes").removeClass(_confirnBtnClass);

            if (buttonClass) {
                _confirnBtnClass = buttonClass;
            } else {
                _confirnBtnClass = "btn-primary";
            }

            $("#modal-btn-yes").addClass(_confirnBtnClass);

            if (typeof callback === "function") {
                _callback = callback;
            }

            $('#confirmModal').modal({ keyboard: false });
        }
    }
}());

