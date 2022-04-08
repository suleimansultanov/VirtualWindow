import React from 'react';
import { BrowserRouter, Route, Switch } from 'react-router-dom';

import '../../../App.css';
import { PosList } from '../../PointOfSales/PosList/PosList';

export class RootRouteComponent extends React.Component {

    render() {
    return <BrowserRouter>
        <Switch>
            <Route exact path='/Catalog/GetCatalogs' >
                <PosList />
            </Route>
            <Route exact path='/Catalog/Index' >
                <PosList />
            </Route>
        </Switch>
    </BrowserRouter>;
    }
}
