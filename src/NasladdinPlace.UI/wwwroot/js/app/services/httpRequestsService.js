const HttpRequestsService = function() {
    
    const performPostRequest = function(url, body, onSuccess, onFailure)  {
        $.ajax({
            url: url,
            contentType: 'application/json',
            type: 'POST',
            data: JSON.stringify(body)
        })
            .done(onSuccess)
            .fail(onFailure);
    };
    
    const performPutRequest = function(url, body, onSuccess, onFailure)  {
        $.ajax({
            url: url, 
            contentType: 'application/json',
            type: 'PUT',
            data: JSON.stringify(body)
        })
            .done(onSuccess)
            .fail(onFailure);
    };
    
    const performPatchRequest = function (url, body, onSuccess, onFailure) {
        $.ajax({
            url: url,
            contentType: 'application/json',
            type: 'PATCH',
            data: JSON.stringify(body)
        })
            .done(onSuccess)
            .fail(onFailure);
    };
    
    const performDeleteRequest = function (url, onSuccess, onFailure) {
        $.ajax(url,
            {
                type: 'DELETE'
            })
            .done(onSuccess)
            .fail(onFailure);
    };

    const performGetRequest = function (url, onSuccess, onFailure) {
        $.ajax(url,
            {
                type: 'GET'
            })
            .done(onSuccess)
            .fail(onFailure);
    };
    
    return {
        performPutRequest: performPutRequest,
        performPatchRequest: performPatchRequest,
        performDeleteRequest: performDeleteRequest,
        performPostRequest: performPostRequest,
        performGetRequest: performGetRequest,
    }
}();