import React from 'react';
import PropTypes from 'prop-types';

const GridRow = ({ children }) => <div className="row">{children}</div>;

GridRow.propTypes = {
  children: PropTypes.array.isRequired,
};

export default GridRow;
