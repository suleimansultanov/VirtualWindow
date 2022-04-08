function initGrid(vm, cfg, vmroot) {
    cfg = cfg || {};
    var customHandler = (window.getGridCustomHandler ? window.getGridCustomHandler() : new GridCustomHandler());
    var serviceProxy = cfg.serviceProxy || new ServiceProxy({ contentType: "application/json" });
    var formEdit = cfg.formEdit || "#modal-grid-edit-form";
    vmroot = vmroot || vm;
    var mapping = {
        'ignore': ["extend"]
    }
    vm.cfg = cfg;
    vm.cfg.mapping = mapping;
    vm.serviceProxy = serviceProxy;

    vmroot.DependencyReferences = function (root, referenceName, dependency) {
        if (dependency) {
            return root.References[referenceName].Data.filter(x=>x.Dependency == dependency());
        }
        else {
            return root.References[referenceName].Data;
        }
    }

    vm.isLoading = ko.observable(false);

    vm.selectedRow = ko.observable();
    vm.selectRow = (item, e) => {
        vm.selectedRow(item);
        return true;
    };

    vm.isSelectRow = (item) => item === vm.selectedRow();

    vm.hasCustomBackground = function (item) {
        return null;
    };

    vm.hasCustomTextColor = function (item) {
        return null;
    };

    vm.selectedArrayPropertyName = null;
    vm.selectedArray = ko.observableArray([]);
    vm.isAllSelectedArray = ko.observable(false);
    vm.clearSelectedArray = function () {
        vm.selectedArray([]);
    }
    vm.addPageSelectedArray = function (propertyName) {
        if (vm.isAllSelectedArray()) {
            vm.Items().forEach(i => {
                let val = i[propertyName]();
                if (vm.selectedArray().indexOf(val) < 0 && (!i.extend || !i.extend.isnew)) {
                    vm.selectedArray.push(val);
                }
            });
        } else {
            vm.Items().forEach(i => {
                vm.selectedArray.remove(i[propertyName]());
            });
        }
        return true;
    }
    vm.calcIsAllSelectedArray = function () {
        const propertyName = cfg.selectPropertyName;
        if (propertyName) {
            const items = vm.Items();
            const selectedArray = vm.selectedArray();
            let res = items.length > 0 ? true : false;

            for (let j = 0; j < items.length; j++) {
                if (selectedArray.indexOf(items[j][propertyName]()) < 0 && (!items[j].extend || !items[j].extend.isnew)) {
                    res = false;
                    break;
                }
            }
            if (vm.isAllSelectedArray() !== res) {
                vm.isAllSelectedArray(res);
            }
        }
    }
    vm.selectedArray.subscribe(function (changes) {
        vm.calcIsAllSelectedArray();
    }, null, "arrayChange");

    vm.selectAllActionName = "SelectAll";
    vm.selectAllSelectedArray = function () {
        const isPagination = vm.Pagination && typeof vm.Pagination !== "function";
        if (isPagination) {
            const filters = ko.mapping.toJS(vm.Filter),
                context = ko.mapping.toJS(vm.Context),
                map = jQuery.map(filters, f => f),
                mapContext = jQuery.map(context, f => f),
                data = { Filter: map, ReferenceType: vm.ReferenceType, context: mapContext, propertyName: cfg.selectPropertyName };
            serviceProxy.postJsonResult(cfg.url + '/' + vm.selectAllActionName, JSON.stringify(data), function (result) {
                const totalItems = vm.Pagination && typeof vm.Pagination !== "function" ? vm.Pagination.TotalItems() : vm.Items().length;
                if (result.data.length === totalItems) {
                    vm.selectedArray(result.data);
                } else {
                    bootstrapAlert(cfg.viewbagTitle, "Внимание!!! Данные на сервере изменились необходимо обновить данные");
                }
            });
        } else {
            vm.isAllSelectedArray(true);
            vm.addPageSelectedArray(cfg.selectPropertyName);
        }
    }

    vm.totalItemCount = ko.computed(function () {
        return vm.Pagination && typeof vm.Pagination !== "function" ? vm.Pagination.TotalItems() : vm.Items().length;
    }, this);

    vm.refreshCallback = function (result) { }

    vm.InitOriginalData = function () {
        vm.Items().forEach(i => {
            i.extend = i.extend || {};
            i.extend.original = ko.mapping.toJS(i);
        });
        vm.RemovedItems = [];
    }
    
    vm.currentFilter = null;
    vm.setCurrentFilter = function () {
        const filterData = ko.mapping.toJS(vm.Filter, mapping),
            mapFilter = jQuery.map(filterData, v => v);
        vm.currentFilter = mapFilter;
    }
    vm.applyFilter = function () {
        const filters = ko.mapping.toJS(vm.Filter, mapping),
            map = jQuery.map(filters, f => f);
        vm.currentFilter = map;
        if (typeof vm.Pagination.Page === "function") {
            vm.Pagination.Page(1);
        } else {
            vm.Pagination.Page = 1;
        }
        vm.refresh();
    }
    vm.refresh = function () {
        const context = ko.mapping.toJS(vm.Context, mapping),

        mapContext = jQuery.map(context, f => f),

        data = { Filter: vm.currentFilter, ReferenceType: vm.ReferenceType(), context: mapContext };
        if (vm.Pagination) { data.Pagination = ko.mapping.toJS(vm.Pagination); }

        serviceProxy.postJsonResult(cfg.loadUrl, JSON.stringify(data), function (result) {
            if (result.error) {
                bootstrapErrorWithTitle(cfg.viewbagTitle, result.error);
            } else {
                vm.refreshCallback(result);
                vm.selectedRow(null);
                if (vm.Pagination) ko.mapping.fromJS(result.data.Pagination, {}, vm.Pagination);
                ko.mapping.fromJS(result.data, {}, vm);

                vm.InitOriginalData();

                vm.calcIsAllSelectedArray();
                if (vm.afterRefresh) vm.afterRefresh();
            }
        });
    }
    vm.sortIndex = 0;
    vmroot.sort = function (item) {
        var t = item.SortType() + 1;
        item.SortType(4 % t > 0 ? 0 : t);
        item.SortOrder(vm.sortIndex);
        vm.sortIndex++;
    }
    vm.clearSort = function () {
        vm.sortIndex = 0;
        var array = vm.Filter;
        for (var index in array) {
            if (!array.hasOwnProperty(index)) {
                continue;
            }
            array[index].SortType(0);
            array[index].SortOrder(0);
        }
    }
    vm.clearFilter = function () {
        var array = vm.Filter;
        for (var index in array) {
            if (!array.hasOwnProperty(index) || array[index].IsNotCleanValue()) {
                continue;
            }
            array[index].Value(null);
        }
    }
    vm.clearAll = function () {
        vm.clearSort();
        vm.clearFilter();
    }

    vm.formData = ko.observable();
    vm.formIsAddState = ko.observable(false);
    vm.formEdit = function () {
        vm.formIsAddState(false);        
        var copyData = ko.mapping.fromJS(ko.mapping.toJS(vm.selectedRow(), mapping), mapping);
        vm.formData(copyData);
        jQuery(formEdit).modal();
    }
    vm.formAdd = function () {
        vm.formIsAddState(true);
        vm.formData(vm.createNewItem());
        jQuery(formEdit).modal();
    }
    vm.copy = function (dst, src, level) {
        if (level === 2)
            return;
        for (var index in dst) {
            if (!dst.hasOwnProperty(index) || !src.hasOwnProperty(index)) {
                continue;
            }

            if (typeof dst[index] === "function") {
                dst[index](src[index]());
            } else if (typeof dst[index] === "object") {
                vm.copy(dst[index], src[index], level ? level++ : 1);
            } 
        }
    }
    vm.formAccept = function () {
        if (!ko.validation.isValid(vm.formData)) {
            return;
        }
        if (vm.formDataIsValid && !vm.formDataIsValid(vm.formData())) {
            return;
        }

        if (vm.formIsAddState()) {
            vm.Items.push(vm.formData());
        } else {
            var dst = vm.selectedRow();
            var src = vm.formData();
            vm.copy(dst, src);
        }
        jQuery(formEdit).modal('hide');
    }
    vm.formCancel = function () {
        vm.formData(null);
        jQuery(formEdit).modal('hide');
    }

    vm.selectTextReference = function (property, sourceName, sourceTitle, sourceFilter, isLargeData, formData, textReference, context, propertyName, dependencyProperty) {
        showTextReference(sourceName, sourceTitle, sourceFilter, isLargeData, function (data) {
            if (textReference && Array.isArray(data)) {
                textReference.forEach(function (item, i, arr) {
                    formData[item.fieldName](data[item.index]);
                });
            } else {
                property(Array.isArray(data) ? data[0] : data);
                customHandler.onSelectTextReferencValue(vm, data, property, propertyName);
            }
        }, dependencyProperty ? formData[dependencyProperty]() : '');
    }
    vm.selectTextReferenceValueText = function (propertyValue, propertyText, sourceName, sourceTitle, sourceFilter, isLargeData, element, selector) {
        showTextReference(sourceName, sourceTitle, sourceFilter, isLargeData, function (data) {
            var text = Array.isArray(data) ? data[0] : data;
            if (propertyText) {
                propertyText(text);
            } else {
                jQuery("." + selector).val(text);
            }

            propertyValue(Array.isArray(data) ? data[data.length - 1] : data);
        });
    }

    vm.arrayEq = function (a, b) {
        for (var i = 0; i < a.length; i++) {
            if (a[i] !== b[i])
                return false;
        }
        return true;
    }

    vm.getRemove = function (data) {
        var item = [];
        for (var i = 0; i < data.length; i++) {
            if (!(data[i] && data[i].extend && data[i].extend.isnew)) {
                var current = vm.Keys.map(x => ko.unwrap(data[i][x]));
                var compare = vm.Keys.map(x => data[i].extend.original[x]);

                if (!vm.arrayEq(current, compare)) {
                    item.push(data[i].extend.original);
                }
            }
        }
        return item;
    }

    vm.submitModalFilter = function () {
        vm.applyFilter();
        $("#reference-filter-modal").modal('toggle');        
    }

    vm.showModalFilter = function() {
        $("#reference-filter-modal").modal();
    }
       
    vm.InitOriginalData();

    customHandler.initViewModel(vm);

    vm.setCurrentFilter();
}

function GridCustomHandler(initViewModel, onSelectTextReferencValue) {
    return {
        initViewModel: initViewModel || function (vm) { },
        onSelectTextReferencValue: onSelectTextReferencValue || function (vm, data, property, propertyName) { }
    }
}