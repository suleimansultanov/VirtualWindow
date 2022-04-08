import { observer } from 'mobx-react';
import React from 'react';

import {PosLocationStatuses, PosNutrientDefaultValues} from "../../../Enums/PointOfSaleEnums";
import { PosGood } from '../../../Helpers/Http/Pos/DataContracts';
import { BackArrowIcon } from '../Icons';

import {PosItemStore} from './Store';
import { PosItemStyledComponents as c } from './StyledComponets';
import { Row, Col, Container } from 'react-bootstrap';

type Props = {
    store: PosItemStore
};

@observer
export class PosItemGoodDetails extends React.Component<Props> {

    render() {
        const { store } = this.props;
        const good = store.currentGood as PosGood;
 
        const posItemDetails =
        <div>
            <Row>
                <Col xs={12} md={12} className='arrow-back container-without-padding'>
                    <h4>
                        <span onClick={store.backToPos}><BackArrowIcon /></span>
                    </h4>
                </Col>
            </Row>
            <Container className='container container-without-padding' fluid>
                <c.DetailsImage>
                    <Col xs={12} md={12} className='container container-without-padding'>
                        <img className='image-full' src={good.imagePath} />
                    </Col>
                </c.DetailsImage>
                <Col xs={12} md={12} className='height-100'>
                    <div className='container container-without-padding'>
                        <Row>
                            <Col xs={12} md={12} className='pos-item-details-image-text'>
                                <p className='good-title'>
                                    {good.name}
                                </p>
                                {store.currentGoodCategory}
                            </Col>
                        </Row>
                        <Row>
                            <Col xs={12} md={12}>
                                <span className='pos-item-details-info-price'>{good.price} ₽ </span>
                                <span className='pos-item-details-info-weight'>{good.weight} г. </span>
                                <span className='pos-item-details-info-count'>Осталось: {good.count}</span>
                            </Col>
                        </Row>
                        <Row className='pos-item-details-nutrients p-r-15 p-l-15'>
                            <Col xs={3} md={3}>
                                <Col xs={12} md={12} className='vertical-align-div bg-green'>
                                    <p className='nutrients-value'>
                                        {good.nutrients!==null && good.nutrients !== undefined ? good.nutrients.calories : PosNutrientDefaultValues.EmptyValue}
                                    </p>
                                </Col>
                                <h3 className='pos-item-nutrients-title'>Калории</h3>
                            </Col>
                            <Col xs={3} md={3}>
                                <Col xs={12} md={12} className='vertical-align-div bg-purple'>
                                    <p className='nutrients-value'>
                                        {good.nutrients!==null && good.nutrients !== undefined ? good.nutrients.proteins : PosNutrientDefaultValues.EmptyValue}
                                    </p>
                                </Col>
                                <h3 className='pos-item-nutrients-title'>Белки</h3>
                            </Col>
                            <Col xs={3} md={3}>
                                <Col xs={12} md={12} className='vertical-align-div bg-red'>
                                    <p className='nutrients-value'>
                                        {good.nutrients!==null && good.nutrients !== undefined ? good.nutrients.fats : PosNutrientDefaultValues.EmptyValue}
                                    </p>
                                </Col>
                                <h3 className='pos-item-nutrients-title'>Жиры</h3>
                            </Col>
                            <Col xs={3} md={3}>
                                <Col xs={12} md={12} className='vertical-align-div bg-orange'>
                                    <p className='nutrients-value'>
                                        {good.nutrients!==null && good.nutrients !== undefined ? good.nutrients.carbohydrates : PosNutrientDefaultValues.EmptyValue}
                                    </p>
                                </Col>
                                <h3 className='pos-item-nutrients-title'>Углеводы</h3>
                            </Col>
                        </Row>
                    </div>
                    <Row>
                        <Row>
                            <Col xs={12} md={12}>
                                <h3 className='pos-item-details-description-title green-text'>Описание</h3>
                                <div className='pos-item-details-description-text'>
                                    {good.description}
                                </div>
                            </Col>
                            <Col xs={12} md={12}>
                                <h3 className='pos-item-details-description-title green-text'>Состав</h3>
                                <div className='pos-item-details-description-text'>
                                    {good.composition!==null ? good.composition : PosLocationStatuses.NotExist}
                                </div>
                            </Col>
                            <Col xs={12} md={12}>
                                <h3 className='pos-item-details-description-title green-text'>Производитель</h3>
                                <div className='pos-item-details-description-text'>
                                    {good.maker}
                                </div>
                            </Col>
                        </Row>
                    </Row>
                </Col>
            </Container>
        </div>;

        return posItemDetails;
    }
}
