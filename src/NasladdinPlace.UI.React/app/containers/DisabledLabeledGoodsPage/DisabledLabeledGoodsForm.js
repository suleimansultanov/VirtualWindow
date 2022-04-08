import React from 'react';
import PropTypes from 'prop-types';
import {
  Table,
  TableBody,
  TableHeader,
  TableHeaderColumn,
  TableRow,
  TableRowColumn,
} from 'material-ui/Table';
import Button from '../../components/Button';

const DisabledLabeledGoodsForm = ({
  disabledLabeledGoodsGroupedByPointsOfSale,
  onLabeledGoodsSelected,
  onLabeledGoodsEnableClick,
  selectedLabeledGoodIds,
}) => (
  <div>
    <Table onRowSelection={onLabeledGoodsSelected} multiSelectable>
      <TableHeader>
        <TableRow>
          <TableHeaderColumn>Витрина</TableHeaderColumn>
          <TableHeaderColumn>Товар</TableHeaderColumn>
          <TableHeaderColumn>Метка</TableHeaderColumn>
        </TableRow>
      </TableHeader>
      <TableBody deselectOnClickaway={false} showRowHover>
        {disabledLabeledGoodsGroupedByPointsOfSale.map(posGroup =>
          posGroup.items.map(item => (
            <TableRow key={item.id} selected={selectedLabeledGoodIds[item.id]}>
              <TableRowColumn>
                {posGroup.posInfo.id === 0
                  ? 'Без витрины'
                  : posGroup.posInfo.name}
              </TableRowColumn>
              <TableRowColumn>{item.good.name}</TableRowColumn>
              <TableRowColumn>{item.label}</TableRowColumn>
            </TableRow>
          )),
        )}
      </TableBody>
    </Table>
    <Button
      colorType="primary"
      name="Разблокировать"
      style={{ margin: '20pt' }}
      onClick={onLabeledGoodsEnableClick}
    />
  </div>
);

DisabledLabeledGoodsForm.propTypes = {
  disabledLabeledGoodsGroupedByPointsOfSale: PropTypes.array.isRequired,
  onLabeledGoodsSelected: PropTypes.func.isRequired,
  onLabeledGoodsEnableClick: PropTypes.func.isRequired,
  selectedLabeledGoodIds: PropTypes.object.isRequired,
};

export default DisabledLabeledGoodsForm;
