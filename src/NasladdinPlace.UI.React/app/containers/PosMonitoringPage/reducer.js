import { LOAD_POS_REAL_TIME_INFO_SUCCESS } from './constants';

const initialState = {};

export default function posRealTimeInfoReducer(state = initialState, action) {
  switch (action.type) {
    case LOAD_POS_REAL_TIME_INFO_SUCCESS:
      return Object.assign({}, state, {
        [action.posId]: action.posRealTimeInfo,
      });
    default:
      return state;
  }
}
