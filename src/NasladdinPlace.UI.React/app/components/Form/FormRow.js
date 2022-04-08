/* eslint-disable jsx-a11y/label-has-for */
import React from 'react';
import PropTypes from 'prop-types';

const FormRow = ({ title, children }) => (
  <div className="form-group row">
    <label className="col-sm-2 col-form-label">{title}</label>
    <div className="col-sm-10">{children}</div>
  </div>
);

FormRow.propTypes = {
  title: PropTypes.string.isRequired,
  children: PropTypes.oneOfType([PropTypes.object, PropTypes.array]).isRequired,
};

export default FormRow;
