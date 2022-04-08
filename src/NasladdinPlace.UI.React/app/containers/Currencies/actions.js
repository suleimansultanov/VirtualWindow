import {
  LOAD_CURRENCIES_SUCCESS,
  LOAD_CURRENCIES_FAILURE,
  LOAD_CURRENCIES,
} from './constants';

export function loadCurrencies() {
  return {
    type: LOAD_CURRENCIES,
  };
}

export function loadCurrenciesSuccess(currencies) {
  return {
    type: LOAD_CURRENCIES_SUCCESS,
    currencies,
  };
}

export function loadCurrenciesFailure() {
  return {
    type: LOAD_CURRENCIES_FAILURE,
  };
}
