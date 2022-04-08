import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { compose } from 'redux';

import PosRealTimeInfoPage from './PosRealTimeInfoPage';

import { loadPosRealTimeInfo } from './actions';

import injectReducer from '../../utils/injectReducer';
import injectSaga from '../../utils/injectSaga';

import reducer from './reducer';

import saga from './saga';

class PosRealTimeInfoMainPage extends React.PureComponent {
  componentDidMount() {
    setInterval(() => this.props.loadPosRealTimeInfo(this.props.posId), 5000);
    this.props.loadPosRealTimeInfo(this.props.posId);
  }

  render() {
    return (
      <div>
        <PosRealTimeInfoPage pos={this.props.posRealTimeInfo} />
      </div>
    );
  }
}

PosRealTimeInfoMainPage.propTypes = {
  posId: PropTypes.number.isRequired,
  posRealTimeInfo: PropTypes.object.isRequired,
  loadPosRealTimeInfo: PropTypes.func.isRequired,
};

function mapStateToProps(state, ownProps) {
  const {
    match: { params },
  } = ownProps;
  const posId = parseInt(params.posId, 10) || 1;
  
  return {
    posId,
    posRealTimeInfo: state.get('posRealTimeInfo')[posId] || {
      id: 0,
      connectionStatus: 2,
      doorsState: 2,
      sensorsMeasurements: [],
      antennasOutputPower: 0,
      labelsNumber: 0,
      hardToDetectLabels: [],
      overdueGoodsNumber: 0,
      screenResolution: {}
    },
  };
}

function mapDispatchToProps(dispatch) {
  return {
    loadPosRealTimeInfo: posId => dispatch(loadPosRealTimeInfo(posId)),
  };
}

const withConnect = connect(
  mapStateToProps,
  mapDispatchToProps,
);

const withReducer = injectReducer({ key: 'posRealTimeInfo', reducer });

const withSaga = injectSaga({ key: 'posRealTimeInfo', saga });

export default compose(
  withReducer,
  withSaga,
  withConnect,
)(PosRealTimeInfoMainPage);
