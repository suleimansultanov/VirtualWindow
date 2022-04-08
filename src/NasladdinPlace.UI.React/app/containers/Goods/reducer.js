import { LOAD_GOODS_SUCCESS } from './constants';

const initialState = [];

export default function goodsReducer(state = initialState, action) {
  switch (action.type) {
    case LOAD_GOODS_SUCCESS:
      return action.goods;
    default:
      return state;
  }
}
