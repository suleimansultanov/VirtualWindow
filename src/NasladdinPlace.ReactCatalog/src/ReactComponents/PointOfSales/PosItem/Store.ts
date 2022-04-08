import { observable, action } from 'mobx';

import { Pos, PosContent, PosContentPageInfo, PosGood, ScrollCategoryInfo  } from '../../../Helpers/Http/Pos/DataContracts';
import { CatalogServiceHelper  } from '../../../Helpers/Http/Pos/Pos';
import { PosListStore } from '../PosList/Store';

export class PosItemStore {
    @observable
    hasError: boolean = false;

    @observable
    errorMessage: string = '';

    @observable
    posContent: PosContent | null = null;

    @observable
    currentGood: PosGood | null = null;
    currentGoodCategory: string = '';

    @observable
    posContentPageInfo: PosContentPageInfo = {currentPage:1, hasMore: true, dataLength: 0};

    scrollCategoryInfo: ScrollCategoryInfo = {currentPage:2, hasMore: true, isScrolled: false, categoryId: 0};

    divHeight = 0;

    setRatioHeight = () => {
        const imageDivWidth = document.getElementsByClassName('menu-item-wrapper')[0].scrollWidth;
        this.divHeight = imageDivWidth / 16 * 9;
    }

    openGood = (good: PosGood, categoryName: string) => {
        this.currentGood = good; this.currentGoodCategory=categoryName;
    }

    @action
    openLastPos = () => {
        if (this.posListStore.lastVisitedPos) {
            this.posListStore.posInfo = this.posListStore.lastVisitedPos;
            this.posContentPageInfo.currentPage=1;
            this.posContent=null;
            this.fetchPosContentFromApi(this.posListStore.posInfo.id);
        }
    }

    backToPos = () => this.currentGood = null;

    fetchPosContentFromApi = (posInfoId: number) => 
         CatalogServiceHelper.getPosContent(posInfoId, this.posContentPageInfo.currentPage)
        .then(result => {
            if(result.length===0){
                if(this.posContentPageInfo.currentPage===1){
                    this.posContent=result;
                    this.posContentPageInfo.hasMore = false;
                    return;
                }
                this.posContentPageInfo.hasMore = false;
                return;
            }
            this.posContent = this.posContent===null ? result : this.posContent.concat(result);
            this.setRatioHeight();
            this.posContentPageInfo = {
                dataLength: result.length, hasMore:true, currentPage:
                this.posContentPageInfo.currentPage+1
            };
        });
    

    fetchCategoryItemsFromApi = (categoryId: number) => {

        if(!this.scrollCategoryInfo.isScrolled || this.scrollCategoryInfo.categoryId!==categoryId
                                               || this.scrollCategoryInfo.hasMore){
            if(categoryId!==this.scrollCategoryInfo.categoryId){
                this.scrollCategoryInfo.currentPage=2;
            }
            CatalogServiceHelper.getCategoryContent(categoryId, this.posInfo.id,
                this.scrollCategoryInfo.currentPage)
                .then(result=>{
                    if(result.length===0){
                        this.scrollCategoryInfo.isScrolled=true;
                        this.scrollCategoryInfo.hasMore=false;
                        return;
                    }
                    else{
                        if(this.posContent!==null){
                            const currCategoryGoodsIndex = this.posContent
                            .findIndex(c=>c.category.id===categoryId);
                            this.posContent[currCategoryGoodsIndex]
                            .goods = this.posContent[currCategoryGoodsIndex].goods
                            .concat(result);

                            this.setRatioHeight();
                        }
                    }
                });
            this.scrollCategoryInfo = {
                categoryId: categoryId, hasMore:true, currentPage:
                this.scrollCategoryInfo.currentPage+1, isScrolled: false
            };
        }
    }

    constructor(public posListStore: PosListStore, public posInfo: Pos) {
    }
}
