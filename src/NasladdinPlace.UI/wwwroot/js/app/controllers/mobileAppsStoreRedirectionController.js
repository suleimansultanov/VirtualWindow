var MobileAppsStoreRedirectionController = function (mobileOperatingSystemDetector) {
    
    var initialize = function () {
        var os = mobileOperatingSystemDetector.detect();

        redirectToMobileAppsStoreByOS(os);
    };
    
    var redirectToMobileAppsStoreByOS = function (os) {
        switch (os) {
            case ANDROID:
                redirectToPage("https://play.google.com/store/apps/details?id=net.timelysoft.nasladdin");
                break;
            case IOS:
                redirectToPage("https://itunes.apple.com/app/id1247762108");
                break;
            default:
                bootstrapAlert("Mobile Operating System Detector", "Operating system is not detected");
                break;
        }
    };
    
    var redirectToPage = function (url) {
        window.location.href = url;  
    };
    
    return {
        initialize: initialize
    }
}(MobileOperatingSystemDetector);