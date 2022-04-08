import { LOAD_POS_DISPLAY_CONTENT_SUCCESS } from './constants';

export function loadPosDisplayContentSuccess(posDisplayContent) {
  return {
    type: LOAD_POS_DISPLAY_CONTENT_SUCCESS,
    posDisplayContent,
  };
}
