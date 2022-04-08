var ShoppingStatisticsController = function(userBalanceService, refundService) {
   var initialize = function (container) {
       $(container).on('click', '.js-resetUserBalance', onResetUserBalanceButtonClickListener);
       $(container).on('click', '.js-refundUserOperation', onRefundUserOperationButtonClickListener);
   };
   
   var onResetUserBalanceButtonClickListener = function(e) {
       var resetButton = $(e.target);
       
       var userId = resetButton.attr("data-userId");
       userBalanceService.resetUserBalance(userId, onOperationSuccess, onOperationFailure);
   };
   
   var onRefundUserOperationButtonClickListener = function(e) {
       var button = $(e.target);
       
       var userShopTransactionId = button.attr('data-userShopTransactionId');
       var bankTransactionId = button.attr('data-bankTransactionInfoId');
       
       refundService.refundUserOperation(
           userShopTransactionId, 
           bankTransactionId, 
           onOperationSuccess, 
           onOperationFailure);
   };
   
   var onOperationSuccess = function() {
       location.reload();
   };
   
   var onOperationFailure = function () {
       console.log(":(");
   };
   
   return {
       initialize: initialize
   }
}(UserBalanceService, RefundService);