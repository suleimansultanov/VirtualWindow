import React from 'react';
import PropTypes from 'prop-types';
import { Field } from 'redux-form/immutable';

import Select from '../Select';

const SelectField = ({ name, data }) => (
  <Field component={Select} name={name} data={data} />
);

SelectField.propTypes = {
  name: PropTypes.string.isRequired,
  data: PropTypes.array.isRequired,
};

export default SelectField;
