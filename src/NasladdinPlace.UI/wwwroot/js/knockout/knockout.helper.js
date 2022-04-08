
ko.observableArray.fn.filter = function (predicate) {
    return ko.utils.arrayFilter(this(), predicate);
}

ko.observableArray.fn.map = function (mapping) {
    return ko.utils.arrayMap(this(), mapping);
}

ko.observableArray.fn.forEach = function (action) {
    return ko.utils.arrayForEach(this(), action);
}

ko.observableArray.fn.first = function (predicate, predicateOwner) {
    return ko.utils.arrayFirst(this(), predicate, predicateOwner);
}

ko.observableArray.fn.sortBy = function (mapping, isDesc) {
    var more = isDesc ? -1 : 1, less = more * -1;
    return this.sort((left, right) => mapping(left) === mapping(right) ? 0 : (mapping(left) > mapping(right) ? more : less));
}

ko.computed.fn.filter = function (predicate) {
    const array = this();
    if (array.push == undefined) throw new Error("Computed property must return array");
    return ko.utils.arrayFilter(array, predicate);
}

ko.computed.fn.map = function (mapping) {
    const array = this();
    if (array.push == undefined) throw new Error("Computed property must return array");
    return ko.utils.arrayMap(array, mapping);
}

ko.computed.fn.forEach = function (action) {
    const array = this();
    if (array.push == undefined) throw new Error("Computed property must return array");
    return ko.utils.arrayForEach(array, action);
}

ko.computed.fn.first = function (predicate, predicateOwner) {
    const array = this();
    if (array.push == undefined) throw new Error("Computed property must return array");
    return ko.utils.arrayFirst(array, predicate, predicateOwner);
}



//wrapper for a computed observable that can pause its subscriptions
ko.pauseableComputed = function (evaluatorFunction, evaluatorFunctionTarget) {
    var _cachedValue = "";
    var _isPaused = ko.observable(false);

    //the computed observable that we will return
    var result = ko.pureComputed(function () {
        if (!_isPaused()) {
            //call the actual function that was passed in
            return evaluatorFunction.call(evaluatorFunctionTarget);
        }
        return _cachedValue;
    }, evaluatorFunctionTarget);

    //keep track of our current value and set the pause flag to release our actual subscriptions
    result.pause = function () {
        _cachedValue = this();
        _isPaused(true);
    }.bind(result);

    //clear the cached value and allow our computed observable to be re-evaluated
    result.resume = function () {
        _cachedValue = "";
        _isPaused(false);
    }

    return result;
};


ko.observable.fn.silentUpdate = function (value) {
    this.notifySubscribers = function () { };
    this(value);
    this.notifySubscribers = function () {
        ko.subscribable.fn.notifySubscribers.apply(this, arguments);
    };
};

ko.observableArray.fn.silentPush = function (value) {
    this.notifySubscribers = function () { };
    this.push(value);
    this.notifySubscribers = function () {
        ko.subscribable.fn.notifySubscribers.apply(this, arguments);
    };
};


// Связывание для числовых значений (целочисленных и вещественных). 
// Парсит текстовые значения из input'ов в числовые значения и обновляет соответсвующие свойства в модели
ko.bindingHandlers.numeric = {
    init: function (element, valueAccessor, allBindings) {
        if (ko.utils.tagNameLower(element) === "input"
            && (element.type === "checkbox" || element.type === "radio")) return;

        let event = allBindings.get("valueUpdate");
        if (event) {
            event = event.toLowerCase();
            if (event === "afterkeydown")
                event = "keydown";
        }
        else
            event = "change";

        //handle the field changing
        ko.utils.registerEventHandler(element,
            event,
            function() {
                const value = valueAccessor(),
                      strVal = this.value,
                      numVal = strVal ? parseFloat(strVal.replace(",", ".")) : null;
                //console.log("bindingHandlers.numeric: ", numVal);
                value(numVal);
            });
    },
    update: function (element, valueAccessor, allBindings, viewModel) {

        const tagName = ko.utils.tagNameLower(element);

        function updateValue(val) {
            if (tagName === "input" || tagName === "textarea")
                element.value = val;
            else
                element.innerHTML = val;
        }

        const value = valueAccessor();
        let valueUnwrapped = ko.unwrap(value);

        if (valueUnwrapped == null) {
            updateValue("");
            return;
        }

        if (typeof valueUnwrapped !== "function") {
            if (typeof valueUnwrapped === "string") valueUnwrapped = parseFloat(valueUnwrapped.replace(",", "."));
            const decimalPlaces = ko.unwrap(allBindings.get("decimalPlaces")),
                 finalValue = valueUnwrapped == null
                ? null
                : (decimalPlaces == null
                    ? valueUnwrapped
                    : valueUnwrapped.toFixed(parseInt(decimalPlaces))).toString().replace(".", ",");

            updateValue(finalValue);
        }
    }
};


