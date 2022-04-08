var checkItemStatus = {
    unverified: 0,
    unpaid: 1,
    paid: 2,
    refunded: 3,
    deleted: 4,
    paidUnverified: 5
};

function DetailedCheckViewModel(detailedCheck, fiscalizationInfos, auditDateTimeInfos, posOperationTransactions, bankTransactionInfos) {
    var self = this;

    var fiscalizationChecksHandler = (window.getFiscalizationChecksHandler
        ? window.getFiscalizationChecksHandler()
        : new FiscalizationChecksHandler()
    );

    var posOperationTransactionHandler = (window.getPosOerationTransactionHandler
        ? window.getPosOerationTransactionHandler()
        : new PosOerationTransactionHandler()
    );

    self.FiscalizationInfos = ko.observableArray(fiscalizationInfos);
    self.BankTransactionInfos = ko.observableArray(bankTransactionInfos);
    self.PosOperationTransactions = ko.observableArray(posOperationTransactions);
    self.bonuses = detailedCheck.summary.bonus;
    self.checkGoods = detailedCheck.checkGoods.map(function (checkGood) {
        return new DetailedCheckGoodViewModel(checkGood, detailedCheck.dateStatusUpdated);
    });

    self.actualPaymentAmount = ko.observable(0.0);
    self.totalPaymentAmount = ko.observable(0.0);
    self.totalBonuses = ko.observable(0.0);
    self.totalRefundSumInMoney = ko.observable(0.0);
    self.totalRefundSumInBonuses = ko.observable(0.0);

    self.actualTotalDiscount = ko.observable(0.0);

    self.auditRequestDateTime = ko.observable(auditDateTimeInfos.auditRequestDateTime);
    self.auditCompletionDateTime = ko.observable(auditDateTimeInfos.auditCompletionDateTime);

    self.formattedTotalPaymentAmount = ko.computed(function () {
        return PriceFormatter.makeFormattedPrice(self.totalPaymentAmount());
    });

    self.formattedTotalBonuses = ko.computed(function () {
        return PriceFormatter.makeFormattedPrice(self.totalBonuses());
    });

    self.formattedActualPaymentAmount = ko.computed(function () {
        return PriceFormatter.makeFormattedPrice(self.actualPaymentAmount());
    });

    self.formattedTotalRefundSumInMoney = ko.computed(function () {
        return PriceFormatter.makeFormattedPrice(self.totalRefundSumInMoney());
    });

    self.formattedTotalRefundSumInBonuses = ko.computed(function () {
        return PriceFormatter.makeFormattedPrice(self.totalRefundSumInBonuses());
    });

    self.formattedActualTotalDiscount = ko.computed(function () {
        return PriceFormatter.makeFormattedPrice(self.actualTotalDiscount());
    });

    self.formattedTotalSumInMoney = ko.computed(function() {
        return PriceFormatter.makeFormattedPrice(self.actualPaymentAmount());
    });

    var iterateThroughCheckGoodInstances = function (checkGoodInstanceHandler) {
        self.checkGoods.forEach(function (cg) {
            cg.instances.forEach(function (i) {
                checkGoodInstanceHandler(i);
            });
        });
    };

    self.cancelCheckGoodInstancesModification = function () {
        iterateThroughCheckGoodInstances(function (checkGoodInstance) {
            checkGoodInstance.cancelStatusModification();
        });
    };

    self.hasUnverifiedInstances = ko.computed(function () {
        var hasUnverifiedInstances = false;

        if (self.auditCompletionDateTime() != undefined || self.auditRequestDateTime() == undefined) {
            hasUnverifiedInstances = true;
        } else {
            iterateThroughCheckGoodInstances(function (checkGoodInstance) {
                if (checkGoodInstance.status === checkItemStatus.unverified ||
                    checkGoodInstance.status === checkItemStatus.paidUnverified ||
                    checkGoodInstance.isStatusModified()) {
                    hasUnverifiedInstances = true;
                }
            });
        }

        return hasUnverifiedInstances;
    });

    self.hasModifiedCheckGoodInstances = ko.computed(function () {
        var hasModifiedCheckGoodInstances = false;

        iterateThroughCheckGoodInstances(function (checkGoodInstance) {
            if (checkGoodInstance.isStatusModified()) {
                hasModifiedCheckGoodInstances = true;
            }
        });

        return hasModifiedCheckGoodInstances;
    });

    self.checkItemIdsToDelete = ko.computed(function () {
        var checkItemIdsToDelete = [];

        iterateThroughCheckGoodInstances(function (checkGoodInstance) {
            if (!checkGoodInstance.isStatusModified()) return;

            if (checkGoodInstance.modifiedStatus() === checkItemStatus.deleted ||
                checkGoodInstance.modifiedStatus() === checkItemStatus.refunded) {
                checkItemIdsToDelete.push(checkGoodInstance.id);
            }
        });

        return checkItemIdsToDelete;
    });

    self.checkItemIdsToConfirm = ko.computed(function () {
        var checkItemIdsToConfirm = [];

        iterateThroughCheckGoodInstances(function (checkGoodInstance) {

            if (!checkGoodInstance.isStatusModified()) return;

            if (checkGoodInstance.modifiedStatus() === checkItemStatus.paid) {

                checkItemIdsToConfirm.push(checkGoodInstance.id);
            }
        });

        return checkItemIdsToConfirm;
    });

    var recalculateCheckTotals = function () {
        var totalPaymentAmount = 0.0;
        var totalDiscount = 0.0;
        var posOperationBonuses = detailedCheck.summary.bonus;
        var userBonuses = detailedCheck.userInfo.totalBonusAmount;
        var totalBonuses = 0.0;

        var refundBonus = 0.0;

        var priceWithDiscount = 0.0;

        var paymentAmount = 0.0;
        var refundMoney = 0.0;
        var paidModifiedPriceWithDiscount = 0.0;

        var posOperationPriceWithDiscount = 0.0;

        iterateThroughCheckGoodInstances(function (checkGoodInstance) {
            if (!checkGoodInstance.isModificationAllowed) return;

            var modifiedStatus = checkGoodInstance.modifiedStatus();
            if (modifiedStatus === checkItemStatus.paid ||
                modifiedStatus === checkItemStatus.paidUnverified) {
                totalPaymentAmount += checkGoodInstance.price;
                totalDiscount += checkGoodInstance.discount;
                priceWithDiscount += (checkGoodInstance.price - checkGoodInstance.discount);
            }

            if (checkGoodInstance.status === modifiedStatus) {
                if (modifiedStatus === checkItemStatus.paid ||
                    modifiedStatus === checkItemStatus.paidUnverified)
                    posOperationPriceWithDiscount += (checkGoodInstance.price - checkGoodInstance.discount);
                return;
            }

            if (modifiedStatus === checkItemStatus.deleted) return;

            else if (modifiedStatus === checkItemStatus.refunded) {
                refundMoney += checkGoodInstance.price - checkGoodInstance.discount;
            } else if (modifiedStatus === checkItemStatus.paid ||
                modifiedStatus === checkItemStatus.unpaid) {
                if (checkGoodInstance.status === checkItemStatus.paidUnverified) return;
                if (modifiedStatus === checkItemStatus.paid) {
                    paidModifiedPriceWithDiscount += (checkGoodInstance.price - checkGoodInstance.discount);
                }
            }
        });

        var calculateTotalBonusPoints = function() {
            totalBonuses = paidModifiedPriceWithDiscount > 0 ? posOperationBonuses + userBonuses : posOperationBonuses;
            var operationBonusRest = posOperationPriceWithDiscount - posOperationBonuses;
            if (operationBonusRest <= 0)
                totalBonuses += operationBonusRest;
            if (refundMoney > 0)
                totalBonuses = totalBonuses > priceWithDiscount ? priceWithDiscount : totalBonuses;
            totalBonuses = totalBonuses > priceWithDiscount ? priceWithDiscount : totalBonuses;
            return operationBonusRest;
        }

        var posOperationBonusRest = calculateTotalBonusPoints();

        var calculateRefundAmount = function (operationBonusRest) {
            if (posOperationBonuses > posOperationPriceWithDiscount)
                refundMoney = refundMoney + operationBonusRest;
            if (operationBonusRest < 0)
                refundBonus = -operationBonusRest;

            paymentAmount = paidModifiedPriceWithDiscount - userBonuses;
            if (paymentAmount < 0)
                paymentAmount = 0;
        }

        calculateRefundAmount(posOperationBonusRest);

        self.totalPaymentAmount(totalPaymentAmount);
        self.actualTotalDiscount(totalDiscount);
        self.totalBonuses(totalBonuses);

        self.actualPaymentAmount(paymentAmount);
        self.totalRefundSumInMoney(refundMoney);
        self.totalRefundSumInBonuses(refundBonus);
    }

    var subscribeForCheckGoodInstanceStatusChangeUpdates = function () {
        iterateThroughCheckGoodInstances(function (checkGoodInstance) {
            checkGoodInstance.modifiedStatus.subscribe(function () {
                recalculateCheckTotals();
            });
        });
    };

    subscribeForCheckGoodInstanceStatusChangeUpdates();
    recalculateCheckTotals();

    fiscalizationChecksHandler.initViewModel(self);
    posOperationTransactionHandler.initViewModel(self);
}

