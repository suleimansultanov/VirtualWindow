import { call, put, takeLatest } from 'redux-saga/effects';

import jsonWebClient from '../../utils/jsonWebClient';
import { BASE_FRONT_URL } from '../../constants/urls';

import { LOAD_GOODS } from './constants';
import { loadGoodsSuccess, loadGoodsFailure } from './actions';

export function* loadGoods() {
  const requestURL = `${BASE_FRONT_URL}/api/goods/`;

  try {
    const goods = yield call(jsonWebClient.fetchJson, requestURL);
    yield put(loadGoodsSuccess(goods));
  } catch (error) {
    yield put(loadGoodsFailure(error));
  }
}

/**
 * Root saga manages watcher lifecycle
 */
export default function* goodsData() {
  yield takeLatest(LOAD_GOODS, loadGoods);
}
