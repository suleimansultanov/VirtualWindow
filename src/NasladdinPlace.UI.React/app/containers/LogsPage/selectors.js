import { createSelector } from 'reselect';
import { initialState } from './reducer';

const selectLogs = state => state.get('logs', initialState);

const makeSelectLogs = () => createSelector(selectLogs, logsState => logsState);

export { selectLogs, makeSelectLogs };
