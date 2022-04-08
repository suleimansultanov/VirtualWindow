import { observer } from 'mobx-react';
import InfiniteScroll from 'react-infinite-scroll-component';
import React from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom';

import { Spinner} from '../../Shared/Spinner';
import { PosLocationStatuses} from '../../../Enums/PointOfSaleEnums';
import { PosItem } from '../PosItem/PosItem';
import { ErrorBoundary} from '../../ErrorHandler/ErrorBoundary';
import { Exclamation, PosTemperatureIcon, RightArrowIcon } from '../Icons';
import { CatalogServiceHelper } from '../../../Helpers/Http/Pos/Pos';

import { PosListStore } from './Store';
import { PosListStyledComponents as c } from './StyledComponents';

import { Container, Col, Row } from 'react-bootstrap'

@observer
class PosListComponent extends React.Component<RouteComponentProps> {

    private store = new PosListStore;

    async componentDidMount() {
        try{
            await CatalogServiceHelper.getToken();
            await this.store.fetchPointOfSalesFromApi();
        }
        catch(error){
            this.store.hasError=true;
            this.store.errorMessage=error.message;
        }
    }

    render() {
        const pointsOfSaleList = this.store.pointsOfSale;
        if(this.store.hasError){
            return <ErrorBoundary errorMessage={this.store.errorMessage}/>;
        }
        if(!pointsOfSaleList) {
            return Spinner;
        }

        const catalogItems = pointsOfSaleList.map((pos, i) =>
            <c.ListRow key={i} onClick={() => this.store.openPos(pos)} id={i.toString()}>
                    <c.ItemContainer>
                        <c.Title>
                            {pos.name}
                        </c.Title>
                        <c.Footer>
                            <div>
                                <c.IconContainer>
                                    <PosTemperatureIcon />
                                </c.IconContainer>
                                <c.TemperatureValue>
                                    {pos.temperature} &#176; C
                                </c.TemperatureValue>
                                {pos.location &&
                                <c.PosLocation>
                                    {pos.location!==null ? pos.location : PosLocationStatuses.Empty}
                                </c.PosLocation>
                                }
                            </div>
                        </c.Footer>
                    </c.ItemContainer>
                    <c.ArrowContainer>
                        <RightArrowIcon />
                    </c.ArrowContainer>
            </c.ListRow>
        );

        return this.store.posInfo
           ? <PosItem
                store={this.store}
                posInfo = {this.store.posInfo}
            />
            : <Container className='container-without-padding' fluid>
                <Row className='show-grid'>
                    <Col xs={12} sm={12} md={12}>
                        <div id='snackbar' ref={this.store.snackbar}>
                            <div id='snackbar-icon'><Exclamation/></div>
                            <div id='snackbar-text'>Виртуальная витрина открыта</div>
                        </div>
                        <InfiniteScroll onScroll={()=>this.store.fixVirtualPosBlock()}
                            dataLength={this.store.posPageInfo.dataLength}
                            loader={<h3>Загрузка...</h3>}
                            hasMore={this.store.posPageInfo.hasMore}
                            next={this.store.fetchPointOfSalesFromApi}
                            endMessage={
                                <p id='scroll-end-message' style={{textAlign: 'center'}}>
                                    <b>Витрин больше нет.</b>
                                </p>
                            }>
                            {catalogItems}
                        </InfiniteScroll>
                    </Col>
                </Row>
            </Container>;
    }
}

export const PosList = withRouter(PosListComponent);