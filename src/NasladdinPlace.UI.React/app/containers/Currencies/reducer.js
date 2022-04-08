import { LOAD_CURRENCIES_SUCCESS } from './constants';

const initialState = [];

export default function currenciesReducer(state = initialState, action) {
  switch (action.type) {
    case LOAD_CURRENCIES_SUCCESS:
      return action.currencies;
    default:
      return state;
  }
}
