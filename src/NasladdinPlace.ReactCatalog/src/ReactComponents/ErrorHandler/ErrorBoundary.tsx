import React from 'react';

import {SadSmileIcon} from '../PointOfSales/Icons';

type Props = {
    errorMessage: string
   };

export class ErrorBoundary extends React.Component<Props> {

    render() {
		const { errorMessage } = this.props;
        return <div id='error'>
		<div className='error'>
			<div className='error-status'>
				<div className='sad-smile-icon'>
					<SadSmileIcon/>
				</div>
				<h1>Ой, что-то пошло не так...</h1>
				<h2>{errorMessage}</h2>
			</div>
			<a className='description' onClick={()=>window.location.reload()}>Обновить страницу</a>
		</div>
	</div>;

    }
  }