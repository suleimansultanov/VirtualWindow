var PriceFormatter = function() {
   var makeFormattedPrice = function(price) {
       return price.toFixed(2).replace(".", ",");
   };

   return {
       makeFormattedPrice: makeFormattedPrice
   }
}();