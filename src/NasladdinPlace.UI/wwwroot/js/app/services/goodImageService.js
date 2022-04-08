var GoodImageService = function() {
    var deleteGoodImage = function(goodImageId, done, fail) {
        $.ajax("/api/goodImages/" + goodImageId,
                {
                    type: "DELETE"
                })
            .done(done)
            .fail(fail);
    };

    var deletePlantImage = function(plantImageId, done, fail) {
        $.ajax("/api/plantImages/" + plantImageId,
            {
                type: "DELETE"
            })
            .done(done)
            .fail(fail);
    };

    return {
        deleteGoodImage: deleteGoodImage,
        deletePlantImage: deletePlantImage
    };
}();