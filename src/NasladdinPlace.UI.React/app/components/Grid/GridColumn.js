import React from 'react';
import PropTypes from 'prop-types';

const getColumnClassNameBySize = size => {
  if (size === 'small') {
    return 'col-sm';
  }
  if (size === 'medium') {
    return 'col-md';
  }
  if (size === 'large') {
    return 'col-lg';
  }
  if (size === 'extraLarge') {
    return 'col-xl';
  }
  return 'col-sm';
};

const GridColumn = ({ size, ...children }) => (
  <div className={getColumnClassNameBySize(size)}>{...children}</div>
);

GridColumn.propTypes = {
  size: PropTypes.string,
};

export default GridColumn;
