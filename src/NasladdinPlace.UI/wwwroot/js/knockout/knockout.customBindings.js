ko.bindingHandlers.safeClick = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        const bindingsAccessor = allBindingsAccessor();
        var injectedBindingValues = {
            click: function (data, event) {
                element.disabled = true;
                element.disabled = false;
                return bindingsAccessor.safeClick;
            }
        };
        ko.applyBindingAccessorsToNode(element, injectedBindingValues);
        return { controlsDescendantBindings: true };
    }
}
ko.bindingHandlers.onRender = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        valueAccessor()(element);
    }
}

ko.bindingHandlers.contentEnable = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var data = ko.unwrap(valueAccessor());
        jQuery(element).find("input").attr("disabled", !data);
        jQuery(element).find("select").attr("disabled", !data);
        jQuery(element).find("button").attr("disabled", !data);
    }
};

ko.bindingHandlers.textOptions = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var data = ko.unwrap(valueAccessor()),
            value = ko.utils.unwrapObservable(allBindings().value);

        if (!Array.isArray(data)) {
            data = data.root.References[data.referenceName].Data;
        }

        const item = data.find(x => (typeof x.Value === "function" && x.Value() === value) || x.Value === value);
        if (item) {
            element.innerHTML = (typeof item.Text === "function") ? item.Text() : item.Text ;
        }
        else element.innerHTML = "";
    }
};
ko.bindingHandlers.dependencyOptions = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var bindingsAccessor = allBindingsAccessor(),
            data = valueAccessor();
        var injectedBindingValues = {
            options: function () {
                if (data.dependency) {
                    const dv = data.dependency();
                    return data.root.References[data.referenceName].Data.filter(x => x.Dependency === null || x.Dependency == dv);
                }
                else {
                    return data.root.References[data.referenceName].Data;
                }
            },
            optionsValue: function () { return bindingsAccessor.optionsValue },
            optionsText: function () { return bindingsAccessor.optionsText },
            optionsCaption: function () { return bindingsAccessor.optionsCaption },
            optionsAfterRender: function () {
                setTimeout(function () { $(".selectpicker").selectpicker('refresh'); }, 500);
            }
        };

        ko.applyBindingAccessorsToNode(element, injectedBindingValues);

        return { controlsDescendantBindings: true };
    }
}

function setElementValue(element, value) {
    if (ko.utils.tagNameLower(element) === "input")
        element.value = value;
    else
        element.innerHTML = value;
}

