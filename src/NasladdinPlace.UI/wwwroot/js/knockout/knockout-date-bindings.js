/*
 * knockout-date-bindings
 * Copyright 2014 Muhammad Safraz Razik
 * All Rights Reserved.
 * Use, reproduction, distribution, and modification of this code is subject to the terms and
 * conditions of the MIT license, available at http://www.opensource.org/licenses/mit-license.php
 *
 * Author: Muhammad Safraz Razik
 * Project: https://github.com/adrotec/knockout-date-bindings
 */
(function(factory) {
    // Module systems magic dance.

    if (typeof require === "function" && typeof exports === "object" && typeof module === "object") {
        // CommonJS or Node: hard-coded dependency on "knockout"
        factory(require("knockout"), require("moment"));
    } else if (typeof define === "function" && define["amd"]) {
        // AMD anonymous module with hard-coded dependency on "knockout"
        define(["knockout", "moment"], factory);
    } else {
        // <script> tag: use the global `ko` object, attaching a `mapping` property
        factory(ko, moment);
    }
}(function(ko, moment) {

    var DefaultDateFormat = "DD.MM.YYYY"; // "L"

    function getDateFormat(type, defaultDateFormat) {
        switch (type) {
            case "text":
            case "":
                return defaultDateFormat;
            case "date":
                return "DD.MM.YYYY";
            case "datetime":
            case "datetime-local":
                return "DD.MM.YYYY HH:mm";
            case "month":
                return "YYYY-MM";
            case "time":
                return "hh:mm";
            case "week":
                return  "GGGG-[W]WW";
            default :
                return defaultDateFormat;
        }
    }

    ko.bindingHandlers.date = {
        init: function(element, valueAccessor, allBindings) {
            if (element.tagName == "INPUT") {
                var dateFormat = allBindings.get("dateFormat") || DefaultDateFormat;

                var datePickerSettings = {
                    locale: 'ru',
                    format: getDateFormat(element.type, dateFormat),
                    useCurrent: false
                };

                $(element).datetimepicker(datePickerSettings);
            }

            ko.utils.registerEventHandler(element,
                "dp.change",
                function() {
                    var value = valueAccessor();
                    var dateFormat = allBindings.get("dateFormat") || DefaultDateFormat;
                    
                    var d; // object Date
                    if (element.tagName == "INPUT" && element.value) {
                        const type = element.type;
                        dateFormat = getDateFormat(type, dateFormat);
                        d = moment(element.value, dateFormat);
                        if (type == "date" || type == "month" || type == "week") {
                            const newD = moment();
                            d.hour(newD.hour());
                            d.minute(newD.minute());
                            d.second(newD.second());
                            if (type == "month" || type == "week") {
                                d.date(newD.date());
                            }
                        }
                    } else if (element.textContent) {
                        d = moment(element.textContent, dateFormat);
                    }

                    if (d) {
                        if (typeof value === "function") {
                            value(d.format("YYYY-MM-DDTHH:mm:ss"));
                        } else if (value instanceof Date) {
                            value.setTime(d.toDate().getTime());
                        } else {
                            value = d.toDate();
                        }
                    } else {
                        if (typeof value === "function") {
                            value(null);
                        } else if (value instanceof Date) {
                            value.setTime(0);
                        } else {
                            value = null;
                        }
                    }
                });
        },
        update: function(element, valueAccessor, allBindings, viewModel) {
            
            const isInput = element.tagName == "INPUT",
                value = valueAccessor();

            var valueUnwrapped = ko.utils.unwrapObservable(value);
            if (valueUnwrapped == null) {
                if (isInput) element.value = "";
                else element.textContent = "";
                return;
            }

            function updateTimeValue() {
                //element.value = moment(valueUnwrapped).format('L');
                var dateFormat = allBindings.get("dateFormat") || DefaultDateFormat;
                if (isInput) {
                    if (valueUnwrapped instanceof Date && valueUnwrapped.getTime() === 0) {
                        element.value = "";
                    } else {
                        dateFormat = getDateFormat(element.type, dateFormat);
                        element.value = moment(valueUnwrapped).format(dateFormat);
                    }
                } else {
                    element.textContent = moment(valueUnwrapped).format(dateFormat);
                }
            }

            var setTimeOld = valueUnwrapped.setTime;
            valueUnwrapped.setTime = function(time) {
                setTimeOld.apply(valueUnwrapped, arguments);
                updateTimeValue();
            };
            updateTimeValue();
        }
    };

    return {}; // let require js work fine
}));