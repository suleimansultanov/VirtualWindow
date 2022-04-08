import { fromJS } from 'immutable';

import { ADD_LOG_SUCCESS, LOAD_LOGS_SUCCESS } from './constants';

export const initialState = fromJS([]);

function logsReducer(state = initialState, action) {
  switch (action.type) {
    case LOAD_LOGS_SUCCESS: {
      return fromJS(action.logs);
    }
    case ADD_LOG_SUCCESS: {
      const log = Object.assign({}, action.log);
      log.orderNumber = state.size + 1;
      return fromJS([log, ...state]);
    }
    default:
      return state;
  }
}

export default logsReducer;
