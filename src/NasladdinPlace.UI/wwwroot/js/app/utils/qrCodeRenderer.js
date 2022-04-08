const QrCodeRenderer = function() {
    const render = function(qrCodeElementId, qrCode, sizePercentOfParent) {
        if (qrCodeElementId === undefined) {
            console.log("Cannot render qr code because qr code element id is undefined.");
            return;
        }
        
        if (qrCode === undefined || qrCode === "") {
            console.log("Cannot render qr code because qr code is empty or undefined.");
            return;
        }
        
        if (sizePercentOfParent === undefined) {
            sizePercentOfParent = 1;
        }
        
        const qrCodeNode = document.getElementById(qrCodeElementId);
        
        if (qrCodeNode === undefined) {
            console.log(`Cannot render qr code because cannot find element with id equals to ${qrCodeElementId}.`)
            return;
        }

        const qrCodeWidth = qrCodeNode.offsetWidth * sizePercentOfParent;

        new QRCode(qrCodeNode, {
            text: qrCode,
            width: qrCodeWidth,
            height: qrCodeWidth
        });
    };
    
    return {
        render: render
    }
}();