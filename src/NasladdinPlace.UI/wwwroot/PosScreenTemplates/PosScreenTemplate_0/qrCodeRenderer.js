const QrCodeRenderer = function() {
    const render = function(element, qrCode, qrCodeSize) {
        if (element === undefined) {
            return;
        }
        
        if (qrCode === undefined || qrCode === "") {
            return;
        }   

        new QRCode(element, {
            text: qrCode,
            width: qrCodeSize,
            height: qrCodeSize,
            colorDark: "#333333",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.H
        });
    };
    
    return {
        render: render
    };
}();