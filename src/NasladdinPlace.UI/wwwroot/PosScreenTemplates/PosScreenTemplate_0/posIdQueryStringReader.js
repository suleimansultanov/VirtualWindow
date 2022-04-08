const PosIdQueryStringReader = function () {
    const read = function() {
        if (document.location.href == null)
            return 0;

        const paramsString = document.location.href.split("?")[1];

        if (paramsString == null)
            return 0;

        const paramValues = paramsString.split("&");

        let posId = undefined;
        
        Object.keys(paramValues).forEach(function(key) {
            const paramValue = paramValues[key].split("=");
            if (paramValue[0] == 'posId') {
                posId = paramValue[1];
            }
        });

        return posId;
    };

    return {
        read: read
    };
}();