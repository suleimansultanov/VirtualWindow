import {
  LOAD_UNTIED_LABELED_GOODS_SUCCESS,
  LOAD_UNTIED_LABELED_GOODS_FAILURE,
  LOAD_UNTIED_LABELED_GOODS,
  PERFORM_OPERATION_WITH_LABELED_GOODS_SUCCESS,
  TIE_LABELS_TO_GOOD,
  BLOCK_LABELED_GOODS,
  OPEN_LEFT_DOOR,
  OPEN_RIGHT_DOOR,
  CLOSE_DOORS,
  REQUEST_CONTENT,
} from './constants';

export function performOperationWithLabeledGoodsSuccess(posId, labels) {
  return {
    type: PERFORM_OPERATION_WITH_LABELED_GOODS_SUCCESS,
    posId,
    labels,
  };
}

export function loadUntiedLabeledGoods(posId) {
  return {
    type: LOAD_UNTIED_LABELED_GOODS,
    posId,
  };
}

export function loadUntiedLabeledGoodsSuccess(posId, labeledGoods) {
  return {
    type: LOAD_UNTIED_LABELED_GOODS_SUCCESS,
    posId,
    labeledGoods,
  };
}

export function loadUntiedLabeledGoodsFailure(posId) {
  return {
    type: LOAD_UNTIED_LABELED_GOODS_FAILURE,
    posId,
  };
}

export function tieLabelsToGood(posId, labelsToGoodModel) {
  return {
    type: TIE_LABELS_TO_GOOD,
    posId,
    labelsToGoodModel,
  };
}

export function blockLabels(posId, labelsModel) {
  return {
    type: BLOCK_LABELED_GOODS,
    posId,
    labelsModel,
  };
}

export function openLeftDoor(posId) {
  return {
    type: OPEN_LEFT_DOOR,
    posId,
  };
}

export function openRightDoor(posId) {
  return {
    type: OPEN_RIGHT_DOOR,
    posId,
  };
}

export function closeDoors(posId) {
  return {
    type: CLOSE_DOORS,
    posId,
  };
}

export function requestContent(posId) {
  return {
    type: REQUEST_CONTENT,
    posId,
  };
}
