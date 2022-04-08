import JsonWebClient from '../../utils/jsonWebClient';
import { BASE_FRONT_URL, BASE_BACK_URL } from '../../constants/urls';

export default class Api {
  static loadPosRealTimeInfo(posId) {
    const url = `${BASE_BACK_URL}/api/plants/${posId}/realTimeInfo`;
    return JsonWebClient.fetchJson(url);
  }

  static setPosAntennasOutputPower(posId, outputPower) {
    const url = `${BASE_FRONT_URL}/api/plants/${posId}/antennasOutputPower`;
    return JsonWebClient.postJson(url, { outputPower });
  }
}
