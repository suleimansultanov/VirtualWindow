import {
  ENABLE_LABELED_GOODS_SUCCESS,
  ENABLE_LABELED_GOODS,
  ENABLE_LABELED_GOODS_FAILURE,
  LOAD_DISABLED_LABELED_GOODS,
  LOAD_DISABLED_LABELED_GOODS_SUCCESS,
  LOAD_DISABLED_LABELED_GOODS_FAILURE,
} from './constants';

export function loadDisabledLabeledGoodsGroupedByPointsOfSale() {
  return {
    type: LOAD_DISABLED_LABELED_GOODS,
  };
}

export function loadDisabledLabeledGoodsGroupedByPointsOfSaleSuccess(
  disabledLabeledGoodsGroupedByPointsOfSale,
) {
  return {
    type: LOAD_DISABLED_LABELED_GOODS_SUCCESS,
    disabledLabeledGoodsGroupedByPointsOfSale,
  };
}

export function loadDisabledLabeledGoodsGroupedByPointsOfSaleFailure(error) {
  return {
    type: LOAD_DISABLED_LABELED_GOODS_FAILURE,
    error,
  };
}

export function enableLabeledGoods(labeledGoodsIdsToEnable) {
  return {
    type: ENABLE_LABELED_GOODS,
    labeledGoodsIdsToEnable,
  };
}

export function enableLabeledGoodsSuccess(enabledLabeledGoodsIds) {
  return {
    type: ENABLE_LABELED_GOODS_SUCCESS,
    enabledLabeledGoodsIds,
  };
}

export function enableLabeledGoodsFailure(error) {
  return {
    type: ENABLE_LABELED_GOODS_FAILURE,
    error,
  };
}
