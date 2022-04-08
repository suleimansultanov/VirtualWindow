import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Modal from 'react-responsive-modal';
import Api from './api';

import Button from '../../components/Button';

class PosAntennasOutputPowerButton extends Component {
  constructor(props) {
    super(props);

    this.state = {
      posId: props.posId,
      open: false,
      possibleOutputPowers: [
        20,
        21,
        22,
        23,
        24,
        25,
        26,
        27,
        28,
        29,
        30,
        31,
        32,
        33,
      ],
      chosenOutputPower: '',
    };

    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleChange = this.handleChange.bind(this);
    this.onOpenModal = this.onOpenModal.bind(this);
    this.onCloseModal = this.onCloseModal.bind(this);
  }

  componentWillReceiveProps(nextProps) {
    this.setState({ posId: nextProps.posId });
  }

  onOpenModal = () => {
    this.setState({ open: true });
  };

  onCloseModal = () => {
    this.setState({ open: false });
  };

  handleChange = e => {
    this.setState({ chosenOutputPower: e.target.value });
  };

  handleSubmit() {
    this.onCloseModal();
    Api.setPosAntennasOutputPower(
      this.state.posId,
      this.state.chosenOutputPower,
    );
  }

  render() {
    const { open } = this.state;
    return (
      <div>
        <Button
          name="Изменить"
          colorType="primary"
          onClick={this.onOpenModal}
          size="mini"
        />
        <Modal open={open} onClose={this.onCloseModal} center>
          <br />
          <h3>Выберете новое значение:</h3>
          <div className="form-group">
            <select onChange={this.handleChange} className="form-control">
              {this.state.possibleOutputPowers.map((g, key) => (
                /* eslint-disable react/no-array-index-key */
                <option key={key} value={g}>
                  {g} Вт
                </option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <Button name="Ok" onClick={this.handleSubmit} colorType="primary" />
          </div>
        </Modal>
      </div>
    );
  }
}

PosAntennasOutputPowerButton.propTypes = {
  posId: PropTypes.number.isRequired,
};

export default PosAntennasOutputPowerButton;
