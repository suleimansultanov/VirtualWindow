import { observable } from 'mobx';
import React from 'react';

import {PointOfSaleMode} from '../../../Enums/PointOfSaleEnums';
import { Pos, PosPageInfo } from '../../../Helpers/Http/Pos/DataContracts';
import { CatalogServiceHelper  } from '../../../Helpers/Http/Pos/Pos';

export class PosListStore {
    @observable
    hasError: boolean = false;

    @observable
    errorMessage: string = '';

    @observable
    pointsOfSale: Pos[] | null = null;

    @observable
    posInfo: Pos | null = null;

    @observable
    lastVisitedPos: Pos | null = null;

    @observable
    posPageInfo: PosPageInfo = {currentPage:1, hasMore: true, dataLength:0};

    @observable
    posMode: string = PointOfSaleMode.Normal;
    snackbar: any;

    constructor(){
        this.snackbar = React.createRef();
    }

    fetchPointOfSalesFromApi = async () => {
        await CatalogServiceHelper.getPointsOfSale(this.posPageInfo.currentPage).then(result => {
            if(result.items===null){
                this.posPageInfo.hasMore = false;
                return;
            }
            this.pointsOfSale = this.pointsOfSale===null ? result.items : this.pointsOfSale.concat(result.items);
            this.lastVisitedPos = result.lastVisited;
            this.posPageInfo = {dataLength: result.items.length, hasMore:true, currentPage: this.posPageInfo.currentPage+1};
        });
    }

    backToPosList = () => this.posInfo = null;

    openPos = (pos: Pos) => {
        this.posInfo = pos;
    }

    openVirtualPos = () => {
        const virtualPos: Pos = {
            id: 0,
            name: 'Виртуальная витрина',
            temperature: 0,
            restrictedAccess: false
        };
        if(this.pointsOfSale){
            const snackbar = this.snackbar.current;
            snackbar.className = 'show';
            setTimeout(() => { snackbar.className = snackbar.className.replace('show', ''); }, 1500);
            this.pointsOfSale.unshift(virtualPos);
            this.posMode = PointOfSaleMode.Virtual;
        }
    }

    fixVirtualPosBlock = () => {
        if(this.posMode===PointOfSaleMode.Virtual){
            const virtualPos = document.getElementById('0');
            if(virtualPos!==null){
                const sticky = virtualPos.offsetTop;
                if (window.pageYOffset > sticky) {
                    virtualPos.classList.add('sticky');
                  } else {
                    virtualPos.classList.remove('sticky');
                  }
            }
        }
        this.changeEndMessageColor(this.posPageInfo.hasMore);
    }
    
    changeEndMessageColor = (hasMore:boolean) => {
        if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight && !hasMore) {
            const endMessage = document.getElementById('scroll-end-message');
            if(endMessage!==null){
                endMessage.style.color='black';
                setTimeout(()=>{endMessage.style.color='white';}, 1500);
            }
        }
    }
}