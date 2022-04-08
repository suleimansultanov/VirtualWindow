import { fromJS } from 'immutable';

import { LOAD_POS_DISPLAY_CONTENT_SUCCESS } from './constants';

import { PosDisplayContentType } from './posDisplayContentTypes';

export const initialState = fromJS({
  contentType: PosDisplayContentType.QR_CODE,
  content: { qrCode: 'invalid' },
});

export default function posDisplayContentReducer(state = initialState, action) {
  switch (action.type) {
    case LOAD_POS_DISPLAY_CONTENT_SUCCESS:
      return fromJS(action.posDisplayContent);
    default:
      return state;
  }
}
