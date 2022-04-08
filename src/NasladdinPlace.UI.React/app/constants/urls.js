export const BASE_FRONT_URL = getBaseAdminUrl();
export const BASE_BACK_URL = getApiUrl();
export const WEB_SOCKET_URL = getWsUrl();

function getBaseAdminUrl() {
    return AdminUrl;
}

function getApiUrl(){
  return ApiUrl;
}

function getWsUrl(){
  return WsUrl;
}

