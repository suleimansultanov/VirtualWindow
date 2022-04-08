import styled from 'styled-components';

export class PosListStyledComponents {

    static ListContainer = styled.div`
    margin: 12px;
    margin-left: 16px;
`;

    static ItemContainer = styled.div`
    display: inline-block;
    width: 80%;
`;

    static Title = styled.div`
	padding-left: 10px;
	padding-top: 10px;
	padding-bottom: 10px;
	font-style: normal;
	font-weight: 600;
	font-size: 16px;
	line-height: 24px;
	color: #333333;
`;

    static Footer = styled.div`
	padding-bottom: 1.5em;
`;

    static TemperatureValue = styled.div`
	font-style: normal;
	font-weight: bold;
	font-size: 14px;
	line-height: 143%;
	color: #0085FF;
	float: left;
	padding-right: 10px;
	margin-left: 5px;
`;

    static IconContainer = styled.div`
	float: left;
	margin-left: 10px;
`;

    static PosLocation = styled.div`
	font-style: normal;
	font-weight: normal;
	font-size: 14px;
	line-height: 20px;
	color: #333333;
	margin-left: 60px;
	border-left: 1px solid #EBEBEB;
`;

    static ArrowContainer = styled.div`
	width: 7%;
	float: right;
	margin-top: 30px;
`;

	static ListRow = styled.div`
	font-family: Proxima Nova;
	&:nth-child(even) {
		background-color: #f5f4f4;
	}
`;

}