import { call, put, takeLatest } from 'redux-saga/effects';

import jsonWebClient from '../../utils/jsonWebClient';
import { BASE_FRONT_URL } from '../../constants/urls';

import { showSuccess, showError } from '../Alert/actions';

import { LOAD_DISABLED_LABELED_GOODS, ENABLE_LABELED_GOODS } from './constants';
import {
  loadDisabledLabeledGoodsGroupedByPointsOfSaleSuccess,
  loadDisabledLabeledGoodsGroupedByPointsOfSaleFailure,
  enableLabeledGoodsSuccess,
  enableLabeledGoodsFailure,
} from './actions';

export function* loadDisabledLabeledGoods() {
  const requestURL = `${BASE_FRONT_URL}/api/LabeledGoods/disabled`;

  try {
    const disabledLabeledGoods = yield call(
      jsonWebClient.fetchJson,
      requestURL,
    );
    yield put(
      loadDisabledLabeledGoodsGroupedByPointsOfSaleSuccess(
        disabledLabeledGoods,
      ),
    );
  } catch (err) {
    yield put(loadDisabledLabeledGoodsGroupedByPointsOfSaleFailure(err));
  }
}

export function* enableLabeledGoods(action) {
  const { labeledGoodsIdsToEnable } = action;

  const requestURL = `${BASE_FRONT_URL}/api/LabeledGoods/enabled`;
  const requestBody = {
    values: labeledGoodsIdsToEnable,
  };

  try {
    const enabledLabeledGoods = yield call(
      jsonWebClient.postJsonAndReturnJson,
      requestURL,
      requestBody,
    );
    yield put(enableLabeledGoodsSuccess(enabledLabeledGoods.values));
    showSuccess();
  } catch (err) {
    yield put(enableLabeledGoodsFailure(err));
    showError();
  }
}

/**
 * Root saga manages watcher lifecycle
 */
export default function* disabledLabeledGoodsData() {
  return [
    yield takeLatest(LOAD_DISABLED_LABELED_GOODS, loadDisabledLabeledGoods),
    yield takeLatest(ENABLE_LABELED_GOODS, enableLabeledGoods),
  ];
}
