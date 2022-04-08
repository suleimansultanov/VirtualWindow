/* eslint-disable react/no-array-index-key,no-param-reassign,prefer-destructuring */
import React from 'react';
import PropTypes from 'prop-types';

class GradientText extends React.Component {
  constructor(props) {
    super(props);

    this.tick = this.tick.bind(this);

    this.state = {
      elapsed: 0,
      timer: setInterval(this.tick, props.interval),
    };
  }

  componentWillMount() {
    if (!this.props.animating) {
      clearInterval(this.state.timer);
    }
  }

  componentWillUnmount() {
    clearInterval(this.state.timer);
  }

  tick() {
    this.setState({
      elapsed: this.state.elapsed - 1,
    });
  }

  render() {
    const { data, gradient } = this.props;

    if (data === '') return false;

    const colors = gradient;

    const lengthOfColors = colors.length;

    let chars = this.props.data.split('');
    const charsLength = chars.length;

    const ops = {
      store: [],

      add(fn) {
        this.store.push(fn);
      },

      run(char, i) {
        const accum = {};
        this.store.forEach(fn => {
          fn(char, i, accum);
        });
        return accum;
      },
    };

    if (this.props.opacity !== 100) {
      ops.add((char, i, styles) => {
        styles.opacity = ((i + this.state.elapsed) % charsLength) / charsLength;
      });
    }

    // color mapping
    if (lengthOfColors === 1) {
      ops.add((char, i, styles) => {
        styles.color = colors[0];
      });
    } else if (lengthOfColors > 1) {
      ops.add((char, i, styles) => {
        styles.color = colors[(i + this.state.elapsed) % lengthOfColors];
      });
    }

    chars = chars.map((char, i) => {
      const style = ops.run(char, i);
      return (
        <span key={i} style={style}>
          {char}
        </span>
      );
    });

    return <span>{chars}</span>;
  }
}

GradientText.propTypes = {
  data: PropTypes.string.isRequired,
  gradient: PropTypes.array.isRequired,
  interval: PropTypes.number.isRequired,
  animating: PropTypes.bool.isRequired,
  opacity: PropTypes.number,
};

export default GradientText;
