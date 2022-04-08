import { call, put, takeLatest } from 'redux-saga/effects';

import jsonWebClient from '../../utils/jsonWebClient';
import { BASE_FRONT_URL } from '../../constants/urls';

import { LOAD_CURRENCIES } from './constants';
import { loadCurrenciesSuccess, loadCurrenciesFailure } from './actions';

export function* loadCurrencies() {
  const requestURL = `${BASE_FRONT_URL}/api/currencies`;

  try {
    const currencies = yield call(jsonWebClient.fetchJson, requestURL);
    yield put(loadCurrenciesSuccess(currencies));
  } catch (error) {
    yield put(loadCurrenciesFailure(error));
  }
}

/**
 * Root saga manages watcher lifecycle
 */
export default function* currenciesData() {
  yield takeLatest(LOAD_CURRENCIES, loadCurrencies);
}
