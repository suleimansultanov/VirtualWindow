import {
  LOAD_GOODS_SUCCESS,
  LOAD_GOODS_FAILURE,
  LOAD_GOODS,
} from './constants';

export function loadGoodsSuccess(goods) {
  return {
    type: LOAD_GOODS_SUCCESS,
    goods,
  };
}

export function loadGoodsFailure(goods) {
  return {
    type: LOAD_GOODS_FAILURE,
    goods,
  };
}

export function loadGoods() {
  return {
    type: LOAD_GOODS,
  };
}
