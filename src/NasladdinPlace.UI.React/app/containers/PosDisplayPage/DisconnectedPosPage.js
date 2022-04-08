import React from 'react';
import PropTypes from 'prop-types';
import { FormattedMessage } from 'react-intl';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faSadCry } from '@fortawesome/free-solid-svg-icons';
import messages from './messages';

library.add(faSadCry);

const DisconnectedPosPage = ({ background }) => {
  document.body.style.background = background;
  document.body.style.backgroundRepeat = 'no-repeat';
  document.body.style.backgroundSize = 'cover';
  document.body.style.backgroundPosition = 'center center';
  document.body.style.backgroundColor = 'black';
  return (
    <div
      style={{
        height: '100%'
      }}
    >
      <div
        className="f-b"
        style={{
          position: 'absolute',
          left: '50%',
          top: '50%',
          transform: 'translate(-50%, -50%)',
          color: 'white',
          textAlign: 'center',
          fontFamily: 'Proxima Nova Black, sans-serif',
        }}
      >
        <strong>
          <p>
            <FontAwesomeIcon icon="sad-cry" color="#65c800" size="3x" />{' '}
          </p>
          <p>
            <FormattedMessage {...messages.posTemporarilyDisconnected} />
          </p>
          <p>
            <FormattedMessage {...messages.sorryForInconvenience} />
          </p>
        </strong>
      </div>
    </div>
  );
};

DisconnectedPosPage.propTypes = {
  background: PropTypes.string,
};
export default DisconnectedPosPage;
