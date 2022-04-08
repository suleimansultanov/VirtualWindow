const WebSocketClient = function(posIdQueryStringReader) {
    let posId,
        webSocketClient,
        keepAliveTimer,
        callback,
        isWebSocketReconnecting = false;

    const createWebSocketClient = function(webSocketUrl, _callback) {
        posId = posIdQueryStringReader.read();
        callback = _callback;

        webSocketClient = new WebSocket(webSocketUrl);

        webSocketClient.onopen = function(event) { onOpen(event) };
        webSocketClient.onclose = function(event) { onClose(event) };
        webSocketClient.onmessage = function(event) { onMessage(event) };
        webSocketClient.onerror = function(event) { onError(event) };
    };

    function onOpen(event) {
        sendPosDisplayAliveRequest();
        sendPosBatteryAliveRequest();
        
        keepAliveTimer = setInterval(sendPosDisplayAliveRequest, ConnectionConstants.KEEP_ALIVE_UPDATE_INTERVAL_IN_MILLIS)
        callback({ isSocketConnectionClosed: false });

        keepAliveTimer = setInterval(sendPosBatteryAliveRequest, ConnectionConstants.KEEP_ALIVE_UPDATE_INTERVAL_IN_MILLIS)
        callback({ isSocketConnectionClosed: false });
        
        sendPosAliveRequest();
        webSocketClient.send(
            `{"H":"PlantHub","A":{"group":"PlantDisplay_${posId}"},"M":"addToGroup"}`,
        );

        keepAliveTimer = setInterval(sendPosAliveRequest, ConnectionConstants.KEEP_ALIVE_UPDATE_INTERVAL_IN_MILLIS)
        callback({ isSocketConnectionClosed: false });
    }

    // obsolete, will be removed in future relize
    function sendPosDisplayAliveRequest() {
        const posScreenResolution = { width: window.screen.width, height: window.screen.height };

        webSocketClient.send(
            `{
                "H":"posDisplayActivity",
                "A":{"group":"PlantDisplay_${posId}", "screenResolution": ${JSON.stringify(posScreenResolution)}},
                "M":"notifyPosDisplayActivity"
            }`,
        );
    }

    // obsolete, will be removed in future relize
    function sendPosBatteryAliveRequest() {
        sendPosAliveRequest();
    }

    function sendPosAliveRequest() {
        const posScreenResolution = { width: window.screen.width, height: window.screen.height };
        navigator.getBattery().then(function (battery) {
            const posUserAgent = navigator.userAgent;
            webSocketClient.send(
                `{
                    "H":"posDisplayActivity",
                    "A":{"group":"PlantDisplay_${posId}", "screenResolution": ${JSON.stringify(posScreenResolution)}, "batteryLevel": ${battery.level}, "batteryIsCharging": ${battery.charging}, "userAgent": "${posUserAgent}"},
                    "M":"notifyPosActivity"
                }`,
            );
        });
    }

    function onMessage(event) {
        const message = event.data;
        if (message === '{}') return;

        const parsedMessage = JSON.parse(message);

        sendDeliveryConfirmationRequest(parsedMessage.A);
    }

    function sendDeliveryConfirmationRequest(obj) {
        callback({ contentType: obj.contentType });

        if (obj.hasOwnProperty('content')) {
            const posDisplayContent = obj.content;

            callback({ qrCode: posDisplayContent.qrCode });

            if (posDisplayContent.hasOwnProperty('commandId')) {
                webSocketClient.send(
                    `{"H":"posDisplayActivity","A":{"commandId":"${posDisplayContent.commandId}","posId":"${posId}"},"M":"confirmCommandDelivery"}`
                );
            }
        }
    }

    function onClose(event) {
        callback({ isSocketConnectionClosed: true });
        reconnectWebSocket();
    }

    function onError(event) {
        reconnectWebSocket();
    }

    const reconnectWebSocket = function() {
        if (keepAliveTimer != null) {
            clearInterval(keepAliveTimer);
        }

        if (isWebSocketReconnecting) {
            return;
        }

        isWebSocketReconnecting = true;

        setTimeout(function() {
            isWebSocketReconnecting = false;
            createWebSocketClient(webSocketClient.url, callback);
        }, ConnectionConstants.RECONNECT_INTERVAL_IN_MILLIS);
    };

    window.addEventListener('beforeunload', function() {
        if (webSocketClient == null) return;
        webSocketClient.close();
    });

    return {
        createWebSocketClient: createWebSocketClient,
        reconnectWebSocket: reconnectWebSocket
    };
}(PosIdQueryStringReader);