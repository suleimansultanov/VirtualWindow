import queryString from 'query-string';
import { createSelector } from 'reselect';
import { initialState } from './reducer';

const selectPosDisplayContent = state =>
  state.get('posDisplayContent', initialState);

const makeSelectPosDisplayContent = () =>
  createSelector(
    selectPosDisplayContent,
    posDisplayContentState => posDisplayContentState,
  );

const selectPosId = (state, ownProps) =>
  queryString.parse(ownProps.location.search).plantId;

const makeSelectPosId = () => createSelector(selectPosId, posId => posId);

const selectBrand = (state, ownProps) =>
  queryString.parse(ownProps.location.search).brand || 'nasladdin';

const makeSelectBrand = () => createSelector(selectBrand, brand => brand);

export {
  selectPosDisplayContent,
  makeSelectPosDisplayContent,
  selectPosId,
  makeSelectPosId,
  selectBrand,
  makeSelectBrand,
};
