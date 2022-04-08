var GoodImagesController = function (goodImageService) {
    var deleteButton;

    var init = function(container) {
        $(container).on("click", ".js-delete-goodImage", onDeleteGoodImageButtonClickListener);
        $(container).on("click", ".js-delete-plantImage", onDeletePlantImageButtonClickListener);
    };

    var onDeleteGoodImageButtonClickListener = function (e) {
        bootstrapConfirmDelete("Внимание", "Вы действительно хотите удалить данное изображение?",
            function () {
                deleteButton = $(e.target);

                var goodImageId = deleteButton.attr("data-goodImageId");
                goodImageService.deleteGoodImage(goodImageId, deletionSuccessHandler, deletionFailureHandler);
            });
    };
    
    var onDeletePlantImageButtonClickListener = function (e) {
        bootstrapConfirmDelete("Внимание", "Вы действительно хотите удалить данное изображение?",
            function() {
                deleteButton = $(e.target);

                var goodImageId = deleteButton.attr("data-goodImageId");
                goodImageService.deletePlantImage(goodImageId, deletionSuccessHandler, deletionFailureHandler);
            });
    };

    var deletionSuccessHandler = function() {
        deleteButton.parent().closest("tr").fadeOut("slow",
            function() {
                $(this).remove();
            });
        toastr.success('Операция прошла успешно.');
    };

    var deletionFailureHandler = function() {
        toastr.error('Упссс! Произошла ошибка.');
    };

    return {
        init: init
    };
}(GoodImageService);