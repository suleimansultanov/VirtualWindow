var DateUtilities = function() {
    var findDurationBetweenDates = function(firstDate, secondDate) {
        var durationBetweenFirstAndSecondDatesInMillis = moment.duration(
            firstDate.diff(secondDate)
        ).asMilliseconds();
        durationBetweenFirstAndSecondDatesInMillis = Math.abs(durationBetweenFirstAndSecondDatesInMillis);
        var durationInDays = Math.floor(durationBetweenFirstAndSecondDatesInMillis / 24 / 60 / 60 / 1000);
        var durationInDaysInMillis = durationInDays * 24 * 60 * 60 * 1000;
        var durationInHours = Math.floor((durationBetweenFirstAndSecondDatesInMillis - durationInDaysInMillis) / 60 / 60 / 1000);
        var durationInHoursInMillis = durationInHours * 60 * 60 * 1000;
        var durationInMinutes = Math.floor((durationBetweenFirstAndSecondDatesInMillis - durationInDaysInMillis - durationInHoursInMillis) / 60 / 1000);
        var durationInMinutesInMillis = durationInMinutes * 60 * 1000;
        var durationInSeconds = Math.floor((durationBetweenFirstAndSecondDatesInMillis - durationInDaysInMillis - durationInHoursInMillis - durationInMinutesInMillis) / 1000);

        return {
            isNegative: firstDate < secondDate,
            days: durationInDays,
            hours: durationInHours,
            minutes: durationInMinutes,
            seconds: durationInSeconds
        };
    };
    
    return {
        findDurationBetweenDates: findDurationBetweenDates
    }
}();