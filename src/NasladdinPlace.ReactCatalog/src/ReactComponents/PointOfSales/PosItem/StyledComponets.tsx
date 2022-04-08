import styled from 'styled-components';

// TODO Перенести сюда остальные из app.css

export class PosItemStyledComponents {

    static DetailsImage = styled.div `
        width: 100%;
        height: 100%;
        -o-object-fit: contain;
        object-fit: contain;
    `;

    static ImageCategory = styled.div`
        position: absolute;
        height: 17px;
        left: 16px;
        top: 260px;
        font-style: normal;
        font-weight: 600;
        font-size: 12px;
        line-height: 20px;
        color: #FFFFFF;
        opacity: 0.5;
    `;
}