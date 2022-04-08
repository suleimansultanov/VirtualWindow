var LabeledGoodsController = function (labeledGoodService) {
    var putButton;
    var deleteButton;

    var init = function(container) {
        $(container).on('click', '.js-putLabeledGood', onPutButtonClickListener);
        $(container).on('click', '.js-takeLabeledGood', onTakeButtonClickListener);
        $(container).on('click', '.js-deleteLabeledGood', onDeleteLabeledGoodButtonClickListener);
    };

    var onPutButtonClickListener = function(e) {
        putButton = $(e.target);

        var plantId = putButton.attr("data-plantId");
        var label = putButton.attr("data-label");

        labeledGoodService.putLabeledGood(plantId, label, onLabeledGoodActionSuccess, onLabeledGoodActionFailure);
    };

    var onTakeButtonClickListener = function(e) {
        putButton = $(e.target);

        var plantId = putButton.attr("data-plantId");
        var label = putButton.attr("data-label");

        labeledGoodService.takeLabeledGood(plantId, label, onLabeledGoodActionSuccess, onLabeledGoodActionFailure);
    };

    var onLabeledGoodActionSuccess = function() {
        fadeOutWithRemoveButton(putButton);
    };

    var onLabeledGoodActionFailure = function() {
        bootstrapAlert("Labeled Good", "Something failed!");
    };

    var onDeleteLabeledGoodButtonClickListener = function(e) {
        deleteButton = $(e.target);

        var labeledGoodId = deleteButton.attr("data-labeledGoodId");

        labeledGoodService.deleteLabeledGood(labeledGoodId,
            deleteLabeledGoodSuccessHandler,
            onLabeledGoodActionFailure);
    };

    var deleteLabeledGoodSuccessHandler = function() {
        fadeOutWithRemoveButton(deleteButton);
    };

    var fadeOutWithRemoveButton = function(button) {
        button.parent().closest("tr").fadeOut('slow', function () {
            $(this).remove();
        });
    }

    return {
        init: init
    };
}(LabeledGoodService);