ko.bindingHandlers.loading = {
  
    update: function (element, valueAccessor, allBindings, viewModel) {

        if (ko.utils.tagNameLower(element) !== "button") return;

        const value = valueAccessor();
        const valueUnwrapped = ko.unwrap(value);

        const btn = $(element);
        if (valueUnwrapped)
            btn.button("loading");
        else {
            btn.button("reset");
            setTimeout(() => {
                    if (allBindings.get("enable") === false || allBindings.get("disable"))
                        btn.prop("disabled", true);
                },1); // BUG bootstrap'а: без таймаута не получается отключить элемент
        }
    }
};


ko.bindingHandlers.customOptions = {
    init: function(element, valueAccessor, allBindings) {
        if (ko.utils.tagNameLower(element) === "input") {

            ko.utils.registerEventHandler(element,
                "change",
                function() {
                    const value = valueAccessor();
                    value(element.value);
                });
        }
    },
    update: function(element, valueAccessor, allBindings, viewModel) {
        const  isInput = ko.utils.tagNameLower(element) === "input",
            value = ko.utils.unwrapObservable(isInput ? allBindings.get("value") : allBindings.get("text")),
            optionsValue = allBindings.has("optionsValue") ? ko.utils.unwrapObservable(allBindings.get("optionsValue")) : "Value",
            optionsText = allBindings.has("optionsText") ? ko.utils.unwrapObservable(allBindings.get("optionsText")) : "Text";
        let unwrappedArray = ko.utils.unwrapObservable(valueAccessor());

        if (value === undefined)
            throw new Error(`Parameter '${isInput ? "value" : "text"}' is not specified`);
        if (!unwrappedArray) throw new Error("The parameter passed to the 'customOptions' not found or is not an array");

        if (typeof unwrappedArray.length == "undefined") // Coerce single value into array
            unwrappedArray = [unwrappedArray];

        for (let i = 0; i < unwrappedArray.length; i++) {
            const item = unwrappedArray[i];
            if (ko.unwrap(item[optionsValue]) == value) {
                const text = ko.unwrap(item[optionsText]);
                if (isInput) element.value = text;
                else element.innerText = text;
                return;
            }
        }

        if (isInput) element.value = null;
        else element.innerText = "";
    }
};



ko.bindingHandlers.selectedItem = {
    init: function (element, valueAccessor, allBindings, data, context) {

        const selectedItem = valueAccessor();
        if (!ko.isObservable(selectedItem)) throw new Error("selectedItem is not observable");
        
        const binding = {
            css: {},
            click: () => {
                selectedItem(data);
                return true; // если возвращать void, то будут проблемы с перекрытием событий
            }
        };
        const selectedClass = allBindings.get("selectedClass") || "row-selected";
        binding.css[selectedClass] = ko.computed(() => selectedItem() === data);

        ko.applyBindingsToNode(element, binding);
    }
};




