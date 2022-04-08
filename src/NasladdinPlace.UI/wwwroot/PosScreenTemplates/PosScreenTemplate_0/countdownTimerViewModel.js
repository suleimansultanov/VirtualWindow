function CountDownTimerViewModel(onTimerComplete, initialRemainingTime) {
    const self = this;

    self.isEnabled = false;
    self.remainingTime = ko.observable(initialRemainingTime);
    self.progressInPercents = ko.computed(function () {
        return self.remainingTime() / initialRemainingTime * 100;
    });
    self.progressIndicatorHeight = ko.computed(function () {
        return self.progressInPercents() + '%';
    });

    self.decrementRemainingTime = function () {
        if(!self.isEnabled) return;

        const newRemainingTime = self.remainingTime() - 1;
        if (newRemainingTime < 0) return;

        self.remainingTime(newRemainingTime);

        if (self.isTimerCompleted()) {
            onTimerComplete();
        }
    };

    self.isTimerCompleted = function () {
        return self.remainingTime() == 0;
    };

    self.reset = function () {
        self.disable();
        self.remainingTime(initialRemainingTime);
    };

    self.enable = function(){
        self.isEnabled = true;
    }

    self.disable = function(){
        self.isEnabled = false;
    }

    self.detailedRemainingTime = ko.computed(function () {
        const SECONDS_IN_MINUTE = 60;
        const SECONDS_IN_HOUR = SECONDS_IN_MINUTE * 60;

        let remainingTimeValue = self.remainingTime();

        let timerSecondsValue = Math.floor(remainingTimeValue % (SECONDS_IN_MINUTE));
        let timerMinutesValue = Math.floor((remainingTimeValue % (SECONDS_IN_HOUR)) / (SECONDS_IN_MINUTE));

        let formattedRemainingTimeAsString = "";

        if (timerMinutesValue > 0) {
            formattedRemainingTimeAsString += ("0" + timerMinutesValue + ":").substring(1);
        }
        if (timerSecondsValue < 10 && timerMinutesValue > 0) {
            formattedRemainingTimeAsString += "0";
        }

        formattedRemainingTimeAsString += timerSecondsValue;
        
        return formattedRemainingTimeAsString;
    });
}