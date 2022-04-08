import { call, put, takeLatest } from 'redux-saga/effects';

import jsonWebClient from '../../utils/jsonWebClient';
import { BASE_FRONT_URL } from '../../constants/urls';

import { showSuccess, showError } from '../Alert/actions';

import {
  LOAD_UNTIED_LABELED_GOODS,
  BLOCK_LABELED_GOODS,
  TIE_LABELS_TO_GOOD,
  OPEN_LEFT_DOOR,
  OPEN_RIGHT_DOOR,
  CLOSE_DOORS,
  REQUEST_CONTENT,
} from './constants';

import {
  loadUntiedLabeledGoodsSuccess,
  loadUntiedLabeledGoodsFailure,
  performOperationWithLabeledGoodsSuccess,
} from './actions';

const goodsIdentificationMode = {
  mode: 1,
};

export function* loadUntiedLabeledGoods(action) {
  const { posId } = action;

  const requestURL = `${BASE_FRONT_URL}/api/pointsOfSales/${posId}/labeledGoods/untied`;

  try {
    const untiedLabeledGoods = yield call(jsonWebClient.fetchJson, requestURL);
    yield put(loadUntiedLabeledGoodsSuccess(posId, untiedLabeledGoods));
  } catch (err) {
    yield put(loadUntiedLabeledGoodsFailure(err));
  }
}

export function* blockLabeledGoods(action) {
  const { posId } = action;
  const { labelsModel } = action;

  const requestURL = `${BASE_FRONT_URL}/api/pointsOfSales/${posId}/labeledGoods/untied/blocked`;

  try {
    const response = yield call(jsonWebClient.postJson, requestURL, labelsModel);
    if(response.status >= 200 && response.status < 300){
      yield put(
        performOperationWithLabeledGoodsSuccess(posId, labelsModel.labels),
      );
      showSuccess();
    }else { throw new Error(); }
  } catch (err) {
    showError();
  }
}

export function* tieLabelsToGood(action) {
  const { posId } = action;
  const { labelsToGoodModel } = action;

  const requestURL = `${BASE_FRONT_URL}/api/pointsOfSales/${posId}/labeledGoods/untied`;

  try {
    const response = yield call(jsonWebClient.postJson, requestURL, labelsToGoodModel);
    if(response.status >= 200 && response.status < 300){
      yield;
      yield put(
        performOperationWithLabeledGoodsSuccess(posId, labelsToGoodModel.labels),
      );
      showSuccess();
    }else { throw new Error(); }
  } catch (err) {
    showError();
  }
}

const getDoorOpeningUrl = (posId, isLeft) => {
  const doorPosition = isLeft ? 'leftDoor' : 'rightDoor';

  return `${BASE_FRONT_URL}/api/plants/${posId}/${doorPosition}`;
};

export function* openLeftDoor(action) {
  const { posId } = action;

  const requestURL = getDoorOpeningUrl(posId, true);

  try {
    yield call(jsonWebClient.postJson, requestURL, goodsIdentificationMode);
    showSuccess();
  } catch (err) {
    showError();
  }
}

export function* openRightDoor(action) {
  const { posId } = action;

  const requestURL = getDoorOpeningUrl(posId, false);

  try {
    yield call(jsonWebClient.postJson, requestURL, goodsIdentificationMode);
    showSuccess();
  } catch (err) {
    showError();
  }
}

export function* closeDoors(action) {
  const { posId } = action;

  const requestURL = `${BASE_FRONT_URL}/api/plants/${posId}/doors`;

  try {
    yield call(jsonWebClient.performDeleteRequest, requestURL);
    showSuccess();
  } catch (err) {
    showError();
  }
}

export function* requestContent(action) {
  const { posId } = action;

  const requestURL = `${BASE_FRONT_URL}/api/labeledGoods/plant/${posId}/content`;

  try {
    yield call(jsonWebClient.postJson, requestURL);
    showSuccess();
  } catch (err) {
    showError();
  }
}

/**
 * Root saga manages watcher lifecycle
 */
export default function* untiedLabeledGoodsData() {
  return [
    yield takeLatest(LOAD_UNTIED_LABELED_GOODS, loadUntiedLabeledGoods),
    yield takeLatest(BLOCK_LABELED_GOODS, blockLabeledGoods),
    yield takeLatest(TIE_LABELS_TO_GOOD, tieLabelsToGood),
    yield takeLatest(OPEN_LEFT_DOOR, openLeftDoor),
    yield takeLatest(OPEN_RIGHT_DOOR, openRightDoor),
    yield takeLatest(CLOSE_DOORS, closeDoors),
    yield takeLatest(REQUEST_CONTENT, requestContent),
  ];
}
