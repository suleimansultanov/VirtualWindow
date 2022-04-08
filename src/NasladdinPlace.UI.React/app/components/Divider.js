import React from 'react';
import PropTypes from 'prop-types';

const getDividerClassName = dashed =>
  dashed ? 'hr-line-dashed' : 'hr-line-solid';

const Divider = ({ dashed = false }) => (
  <div className={getDividerClassName(dashed)} />
);

Divider.propTypes = {
  dashed: PropTypes.bool,
};

export default Divider;
