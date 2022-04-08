import {
  PERFORM_OPERATION_WITH_LABELED_GOODS_SUCCESS,
  LOAD_UNTIED_LABELED_GOODS_SUCCESS,
} from './constants';

const initialState = {};

export default function posUntiedLabeledGoodsReducer(
  state = initialState,
  action,
) {
  switch (action.type) {
    case LOAD_UNTIED_LABELED_GOODS_SUCCESS: {
      const id = action.posId;
      return Object.assign({}, state, {
        [id]: action.labeledGoods,
      });
    }
    case PERFORM_OPERATION_WITH_LABELED_GOODS_SUCCESS: {
      const { posId } = action;
      return Object.assign({}, state, {
        [posId]: [
          ...state[posId].filter(lg => !action.labels.includes(lg.label)),
        ],
      });
    }
    default:
      return state;
  }
}
