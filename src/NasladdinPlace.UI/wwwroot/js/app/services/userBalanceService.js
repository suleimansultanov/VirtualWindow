var UserBalanceService = function() {
    var resetUserBalance = function (userId, onSuccess, onFailure) {
        $.ajax("/api/users/" + userId + "/purchases/unfinished",
            {
                type: "DELETE"
            })
            .done(onSuccess)
            .fail(onFailure);
    };
    
    return {
        resetUserBalance: resetUserBalance
    }
}();