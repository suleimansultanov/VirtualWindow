import { call, put, takeLatest } from 'redux-saga/effects';

import jsonWebClient from '../../utils/jsonWebClient';
import { BASE_BACK_URL } from '../../constants/urls';

import { LOAD_POS_REAL_TIME_INFO } from './constants';
import {
  loadPosRealTimeInfoSuccess,
  loadPosRealTimeInfoFailure,
} from './actions';

export function* loadPosRealTimeInfo(action) {
  const { posId } = action;
  const requestURL = `${BASE_BACK_URL}/api/plants/${posId}/realTimeInfo`;

  try {
    const posRealTimeInfo = yield call(jsonWebClient.fetchJson, requestURL);

    yield put(loadPosRealTimeInfoSuccess(posId, posRealTimeInfo));
  } catch (err) {
    yield put(loadPosRealTimeInfoFailure(err));
  }
}

/**
 * Root saga manages watcher lifecycle
 */
export default function* posMonitoringData() {
  yield takeLatest(LOAD_POS_REAL_TIME_INFO, loadPosRealTimeInfo);
}
