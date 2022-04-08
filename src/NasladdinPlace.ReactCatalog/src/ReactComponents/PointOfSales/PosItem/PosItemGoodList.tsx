import { observer } from 'mobx-react';
import React from 'react';
import ScrollMenu from 'react-horizontal-scrolling-menu';
import InfiniteScroll from 'react-infinite-scroll-component';

import {PointOfSaleMode, PosLocationStatuses} from "../../../Enums/PointOfSaleEnums";
import {Spinner} from '../../Shared/Spinner';
import {EndMessageBlock} from '../../Shared/LoaderEndMessage';
import { BackArrowIcon, CheckMark, ClockIcon, PosContentTemperatureIcon } from '../Icons';

import {PosItemStore} from './Store';

import {Container, Row, Col } from 'react-bootstrap';

type Props = {
    store: PosItemStore
   };

@observer
export class PosItemGoodList extends React.Component<Props>{
    render() {
        const { store } = this.props;

        if(!store.posContent) {
            return Spinner;
        }
        const endMessageBlock = store.posContent.length !==0 ?
        EndMessageBlock :
        <p></p>

        const posContentItems = store.posContent.map((content, i) =>
        <div className='pos-content-container' key={i}>
            <h3 className="green-text">
                {content.category.name}
            </h3>
            <div className='pos-content-slider'>
                <ScrollMenu
                    onUpdate={()=>{
                    store.fetchCategoryItemsFromApi(content.category.id);
                }}
                    alignCenter={false}
                    data={content.goods.map((good, i) =>
                        <div key={i} className='pos-content-item-container' onClick={() => store.openGood(good, content.category.name)}>
                        <div>
                            <div className='pos-content-image-container'>
                                <div onMouseDown={(e) => e.preventDefault()} className='pos-content-image' style={{height: `${store.divHeight}px`}}>
                                    <img className="good-image" src={good.imagePath} alt="" />
                                </div>
                            </div>
                        </div>
                        <div className='pos-content-description'>
                            <div className='pos-content-image-text'>
                                {good.name}
                            </div>
                            <div className='pos-content-description-price'>
                                <span className='pos-content-price-value green-text'>{good.price} ₽ </span> {good.weight} г.
                                    {store.posListStore.posMode===PointOfSaleMode.Virtual?
                                good.publishingStatus!==1?
                                    <div className="pos-content-status-icon-circle"><CheckMark/></div> :
                                    <div className="pos-content-status-icon-clock-circle"><ClockIcon /></div>:
                                <span className='pos-content-count-value'>Осталось: {good.count}</span>}
                            </div>
                        </div>
                    </div>
                )}/>
            </div>
            <div className='pos-content-category-border'></div>
        </div>
        );

        const posGoodListItems =
        <Container className='container-without-padding' fluid>
            <Row className="show-grid">
                <div className='arrow-back' onClick={() => store.posListStore.backToPosList()}>
                    <h4>
                        <BackArrowIcon />
                    </h4>
                </div>
                <div className="pos-name-center">
                    <h4>
                        {store.posListStore.posInfo?.name}
                    </h4>
                </div>
                <div className='pos-header-container'>
                    <div className='pos-content'>
                        <div className='pos-content-temperature-title'>
                            <div className='pos-content-temperature-icon'>
                                <PosContentTemperatureIcon />
                            </div>
                            <div className='pos-content-temperature-value'>
                                {store.posListStore.posInfo.temperature} &#176; С
                            </div>
                            <div className='pos-location-title'>
                                {store.posListStore.posInfo.location!==null ? store.posListStore.posInfo.location : PosLocationStatuses.Empty}
                            </div>
                        </div>
                    </div>
                    {store.posListStore.lastVisitedPos &&
                    <div hidden className='pos-last-buy-container'>
                        <div className='pos-last-buy-info'>
                            <div className='pos-last-buy-info-title'>
                                Последняя покупка:
                            </div>
                            <div className='pos-last-buy-info-value'>
                                {store.posListStore.lastVisitedPos.name}
                            </div>
                        </div>
                        <div className='pos-last-buy-button'>
                            <button className='pos-last-buy-button-text' onClick={() => store.openLastPos()}>Открыть</button>
                        </div>
                    </div>
                    }
                </div>
                <Col md={12} xs={12}>
                    <div className='pos-content-category-border'></div>
                </Col>
                <Col md={12} xs={12}>
                    <InfiniteScroll
                        onScroll={()=>store.posListStore.changeEndMessageColor(store.posContentPageInfo.hasMore)}
                        dataLength={store.posContentPageInfo.dataLength}
                        loader={<h3>Загрузка...</h3>}
                        hasMore={store.posContentPageInfo.hasMore}
                        next={()=>store.fetchPosContentFromApi(store.posListStore.posInfo.id)}
                        endMessage={
                            endMessageBlock
                        }>
                        {posContentItems}
                    </InfiniteScroll>
                </Col>
            </Row>
        </Container>;
        return posGoodListItems;
    }
}
