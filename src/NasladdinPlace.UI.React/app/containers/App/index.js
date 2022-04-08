/**
 *
 * App.js
 *
 * This component is the skeleton around the actual pages, and should only
 * contain code that should be seen on all pages. (e.g. navigation bar)
 *
 * NOTE: while this component should technically be a stateless functional
 * component (SFC), hot reloading does not currently support SFCs. If hot
 * reloading is not a necessity for you then you can refactor it and remove
 * the linting exception.
 */

import React from 'react';
import { Switch, Route } from 'react-router-dom';
import { createMuiTheme, MuiThemeProvider } from '@material-ui/core/styles';

import 'react-redux-toastr/lib/css/react-redux-toastr.min.css';

import LogsPage from 'containers/LogsPage/Loadable';
import PosDisplayPage from 'containers/PosDisplayPage/Loadable';
import NotFoundPage from 'containers/NotFoundPage/Loadable';
import PosMonitoringPage from 'containers/PosMonitoringPage/index';
import DisabledLabeledGoodsPage from 'containers/DisabledLabeledGoodsPage/index';
import LabeledGoodsIdentificationPage from 'containers/LabeledGoodsIdentificationPage/index';

import '../../../node_modules/bootstrap/dist/css/bootstrap.min.css';
import '../../../node_modules/font-awesome/css/font-awesome.css';
import '../../../node_modules/animate.css/animate.min.css';

import '../../public/styles/style.css';

const theme = createMuiTheme({
  typography: {
    fontSize: '1rem',
    fontFamily: '"open sans", "Helvetica Neue", Helvetica, Arial, sans-serif',
  },
});

export default function App() {
  return (
    <div className="row">
      <MuiThemeProvider theme={theme}>
        <Switch>
          <Route exact path="/Logs" component={LogsPage} />
          <Route exact path="/PlantDisplay/Index" component={PosDisplayPage} />
          <Route
            exact
            path="/PointsOfSale/:posId/Monitoring"
            component={PosMonitoringPage}
          />
          <Route
            exact
            path="/LabeledGoods/Disabled"
            component={DisabledLabeledGoodsPage}
          />
          <Route
            exact
            path="/PointsOfSale/:posId/LabeledGoods/Identification"
            component={LabeledGoodsIdentificationPage}
          />
          <Route component={NotFoundPage} />
        </Switch>
      </MuiThemeProvider>
    </div>
  );
}
