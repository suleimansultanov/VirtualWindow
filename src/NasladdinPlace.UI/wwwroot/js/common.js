$(document)
    .ready(function() {
        initFilterEvents();
    });

var ServiceProxy = function (options) {
    this.options = options || {};
};

ServiceProxy.prototype =
{
    _defaultErrorHandler: function (jqXhr, exception, innerHideMask) {
        innerHideMask();
        if (jqXhr.responseJSON && jqXhr.responseJSON.unauthorized) {
            bootstrapWarning("Внимание", "Вы не авторизованы, необходимо войти в систему");
        } else if (jqXhr.responseJSON && jqXhr.responseJSON.error) {
            bootstrapError(jqXhr.responseJSON.error);
        } else {
            bootstrapError(jsonErrorMsgHandler(jqXhr, exception));
        }
    },

    _post: function (url, data, fnSuccess, fnError, innerShowMask, innerHideMask, type) {
        var self = this;
        if (!data) data = {};

        if (!fnError) fnError = function (jqXhr, exception) {
            self._defaultErrorHandler(jqXhr, exception, hideMask);
        }
        innerShowMask();

        data = typeof data === "string" ? data : JSON.stringify(data);

        jQuery.ajax({
            type: type,
            url: url,
            data: data,
            contentType: "application/json",
            success: function (response, textStatus, jqXhr) {
                innerHideMask();
                if (response.error) {
                    bootstrapError(response.error);
                } else {
                    fnSuccess(response, textStatus, jqXhr);
                }
            },
            error: fnError
        });
    },
    _postMultipart: function (url, data, fnSuccess, fnError, innerShowMask, innerHideMask, type) {
        var self = this;

        if (!fnError) fnError = function (jqXhr, exception) {
            self._defaultErrorHandler(jqXhr, exception, hideMask);
        }
        innerShowMask();

        jQuery.ajax({
            cache: false,
            type: type,
            data: data,
            processData: false,
            contentType: false,
            url: url,
            success: function (response, textStatus, jqXhr) {
                innerHideMask();
                if (response.error) {
                    bootstrapError(response.error);
                } else {
                    fnSuccess(response, textStatus, jqXhr);
                }
            },
            error: fnError
        });
    },
    postJsonResult: function (url, data, fnSuccess, fnError) {
        this._post(url, data, function (response, textStatus, jqXhr) {
            fnSuccess(response, textStatus, jqXhr);
        }, fnError, showMask, hideMask, "POST");
    },
    postJsonResultCustomMask: function (innerShowMask, innerHideMask, url, data, fnSuccess, fnError) {
        this._post(url, data, function (response, textStatus, jqXhr) {
            fnSuccess(response, textStatus, jqXhr);
        }, fnError, innerShowMask, innerHideMask, "POST");
    },
    deleteJsonResult: function(url, data, fnSuccess, fnError) {
        this._post(url, data, function (response, textStatus, jqXhr) {
            fnSuccess(response, textStatus, jqXhr);
        }, fnError, showMask, hideMask, "DELETE");
    },
    putJsonResult: function (url, data, fnSuccess, fnError) {
        this._post(url, data, function (response, textStatus, jqXhr) {
            fnSuccess(response, textStatus, jqXhr);
        }, fnError, showMask, hideMask, "PUT");
    },
    postMultipartResult: function (url, data, fnSuccess, fnError) {
        this._postMultipart(url, data, function (response, textStatus, jqXhr) {
            fnSuccess(response, textStatus, jqXhr);
        }, fnError, showMask, hideMask, "POST");
    },
    putMultipartResult: function (url, data, fnSuccess, fnError) {
        this._postMultipart(url, data, function (response, textStatus, jqXhr) {
            fnSuccess(response, textStatus, jqXhr);
        }, fnError, showMask, hideMask, "PUT");
    },
    getJsonResult: function (url, data, fnSuccess, fnError) {
        this._post(url, data, function (response, textStatus, jqXhr) {
            fnSuccess(response, textStatus, jqXhr);
        }, fnError, showMask, hideMask, "GET");
    },
    getJsonResultCustomMask: function (innerShowMask, innerHideMask, url, data, fnSuccess, fnError) {
        this._post(url, data, function (response, textStatus, jqXhr) {
            fnSuccess(response, textStatus, jqXhr);
        }, fnError, innerShowMask, innerHideMask, "GET");
    },
    errorHandler: function (jqXhr, exception) {
        ServiceProxy.prototype._defaultErrorHandler(jqXhr, exception);
    }
};

