import {
  LOAD_POS_REAL_TIME_INFO,
  LOAD_POS_REAL_TIME_INFO_SUCCESS,
  LOAD_POS_REAL_TIME_INFO_FAILURE,
} from './constants';

export function loadPosRealTimeInfo(posId) {
  return {
    type: LOAD_POS_REAL_TIME_INFO,
    posId,
  };
}

export function loadPosRealTimeInfoSuccess(posId, posRealTimeInfo) {
  return {
    type: LOAD_POS_REAL_TIME_INFO_SUCCESS,
    posRealTimeInfo,
    posId,
  };
}

export function loadPosRealTimeInfoFailure(posId) {
  return {
    type: LOAD_POS_REAL_TIME_INFO_FAILURE,
    posId,
  };
}
