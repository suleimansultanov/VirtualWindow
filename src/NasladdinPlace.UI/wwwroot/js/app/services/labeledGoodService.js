var LabeledGoodService = function() {
    var putLabeledGood = function (plantId, label, done, fail) {
        $.ajax("/api/labeledGoods/putInOrTakeFromPlant",
                {
                    data: JSON.stringify(
                        {
                            plantId: plantId,
                            I: [label]
                        }),
                    contentType: 'application/json',
                    type: 'POST'
                })
            .done(done)
            .fail(fail);
    };

    var takeLabeledGood = function (plantId, label, done, fail) {
        $.ajax("/api/labeledGoods/putInOrTakeFromPlant",
                {
                    data: JSON.stringify(
                        {
                            plantId: plantId,
                            O: [label]
                        }),
                    contentType: 'application/json',
                    type: 'POST'
                })
            .done(done)
            .fail(fail);
    };

    var deleteLabeledGood = function(labeledGoodId, done, fail) {
        $.ajax("/api/labeledGoods/" + labeledGoodId,
                {
                    type: "DELETE"
            })
            .done(done)
            .fail(fail);
    };

    return {
        putLabeledGood: putLabeledGood,
        takeLabeledGood: takeLabeledGood,
        deleteLabeledGood: deleteLabeledGood
    }
}();