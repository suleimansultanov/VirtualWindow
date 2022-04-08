import React from 'react';
import PropTypes from 'prop-types';

const renderSelectOption = o => (
  <option key={o.value} value={o.value}>
    {o.label}
  </option>
);

const renderOptions = options => options.map(renderSelectOption);

const Select = ({ data, input }) => (
  <select {...input} className="form-control">
    {renderOptions(data)}
  </select>
);

Select.propTypes = {
  data: PropTypes.array.isRequired,
  input: PropTypes.object,
};

export default Select;
