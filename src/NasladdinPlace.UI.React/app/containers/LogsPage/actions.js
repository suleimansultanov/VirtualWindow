import {
  LOAD_LOGS,
  LOAD_LOGS_SUCCESS,
  ADD_LOG_SUCCESS,
  LOAD_LOGS_FAILURE,
  ADD_LOG_FAILURE,
} from './constants';

export function loadLogsSuccess(logs) {
  return {
    type: LOAD_LOGS_SUCCESS,
    logs,
  };
}

export function loadLogsFailure(error) {
  return {
    type: LOAD_LOGS_FAILURE,
    error,
  };
}

export function loadLogs() {
  return {
    type: LOAD_LOGS,
  };
}

export function addLogSuccess(log) {
  return {
    type: ADD_LOG_SUCCESS,
    log,
  };
}

export function addLogFailure(error) {
  return {
    type: ADD_LOG_FAILURE,
    error,
  };
}