function koRegisterPaginationComponent() {

    if (ko.components.isRegistered("pagination")) return;

    // Структура объекта params:
    // data - основные данные (observableArray или computed). Обязательный параметр
    // dataOnPage - данные на текущей странице (observableArray)
    // pageSize - количество элементов на одной странице (number или observable number)
    // pagesCount - максимальное количество видимых кнопок для переключения страниц (number или observable number)
    // page - текущая выбранная страница (observable number)
    // change - событие изменения текущей страницы (event)
    ko.components.register("pagination", {
        viewModel: function (params) {

            const msg = "ko.component.pagination: ";

            if (params == undefined || typeof params != "object") throw new Error(msg + "Required 'params'");

            const data = params.data;
            if (data == null) throw new Error(msg + "Required 'data' parameter.");
            if (!ko.isObservable(data) && !ko.isComputed(data)) throw new Error(msg + "Parameter 'data' must be 'observable' or 'computed'");
            const dataIsArray = data().push !== undefined;
            if (!dataIsArray && typeof data() !== "number") throw new Error(msg + "Parameter 'data' must return array or number (total items count)");
            
            const self = this;

            self.itemsCount = dataIsArray ? ko.pureComputed(() => data().length) : data;                                            // общее количество элементов
            self.pageSize = ko.isObservable(params.pageSize) ? params.pageSize : ko.observable(params.pageSize || 50);          // количество элементов на страницу
            self.pagesCount = ko.isObservable(params.pagesCount) ? params.pagesCount : ko.observable(params.pagesCount || 5);   // максимальное количество видимых страниц

            self.totalPages = ko.pureComputed(() => Math.max(Math.ceil(self.itemsCount() / self.pageSize()), 1));              // общее количество страниц
            

            let pageValue = params.page;
            if (pageValue == undefined)
                pageValue = ko.observable(1);
            else if (!ko.isObservable(pageValue))
                throw new Error(msg + "Parameter 'page' must be observable");
                

            // текущая страница
            self.currentPage = ko.pureComputed({     
                read: function () {
                    const cp = pageValue();
                    if (cp == undefined) return 0;
                    if (cp < 1) return 1;
                    const tp = self.totalPages();
                    if (cp > tp) return tp;
                    return cp;
                },
                write: function (page) {
                    if (isNaN(page)) return;
                    if (page < 1) pageValue(1);
                    else {
                        const tp = self.totalPages();
                        if (page > tp) pageValue(tp);
                        else pageValue(page);
                    }
                }
            });


            self.firstPage = ko.pureComputed(() => Math.max(self.currentPage() - Math.floor(self.pagesCount() / 2), 1));  // первая видимая страница
            self.lastPage = ko.pureComputed(() => Math.min(Math.max(self.totalPages(), 1), self.firstPage() + self.pagesCount() - 1)); // последняя видимая страница

            self.hasPrevious = ko.pureComputed(() => self.currentPage() > 1);             // есть ли следующая страница
            self.hasNext = ko.pureComputed(() => self.totalPages() > self.currentPage()); // есть ли предыдущая страница
            
            // видимые страницы
            self.pages = ko.pureComputed(function () {
                const pages = [],
                    pc = self.pagesCount(),
                    lp = self.lastPage(),
                    fp = self.firstPage(),
                    d = pc - (lp - fp) - 1;
                for (let i = Math.max(fp - d, 1); i <= lp; ++i) pages.push(i);
                return pages;
            });

            self.isVisibleFirstPage = ko.pureComputed(() => self.pages().indexOf(1) < 0);                // видна ли первая страница
            self.isVisibleLastPage = ko.pureComputed(() => self.pages().indexOf(self.totalPages()) < 0); // видна ли последняя страница


            // смена страницы относительно текущей
            self.move = function (n) {
                const newPage = self.currentPage() + n;
                if (newPage > 0 && newPage <= self.totalPages()) self.currentPage(newPage);
            }

            // следующая страница
            self.nextPage = function () {
                self.move(1);
            }

            // предыдущая страница
            self.prevPage = function () {
                self.move(-1);
            }

            // первая страница
            self.moveToFirstPage = function () {
                self.currentPage(1);
            }

            // первая страница
            self.moveToLastPage = function () {
                self.currentPage(self.totalPages());
            }

            // вычислямый массив с данными на текущей странице
            const getDataOnPage = ko.pureComputed(function() {
                const cp = self.currentPage(),
                    pageSize = self.pageSize(),
                    startIndex = (cp - 1) * pageSize,
                    endIndex = startIndex + pageSize;
                return data().slice(startIndex, endIndex);
            });

            const dataOnPage = params.dataOnPage;
            if (dataOnPage != undefined && dataIsArray) {
                if (!ko.isObservable(dataOnPage) || dataOnPage.push === undefined) throw new Error(msg + "Parameter 'dataOnPage' must be 'observableArray'");
                
                data.subscribe(function (a) {
                    dataOnPage(getDataOnPage());
                });

                self.currentPage.subscribe(function(page) {
                    dataOnPage(getDataOnPage());
                });

                self.pageSize.subscribe(function(ps) {
                    dataOnPage(getDataOnPage());
                });
            }

            const onChange = params.change;
            if (onChange != undefined) {
                if (typeof onChange != "function") throw new Error(msg + "Parameter 'change' must be 'function'");
                self.currentPage.subscribe(onChange);
            }
        },
        template: '<ul class="pagination float-right" style="padding: 0px !important;">' +
                    '<li class="footable-page" data-bind="visible: isVisibleFirstPage">' +
                        '<a href="#" aria-label="Первая" data-bind="click: moveToFirstPage">' +
                            '<span aria-hidden="true"><i class="fa fa-angle-double-left"></i></span>' +
                        '</a>' +
                    '</li>' +
                    '<li class="footable-page" data-bind="enable: hasPrevious">' +
                        '<a href="#" aria-label="Предыдущая" data-bind="click: prevPage">' +
                            '<span aria-hidden="true"><i class="fa fa-angle-left"></i></span>' +
                        '</a>' +
                    '</li>' +
                    '<!-- ko foreach: { data: pages, as: \'page\' } -->' +
                    '<li class="footable-page" data-bind="css: {active: $parent.currentPage() === page }" style="white-space: nowrap;">' +
                         '<!-- ko if: $parent.currentPage() === page --> ' +
                            '<a href="#" data-bind="text: $parent.currentPage"/>' +
                        '<!-- /ko -->' +
                        '<!-- ko ifnot: $parent.currentPage() === page --> ' +
                            '<a href="#" data-bind="text: page, click: () => $parent.currentPage(page)"></a>' +
                        '<!-- /ko -->' +
                    '</li>' +
                    '<!-- /ko -->' +
                    '<li class="footable-page" data-bind="enable: hasNext">' +
                        '<a href="#" aria-label="Следующая" data-bind="click: nextPage">' +
                            '<span aria-hidden="true"><i class="fa fa-angle-right"></i></span>' +
                        '</a>' +
                    '</li>' +
                    '<li class="footable-page" data-bind="visible: isVisibleLastPage">' +
                        '<a href="#" aria-label="Последняя" data-bind="click: moveToLastPage">' +
                            '<span aria-hidden="true"><i class="fa fa-angle-double-right"></i></span>' +
                        '</a>' +
                    '</li>' +
                '</ul>'
    });

}

function koMappingCloneObject(mapObj) {
    return ko.mapping.fromJS(ko.mapping.toJS(mapObj));
}