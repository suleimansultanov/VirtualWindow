import React from 'react';
import PropTypes from 'prop-types';
import { FormattedMessage } from 'react-intl';
import QRCode from 'qrcode.react';
import Radium, { StyleRoot } from 'radium';
import messages from './messages';
import AndroidImage from '../../images/android.svg';
import iOSImage from '../../images/ios.svg';
import RubleImage from '../../images/russian_ruble_symbol.svg';
import LogoImage from '../../images/logo.svg';
import Slider from "react-slick";
import 'slick-carousel/slick/slick-theme.css';
import 'slick-carousel/slick/slick.css';

require('../../fonts/fonts.css');

const flipAnimation = Radium.keyframes({
  '0%': { transform: 'perspective(900px) rotateX(0deg) rotateY(0deg)' },
  '2.5%': {
    transform:
      'perspective(900px) rotateX(-180.1deg) rotateY(0deg)',
  },
  '5%': {
    transform:
      'perspective(900px) rotateX(0deg) rotateY(0deg)',
  }
});

const styles = {
  flip: {
    animation: 'infinite 260s easy',
    animationName: flipAnimation,
  },
  qrCode: {
    position: 'absolute',
    bottom: 65,
    left: 80,
  },
  fontRegular: {
    fontFamily: 'Proxima Nova Regular, semi-serif'
  },
  fontBlack: {
    fontFamily: 'Proxima Nova Black, semi-serif'
  }
};

const settings = {
  dots: false,
  arrows: false,
  infinite: true,
  autoplaySpeed: 4000,
  slidesToShow: 1,
  slidesToScroll: 1,
  autoplay: true,
  touchMove: false,
  fade:true
};

const QrCodePage = ({ value, background }) => {
  document.body.style.background = background;
  document.body.style.backgroundRepeat = 'no-repeat';
  document.body.style.backgroundSize = 'cover';
  document.body.style.backgroundPosition = 'center center';
  document.body.style.backgroundColor = '#ffffff';

  return (
    <div className="welcome-screen">
      <div className="w50 l0">
        <div className="h50 t0">
          <div className="row">
          <div className="col-md-12 f-m mt-4 mb-4 text-left">
              {<span style={styles.fontRegular} className="p-l-1"><FormattedMessage {...messages.firstRowQrCodePageText} /></span>}
              {<img style={{marginLeft: 10}} className="logo-s" src={LogoImage}></img>}            
              {<span className="icon"><img src={AndroidImage}></img>
              {' '}<img src={iOSImage}></img></span>}
          </div>
          </div>
          <div className="row text-left">
            <Slider
              {...settings}>
              <div>
                  <div style={styles.fontBlack} className="col-md-12 f-b bold p-t-3"><FormattedMessage {...messages.secondRowBonusGiftQrCodePageText} /></div>
                  <div className="col-md-12">
                      {<div className="bonus-text bold"><FormattedMessage {...messages.hundredRubles} /></div>}
                      {<div className="bonus-image"><img src={RubleImage}></img></div>}
                  </div> 
              </div>
              <div>
                <div style={{...styles.fontBlack,...{textTransform:"uppercase", color:"#EF0330"}}} className="col-md-12 f-b bold p-t-3 discount-text">
                  {<FormattedMessage {...messages.secondRowDiscountQrCodePageTextFirstPart} />}<br/>
                  {<FormattedMessage {...messages.secondRowDiscountQrCodePageTextSecondPart} />}
                </div>
                <div style={{lineHeight:"42px"}} className="col-md-12 f-m mt-3 t-n-w bold">
                  <span style={styles.fontRegular}>{<FormattedMessage {...messages.secondRowDiscountQrCodePageTextThirdPart} />}</span><br/>
                  <span style={{...styles.fontBlack,...{color:"#EF0330"}}}>{<FormattedMessage {...messages.secondRowDiscountQrCodePageTextFourthPart} />}</span>{'.'}
                </div>
              </div>
              <div>
                <div style={{...styles.fontBlack,...{textTransform:"uppercase", color:"#EF0330"}}} className="col-md-12 mobile-app-update-text bold p-t-3">
                  {<FormattedMessage {...messages.secondRowUpdateMobileApplicationQrCodePageTextFirstPart} />}<br/>
                  {<FormattedMessage {...messages.secondRowUpdateMobileApplicationQrCodePageTextSecondPart} />}{','}<br/>
                  {<FormattedMessage {...messages.secondRowUpdateMobileApplicationQrCodePageTextThirdPart} />}<br/>
                  {<FormattedMessage {...messages.secondRowUpdateMobileApplicationQrCodePageTextFourthPart} />}
                </div>
              </div>
            </Slider>           
          </div>
        </div>
        <div className="h50 b0 p-t-2">
          <div className="col-md-6 p0 w50 qr-m-l">
            <StyleRoot>
              <div style={styles.flip} className="qr-s">
                <QRCode
                  value={value}
                  renderAs="svg"
                  size={350}
                  bgColor="#ffffff"
                  fgColor="#000000"
                  level="Q"
                />
              </div>
            </StyleRoot>
          </div>
          <div className="w50 r0">
            <div className="center-absolute text-left">
            <div style={styles.fontBlack} className="col-md-12 f-b bold"><FormattedMessage {...messages.thirdRowQrCodePageTextFirstPart} /></div>
            <div style={styles.fontBlack} className="col-md-12 f-b bold"><FormattedMessage {...messages.thirdRowQrCodePageTextSecondPart} /></div>
            <div style={styles.fontRegular} className="col-md-12 f-m mt-2 t-n-w bold"><FormattedMessage {...messages.thirdRowQrCodePageTextThirdPart} /></div>
          </div>
          </div>
        </div>
      </div>
      <div className="w50 r0">
      </div>       
    </div>
  );
};

QrCodePage.propTypes = {
  value: PropTypes.string.isRequired,
  background: PropTypes.string,
  appName: PropTypes.string.isRequired,
};

export default QrCodePage;
