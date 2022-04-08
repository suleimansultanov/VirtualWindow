jQuery(document).ready(function () {
    SliderRenderer.render($('.single-slide'));

    const webSocketUrl = ConnectionSettings.getWebSocketUrl();
    const posLocalPageViewModel = new PosLocalPageViewModel();
    
    const timerManager = new LocalTimerManager(function() {
        posLocalPageViewModel.handleTimerUpdate();
    });

    WebSocketClient.createWebSocketClient(
        webSocketUrl,
        function (obj) {
            if (obj.hasOwnProperty('contentType')) {
                posLocalPageViewModel.changeState(obj.contentType);
            } else if (obj.hasOwnProperty('qrCode')) {
                posLocalPageViewModel.qrCodeUrl(obj.qrCode);
            } else if (obj.hasOwnProperty('isSocketConnectionClosed')) {
                posLocalPageViewModel.isSocketConnectionClosed(obj.isSocketConnectionClosed);
            }
        });

    ko.applyBindings(posLocalPageViewModel);

    timerManager.startTimer();
});