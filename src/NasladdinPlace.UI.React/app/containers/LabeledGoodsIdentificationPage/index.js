import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { compose } from 'redux';
import { reducer as reduxFormReducer } from 'redux-form';

import { WEB_SOCKET_URL } from '../../constants/urls';

import { loadGoods } from '../Goods/actions';
import {
  tieLabelsToGood,
  blockLabels,
  loadUntiedLabeledGoods,
  loadUntiedLabeledGoodsSuccess,
  openLeftDoor,
  openRightDoor,
  closeDoors,
  requestContent,
} from './actions';
import { loadCurrencies } from '../Currencies/actions';

import LabelsToGoodsForm from './LabelsIdentificationForm';

import { formatDate } from '../../utils/dateFormatter';

import injectReducer from '../../utils/injectReducer';
import injectSaga from '../../utils/injectSaga';

import currenciesSaga from '../Currencies/saga';
import goodsSaga from '../Goods/saga';
import untiedLabeledGoodsSaga from './saga';

import currenciesReducer from '../Currencies/reducer';
import goodsReducer from '../Goods/reducer';
import untiedLabeledGoodsReducer from './reducer';

class LabeledGoodsIdentificationPage extends React.Component {
  constructor(props, context) {
    super(props, context);

    this.state = {
      socketClient: new WebSocket(WEB_SOCKET_URL),
    };

    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleBlockLabelsClick = this.handleBlockLabelsClick.bind(this);
    this.webSocketDidConnect = this.webSocketDidConnect.bind(this);
    this.webSocketDidReceiveMessage = this.webSocketDidReceiveMessage.bind(
      this,
    );
    this.handleOpenLeftDoorClick = this.handleOpenLeftDoorClick.bind(this);
    this.handleOpenRightDoorClick = this.handleOpenRightDoorClick.bind(this);
    this.handleCloseDoorsClick = this.handleCloseDoorsClick.bind(this);
    this.handleRequestContentClick = this.handleRequestContentClick.bind(this);

    const webSocketClient = this.state.socketClient;
    webSocketClient.onopen = this.webSocketDidConnect;
    webSocketClient.onmessage = this.webSocketDidReceiveMessage;
  }

  componentDidMount() {
    this.props.loadGoods();
    this.props.loadUntiedLabeledGoods(this.props.posId);
    this.props.loadCurrencies();
  }

  webSocketDidConnect() {
    const { posId } = this.props;
    this.state.socketClient.send(
      `{"H":"PlantHub","A":{"group":"UntiedLabelsOfPos_${posId}"},"M":"addToGroup"}`,
    );
  }

  webSocketDidReceiveMessage(event) {
    const message = event.data;

    if (message === '{}') {
      return;
    }

    const parsedMessage = JSON.parse(message);

    this.props.loadUntiedLabeledGoodsSuccess(this.props.posId, parsedMessage.A);
  }

  handleSubmit(values) {
    if (this.props.untiedLabeledGoods.length === 0) {
      return;
    }

    const result = {
      manufactureDate: formatDate(values.manufactureDate),
      expirationDate: formatDate(values.expirationDate),
      goodId: values.goodId.value,
      labels: this.props.untiedLabeledGoods.map(lg => lg.label),
      price: values.price,
      currencyId: values.currencyId,
    };
    this.props.tieLabelsToGood(this.props.posId, result);
  }

  handleBlockLabelsClick() {
    if (this.props.untiedLabeledGoods.length === 0) {
      return;
    }

    const result = {
      labels: this.props.untiedLabeledGoods.map(lg => lg.label),
    };
    this.props.blockLabels(this.props.posId, result);
  }

  handleOpenRightDoorClick() {
    this.props.openRightDoor(this.props.posId);
  }

  handleOpenLeftDoorClick() {
    this.props.openLeftDoor(this.props.posId);
  }

  handleCloseDoorsClick() {
    this.props.closeDoors(this.props.posId);
  }

  handleRequestContentClick() {
    this.props.requestContent(this.props.posId);
  }

