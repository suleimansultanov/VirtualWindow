import React from 'react';
import PropTypes from 'prop-types';
import { FormattedMessage } from 'react-intl';
import messages from './messages';
import CircularProgressbar from 'react-circular-progressbar';
import "react-circular-progressbar/dist/styles.css";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faSmileWink } from '@fortawesome/free-solid-svg-icons';

library.add(faSmileWink);

const styles = {
  firstRowTextMessageStyle: {
    fontFamily: 'Proxima Nova Black, semi-serif'
  },
  secondRowTextMessageStyle:{
    fontFamily: 'Proxima Nova Regular, semi-serif'
  }   
};

class TimerPage extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      timeLeft: 0,
      timer: null,
    };
    this.startTimer = this.startTimer.bind(this);
    this.stopTimer = this.stopTimer.bind(this);
    this.updateState = this.updateState.bind(this);
  }

  componentDidMount() {
    this.startTimer(30);
  }

  startTimer(timeLeft) {
    clearInterval(this.state.timer);
    const timer = setInterval(() => {
      const left = this.state.timeLeft - 1;
      if (left === 0) {
        this.stopTimer();
      }
      this.updateState(left);
    }, 1000);
    return this.setState({ timeLeft, timer });
  }
  updateState(left) {
    this.setState({
      timeLeft: left,
    });
  }
  stopTimer() {
    clearInterval(this.state.timer);
    this.props.onTimerComplete();
  }

  render() {
    return (
      <div className="welcome-screen">
      <div className="w50 l0">
        <div className="h50 t0">
          <div className="row text-center">
            <div className="col-md-12 p-t-3 text-timer">
            <div className="bold f-b" style={styles.firstRowTextMessageStyle}>
              <FormattedMessage {...messages.firstRowTimerPageTextFirstPart} /><br/>
              <FormattedMessage {...messages.firstRowTimerPageTextSecondPart} />{' '}
              <FontAwesomeIcon icon="smile-wink" color="#65c800" />
          </div>
          <div style={styles.secondRowTextMessageStyle} className="f-m">
            <FormattedMessage {...messages.secondRowTimerPageText} />{':'}
          </div>
        </div>
          </div>
        </div>
        <div className="h50 b0"  style={styles.secondRowTextMessageStyle}>
          <div className="qr-timer">
            <FormattedMessage {...messages.secondAbbreviation}>
                  {text =>
                    <CircularProgressbar
                      percentage={(100 / 30) * this.state.timeLeft}
                      text={`${this.state.timeLeft} ${text}`}
                      backgroundPadding={4}
                      styles={{
                        text: {
                          fill: "#333333"
                        },
                        path: {
                          stroke: "#65c800"
                        },
                        trail: { stroke: "rgba(0, 0, 0, 0)" }
                      }}
                    />
                  }
              </FormattedMessage>
          </div>
        </div>
      </div>       
    </div>
    );
  }
}

TimerPage.propTypes = {
  onTimerComplete: PropTypes.func.isRequired,
};

export default TimerPage;
