import {
  LOAD_DISABLED_LABELED_GOODS_SUCCESS,
  ENABLE_LABELED_GOODS_SUCCESS,
} from './constants';

const initialState = [];

export default function disabledLabeledGoodsGroupedByPointsOfSaleReducer(
  state = initialState,
  action,
) {
  switch (action.type) {
    case LOAD_DISABLED_LABELED_GOODS_SUCCESS:
      return action.disabledLabeledGoodsGroupedByPointsOfSale;
    case ENABLE_LABELED_GOODS_SUCCESS: {
      const { enabledLabeledGoodsIds } = action;
      const newDisabledLabeledGoodsGroupedByPointsOfSale = [];

      state.forEach(posGroup => {
        const newPosGroup = Object.assign({}, posGroup);
        newPosGroup.items = [];
        posGroup.items.forEach(lg => {
          if (!enabledLabeledGoodsIds.includes(lg.id)) {
            newPosGroup.items.push(lg);
          }
        });
        if (newPosGroup.items.length !== 0) {
          newDisabledLabeledGoodsGroupedByPointsOfSale.push(newPosGroup);
        }
      });

      return newDisabledLabeledGoodsGroupedByPointsOfSale;
    }
    default:
      return state;
  }
}
