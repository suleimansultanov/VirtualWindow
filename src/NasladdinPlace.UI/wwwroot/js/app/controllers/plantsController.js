var PlantsController = function (plantService) {
    var goodsPlacingPosOperationMode = { 
        mode: 2
    };
    
    var init = function (container) {
        $(container).on('click', '.js-deletePlant', onDeletePlantButtonListener);
        $(container).on('click', '.js-closeDoors', onCloseDoorsButtonClickListener);
        $(container).on('click', '.js-sendPlantContentRequest', onSendPlantContentRequestButtonClickListener);
        $(container).on('click', '.js-openLeftDoor', onOpenLeftDoorButtonClickListener);
        $(container).on('click', '.js-openRightDoor', onOpenRightDoorButtonClickListener);
    };

    var onPosDeletionSuccess = function () {
        window.location.href = "/";
    };

    var onPosOperationSuccess = function () {
        toastr.success('Операция прошла успешно.');
    };

    var onPosOperationFailure = function () {
        toastr.error('Упссс! Произошла ошибка.');
    };

    var onDeletePlantButtonListener = function (e) {
        swal({
                title: "Внимание!",
                text: "Вы действительно хотите удалить данный объект?",
                showCancelButton: true,
                confirmButtonColor: "#1ab394",
                confirmButtonText: "Подтвердить",
                cancelButtonText: "Отменить",
                closeOnConfirm: true
            },
            function() {
                var posId = $(e.target).attr("data-posId");
                plantService.deletePlant(posId, onPosDeletionSuccess, onPosOperationFailure);
            });
    };

    var onSendPlantContentRequestButtonClickListener = function(e) {
        var posId = $(e.target).attr("data-posId");
        plantService.sendPlantContentRequest(posId, onPosOperationSuccess, onPosOperationFailure);
    };

    var onOpenLeftDoorButtonClickListener = function (e) {
        var posId = $(e.target).attr("data-posId");
        plantService.openLeftDoor(posId, goodsPlacingPosOperationMode, onPosOperationSuccess, onPosOperationFailure);
    };

    var onOpenRightDoorButtonClickListener = function (e) {
        var posId = $(e.target).attr("data-posId");
        plantService.openRightDoor(posId, goodsPlacingPosOperationMode, onPosOperationSuccess, onPosOperationFailure);
    };

    var onCloseDoorsButtonClickListener = function(e) {
        var posId = $(e.target).attr("data-posId");
        plantService.closeDoors(posId, onPosOperationSuccess, onPosOperationFailure);
    };

    return {
        init: init
    };
}(PlantService);