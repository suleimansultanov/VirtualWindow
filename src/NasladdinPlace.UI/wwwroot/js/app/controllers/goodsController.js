var GoodsController = function (goodService) {
    var deleteButton;

    var onGoodDeletionSuccess = function () {
        deleteButton.parent().closest("tr").fadeOut("slow",
            function () {
                $(this).remove();
            });
        toastr.success('Операция прошла успешно.');
    };

    var onGoodDeletionFailure = function () {
        toastr.error('Упссс! Произошла ошибка.');
    };

    var onDeleteGoodClickListener = function (e) {
        bootstrapConfirmDelete("Внимание", "Вы действительно хотите удалить этот товар?",
            function () {
                deleteButton = $(e.target);
                var goodId = deleteButton.attr('data-goodId');
                goodService.deleteGood(goodId, onGoodDeletionSuccess, onGoodDeletionFailure);
            });
    };

    var init = function (container) {
        $(container).on('click', '.js-delete-good', onDeleteGoodClickListener);
    };

    return {
        init: init
    };
}(GoodService);