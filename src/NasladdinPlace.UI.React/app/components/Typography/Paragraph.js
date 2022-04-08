import React from 'react';
import PropTypes from 'prop-types';

const Paragraph = ({ children }) => <h5>{children}</h5>;

Paragraph.propTypes = {
  children: PropTypes.oneOfType([
    PropTypes.string.isRequired,
    PropTypes.array.isRequired,
    PropTypes.object.isRequired,
  ]).isRequired,
};

export default Paragraph;
