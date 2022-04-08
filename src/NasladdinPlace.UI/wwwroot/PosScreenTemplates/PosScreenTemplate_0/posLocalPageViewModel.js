function PosLocalPageViewModel() {
    const self = this;

    self.resizeChangeTrigger = ko.observable(false);
    self.qrCodeSize = ko.observable();
    self.qrCodeUrl = ko.observable();
    self.isNeedToShowLegendText = ko.observable(false);
    self.isTryingToConnect = ko.observable(true);

    var ping = new PingViewModel();
    
    self.isSocketConnectionClosed = ko.observable(true);
    
    self.state = ko.observable(PosDisplayContentType.QR_CODE);
    
    self.checkCaculationTimerInfo = new CountDownTimerViewModel(
        function(){
            self.changeState(PosDisplayContentType.QR_CODE);
        }, 
        ConnectionConstants.INVENTORY_TIMER_DURATION_IN_SECONDS);

    self.autoClosingDoorsTimerInfo = new CountDownTimerViewModel(
        function(){
            self.changeState(PosDisplayContentType.INVENTORY);
        }, 
        ConnectionConstants.DOOR_CLOSE_TIMER_DURATION_IN_SECONDS);
    
    self.handleTimerUpdate = function() {
        self.autoClosingDoorsTimerInfo.decrementRemainingTime();
        self.checkCaculationTimerInfo.decrementRemainingTime();
    };

    self.changeState = function (state,  isHardSwitchState = false) {    
        self.resetTimerInfos();
        if ((self.isSocketConnectionClosed() && !isHardSwitchState) || state == PosDisplayContentType.DISCONNECT) 
        {
            tryConnectWithTimeout();
        } 
        else if (state == PosDisplayContentType.INVENTORY) 
        {
            return self.state(PosDisplayContentType.INVENTORY);
        } 
        else if (state == PosDisplayContentType.REFRESH) {
            window.location.reload(true);
        } 
        else if (state == PosDisplayContentType.ACTIVE_PURCHASE) {
            self.autoClosingDoorsTimerInfo.enable();
            return self.state(PosDisplayContentType.ACTIVE_PURCHASE);
        } 
        else if (state == PosDisplayContentType.QR_CODE) {
            if(isHardSwitchState)
                return self.state(PosDisplayContentType.QR_CODE);
            tryConnectWithTimeout();
        }
        else if (state == PosDisplayContentType.PURCHASES_RESTRICTED) {
            self.isNeedToShowLegendText(false);
            return self.state(PosDisplayContentType.PURCHASES_RESTRICTED);
        }
    }; 

    function tryConnectWithTimeout(){
        if(self.isSocketConnectionClosed())
            self.isTryingToConnect(true);

        ping.pingServers();
        setTimeout(() => { 
            self.isTryingToConnect(false);
            if(!ping.areAllServersResponded() && !ping.isNasladdinAvailable()){
                self.isNeedToShowLegendText(true);
                return self.state(PosDisplayContentType.DISCONNECT);
            }
            else{
                self.isNeedToShowLegendText(false);
                return self.state(PosDisplayContentType.QR_CODE);
            }
        }, 15000);
    }

    self.resetTimerInfos = function () {
        self.autoClosingDoorsTimerInfo.reset();
        self.checkCaculationTimerInfo.reset();
    };

    self.isSocketConnectionClosed.subscribe(function() {
        self.changeState(self.state());
    });

    self.isSocketConnectionClosed(false);

    let windowWidth = 0;
    let resizeTimeoutId;
    let $window = $(window);
    $window.resize(function () {
        self.calculateQrCodeSize();
    });

    self.calculateQrCodeSize = function () {
        if (resizeTimeoutId) {
            window.clearTimeout(resizeTimeoutId);
        }
        const newWidth = $window.width();
        if (windowWidth != newWidth) {
            resizeTimeoutId = window.setTimeout(function () {
                self.resizeChangeTrigger(!self.resizeChangeTrigger());
                windowWidth = newWidth;
                self.setupQrCodeSize(windowWidth);
            }, 200);
        }
    };

    self.setupQrCodeSize = function (innerWindowWidth) {
        if (innerWindowWidth >= 1600) {
            self.qrCodeSize(350);
        } else if (innerWindowWidth >= 1400) {
            self.qrCodeSize(300);
        } else if (innerWindowWidth >= 1100) {
            self.qrCodeSize(250);
        } else if(innerWidth <= 1000){
            self.qrCodeSize(180);
        } else {
            self.qrCodeSize(200);
        }
    };

    self.makeQrCode = function (element) {
        $(element).empty();
        let qrCode = self.qrCodeUrl() == null ? 'https://nasladdin.ru/contacts/' : self.qrCodeUrl();

        QrCodeRenderer.render(element, qrCode, self.qrCodeSize());
    };

    ko.bindingHandlers.qrCode = {
        update: function (element) {
            self.calculateQrCodeSize();
            self.makeQrCode(element);
        }
    };
}