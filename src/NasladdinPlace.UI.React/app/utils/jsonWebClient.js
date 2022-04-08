import 'whatwg-fetch';

class JsonWebClient {
  static fetchJson(url) {
    return fetch(url, {
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'same-origin',
    }).then(response => response.json());
  }

  static postJson(url, body) {
    return fetch(url, {
      method: 'POST',
      body: JSON.stringify(body),
      headers: {
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': '*',
      },
      credentials: 'same-origin',
    });
  }

  static postJsonAndReturnJson(url, body) {
    return JsonWebClient.postJson(url, body).then(response => response.json());
  }

  static postJsonAndReturnText(url, body) {
    return JsonWebClient.postJson(url, body).then(response => response.text());
  }

  static performDeleteRequest(url) {
    return fetch(url, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': '*',
      },
      credentials: 'same-origin',
    });
  }
}

export default JsonWebClient;
