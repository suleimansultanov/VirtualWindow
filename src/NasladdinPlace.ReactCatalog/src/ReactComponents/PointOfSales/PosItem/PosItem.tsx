import { observer } from 'mobx-react';
import React from 'react';

import { ErrorBoundary } from '../../ErrorHandler/ErrorBoundary';
import { Pos } from '../../../Helpers/Http/Pos/DataContracts';
import { PosListStore } from '../PosList/Store';

import { PosItemGoodDetails } from './PosItemGoodDetails';
import { PosItemGoodList } from './PosItemGoodList';
import { PosItemStore } from './Store';

type Props = {
    store: PosListStore,
    posInfo: Pos
};

@observer
export class PosItem extends React.Component<Props> {

    private store = new PosItemStore(this.props.store, this.props.posInfo);
    
    async componentDidMount() {
        try{
            await this.store.fetchPosContentFromApi(this.props.posInfo.id);
        }
        catch(error){
            this.store.hasError=true;
            this.store.errorMessage=error.message;
        }
    }

    render() {

        const {store} = this;
        if(this.store.hasError){
            return <ErrorBoundary errorMessage={store.errorMessage}/>;
        }
        return store.currentGood ?
                <PosItemGoodDetails
                    store={store}
                /> :
                <PosItemGoodList
                    store={store}
                />;
    }
}