
$.validator.addMethod("double", function (value, element, parameters) {
    return /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
});

$.validator.unobtrusive.adapters.add("double", [], function (options) {
    options.rules.double = {};
    options.messages["double"] = options.message;
});

$.validator.addMethod("futuredate", function (value, element, parameters) {
    return moment(new Date(value.replace(/(\d{2})\/(\d{2})\/(\d{4})/, "$2/$1/$3")))
        .diff(moment(new Date()), 'hours') > 0;
});

$.validator.unobtrusive.adapters.add("futuredate", [], function(options) {
    options.rules.futuredate = {};
    options.messages["futuredate"] = options.message;
});

$.validator.addMethod("pastdate", function(value, element, parameters) {
    return moment(new Date(value.replace(/(\d{2})\/(\d{2})\/(\d{4})/, "$2/$1/$3")))
        .diff(moment(new Date()), 'hours') < 0;
});

$.validator.unobtrusive.adapters.add("pastdate", [], function(options) {
    options.rules.pastdate = {};
    options.messages["pastdate"] = options.message;
});

$.validator.methods.length = function(value, element, param) {
    return this.optional(element) || value.length < param[0];
}

$.validator.methods.range = function (value, element, param) {
    var globalizedValue = value.replace(",", ".");
    return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
}

$.validator.methods.number = function(value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
}