function DetailedCheckGoodViewModel(detailedCheckGood, checkDate) {
    var self = this;

    self.name = detailedCheckGood.name;
    self.detailedCheckGood = detailedCheckGood;
    self.instances = detailedCheckGood.instances.map(function (innerDetailedCheckGood) {
        return new CheckGoodInstanceViewModel(innerDetailedCheckGood, checkDate);
    });
}

function CheckGoodInstanceViewModel(checkGoodInstance, checkDate) {
    var self = this;
    self.checkGoodInstance = checkGoodInstance;
    self.id = checkGoodInstance.id;
    self.status = checkGoodInstance.status;
    self.isModifiedByAdmin = checkGoodInstance.isModifiedByAdmin;
    self.modifiedStatus = ko.observable(self.status);
    self.isStatusModified = ko.computed(function() {
       return self.status !== self.modifiedStatus();
    });
    self.price = checkGoodInstance.price;
    self.discount = checkGoodInstance.discount;
    self.formattedPrice = PriceFormatter.makeFormattedPrice(checkGoodInstance.price);
    self.labeledGoodInfo = ko.mapping.fromJS(checkGoodInstance.labeledGoodInfo);

    self.isDeletionAllowed = ko.computed(function() {
        return (
                self.status === checkItemStatus.unpaid ||
                self.status === checkItemStatus.unverified
            ) && !self.isStatusModified();
    });

    self.isRefundAllowed = ko.computed(function() {
        return (self.status === checkItemStatus.paid || self.status === checkItemStatus.paidUnverified) && !self.isStatusModified();
    });

    self.isConfirmationAllowed = ko.computed(function() {
        return (self.status === checkItemStatus.unverified || self.status === checkItemStatus.paidUnverified) && !self.isStatusModified();
    });

    self.isModificationAllowed = self.status !== checkItemStatus.deleted && self.status !== checkItemStatus.refunded;

    self.markAsDeleted = function() {
        if (!self.isDeletionAllowed()) return;

        self.modifiedStatus(checkItemStatus.deleted);
    };

    self.markAsRefunded = function() {
        if (!self.isRefundAllowed()) return;

        self.modifiedStatus(checkItemStatus.refunded);
    };

    self.markAsConfirmed = function() {
        if (!self.isConfirmationAllowed()) return;

        self.modifiedStatus(checkItemStatus.paid);
    };

    self.cancelStatusModification = function() {
        self.modifiedStatus(self.status);
    };

    var checkDateTime = moment(checkDate);

    if (self.labeledGoodInfo.foundDateTime && self.labeledGoodInfo.foundDateTime()) {
        var labelFoundDateTime = moment(self.labeledGoodInfo.foundDateTime());
        self.labeledGoodInfo.foundDateTime = labelFoundDateTime
            .add(3, 'hours')
            .format("DD.MM.YYYY HH:mm:ss");

        self.labeledGoodInfo.durationBetweenFoundDateAndCheckDate = DateUtilities.findDurationBetweenDates(
            labelFoundDateTime,
            checkDateTime
        );
    }

    if (self.labeledGoodInfo.lostDateTime && self.labeledGoodInfo.lostDateTime()) {
        var labelLostDateTime = moment(self.labeledGoodInfo.lostDateTime());
        self.labeledGoodInfo.lostDateTime = labelLostDateTime
            .add(3, 'hours')
            .format("DD.MM.YYYY HH:mm:ss");
        self.labeledGoodInfo.durationBetweenLostDateAndCheckDate = DateUtilities.findDurationBetweenDates(
            labelLostDateTime,
            checkDateTime
        );
    }
}

function FiscalizationChecksHandler(initViewModel, onSelectTextReferencValue) {
    return {
        initViewModel: initViewModel || function(vm) {},
        onSelectTextReferencValue: onSelectTextReferencValue || function(vm, data, property, propertyName) {}
    };
}

function PosOerationTransactionHandler(initViewModel, onSelectTextReferencValue) {
    return {
        initViewModel: initViewModel || function (vm) { },
        onSelectTextReferencValue: onSelectTextReferencValue || function (vm, data, property, propertyName) { }
    };
}