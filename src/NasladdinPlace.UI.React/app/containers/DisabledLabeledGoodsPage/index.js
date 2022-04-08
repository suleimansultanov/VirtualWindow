import React from 'react';
import PropTypes from 'prop-types';
import { compose } from 'redux';
import { connect } from 'react-redux';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';

import {
  loadDisabledLabeledGoodsGroupedByPointsOfSale,
  enableLabeledGoods,
} from './actions';

import FilteredDisabledLabeledGoodsPage from './FilteredDisabledLabeledGoodsPage';

import injectReducer from '../../utils/injectReducer';
import injectSaga from '../../utils/injectSaga';

import reducer from './reducer';
import saga from './saga';

class DisabledLabeledGoodsPage extends React.Component {
  constructor(props, context) {
    super(props, context);

    this.state = {
      selectedLabeledGoodsIds: [],
    };

    this.handleLabeledGoodsSelection = this.handleLabeledGoodsSelection.bind(
      this,
    );
    this.handleEnableLabeledGoodsClick = this.handleEnableLabeledGoodsClick.bind(
      this,
    );
    this.updateSelectedLabeledGoodsIds = this.updateSelectedLabeledGoodsIds.bind(
      this,
    );
  }

  componentDidMount() {
    this.props.loadDisabledLabeledGoodsGroupedByPointsOfSale();
  }

  handleLabeledGoodsSelection(values) {
    this.updateSelectedLabeledGoodsIds(values);
  }

  updateSelectedLabeledGoodsIds(values) {
    this.setState({ selectedLabeledGoodsIds: values });
  }

  handleEnableLabeledGoodsClick() {
    const { selectedLabeledGoodsIds } = this.state;

    if (selectedLabeledGoodsIds.length === 0) {
      return;
    }

    this.props.enableLabeledGoods(selectedLabeledGoodsIds);
  }

  render() {
    return (
      <MuiThemeProvider>
        <FilteredDisabledLabeledGoodsPage
          disabledLabeledGoodsGroupedByPointsOfSale={
            this.props.disabledLabeledGoodsGroupedByPointsOfSale
          }
          onLabeledGoodsSelected={this.handleLabeledGoodsSelection}
          onLabeledGoodsEnableClick={this.handleEnableLabeledGoodsClick}
        />
      </MuiThemeProvider>
    );
  }
}

DisabledLabeledGoodsPage.propTypes = {
  loadDisabledLabeledGoodsGroupedByPointsOfSale: PropTypes.func.isRequired,
  enableLabeledGoods: PropTypes.func.isRequired,
  disabledLabeledGoodsGroupedByPointsOfSale: PropTypes.array.isRequired,
};

function mapStateToProps(state) {
  return {
    disabledLabeledGoodsGroupedByPointsOfSale:
      state.get('disabledLabeledGoods') || [],
  };
}

function mapDispatchToProps(dispatch) {
  return {
    loadDisabledLabeledGoodsGroupedByPointsOfSale: () =>
      dispatch(loadDisabledLabeledGoodsGroupedByPointsOfSale()),
    enableLabeledGoods: labeledGoodsIdsToEnable =>
      dispatch(enableLabeledGoods(labeledGoodsIdsToEnable)),
  };
}

const withConnect = connect(
  mapStateToProps,
  mapDispatchToProps,
);

const withReducer = injectReducer({ key: 'disabledLabeledGoods', reducer });

const withSaga = injectSaga({ key: 'disabledLabeledGoods', saga });

export default compose(
  withSaga,
  withReducer,
  withConnect,
)(DisabledLabeledGoodsPage);
