const FiscalizationQrCodeRenderer = function(qrCodeRenderer) {
    const render = function(elementId, fiscalizationInfo) {
        if (!fiscalizationInfo || !fiscalizationInfo.qrCodeValue) {
            return;
        }

        qrCodeRenderer.render(elementId, fiscalizationInfo.qrCodeValue);
    };
    
    return {
        render: render
    }
}(QrCodeRenderer);