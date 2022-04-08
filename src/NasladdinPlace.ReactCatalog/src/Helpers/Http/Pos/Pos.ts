import axios from 'axios';

import {LocalStorageHelpers} from '../../Browser/LocalStorage';
import {HttpConstants} from '../Constants';

import { PosContent, PosGood, PosListResponse } from './DataContracts';

const login = HttpConstants.login;
const password = HttpConstants.password;
const apiUrl = HttpConstants.rootUrl;

export class CatalogServiceHelper  {

    static getToken = async (): Promise<void> => {
        const body = new URLSearchParams();
        body.set('username', login);
        body.set('password', password);
        body.set('grant_type', 'password');

        const headers = {
            'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8'
          };
        await axios.post(apiUrl + '/connect/token', body, {headers: headers})
            .then(response => LocalStorageHelpers.setValue('AuthToken', response.data.access_token as string));
    }

    static getPointsOfSale = async(page: number): Promise<PosListResponse> =>
        axios.get(`${apiUrl}/api/catalog/pointsOfSale?page=${page}`, {
            headers: {'Authorization': `Bearer ${LocalStorageHelpers.getValue('AuthToken')}` }
        })
            .then(response => response.data as PosListResponse)

    static getPosContent = (posId: number, page: number): Promise<PosContent> =>
        axios.get(posId!==0 ? `${apiUrl}/api/catalog/posContent?posId=${posId}&page=${page}` :
                              `${apiUrl}/api/catalog/virtual/posContent?page=${page}`, {
            headers: { 'Authorization': `Bearer ${LocalStorageHelpers.getValue('AuthToken')}` }
        })
            .then(response =>response.data as PosContent)

    static getCategoryContent = (categoryId: number, posId: number, page: number): Promise<PosGood[]> =>
        axios.post(posId!==0 ? `${apiUrl}/api/catalog/categoryItems ` :
                               `${apiUrl}/api/catalog/virtual/categoryItems `,
        {
            PosId: posId,
            CategoryId: categoryId,
            Page: page
        },
        {
           headers: { 'Authorization': `Bearer ${LocalStorageHelpers.getValue('AuthToken')}` }
        }
        )
        .then(response =>response.data as PosGood[])

}
