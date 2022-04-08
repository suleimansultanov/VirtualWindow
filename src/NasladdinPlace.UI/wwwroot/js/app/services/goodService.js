var GoodService = function() {
    var deleteGood = function(goodId, done, fail) {
        $.ajax("/api/goods/" + goodId,
                {
                    type: "DELETE"
                })
            .done(done)
            .fail(fail);
    };

    return {
        deleteGood: deleteGood
    };
}();