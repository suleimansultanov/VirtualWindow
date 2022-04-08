import React from 'react';
import PropTypes from 'prop-types';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import { reduxForm, Field } from 'redux-form/immutable';
import MenuItem from 'material-ui/MenuItem';
import { SelectField, DatePicker, TextField } from 'redux-form-material-ui';
import { withStyles } from '@material-ui/core/styles';

import Button from '../../components/Button';
import Form from '../../components/Form/Form';
import FormRow from '../../components/Form/FormRow';
import Paragraph from '../../components/Typography/Paragraph';

import { formatDate } from '../../utils/dateFormatter';
import AutoCompleteField from '../../components/FormFields/AutoCompleteField';

const IntlPolyfill = require('intl');
const { DateTimeFormat } = IntlPolyfill;
require('intl/locale-data/jsonp/ru');

const styles = theme => ({
  root: {
    flexGrow: 1,
  },
  paper: {
    padding: theme.spacing.unit * 2,
    textAlign: 'center',
    color: theme.palette.text.secondary,
  },
});

const required = value => (value == null ? 'Обязательное поле' : undefined);

const buttonStyle = {
  margin: 8,
};

const textFieldStyle = {
  minWidth: 250,
};

const gridColumnStyle = {
  paddingLeft: 0,
  minWidth: 280,
};

function getTomorrow() {
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  return tomorrow;
}

const LabeledGoodsIdentificationForm = ({
  goods,
  currencies,
  labels,
  handleSubmit,
  onBlockLabelsClick,
  onOpenLeftDoorClick,
  onOpenRightDoorClick,
  onCloseDoorsClick,
  onRequestContentClick,
}) => (
  <MuiThemeProvider>
    <Form onSubmit={handleSubmit}>
      <FormRow title="Режим идентификации">
        <Button
          name="Открыть левую дверь"
          colorType="success"
          style={buttonStyle}
          onClick={onOpenLeftDoorClick}
        />
        <Button
          name="Открыть правую дверь"
          colorType="success"
          style={buttonStyle}
          onClick={onOpenRightDoorClick}
        />
        <Button
          name="Закрыть двери"
          colorType="success"
          style={buttonStyle}
          onClick={onCloseDoorsClick}
        />
        <Button
          name="Запросить содержимое"
          colorType="success"
          style={buttonStyle}
          onClick={onRequestContentClick}
        />
      </FormRow>
      <FormRow title="Меток для идентификации">
        <Paragraph>{labels.length} шт.</Paragraph>
        <Paragraph>
          {labels.map((item, i, arr) => {
            const divider = i < arr.length - 1 && <strong> / </strong>;
            return (
              <span>
                {`.. ${item.substring(item.length - 5)}`}
                {divider}
              </span>
            );
          })}
        </Paragraph>
      </FormRow>
      <FormRow title="Товар">
        <AutoCompleteField
          floatingLabelText="Товар"
          name="goodId"
          hintText="Товар"
          validate={required}
          data={goods.map(g => ({
            label: g.name,
            value: g.id,
          }))}
        />
      </FormRow>
      <FormRow title="Срок годности">
        <div className="col-md-4" style={gridColumnStyle}>
          <Field
            name="manufactureDate"
            component={DatePicker}
            floatingLabelText="Дата изготовления"
            formatDate={formatDate}
            format={null}
            validate={required}
            DateTimeFormat={DateTimeFormat}
            locale="ru"
            maxDate={new Date()}
            openToYearSelection
            textFieldStyle={textFieldStyle}
          />
        </div>
        <div className="col-md-4" style={gridColumnStyle}>
          <Field
            name="expirationDate"
            component={DatePicker}
            floatingLabelText="Дата окончания срока годности"
            formatDate={formatDate}
            format={null}
            validate={required}
            minDate={getTomorrow()}
            DateTimeFormat={DateTimeFormat}
            locale="ru"
            openToYearSelection
            textFieldStyle={textFieldStyle}
          />
        </div>
      </FormRow>
      <FormRow title="Цена">
        <div className="col-md-4" style={gridColumnStyle}>
          <Field
            name="price"
            component={TextField}
            floatingLabelText="Цена"
            type="number"
            validate={required}
            style={textFieldStyle}
          />
        </div>
        <div className="col-md-4" style={gridColumnStyle}>
          <Field
            component={SelectField}
            floatingLabelText="Валюта"
            hintText="Валюта"
            fullWidth
            name="currencyId"
            validate={required}
            style={textFieldStyle}
          >
            {currencies.map(c => (
              <MenuItem key={c.id} value={c.id} primaryText={c.name} />
            ))}
          </Field>
        </div>
      </FormRow>
      <div>
        <Button
          name="Привязать"
          submit
          colorType="primary"
          style={buttonStyle}
        />
        <Button
          name="Заблокировать"
          colorType="danger"
          onClick={onBlockLabelsClick}
          style={buttonStyle}
        />
      </div>
    </Form>
  </MuiThemeProvider>
);

LabeledGoodsIdentificationForm.propTypes = {
  goods: PropTypes.array.isRequired,
  currencies: PropTypes.array.isRequired,
  labels: PropTypes.array.isRequired,
  handleSubmit: PropTypes.func.isRequired,
  onBlockLabelsClick: PropTypes.func.isRequired,
  onOpenLeftDoorClick: PropTypes.func.isRequired,
  onOpenRightDoorClick: PropTypes.func.isRequired,
  onCloseDoorsClick: PropTypes.func.isRequired,
  onRequestContentClick: PropTypes.func.isRequired,
};

export default reduxForm({
  form: 'labelsToGoodsForm',
  initialValues: {
    currencyId: 1,
  },
})(withStyles(styles)(LabeledGoodsIdentificationForm));
