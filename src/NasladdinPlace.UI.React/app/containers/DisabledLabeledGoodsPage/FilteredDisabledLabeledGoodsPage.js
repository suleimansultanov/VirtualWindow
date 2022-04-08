import React from 'react';
import PropTypes from 'prop-types';

import PointsOfSaleGroupsSelectInput from './PointOfSaleGroupsSelectInput';
import DisabledLabeledGoodsForm from './DisabledLabeledGoodsForm';

const POS_GROUP_ALL_ID = -1;

class FilteredDisabledLabeledGoodsPage extends React.Component {
  constructor(props, context) {
    super(props, context);

    this.state = Object.assign(
      {},
      {
        disabledLabeledGoodsGroupedByPointsOfSale: [],
        selectedPosGroupId: POS_GROUP_ALL_ID,
        selectedLabeledGoodIds: {},
      },
      this.createStateUpdateForDisabledLabeledGoodsGroupedByPointsOfSale(
        POS_GROUP_ALL_ID,
        props.disabledLabeledGoodsGroupedByPointsOfSale,
      ),
    );

    this.handleLabeledGoodsSelection = this.handleLabeledGoodsSelection.bind(
      this,
    );
    this.handleFilterItemSelected = this.handleFilterItemSelected.bind(this);
    this.updateDisabledLabeledGoodsGroupedByPointsOfSale = this.updateDisabledLabeledGoodsGroupedByPointsOfSale.bind(
      this,
    );
  }

  componentWillReceiveProps(props) {
    if (
      this.props.disabledLabeledGoodsGroupedByPointsOfSale ===
      props.disabledLabeledGoodsGroupedByPointsOfSale
    ) {
      return;
    }

    this.updateDisabledLabeledGoodsGroupedByPointsOfSale(
      this.state.selectedPosGroupId,
      props.disabledLabeledGoodsGroupedByPointsOfSale,
    );
  }

  handleLabeledGoodsSelection(values) {
    const labeledGoodIds = [];

    const selectedLabeledGoodIds = {};

    if (values === 'none') {
      this.props.onLabeledGoodsSelected(labeledGoodIds);
      this.setState({ selectedLabeledGoodIds });
      return;
    }

    let currentIndex = 0;

    const labeledGoodPredicate =
      values === 'all' ? () => true : index => values.includes(index);

    this.state.disabledLabeledGoodsGroupedByPointsOfSale.forEach(posGroup => {
      posGroup.items.forEach(lg => {
        if (labeledGoodPredicate(currentIndex)) {
          labeledGoodIds.push(lg.id);
          selectedLabeledGoodIds[lg.id] = true;
        }
        currentIndex += 1;
      });
    });

    this.props.onLabeledGoodsSelected(labeledGoodIds);

    this.setState({ selectedLabeledGoodIds });
  }

  handleFilterItemSelected(id) {
    this.setState({ selectedPosGroupId: id, selectedLabeledGoodIds: {} });
    this.updateDisabledLabeledGoodsGroupedByPointsOfSale(
      id,
      this.props.disabledLabeledGoodsGroupedByPointsOfSale,
    );
  }

  createStateUpdateForDisabledLabeledGoodsGroupedByPointsOfSale(
    groupId,
    disabledLabeledGoods,
  ) {
    if (groupId === POS_GROUP_ALL_ID) {
      return {
        disabledLabeledGoodsGroupedByPointsOfSale: disabledLabeledGoods,
      };
    }

    const filteredDisabledLabeledGoods = disabledLabeledGoods.filter(
      posGroup => posGroup.id === groupId,
    );

    if (filteredDisabledLabeledGoods.length === 0) {
      return Object.assign(
        {},
        this.createStateUpdateForDisabledLabeledGoodsGroupedByPointsOfSale(
          POS_GROUP_ALL_ID,
          disabledLabeledGoods,
        ),
        { selectedPosGroupId: POS_GROUP_ALL_ID },
      );
    }

    return {
      disabledLabeledGoodsGroupedByPointsOfSale: filteredDisabledLabeledGoods,
    };
  }

  updateDisabledLabeledGoodsGroupedByPointsOfSale(
    groupId,
    disabledLabeledGoods,
  ) {
    this.setState(
      this.createStateUpdateForDisabledLabeledGoodsGroupedByPointsOfSale(
        groupId,
        disabledLabeledGoods,
      ),
    );
  }

  render() {
    return (
      <div>
        <PointsOfSaleGroupsSelectInput
          disabledLabeledGoodsGroupedByPointsOfSale={
            this.props.disabledLabeledGoodsGroupedByPointsOfSale
          }
          onItemSelected={this.handleFilterItemSelected}
          selectedPosGroupId={this.state.selectedPosGroupId}
        />
        <DisabledLabeledGoodsForm
          disabledLabeledGoodsGroupedByPointsOfSale={
            this.state.disabledLabeledGoodsGroupedByPointsOfSale
          }
          onLabeledGoodsSelected={this.handleLabeledGoodsSelection}
          onLabeledGoodsEnableClick={this.props.onLabeledGoodsEnableClick}
          selectedLabeledGoodIds={this.state.selectedLabeledGoodIds}
        />
      </div>
    );
  }
}

FilteredDisabledLabeledGoodsPage.propTypes = {
  disabledLabeledGoodsGroupedByPointsOfSale: PropTypes.array.isRequired,
  onLabeledGoodsSelected: PropTypes.func.isRequired,
  onLabeledGoodsEnableClick: PropTypes.func.isRequired,
};

export default FilteredDisabledLabeledGoodsPage;
