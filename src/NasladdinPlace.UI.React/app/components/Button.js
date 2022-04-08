import React from 'react';
import PropTypes from 'prop-types';

const getSizeClassName = size => {
  if (size === 'mini') {
    return 'btn-xs';
  }
  if (size === 'small') {
    return 'btn-sm';
  }
  if (size === 'large') {
    return 'btn-lg';
  }
  return '';
};

const getVariantClassName = variant => {
  if (variant === 'outline') {
    return 'btn-outline';
  }
  return 'btn-w-m';
};

const getColorTypeClassName = colorType => {
  if (colorType === 'primary') {
    return 'btn-primary';
  }
  if (colorType === 'success') {
    return 'btn-success';
  }
  if (colorType === 'info') {
    return 'btn-info';
  }
  if (colorType === 'warning') {
    return 'btn-warning';
  }
  if (colorType === 'danger') {
    return 'btn-danger';
  }
  if (colorType === 'link') {
    return 'btn-link';
  }
  return 'btn-default';
};

const Button = ({
  name,
  onClick = () => {},
  variant,
  colorType,
  size,
  submit = false,
  style,
}) => (
  <button
    type={submit ? 'submit' : 'button'}
    className={[
      'btn',
      getVariantClassName(variant),
      getColorTypeClassName(colorType),
      getSizeClassName(size),
    ].join(' ')}
    onClick={onClick}
    style={style}
  >
    {name}
  </button>
);

Button.propTypes = {
  name: PropTypes.string.isRequired,
  onClick: PropTypes.func,
  variant: PropTypes.string,
  colorType: PropTypes.string,
  size: PropTypes.string,
  submit: PropTypes.bool,
  style: PropTypes.object
};

export default Button;
