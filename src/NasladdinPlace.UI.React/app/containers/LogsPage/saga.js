import { call, put, takeLatest } from 'redux-saga/effects';

import jsonWebClient from '../../utils/jsonWebClient';
import { BASE_BACK_URL } from '../../constants/urls';

import { LOAD_LOGS } from './constants';
import { loadLogsSuccess, loadLogsFailure } from './actions';

export function* loadLogs() {
  const requestURL = `${BASE_BACK_URL}/api/logs`;

  try {
    // Call our request helper (see 'utils/request')
    const logs = yield call(jsonWebClient.fetchJson, requestURL);
    yield put(loadLogsSuccess(logs));
  } catch (err) {
    yield put(loadLogsFailure(err));
  }
}

/**
 * Root saga manages watcher lifecycle
 */
export default function* logsData() {
  yield takeLatest(LOAD_LOGS, loadLogs);
}