function jsonErrorMsgHandler(jqXhr, exception) {

    var setStrong = txt => "<strong>" + txt + "</strong>";

    var msg = `[${jqXhr.status}] `;

    switch (jqXhr.status) {
    case 0:
        msg += "No connect. Verify Network.\n\n";
        break;
    case 403:
        msg = "Отказано в доступе. \n\n";
        break;
    case 404:
        msg += "Requested page not found.\n\n";
        break;
    case 500:
        msg += "Internal Server Error.\n\n";
        break;
    default:
        break;
    }

    msg = setStrong(msg);
    switch (exception) {
    case "parsererror":
        msg += "Requested JSON parse failed.";
        break;
    case "timeout":
        msg += "Time out error.";
        break;
    case "abort":
        msg += "Ajax request aborted.";
        break;
    case jqXhr.status === 403 && "error":
        msg += "У вас недостаточно прав для этой операции.";
        break;
    default:
        msg += `Uncaught Error:\n${getErrorMessageByResponse(jqXhr)}`;
        break;
    }

    return msg;
}

function getErrorMessageByResponse(response) {
    const json = response.responseJSON;
    return json && json.error ? json.error : (response.responseText || response);
}

function bootstrapError(message) {
    var title = "Ошибка";
    if (message === undefined) {
        message = "Причина ошибки не ясна";
    }

    toastr.error(message, title);
}

function bootstrapErrorWithTitle(title, message) {
    if (message === undefined) {
        message = title;
        title = "Ошибка";
    }

    toastr.error(message, title);
}

function bootstrapWarning(title, message) {
    toastr.warning(message, title);
}

function bootstrapAlert(title, message) {
    toastr.info(message, title);
}

function bootstrapSuccess(title, message) {
    toastr.success(message, title);
}

function bootstrapConfirmSave(title, text, callback) {
    bootstrapConfirm(title, text, callback, 'Сохранить', 'Отмена', "#1ab394");
}

function bootstrapConfirmDelete(title, text, callback) {
    bootstrapConfirm(title, text, callback, 'Удалить', 'Отмена', "#dd6b55");
}

function bootstrapConfirm(title, text, callback, confirmButtonText, cancelButtonText, confirmButtonColor) {
swal({
        title: title,
        text: text,
        showCancelButton: true,
        confirmButtonColor: confirmButtonColor,
        confirmButtonText: confirmButtonText,
        cancelButtonText: cancelButtonText,
        closeOnConfirm: true
    },
    function () {
        callback();
    });
}


function showMask() {
    $('#reference-list').addClass('sk-loading');    
}

function hideMask() {
    setTimeout(function () {
        $('#reference-list').removeClass('sk-loading');
    }, 300);
    
}

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function initFilterEvents() {

    $("body")
        .on("click",
            ".filter-toggle",
            function () {                
                $(this).next().toggle("show");
            });

    $(document).mouseup(function (e) {

        let container = $(".filter-container:visible");

        if (!container.is(e.target) // if the target of the click isn't the container...
            && container.has(e.target).length === 0) // ... nor a descendant of the container
        {
            const toggle = $(e.target);
            if (toggle.hasClass("filter-toggle")) container = container.not(toggle.next());
            container.hide("slow");
        }
    });
}