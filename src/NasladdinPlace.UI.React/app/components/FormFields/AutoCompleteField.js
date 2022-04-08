/* eslint-disable react/prefer-stateless-function,react/prop-types */
import React from 'react';
import PropTypes from 'prop-types';
import { Field } from 'redux-form/immutable';

import AutoComplete from '../AutoComplete';

const renderAutoCompleteField = ({ input, ...custom }) => (
  <AutoComplete onChange={value => input.onChange(value)} {...custom} />
);

class AutoCompleteField extends React.Component {
  render() {
    return (
      <Field
        name={this.props.name}
        component={renderAutoCompleteField}
        placeholder={this.props.hintText}
        floatingLabelText={this.props.floatingLabelText}
        data={this.props.data}
        validate={this.props.validate}
      />
    );
  }
}

AutoCompleteField.propTypes = {
  data: PropTypes.array.isRequired,
  hintText: PropTypes.string.isRequired,
  floatingLabelText: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  validate: PropTypes.func.isRequired,
};

export default AutoCompleteField;