ko.bindingHandlers.controlGrid = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var gridContainer = ko.unwrap(valueAccessor()),
            root = ko.utils.unwrapObservable(allBindings().root),
            cfg = allBindings().controlGridCfg,
            mapping = {
                'ignore': ["extend"]
            },
            selectPropertyName = "MiltiSelectId";;

        /* Выбор одной строки ****************************************************************************** */
        var onCollectionChanged = function () {
            if (gridContainer.onRefresh) {
                gridContainer.onRefresh();
            }
        }

        gridContainer.selectedIndex = null;
        gridContainer.selectedRow = ko.observable();
        if (gridContainer.onSelectedRowChanged) {
            gridContainer.selectedRow.subscribe(function (newValue) {
                gridContainer.onSelectedRowChanged(newValue);
            });
        }
        gridContainer.selectRow = (item, index) => {
            gridContainer.selectedRow(item);
            gridContainer.selectedIndex = index();
            if (cfg.MultiSelect) {
                if (gridContainer.selectedArray().indexOf(item[selectPropertyName]()) >= 0) {
                    gridContainer.selectedArray.remove(item[selectPropertyName]());
                } else {
                    gridContainer.selectedArray.push(item[selectPropertyName]());
                }
            }
            return true;
        }
        gridContainer.isSelectRow = (item) => item === gridContainer.selectedRow();
        /* ************************************************************************************************* */

        if (cfg.MultiSelect) {
            /* Выбор нескольких строк*************************************************************************** */
            gridContainer.selectedArrayPropertyName = null;
            gridContainer.selectedArray = ko.observableArray([]);
            gridContainer.isAllSelectedArray = ko.observable(false);
            gridContainer.clearSelectedArray = function () {
                gridContainer.selectedArray([]);
            }
            gridContainer.addPageSelectedArray = function (propertyName) {
                if (gridContainer.isAllSelectedArray()) {
                    gridContainer.Items().forEach(i => {
                        let val = i[propertyName]();
                        if (gridContainer.selectedArray().indexOf(val) < 0) {
                            gridContainer.selectedArray.push(val);
                        }
                    });
                } else {
                    gridContainer.Items().forEach(i => {
                        gridContainer.selectedArray.remove(i[propertyName]());
                    });
                }
                return true;
            }
            gridContainer.calcIsAllSelectedArray = function () {
                const propertyName = selectPropertyName;
                if (propertyName) {
                    const items = gridContainer.Items();
                    const selectedArray = gridContainer.selectedArray();
                    let res = items.length > 0 ? true : false;

                    for (let j = 0; j < items.length; j++) {
                        if (selectedArray.indexOf(items[j][propertyName]()) < 0) {
                            res = false;
                            break;
                        }
                    }
                    if (gridContainer.isAllSelectedArray() !== res) {
                        gridContainer.isAllSelectedArray(res);
                    }
                }
            }
            gridContainer.selectedArray.subscribe(function (changes) {
                gridContainer.calcIsAllSelectedArray();
            }, null, "arrayChange");
            gridContainer.selectAllSelectedArray = function () {
                const isPagination = gridContainer.Pagination && typeof gridContainer.Pagination !== "function";
                if (isPagination) {
                    const filters = ko.mapping.toJS(gridContainer.Filter),
                        context = ko.mapping.toJS(gridContainer.Context),
                        map = jQuery.map(filters, f => f),
                        mapContext = jQuery.map(context, f => f),
                        data = { Filter: map, ReferenceType: gridContainer.ReferenceType(), context: mapContext, propertyName: selectPropertyName };

                    root.serviceProxy.postJsonResult(root.cfg.url + '/SelectAll', JSON.stringify(data), function (result) {
                        const totalItems = gridContainer.Pagination && typeof gridContainer.Pagination !== "function" ? gridContainer.Pagination.TotalItems() : gridContainer.Items().length;
                        if (result.data.length === totalItems) {
                            gridContainer.selectedArray(result.data);
                        } else {
                            bootstrapAlert(root.cfg.viewbagTitle, "Внимание!!! Данные на сервере изменились необходимо обновить данные");
                        }
                    });
                } else {
                    gridContainer.isAllSelectedArray(true);
                    gridContainer.addPageSelectedArray(selectPropertyName);
                }
            }
            gridContainer.totalItemCount = ko.computed(function () {
                return gridContainer.Pagination && typeof gridContainer.Pagination !== "function" ? gridContainer.Pagination.TotalItems() : (gridContainer.Items() ? gridContainer.Items().length : 0);
            }, this);
            /* ************************************************************************************************* */
        }


        if (!cfg.IsSimple) {

            /* Фильтрация и сортировка ************************************************************************* */
            gridContainer.refresh = gridContainer.refresh || function () {
                const filters = ko.mapping.toJS(gridContainer.Filter, mapping),
                    context = ko.mapping.toJS(gridContainer.Context, mapping),
                    map = jQuery.map(filters, f => f),
                    mapContext = jQuery.map(context, f => f),
                    data = { Filter: map, ReferenceType: gridContainer.ReferenceType(), context: mapContext };

                if (gridContainer.Pagination) data.Pagination = ko.mapping.toJS(gridContainer.Pagination);

                root.serviceProxy.postJsonResult(root.Url() + "/Load", JSON.stringify(data), function (result) {
                    gridContainer.selectedRow(null);

                    if (gridContainer.Pagination) ko.mapping.fromJS(result.data.Pagination, {}, gridContainer.Pagination);
                    ko.mapping.fromJS(result.data.Items, {}, gridContainer.Items);
                    onCollectionChanged();

                    if (gridContainer.selectedIndex != null && result.data.Items.length >= gridContainer.selectedIndex) gridContainer.selectedRow(gridContainer.Items()[gridContainer.selectedIndex]);
                });
            }
            root.sortIndex = root.sortIndex || 0;
            root.sort = root.sort || function (item) {
                var t = item.SortType() + 1;
                item.SortType(4 % t > 0 ? 0 : t);
                item.SortOrder(root.sortIndex);
                root.sortIndex++;
            }
            gridContainer.clearSort = function () {
                root.sortIndex = 0;
                var array = gridContainer.Filter;
                for (var index in array) {
                    if (!array.hasOwnProperty(index)) {
                        continue;
                    }
                    array[index].SortType(0);
                    array[index].SortOrder(0);
                }
            }
            gridContainer.clearFilter = function () {
                var array = gridContainer.Filter;
                for (var index in array) {
                    if (!array.hasOwnProperty(index) || array[index].IsNotCleanValue()) {
                        continue;
                    }
                    array[index].Value(null);
                }
            }
            gridContainer.clearAll = function () {
                gridContainer.clearSort();
                gridContainer.clearFilter();
                onCollectionChanged();
            }
            /* ************************************************************************************************* */

        }

        gridContainer.clearData = function () {
            gridContainer.Items([]);

            if (gridContainer.Pagination && typeof gridContainer.Pagination !== "function") {
                gridContainer.Pagination.TotalItems(0);
                gridContainer.Pagination.Page(1);
            }

            if (gridContainer.clearSelectedArray) {
                gridContainer.clearSelectedArray();
            }
        }


        gridContainer.remove = function (item) {
            gridContainer.Items.remove(item);

            if (!(item && item.extend && item.extend.isnew)) {
                gridContainer.RemovedItems.push(item);
            }
        }
        gridContainer.createNewItem = function (itemObject) {
            var item = ko.mapping.fromJS(ko.mapping.toJS(itemObject || gridContainer.Default));
            delete item.__ko_mapping__;
            item.extend = item.extend || {};
            item.extend.isnew = true;
            return item;
        }
        gridContainer.addBase = function (item) {
            gridContainer.Items.push(item);
            initFields(jQuery(element).find(".grid-body tr:last"));
        }
        gridContainer.add = function () {
            gridContainer.addBase(gridContainer.createNewItem());
        }

        gridContainer.Inited(true);
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
    }
};

