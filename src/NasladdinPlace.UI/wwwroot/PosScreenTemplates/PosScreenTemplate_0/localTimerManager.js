function LocalTimerManager(updateCallback) {
    const self = this;  
    const UPDATE_INTERVAL_IN_MILLIS = 1000;
    
    let timerId;
    
    self.startTimer = function() {
        if (timerId) return;       
        
        timerId = setInterval(function() {
            updateCallback();
        }, UPDATE_INTERVAL_IN_MILLIS);
    };
    
    self.stopTimer = function() {
        if (!timerId) return;
        
        clearInterval(timerId);
        timerId = undefined;
    }
}