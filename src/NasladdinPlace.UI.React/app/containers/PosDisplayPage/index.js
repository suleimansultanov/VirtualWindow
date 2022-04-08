import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { createStructuredSelector } from 'reselect';
import { compose } from 'redux';

import { WEB_SOCKET_URL, BASE_FRONT_URL } from '../../constants/urls';
import QrCodePage from './QrCodePage';
import DisconnectedPosPage from './DisconnectedPosPage';
import TimerPage from './TimerPage';
import ScreenCap from './PosScreenCapPage';
import injectReducer from '../../utils/injectReducer';

import {
  makeSelectBrand,
  makeSelectPosDisplayContent,
  makeSelectPosId,
} from './selectors';

import { loadPosDisplayContentSuccess } from './actions';
import { PosDisplayContentType } from './posDisplayContentTypes';

import reducer from './reducer';

const createWebSocket = () => new WebSocket(WEB_SOCKET_URL);

const millisInSecond = 1000;
const keepAliveUpdateIntervalInSeconds = 60 * millisInSecond;
class PosDisplayPage extends React.Component {
  constructor(props, context) {
    super(props, context);

    this.state = {
      socketClient: createWebSocket(),
      isSocketConnectionClosed: false,
      isWebSocketReconnecting: false,
    };

    this.webSocketDidConnect = this.webSocketDidConnect.bind(this);
    this.webSocketDidReceiveMessage = this.webSocketDidReceiveMessage.bind(
      this,
    );
    this.webSocketDidReceiveError = this.webSocketDidReceiveError.bind(this);
    this.webSocketDidClose = this.webSocketDidClose.bind(this);
    this.getComponentToRender = this.getComponentToRender.bind(this);
    this.reconnectWebSocket = this.reconnectWebSocket.bind(this);
    this.setupWebSocketClient = this.setupWebSocketClient.bind(this);
    this.getQrCodePage = this.getQrCodePage.bind(this);
    this.handleTimerComplete = this.handleTimerComplete.bind(this);
    this.getDisconnectPage = this.getDisconnectPage.bind(this);
    this.handlerIntervalPosDisplayAlive = this.handlerIntervalPosDisplayAlive.bind(this);
    this.sendPosDisplayAliveRequest = this.sendPosDisplayAliveRequest.bind(this);
    this.sendDeliveryConfirmationRequest = this.sendDeliveryConfirmationRequest.bind(this);
  }

  componentDidMount() {
    this.setupWebSocketClient();
    this.reconnectWebSocket();
    this.handlerIntervalPosDisplayAlive();
  }

  componentWillUnmount() {
    clearInterval(this.intervaPosDisplayAlive);
  }

  handleTimerComplete(){
    this.reconnectWebSocket();
  }

  handlerIntervalPosDisplayAlive(){
    this.intervaPosDisplayAlive = setInterval(() => this.sendPosDisplayAliveRequest(), keepAliveUpdateIntervalInSeconds );
  }

  sendPosDisplayAliveRequest() {
    const { posId } = this.props;
    this.state.socketClient.send(
      `{"H":"posDisplayActivity","A":{"group":"PlantDisplay_${posId}"},"M":"notifyPosDisplayActivity"}`,
    );
  }

  sendDeliveryConfirmationRequest() {
    if(this.props.posDisplayContent.get('content') != undefined) {  
      const commandId = this.props.posDisplayContent.get('content').find(function(value, key) {
        if(key == 'commandId') {
          return value;
        }
      });

      if(commandId != undefined) {
        this.state.socketClient.send(
          `{"H":"posDisplayActivity","A":{"commandId":"${commandId}","posId":"${this.props.posId}"},"M":"confirmCommandDelivery"}`
        );
      }
    }
    
    return;
  }

  getDisconnectPage() {
    return <DisconnectedPosPage
    background={`url(${BASE_FRONT_URL}/api/posMediaContents/${this.props.posId}/1)`} />;
  }

  getQrCodePage() {
    const qrCode = this.props.posDisplayContent.get('content').get('qrCode');
    return <QrCodePage
        value={qrCode}
        appName="NASLADDIN"
        background={`url(${BASE_FRONT_URL}/api/posMediaContents/${this.props.posId}/0)`}
      />;
  }

  getComponentToRender() {
    if (this.state.isSocketConnectionClosed) {
      return this.getDisconnectPage();
    }

    switch(this.props.posDisplayContent.get('contentType')){
      case PosDisplayContentType.DISCONNECT:
        return this.getDisconnectPage();
      case PosDisplayContentType.TIMER:
        return <TimerPage onTimerComplete={this.handleTimerComplete}/>;
      case PosDisplayContentType.REFRESH:
        window.location.reload(true);
      case PosDisplayContentType.HIDE:
        return <ScreenCap/>
      default: 
        return this.getQrCodePage();
    }
  }

  setupWebSocketClient() {
    const { socketClient } = this.state;
    socketClient.onopen = this.webSocketDidConnect;
    socketClient.onmessage = this.webSocketDidReceiveMessage;
    socketClient.onerror = this.webSocketDidReceiveError;
    socketClient.onclose = this.webSocketDidClose;
  }

  webSocketDidReceiveMessage(event) {
    const message = event.data;

    if (message === '{}') {
      return;
    }

    const parsedMessage = JSON.parse(message);

    this.props.loadPosDisplayContentSuccess(parsedMessage.A); 
    
    this.sendDeliveryConfirmationRequest();
  }

  webSocketDidReceiveError() {
    this.reconnectWebSocket();
    this.setupWebSocketClient();
  }

  webSocketDidClose() {
    this.setState({ isSocketConnectionClosed: true });

    this.reconnectWebSocket();
  }

  reconnectWebSocket() {
    if (this.state.isWebSocketReconnecting) {
      return;
    }

    this.setState({ isWebSocketReconnecting: true });

    setTimeout(() => {
      this.setState(() => ({
        socketClient: createWebSocket(),
        isWebSocketReconnecting: false,
      }));
      this.setupWebSocketClient();
    }, 5000);
  }

  webSocketDidConnect() {
    const { posId } = this.props;
    this.state.socketClient.send(
      `{"H":"PlantHub","A":{"group":"PlantDisplay_${posId}"},"M":"addToGroup"}`,
    );
    this.setState({ isSocketConnectionClosed: false });
  }

  render() {
    return (
      <div id="plant-display">
        {this.getComponentToRender()}
      </div>
    );
  }
}

PosDisplayPage.propTypes = {
  posId: PropTypes.string.isRequired,
  posDisplayContent: PropTypes.object.isRequired,
  brand: PropTypes.string.isRequired,
  loadPosDisplayContentSuccess: PropTypes.func.isRequired,
};

const mapStateToProps = createStructuredSelector({
  brand: makeSelectBrand(),
  posId: makeSelectPosId(),
  posDisplayContent: makeSelectPosDisplayContent(),
});

function mapDispatchToProps(dispatch) {
  return {
    loadPosDisplayContentSuccess: posContent =>
      dispatch(loadPosDisplayContentSuccess(posContent)),
  };
}

const withConnect = connect(
  mapStateToProps,
  mapDispatchToProps,
);

const withReducer = injectReducer({ key: 'posDisplayContent', reducer });

export default compose(
  withReducer,
  withConnect,
)(PosDisplayPage);
