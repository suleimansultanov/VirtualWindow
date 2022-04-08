import React from 'react';
import PropTypes from 'prop-types';

import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';

import { FormattedMessage } from 'react-intl';

import LogListRow from './LogListRow';

import messages from './messages';

const LogList = ({ logs }) => (
  <Table>
    <TableHead>
      <TableRow>
        <TableCell>
          <FormattedMessage {...messages.time} />
        </TableCell>
        <TableCell>
          <FormattedMessage {...messages.type} />
        </TableCell>
        <TableCell>
          <FormattedMessage {...messages.message} />
        </TableCell>
      </TableRow>
    </TableHead>
    <TableBody>
      {logs.map(log => <LogListRow key={log.get('orderNumber')} log={log} />)}
    </TableBody>
  </Table>
);

LogList.propTypes = {
  logs: PropTypes.object.isRequired,
};

export default LogList;
