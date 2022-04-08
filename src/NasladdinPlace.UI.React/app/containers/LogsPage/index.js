import { connect } from 'react-redux';
import React from 'react';
import PropTypes from 'prop-types';
import { createStructuredSelector } from 'reselect';
import { compose } from 'redux';

import injectReducer from '../../utils/injectReducer';
import injectSaga from '../../utils/injectSaga';

import { WEB_SOCKET_URL } from '../../constants/urls';
import { loadLogs, addLogSuccess } from './actions';

import LogList from './LogList';

import reducer from './reducer';
import saga from './saga';

import { makeSelectLogs } from './selectors';

class LogsPage extends React.Component {
  constructor(props, context) {
    super(props, context);

    this.state = {
      socketClient: new WebSocket(WEB_SOCKET_URL),
    };

    this.webSocketDidConnect = this.webSocketDidConnect.bind(this);
    this.webSocketDidReceiveMessage = this.webSocketDidReceiveMessage.bind(
      this,
    );
    this.webSocketDidReceiveError = this.webSocketDidReceiveError.bind(this);
  }

  componentDidMount() {
    this.props.loadLogs();

    const { socketClient } = this.state;
    socketClient.onopen = this.webSocketDidConnect;
    socketClient.onmessage = this.webSocketDidReceiveMessage;
    socketClient.onerror = this.webSocketDidReceiveError;
  }

  webSocketDidConnect() {
    this.state.socketClient.send(
      '{"H":"PlantHub","A":{"group":"Logs"},"M":"addToGroup"}',
    );
  }

  webSocketDidReceiveMessage(event) {
    const message = event.data;

    if (message === '{}') {
      return;
    }

    this.props.addLog(JSON.parse(message));
  }

  webSocketDidReceiveError(error) {
    console.error(error.message);
  }

  render() {
    return (
      <div>
        <LogList logs={this.props.logs} />
      </div>
    );
  }
}

LogsPage.propTypes = {
  logs: PropTypes.object.isRequired,
  loadLogs: PropTypes.func.isRequired,
  addLog: PropTypes.func.isRequired,
};

const mapStateToProps = createStructuredSelector({
  logs: makeSelectLogs(),
});

export function mapDispatchToProps(dispatch) {
  return {
    addLog: log => dispatch(addLogSuccess(log)),
    loadLogs: () => dispatch(loadLogs()),
  };
}

const withConnect = connect(
  mapStateToProps,
  mapDispatchToProps,
);

const withReducer = injectReducer({ key: 'logs', reducer });
const withSaga = injectSaga({ key: 'logs', saga });

export default compose(
  withReducer,
  withSaga,
  withConnect,
)(LogsPage);
