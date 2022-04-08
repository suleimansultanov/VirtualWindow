import React from 'react';
import PropTypes from 'prop-types';

import TableCell from '@material-ui/core/TableCell';
import TableRow from '@material-ui/core/TableRow';

const LogListRow = ({ log }) => (
  <TableRow>
    <TableCell component="th" scope="row">
      {log.get('timestamp')}
    </TableCell>
    <TableCell>{log.get('level')}</TableCell>
    <TableCell>{log.get('content')}</TableCell>
  </TableRow>
);

LogListRow.propTypes = {
  log: PropTypes.object.isRequired,
};

export default LogListRow;