  render() {
    return (
      <LabelsToGoodsForm
        goods={this.props.goods}
        currencies={this.props.currencies}
        labels={this.props.untiedLabeledGoods.map(lg => lg.label)}
        onSubmit={this.handleSubmit}
        onBlockLabelsClick={this.handleBlockLabelsClick}
        onOpenLeftDoorClick={this.handleOpenLeftDoorClick}
        onOpenRightDoorClick={this.handleOpenRightDoorClick}
        onRequestContentClick={this.handleRequestContentClick}
        onCloseDoorsClick={this.handleCloseDoorsClick}
      />
    );
  }
}

LabeledGoodsIdentificationPage.propTypes = {
  goods: PropTypes.array.isRequired,
  untiedLabeledGoods: PropTypes.array.isRequired,
  currencies: PropTypes.array.isRequired,
  posId: PropTypes.string.isRequired,
  loadGoods: PropTypes.func.isRequired,
  loadUntiedLabeledGoods: PropTypes.func.isRequired,
  blockLabels: PropTypes.func.isRequired,
  tieLabelsToGood: PropTypes.func.isRequired,
  loadCurrencies: PropTypes.func.isRequired,
  loadUntiedLabeledGoodsSuccess: PropTypes.func.isRequired,
  openLeftDoor: PropTypes.func.isRequired,
  openRightDoor: PropTypes.func.isRequired,
  closeDoors: PropTypes.func.isRequired,
  requestContent: PropTypes.func.isRequired,
};

function mapStateToProps(state, ownProps) {
  const {
    match: { params },
  } = ownProps;

  const posId = params.posId || '1';
  return {
    goods: state.get('goods') || [],
    currencies: state.get('currencies') || [],
    untiedLabeledGoods: state.get('posUntiedLabeledGoods')[posId] || [],
    posId,
  };
}

function mapDispatchToProps(dispatch) {
  return {
    loadGoods: () => dispatch(loadGoods()),
    loadCurrencies: () => dispatch(loadCurrencies()),
    loadUntiedLabeledGoods: posId => dispatch(loadUntiedLabeledGoods(posId)),
    tieLabelsToGood: (posId, labelsToGoodModel) =>
      dispatch(tieLabelsToGood(posId, labelsToGoodModel)),
    blockLabels: (posId, labelsModel) =>
      dispatch(blockLabels(posId, labelsModel)),
    loadUntiedLabeledGoodsSuccess: (posId, labeledGoods) =>
      dispatch(loadUntiedLabeledGoodsSuccess(posId, labeledGoods)),
    openLeftDoor: posId => dispatch(openLeftDoor(posId)),
    openRightDoor: posId => dispatch(openRightDoor(posId)),
    closeDoors: posId => dispatch(closeDoors(posId)),
    requestContent: posId => dispatch(requestContent(posId)),
  };
}

const withConnect = connect(
  mapStateToProps,
  mapDispatchToProps,
);

const withUntiedLabeledGoodsReducer = injectReducer({
  key: 'posUntiedLabeledGoods',
  reducer: untiedLabeledGoodsReducer,
});

const withCurrenciesReducer = injectReducer({
  key: 'currencies',
  reducer: currenciesReducer,
});

const withGoodsReducer = injectReducer({
  key: 'goods',
  reducer: goodsReducer,
});

const withReduxFormReducer = injectReducer({
  key: 'form',
  reducer: reduxFormReducer,
});

const withUntiedLabeledGoodsSaga = injectSaga({
  key: 'untiedLabeledGoods',
  saga: untiedLabeledGoodsSaga,
});

const withCurrenciesSaga = injectSaga({
  key: 'currencies',
  saga: currenciesSaga,
});

const withGoodsSaga = injectSaga({
  key: 'goods',
  saga: goodsSaga,
});

export default compose(
  withUntiedLabeledGoodsReducer,
  withCurrenciesReducer,
  withGoodsReducer,
  withReduxFormReducer,
  withUntiedLabeledGoodsSaga,
  withCurrenciesSaga,
  withGoodsSaga,
  withConnect,
)(LabeledGoodsIdentificationPage);
