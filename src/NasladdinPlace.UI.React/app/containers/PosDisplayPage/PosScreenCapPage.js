import React from 'react';
import { FormattedMessage } from 'react-intl';
import messages from './messages';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import LogoImage from '../../images/logo.svg';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

library.add(faInfoCircle);

const styles = {
    hintRowTextMessageStyle:{
      fontFamily: 'Proxima Nova Regular, semi-serif'
    } 
};

const PosScreenCapPage = () => {
    document.body.style.backgroundColor = '#ffffff';

    return (
        <div className="welcome-screen">
          <div className="w50" style={{left:'50px'}}>
              <div className="text-center" style={{height:'100%'}}>
                    <div className="scaling-container">
                        <img className="scaling-div" src={LogoImage} alt="logo_nasladdin"></img>
                        <div className="bold f-m" style={styles.hintRowTextMessageStyle}>
                            <br/>
                            {<FontAwesomeIcon icon="info-circle" color="#65c800" size="1x" />}{' '}{<FormattedMessage {...messages.capPageHintText} />}
                        </div>              
                    </div>          
               </div>
              </div>
        </div>
    );
  };
  
  export default PosScreenCapPage;