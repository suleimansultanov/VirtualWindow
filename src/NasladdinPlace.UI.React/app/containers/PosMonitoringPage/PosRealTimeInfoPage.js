import React from 'react';
import PropTypes from 'prop-types';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';

import EnhancedTable from './EnhancedTable';
import PosAntennasOutputPowerButton from './PosAntennasOutputPowerButton';

import {
  SENSOR_INSIDE_POS,
  SENSOR_OUTSIDE_POS,
  SENSOR_EVAPORATOR,
  SENSOR_TOOLBOX,
  SENSOR_NOTUSED
} from './sensors';

const getDoorsStateById = id => {
  switch (id) {
    case 0:
      return 'Левая дверь открыта';
    case 1:
      return 'Правая дверь открыта';
    case 2:
      return 'Двери закрыты';
    default:
      return '';
  }
};

const getConnectionStatusById = id => {
  switch (id) {
    case 1:
      return 'Подключен';
    case 2:
      return 'Отключен';
    default:
      return '';
  }
};

const emptySensorMeasurement = {
  temperature: 0,
  humidity: 0,
};

const getSensorMeasurements = (sensorsMeasurements, sensor) =>
  sensorsMeasurements.filter(sm => sm.sensorId === sensor)[0] ||
  emptySensorMeasurement;

const PosRealTimeInfoPage = ({ pos }) => (
  <div>
    <Table>
      <TableHead>
        <TableRow>
          <TableCell>Свойство</TableCell>
          <TableCell>Значение</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
      <TableRow>
          <TableCell>Температура RFID сканера витрины</TableCell>
          <TableCell>  
            {pos.rfidTemperature} ℃
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Температура внутри витрины</TableCell>
          <TableCell>
            {
              getSensorMeasurements(pos.sensorsMeasurements, SENSOR_INSIDE_POS)
                .temperature
            }{' '}
            ℃
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Влажность внутри витрины</TableCell>
          <TableCell>
            {
              getSensorMeasurements(pos.sensorsMeasurements, SENSOR_INSIDE_POS)
                .humidity
            }{' '}
            %
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Температура снаружи витрины</TableCell>
          <TableCell>
            {
              getSensorMeasurements(pos.sensorsMeasurements, SENSOR_OUTSIDE_POS)
                .temperature
            }{' '}
            ℃
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Влажность снаружи витрины</TableCell>
          <TableCell>
            {
              getSensorMeasurements(pos.sensorsMeasurements, SENSOR_OUTSIDE_POS)
                .humidity
            }{' '}
            %
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Температура внутри ящика с оборудованием</TableCell>
          <TableCell>
            {
              getSensorMeasurements(pos.sensorsMeasurements, SENSOR_TOOLBOX)
                .temperature
            }{' '}
            ℃
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Температура испарителя</TableCell>
          <TableCell>
            {
              getSensorMeasurements(pos.sensorsMeasurements, SENSOR_EVAPORATOR)
                .temperature
            }{' '}
            ℃
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Мощность антенн</TableCell>
          <TableCell>
            <div
              className="row"
              style={{ marginLeft: '0px', minWidth: '150px' }}
            >
              <div style={{ float: 'left', overflow: 'hidden' }}>
                {pos.antennasOutputPower} Вт
              </div>
              <div
                style={{
                  marginLeft: '10px',
                  float: 'left',
                  overflow: 'hidden',
                }}
              >
                <PosAntennasOutputPowerButton posId={pos.id} />
              </div>
            </div>
          </TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Состояние дверей</TableCell>
          <TableCell>{getDoorsStateById(pos.doorsState)}</TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Статус соединения</TableCell>
          <TableCell>{getConnectionStatusById(pos.connectionStatus)}</TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Фактичекое разрешение экрана</TableCell>
          <TableCell>{pos.screenResolution.width}{'x'}{pos.screenResolution.height}</TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Количество товаров внутри витрины</TableCell>
          <TableCell>{pos.labelsNumber}</TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Количество поврежденных RFID меток</TableCell>
          <TableCell>{pos.hardToDetectLabels.length}</TableCell>
        </TableRow>
        <TableRow>
          <TableCell>Количество просрочнных товаров</TableCell>
          <TableCell>{pos.overdueGoodsNumber}</TableCell>
        </TableRow>
      </TableBody>
    </Table>
    <EnhancedTable
      classes={<div id="#root" />}
      posId={pos.id}
      data={pos.hardToDetectLabels}
    />
  </div>
);

PosRealTimeInfoPage.propTypes = {
  pos: PropTypes.object.isRequired,
};

export default PosRealTimeInfoPage;
