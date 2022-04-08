var RefundService = function (httpRequestsService) {
  
  var refundUserOperation = function (userShopTransactionId, bankTransactionInfoId, done, fail) {
      httpRequestsService.performPatchRequest(
          "/api/purchases/" + userShopTransactionId + "/bankTransactionInfos/" + bankTransactionInfoId + "/refund",
          null, 
          done, 
          fail);
  };
  
  return {
    refundUserOperation: refundUserOperation
  }
}(HttpRequestsService);