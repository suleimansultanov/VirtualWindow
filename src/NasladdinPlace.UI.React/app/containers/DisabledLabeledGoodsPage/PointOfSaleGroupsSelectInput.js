import React from 'react';
import PropTypes from 'prop-types';
import SelectField from 'material-ui/SelectField';
import MenuItem from 'material-ui/MenuItem';

const PointOfSaleGroupsSelectInput = ({
  disabledLabeledGoodsGroupedByPointsOfSale,
  onItemSelected,
  selectedPosGroupId,
}) => (
  <div style={{ marginLeft: '20pt', marginRight: '20pt' }}>
    <SelectField
      floatingLabelText="Витрина"
      onChange={(event, key, payload) => onItemSelected(payload)}
      value={selectedPosGroupId}
      style={{ width: '100%' }}
    >
      <MenuItem key={-1} value={-1} primaryText="Все" />
      {disabledLabeledGoodsGroupedByPointsOfSale.map(posGroup => (
        <MenuItem
          key={posGroup.id}
          value={posGroup.id}
          primaryText={
            posGroup.posInfo.id === 0 ? 'Без витрины' : posGroup.posInfo.name
          }
        />
      ))}
    </SelectField>
  </div>
);

PointOfSaleGroupsSelectInput.propTypes = {
  disabledLabeledGoodsGroupedByPointsOfSale: PropTypes.array.isRequired,
  onItemSelected: PropTypes.func.isRequired,
  selectedPosGroupId: PropTypes.number.isRequired,
};

export default PointOfSaleGroupsSelectInput;
