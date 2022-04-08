/* eslint-disable react/no-array-index-key */
import React from 'react';
import PropTypes from 'prop-types';

import Divider from '../Divider';

import Paragraph from '../Typography/Paragraph';

const renderChildren = children => {
  let isFirstChild = true;
  return children.map((c, index) => {
    const newChild = (
      <div key={index}>
        {!isFirstChild && <Divider dashed />}
        {c}
      </div>
    );
    isFirstChild = false;
    return newChild;
  });
};

const Form = ({ onSubmit, title, children }) => (
  <div className="ibox">
    {title && (
      <div className="ibox-title">
        <Paragraph>{title}</Paragraph>
      </div>
    )}
    <div className="ibox-content">
      <form onSubmit={onSubmit}>{renderChildren(children)}</form>
    </div>
  </div>
);

Form.propTypes = {
  onSubmit: PropTypes.func.isRequired,
  title: PropTypes.string,
  children: PropTypes.array.isRequired,
};

export default Form;