/// связывание для кнопок, повзоляющее переключать состояние: true/false
/// для описания состояний используйте атрибуты data-on-text/data-off-text
/// пример: <button type="button" data-on-text="Включено" data-off-text="Выключено" data-bind="toggle: isOn"></button>
ko.bindingHandlers.toggle = {
    init: function (element, valueAccessor) {

        if (ko.utils.tagNameLower(element) !== "button") throw "Binding 'toggle' set only on buttons";

        const value = valueAccessor();
        if (!ko.isWriteableObservable(value)) throw "You must pass an observable or writeable computed";

        ko.utils.registerEventHandler(element,
        "click",
        function () {
            value(!value());
        });

        const $element = $(element);
        ko.computed({
            disposeWhenNodeIsRemoved: element,
            read: function () {
                $element.toggleClass("active", value());
            }
        });
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        const value = valueAccessor();
        element.innerHTML = value() ? element.getAttribute("data-on-text") || "ON" : element.getAttribute("data-off-text") || "OFF";
    }
};

ko.bindingHandlers.enterkey = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        $(element).keypress(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode === 13) {
                callback.call(viewModel);
                return false;
            }
            return true;
        });
    }
};
ko.bindingHandlers.customHandler = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var cfg = ko.unwrap(valueAccessor());
        if (cfg && cfg.init && typeof cfg.init === "function") {
            cfg.init(cfg.data, element, viewModel, bindingContext);
        }
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var cfg = ko.unwrap(valueAccessor());
        if (cfg && cfg.update && typeof cfg.update === "function") {
            cfg.update(cfg.data, element, viewModel, bindingContext);
        }
    }
};

ko.bindingHandlers.shortTime = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        $(element).change(function() {
            var value = $(this).val();
            if (/^[0-9]{2}:[0-9]{2}$/gi.test(value)) {
                valueAccessor()(value + ":00");
            } else {
                valueAccessor()("00:00:00");
            }
        });
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.unwrap(valueAccessor());
        if (/^[0-9]{2}:[0-9]{2}:[0-9]{2}$/gi.test(value)) {
            $(element).val(value.substring(0, 5));
        } else {
            $(element).val("00:00");
        }
    }
};