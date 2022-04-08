var PlantService = function(httpRequestService) {
    
    var deletePlant = function (plantId, done, fail) {
        httpRequestService.performDeleteRequest("/api/plants/" + plantId, done, fail);
    };

    var closeDoors = function(plantId, done, fail) {
        httpRequestService.performDeleteRequest("/api/plants/" + plantId + "/doors", done, fail);
    };

    var sendPlantContentRequest = function (plantId, done, fail) {
        httpRequestService.performPostRequest("/api/labeledGoods/plant/" + plantId + "/content", null, done, fail);
    };

    var openRightDoor = function (plantId, mode, done, fail) {
        httpRequestService.performPostRequest("/api/plants/" + plantId + "/rightDoor", mode, done, fail);
    };

    var openLeftDoor = function (plantId, mode, done, fail) {
        httpRequestService.performPostRequest("/api/plants/" + plantId + "/leftDoor", mode, done, fail);
    };

    return {
        deletePlant: deletePlant,
        openRightDoor: openRightDoor,
        openLeftDoor: openLeftDoor,
        closeDoors: closeDoors,
        sendPlantContentRequest: sendPlantContentRequest
    };
}(HttpRequestsService);